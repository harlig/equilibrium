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
        textElement.text = string.Format("{0:N2}", maxHp - damageTaken.TotalDamage());
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
        float totalDamage = damageTaken.FireDamage + damageTaken.IceDamage;

        float fireProbability = damageTaken.FireDamage / totalDamage;

        // Ensure the probability is never higher than 75% or less than 25%
        float minProbability = 0.25f;
        float maxProbability = 0.75f;

        if (fireProbability > maxProbability)
            return false;
        else if (fireProbability < minProbability)
            return true;
        else
            return Random.Range(0.0f, 1.0f) < fireProbability;
    }

    public void DropFireOrb(float xp)
    {
        var iceOrb = OrbController.Create(iceOrbPrefab, transform.position, xp);
    }

    public void DropIceOrb(float xp)
    {
        var iceOrb = OrbController.Create(iceOrbPrefab, transform.position, xp);
    }
}
