#!/bin/bash

# Directory containing the prefab files
DIRECTORY="../Assets/Prefabs/Game/Mechanics/Offers"

# Temporary file to store the intermediate results
TMP_FILE=$(mktemp)

# Find all .prefab files and process them
find "$DIRECTORY" -type f -name '*.prefab' | while read -r prefab_file; do
    # Extracting the GameObject name
    name=$(grep 'm_Name:' "$prefab_file" | head -1 | awk -F': ' '{print $2}')

    # Extracting the OfferPool and Value from MonoBehaviour section
    offer_pool=$(awk '/MonoBehaviour:/{flag=1;next}/--- /{flag=0}flag' "$prefab_file" | grep 'OfferPool:' | awk -F': ' '{print $2}')
    value=$(awk '/MonoBehaviour:/{flag=1;next}/--- /{flag=0}flag' "$prefab_file" | grep 'Value:' | awk -F': ' '{print $2}')

	# Check if Offer Pool is set (not empty)
    if [ ! -z "$offer_pool" ]; then
        # Storing results in the temporary file with a format suitable for sorting
        echo "$offer_pool,$value,$name" >> "$TMP_FILE"
    fi
done

# Sorting the results by the Offer Pool value (numerically) and formatting the output
sort -t, -k1n "$TMP_FILE" | {
    current_offer_pool=-1
    while IFS=, read -r offer_pool value name; do
        # Check if this is a new Offer Pool section
        if [ "$offer_pool" -ne "$current_offer_pool" ]; then
            if [ "$current_offer_pool" -ne -1 ]; then
				echo "#################"
				echo ""
            fi
            echo "#################"
            echo "OFFER POOL $offer_pool"
            echo "--------------"
            current_offer_pool=$offer_pool
        fi
        # Display the details
        echo "Name: $name"
        echo "Value: $value"
        echo "Offer Pool: $offer_pool"
        echo "---------------"
    done
}

# Clean up the temporary file
rm "$TMP_FILE"
