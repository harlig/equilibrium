using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OrbCollector
{
    public Dictionary<OrbController.OrbType, int> OrbsCollected { get; } = new();
    private Dictionary<OrbController.OrbType, TextMeshProUGUI> orbTypeToTextElements;
    public float XpCollected { get; private set; } = 0;

    public OrbCollector(Dictionary<OrbController.OrbType, TextMeshProUGUI> orbTypesAndTextElements)
    {
        orbTypeToTextElements = orbTypesAndTextElements;
        foreach (OrbController.OrbType type in orbTypesAndTextElements.Keys)
        {
            OrbsCollected[type] = 0;
            SetTextForType(type);
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

        foreach (OrbController.OrbType type in orbTypeToTextElements.Keys)
        {
            SetTextForType(type);
        }

        // destroy when we're done collecting
        Object.Destroy(orb.gameObject);
    }

    private void SetTextForType(OrbController.OrbType type)
    {
        orbTypeToTextElements[type].text = $"{OrbsCollected[type]} {type} collected";
    }
}
