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

    private static bool ShouldDropFireOrb(DamageTaken damageTaken)
    {
        float fireProbability = damageTaken.FireDamage / damageTaken.TotalDamage();

        return Random.Range(0.0f, 1.0f)
            < Mathf.Clamp(fireProbability, MIN_PROBABILITY, MAX_PROBABILITY);
    }

    public void DoOrbDrop(DamageTaken damageTaken, float totalXp, int numToDrop = 10)
    {
        var shouldDropFireOrb = ShouldDropFireOrb(damageTaken);

        int baseXp = (int)(totalXp / numToDrop);
        int remainingXp = (int)(totalXp % numToDrop);

        // TODO: if there are more than one to drop, we need to scatter them a little
        for (int ndx = 0; ndx < numToDrop; ndx++)
        {
            int xp = baseXp;
            if (remainingXp > 0)
            {
                xp++;
                remainingXp--;
            }

            if (shouldDropFireOrb)
            {
                OrbController.Create(fireOrbPrefab, this, OrbController.OrbType.FIRE, xp);
            }
            else
            {
                OrbController.Create(iceOrbPrefab, this, OrbController.OrbType.ICE, xp);
            }
        }
    }
}
