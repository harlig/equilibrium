using TMPro;
using UnityEngine;

public class DamageTaken
{
    public float FireDamage { get; set; }
    public float IceDamage { get; set; }

    public float TotalDamage()
    {
        return FireDamage + IceDamage;
    }

    // TODO: this shouldn't be static I think
    public static void SetDamageTakenTextOnTextElement(
        float maxHp,
        DamageTaken damageTaken,
        TextMeshPro textElement
    )
    {
        // format to two decimal places
        textElement.text = string.Format("{0:N1}", maxHp - damageTaken.TotalDamage());
    }
}

public class OrbDropper : MonoBehaviour
{
    [SerializeField]
    private OrbController fireOrbPrefab;

    [SerializeField]
    private OrbController iceOrbPrefab;
    const float MIN_PROBABILITY = 0.25f;
    const float MAX_PROBABILITY = 0.75f;

    public float scatterRange = 1.0f;

    private static bool ShouldDropFireOrb(DamageTaken damageTaken)
    {
        float fireProbability = damageTaken.FireDamage / damageTaken.TotalDamage();

        return Random.Range(0.0f, 1.0f)
            < Mathf.Clamp(fireProbability, MIN_PROBABILITY, MAX_PROBABILITY);
    }

    public void DoOrbDrop(DamageTaken damageTaken, float totalXp, int desiredNumToDrop = 10)
    {
        var shouldDropFireOrb = ShouldDropFireOrb(damageTaken);

        int minNumToDrop = Mathf.Max(Mathf.FloorToInt(desiredNumToDrop * 0.8f), 1); // 80% of the desired to drop but never below 1
        int maxNumToDrop = Mathf.FloorToInt(desiredNumToDrop * 1.2f); // 120% of the desired to drop

        int numToDrop = Random.Range(minNumToDrop, maxNumToDrop + 1);

        int baseXp = (int)(totalXp / numToDrop);
        int remainingXp = (int)(totalXp % numToDrop);

        for (int ndx = 0; ndx < numToDrop; ndx++)
        {
            int xp = baseXp;
            if (remainingXp > 0)
            {
                xp++;
                remainingXp--;
            }

            Vector2? scatter = null;

            // Apply scattering only when there's more than one item to drop
            if (numToDrop > 1)
            {
                // Calculate random offsets for the X and Y positions
                float xOffset = Random.Range(-scatterRange, scatterRange);
                float yOffset = Random.Range(-scatterRange, scatterRange);

                scatter = new Vector2(xOffset, yOffset);
            }

            if (shouldDropFireOrb)
            {
                OrbController.Create(fireOrbPrefab, this, OrbController.OrbType.FIRE, xp, scatter);
            }
            else
            {
                OrbController.Create(iceOrbPrefab, this, OrbController.OrbType.ICE, xp, scatter);
            }
        }
    }
}
