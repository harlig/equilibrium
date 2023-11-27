#!/bin/bash

# Directory containing the prefab files
DIRECTORY="../Assets/Prefabs/Game/Mechanics/Offers"

# Temporary file to store the intermediate results
TMP_FILE=$(mktemp)

# Define a function to map CorrespondingState integer to EquilibriumState
map_state_to_enum() {
    case $1 in
        0) echo "FROZEN" ;;
        1) echo "COLD" ;;
        2) echo "BRISK" ;;
        3) echo "NEUTRAL" ;;
        4) echo "WARM" ;;
        5) echo "HOT" ;;
        6) echo "INFERNO" ;;
        *) echo "UNKNOWN" ;;
    esac
}

# Find all .prefab files and process them
find "$DIRECTORY" -type f -name '*.prefab' | while read -r prefab_file; do
    # Extracting the GameObject name
    name=$(grep 'm_Name:' "$prefab_file" | head -1 | awk -F': ' '{print $2}')

    # Extracting the OfferPool, Value, and CorrespondingState from MonoBehaviour section
    offer_pool=$(awk '/MonoBehaviour:/{flag=1;next}/--- /{flag=0}flag' "$prefab_file" | grep 'OfferPool:' | awk -F': ' '{print $2}')
    value=$(awk '/MonoBehaviour:/{flag=1;next}/--- /{flag=0}flag' "$prefab_file" | grep 'Value:' | awk -F': ' '{print $2}')
    corresponding_state=$(awk '/MonoBehaviour:/{flag=1;next}/--- /{flag=0}flag' "$prefab_file" | grep 'CorrespondingState:' | awk -F': ' '{print $2}')

    # Check if Offer Pool is set (not empty)
    if [ ! -z "$offer_pool" ]; then
        # Mapping the CorrespondingState
        state=$(map_state_to_enum "$corresponding_state")
        # Storing results in the temporary file with a format suitable for sorting
        echo "$corresponding_state,$offer_pool,$value,$name,$state" >> "$TMP_FILE"
    fi
done

# Sorting the results by Equilibrium State (numerically) and then by Offer Pool (numerically)
sort -t, -k1n,1 -k2n,2 "$TMP_FILE" | {
    current_state=-1
    while IFS=, read -r corresponding_state offer_pool value name state; do
        # Check if this is a new Equilibrium State section
        if [ "$corresponding_state" -ne "$current_state" ]; then
            if [ "$current_state" -ne -1 ]; then
                echo "###################################################"
                echo
            fi
            echo "###################################################"
            echo "EQUILIBRIUM STATE $state"
            echo "--------------"
            current_state=$corresponding_state
        fi
        # Display the details
        echo "Name: $name"
        echo "Value: $value"
        echo "Offer Pool: $offer_pool"
        echo "---------------"
    done
    echo "###################################################"
    echo
}

# Clean up the temporary file
rm "$TMP_FILE"
