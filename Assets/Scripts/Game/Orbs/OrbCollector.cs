using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OrbCollector
{
    public Dictionary<OrbController.OrbType, int> OrbsCollected { get; } = new();
    public float XpCollected { get; private set; } = 0;

    public OrbCollector(OrbController.OrbType[] orbsToSupport)
    {
        foreach (OrbController.OrbType type in orbsToSupport)
        {
            OrbsCollected[type] = 0;
        }
    }

    public void Collect(OrbController orb)
    {
        if (!OrbsCollected.ContainsKey(orb.Type))
        {
            Debug.LogError(
                $"Cannot collect orb! This orb collector is not configured to collect orbs of type: {orb.Type}"
            );
            return;
        }
        OrbsCollected[orb.Type] = OrbsCollected[orb.Type] + 1;
        XpCollected += orb.Xp;

        // destroy when we're done collecting
        Object.Destroy(orb.gameObject);
    }

    public int NumOrbsCollectedForType(OrbController.OrbType orbType)
    {
        if (!OrbsCollected.ContainsKey(orbType))
        {
            Debug.LogError(
                $"Cannot get number of orbs collected! This orb collector is not configured to collect orbs of type: {orbType}"
            );
            return -1;
        }

        return OrbsCollected[orbType];
    }
}
