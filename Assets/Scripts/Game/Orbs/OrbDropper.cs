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
    const float MIN_PROBABILITY = 0.25f;
    const float MAX_PROBABILITY = 0.75f;

    public static bool ShouldDropFireOrb(DamageTaken damageTaken)
    {
        float fireProbability = damageTaken.FireDamage / damageTaken.TotalDamage();

        return Random.Range(0.0f, 1.0f)
            < Mathf.Clamp(fireProbability, MIN_PROBABILITY, MAX_PROBABILITY);
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
