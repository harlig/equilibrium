using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfferSystem : MonoBehaviour
{
    // since all offers each belong to a pool, you can just add them to this system in the inspector
    [SerializeField]
    private List<OfferData> allOfferPrefabs;
    private List<OfferData>[] offerPools;
    private readonly System.Random random = new();

    public static OfferSystem Create(OfferSystem prefab, Transform parent)
    {
        return Instantiate(prefab, parent);
    }

    private void Awake()
    {
        HashSet<int> numPools = new();
        foreach (OfferData offer in allOfferPrefabs)
        {
            if (!numPools.Contains(offer.OfferPool))
            {
                numPools.Add(offer.OfferPool);
            }
            if (offer.Sprite == null)
            {
                throw new Exception(
                    $"Offer named {offer.name} has no image! Set an image on this offer or remove it from the offer system"
                );
            }
        }

        // arrays are fast but idk if it's worth the extra effort to loop over allOffers twice
        offerPools = new List<OfferData>[numPools.Count];
        for (int ndx = 0; ndx < offerPools.Length; ndx++)
        {
            offerPools[ndx] = new List<OfferData>();
        }

        foreach (OfferData offer in allOfferPrefabs)
        {
            if (offer.OfferPool >= allOfferPrefabs.Count)
            {
                throw new Exception(
                    string.Format(
                        "Tried to add offer for pool {0} when system is only configured to handle {1} pools! Offer {2}",
                        offer.OfferPool,
                        allOfferPrefabs.Count,
                        offer
                    )
                );
            }

            offerPools[offer.OfferPool].Add(offer);
        }
    }

    public List<OfferData> GetOffers(
        int numOffersToRetrieve,
        int playerLevel,
        EquilibriumManager.EquilibriumState currentEquilibriumState,
        AcquisitionManager acquisitionManager
    )
    {
        List<OfferData> selectedOffersInstantiated = new();
        HashSet<OfferData> alreadySelectedOfferPrefabs = new();

        for (int ndx = 0; ndx < numOffersToRetrieve; ndx++)
        {
            int poolIndex = SelectPoolIndex(playerLevel);
            OfferData offerPrefab = SelectOfferFromPool(
                poolIndex,
                currentEquilibriumState,
                acquisitionManager.OfferAcquisitions,
                alreadySelectedOfferPrefabs
            );

            // if we can't get an offer from this pool, try from the pool below
            while (offerPrefab == null && poolIndex > 0)
            {
                poolIndex--;
                offerPrefab = SelectOfferFromPool(
                    poolIndex,
                    currentEquilibriumState,
                    acquisitionManager.OfferAcquisitions,
                    alreadySelectedOfferPrefabs
                );
            }

            if (offerPrefab != null)
            {
                selectedOffersInstantiated.Add(OfferData.Create(offerPrefab, transform));
            }
        }

        if (selectedOffersInstantiated.Count == 0)
        {
            throw new Exception("Found no offers! Can't keep going");
        }

        return selectedOffersInstantiated;
    }

    private int SelectPoolIndex(int playerLevel)
    {
        int totalNumPlayerLevels = GameManager.XpNeededForLevelUpAtIndex.Count;
        int totalNumPools = offerPools.Length;
        playerLevel = Mathf.Clamp(playerLevel, 0, totalNumPlayerLevels - 1);

        // Calculate the relative level of the player
        float relativeLevel = (float)playerLevel / totalNumPlayerLevels;

        List<int> poolChances = new();

        // Distribute the chances across pools based on relative level
        for (int ndx = 0; ndx < totalNumPools; ndx++)
        {
            float poolPosition = (float)ndx / totalNumPools;

            // Adjust the chance based on how close the pool is to the player's relative level
            if (poolPosition <= relativeLevel)
            {
                int chance = (int)(100 * (1 - Mathf.Abs(relativeLevel - poolPosition)));
                poolChances.Add(chance);
            }
            else
            {
                poolChances.Add(0); // No chance for pools too far ahead
            }
        }

        // Convert the pool chances into a cumulative distribution
        for (int ndx = 1; ndx < poolChances.Count; ndx++)
        {
            poolChances[ndx] += poolChances[ndx - 1];
        }

        // Generate a random number
        int randomNumber = random.Next(poolChances[^1]);

        // Use the cumulative distribution to select the pool
        for (int ndx = 0; ndx < poolChances.Count; ndx++)
        {
            if (randomNumber < poolChances[ndx])
            {
                return ndx;
            }
        }

        Debug.LogWarningFormat("Was not able to select any pool for player level {0}", playerLevel);
        // Fallback in case no pool is selected (should not happen)
        return 0;
    }

    private const int MAX_ENTIRES_FOR_OFFER = 100;

    private OfferData SelectOfferFromPool(
        int poolIndex,
        EquilibriumManager.EquilibriumState equilibriumState,
        List<OfferData> offersAcquired,
        HashSet<OfferData> alreadySelectedOffers
    )
    {
        if (poolIndex >= offerPools.Length)
        {
            return null;
        }

        Debug.LogFormat(
            "Getting offer pool for pool {0} with ofer pools list of {1}",
            poolIndex,
            offerPools.Length
        );
        List<OfferData> pool = new List<OfferData>(offerPools[poolIndex]);
        List<OfferData> offerEntriesInRaffle = new();

        foreach (OfferData offer in pool)
        {
            if (!offer.PrerequisitesMet(offersAcquired) || alreadySelectedOffers.Contains(offer))
            {
                continue;
            }

            int numEntiresForThisOffer = Mathf.RoundToInt(
                CalculateWeight(offer.CorrespondingState, equilibriumState) * MAX_ENTIRES_FOR_OFFER
            );
            for (int i = 0; i < numEntiresForThisOffer; i++)
            {
                offerEntriesInRaffle.Add(offer);
            }
        }

        if (offerEntriesInRaffle.Count > 0)
        {
            int randomIndex = random.Next(offerEntriesInRaffle.Count);
            OfferData selectedOffer = offerEntriesInRaffle[randomIndex];
            alreadySelectedOffers.Add(selectedOffer); // Add the selected offer to the already selected list
            return selectedOffer;
        }

        return null;
    }

    private float CalculateWeight(
        EquilibriumManager.EquilibriumState offerState,
        EquilibriumManager.EquilibriumState currentState
    )
    {
        // Calculate weight based on the distance in the enum index
        int distance = Mathf.Abs(offerState - currentState);
        if (distance > Enum.GetValues(typeof(EquilibriumManager.EquilibriumState)).Length - 3)
        {
            return 0;
        }
        // The weight decreases as the distance increases
        // This is a simple linear calculation, can change if we want
        float weight = 1.0f / (distance + 1);
        return weight;
    }
}
