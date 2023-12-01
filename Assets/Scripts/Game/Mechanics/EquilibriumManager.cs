using System;
using System.Collections.Generic;
using UnityEngine;

public class EquilibriumManager
{
    // THIS IS ORDER DEPENDENT!!!!!!
    public enum EquilibriumState
    {
        FROZEN,
        COLD,
        BRISK,
        NEUTRAL,
        WARM,
        HOT,
        INFERNO
    }

    public static EquilibriumState DefaultState()
    {
        return EquilibriumState.NEUTRAL;
    }

    static readonly Dictionary<
        (float, float),
        EquilibriumState
    > percentageTotalOrbsAreFireOrbsToEquilibriumStateMapping =
        GeneratePercentageTotalOrbsAreFireOrbsToEquilibriumStateMapping();

    private const float NUM_ORBS_FOR_UNLOCK_HOT = 50;
    private const float NUM_ORBS_FOR_UNLOCK_INFERNO = 150;

    private static Dictionary<
        (float, float),
        EquilibriumState
    > GeneratePercentageTotalOrbsAreFireOrbsToEquilibriumStateMapping()
    {
        Dictionary<(float, float), EquilibriumState> mapping =
            new()
            {
                { (0.0f, 30.0f), EquilibriumState.FROZEN },
                { (30.0f, 38.0f), EquilibriumState.COLD },
                { (38.0f, 45.0f), EquilibriumState.BRISK },
                { (45.0f, 55.0f), EquilibriumState.NEUTRAL },
                { (55.0f, 62.0f), EquilibriumState.WARM },
                { (62.0f, 70.0f), EquilibriumState.HOT },
                { (70.0f, 100.0f), EquilibriumState.INFERNO }
            };

        return mapping;
    }

    public static EquilibriumState ManageEquilibrium(OrbCollector orbCollector)
    {
        var percFireOrbs = orbCollector.PercTypeOrbsCollectedOfTotal(OrbController.OrbType.FIRE);
        var totalOrbs = orbCollector.TotalOrbsCollected();
        if (percFireOrbs is null)
        {
            return EquilibriumState.NEUTRAL;
        }

        foreach (var rangeMapping in percentageTotalOrbsAreFireOrbsToEquilibriumStateMapping)
        {
            var range = rangeMapping.Key;
            if (percFireOrbs >= range.Item1 && percFireOrbs <= range.Item2)
            {
                if (totalOrbs < NUM_ORBS_FOR_UNLOCK_HOT)
                {
                    if (rangeMapping.Value > EquilibriumState.WARM)
                    {
                        return EquilibriumState.WARM;
                    }
                    if (rangeMapping.Value < EquilibriumState.BRISK)
                    {
                        return EquilibriumState.BRISK;
                    }
                }

                if (totalOrbs < NUM_ORBS_FOR_UNLOCK_INFERNO)
                {
                    if (rangeMapping.Value > EquilibriumState.HOT)
                    {
                        return EquilibriumState.HOT;
                    }
                    if (rangeMapping.Value < EquilibriumState.COLD)
                    {
                        return EquilibriumState.COLD;
                    }
                }
                return rangeMapping.Value;
            }
        }

        throw new Exception(
            $"No mapping for this percentage of fire orbs {percFireOrbs}. Mapping is {percentageTotalOrbsAreFireOrbsToEquilibriumStateMapping}"
        );
    }
}
