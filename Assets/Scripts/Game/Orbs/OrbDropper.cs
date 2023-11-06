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

    // TODO should this be static?
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

    public static bool ShouldDropFireOrb(DamageTaken damageTaken)
    {
        // TODO can replace this with Mathf.Clamp
        float fireProbability = damageTaken.FireDamage / damageTaken.TotalDamage();

        // Ensure the probability is never higher than 75% or less than 25%
        float minProbability = 0.25f;
        float maxProbability = 0.75f;

        if (fireProbability > maxProbability)
            fireProbability = maxProbability;
        else if (fireProbability < minProbability)
            fireProbability = minProbability;

        return Random.Range(0.0f, 1.0f) < fireProbability;
    }

    public void DropFireOrb(float xp)
    {
        OrbController.Create(fireOrbPrefab, this, OrbController.OrbType.FIRE, xp);
    }

    public void DropIceOrb(float xp)
    {
        OrbController.Create(iceOrbPrefab, this, OrbController.OrbType.ICE, xp);
    }
}
