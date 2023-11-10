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

    public static OfferSystem Create(OfferSystem prefab)
    {
        return Instantiate(prefab);
    }

    private void Awake()
    {
        HashSet<int> numPools = new();
        foreach (OfferData offer in allOfferPrefabs)
        {
            Debug.Log($"offer {offer.OfferPool}");
            if (!numPools.Contains(offer.OfferPool))
            {
                numPools.Add(offer.OfferPool);
            }
        }

        Debug.Log($"num pool {numPools.Count}");

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
                throw new System.Exception(
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
        EquilibriumManager.EquilibriumState currentEquilibriumState
    )
    {
        List<OfferData> selectedOffers = new();

        for (int ndx = 0; ndx < numOffersToRetrieve; ndx++)
        {
            int poolIndex = SelectPoolIndex(playerLevel);
            OfferData offerPrefab = SelectOfferFromPool(poolIndex, currentEquilibriumState);

            // if we can't get an offer from this pool, try from the pool below
            while (offerPrefab == null && poolIndex >= 0)
            {
                poolIndex--;
                offerPrefab = SelectOfferFromPool(poolIndex, currentEquilibriumState);
            }

            if (offerPrefab != null)
            {
                selectedOffers.Add(OfferData.Create(offerPrefab));
            }
        }

        return selectedOffers;
    }

    private int SelectPoolIndex(int playerLevel)
    {
        // Determine the number of levels from the GameManager
        int totalNumPlayerLevels = GameManager.XpNeededForLevelUpAtIndex.Count;

        // Ensure the playerLevel is within bounds
        playerLevel = Mathf.Clamp(playerLevel, 0, totalNumPlayerLevels - 1);

        // Generate a random number between 0 and 99
        int randomNumber = random.Next(100);

        // This list will hold the cumulative probability for each pool
        List<int> poolChances = new();

        // Calculate the distribution of chances based on player level
        // The following is a simple distribution logic for demonstration purposes:
        // The chance for the current level pool is (100 - playerLevel * 10)%
        // Each lower level pool has a 10% chance
        for (int ndx = 0; ndx < totalNumPlayerLevels; ndx++)
        {
            if (ndx == playerLevel)
            {
                poolChances.Add(100 - playerLevel * 10); // Chance for the current level pool
            }
            else if (ndx < playerLevel)
            {
                poolChances.Add(10); // Flat chance for each lower level pool
            }
            else
            {
                poolChances.Add(0); // No chance for higher level pools
            }
        }

        // Convert the pool chances into a cumulative distribution
        for (int ndx = 1; ndx < poolChances.Count; ndx++)
        {
            poolChances[ndx] += poolChances[ndx - 1];
        }

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
        EquilibriumManager.EquilibriumState equilibriumState
    )
    {
        if (poolIndex >= offerPools.Length)
        {
            return null;
        }

        List<OfferData> pool = offerPools[poolIndex];
        List<OfferData> offerEntriesInRaffle = new();

        foreach (OfferData offer in pool)
        {
            // each offer gets somewhere between 0 and MAX_ENTRIES_FOR_OFFER entries in the raffle
            int numEntiresForThisOffer = Mathf.RoundToInt(
                CalculateWeight(offer.CorrespondingState, equilibriumState) * MAX_ENTIRES_FOR_OFFER
            );
            Debug.LogFormat("Offer {0}; weight {1}", offer, numEntiresForThisOffer);
            for (int i = 0; i < numEntiresForThisOffer; i++)
            {
                offerEntriesInRaffle.Add(offer);
            }
        }

        // do the raffle for an offer
        if (offerEntriesInRaffle.Count > 0)
        {
            int randomIndex = random.Next(offerEntriesInRaffle.Count);
            return offerEntriesInRaffle[randomIndex];
        }

        Debug.LogWarningFormat(
            "Was not able to select any offer from pool {0} with equilibriumState {1}",
            poolIndex,
            equilibriumState
        );
        return null;
    }

    private float CalculateWeight(
        EquilibriumManager.EquilibriumState offerState,
        EquilibriumManager.EquilibriumState currentState
    )
    {
        // Calculate weight based on the distance in the enum index
        int distance = Mathf.Abs(offerState - currentState);
        // The weight decreases as the distance increases
        // This is a simple linear calculation, can change if we want
        float weight = 1.0f / (distance + 1);
        Debug.LogFormat(
            "offerState {0}; currentState {1}; distance {2}; weight {3}",
            offerState,
            currentState,
            distance,
            weight
        );
        return weight;
    }
}
