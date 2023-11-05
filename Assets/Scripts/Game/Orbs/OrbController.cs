using UnityEngine;

public class OrbController : MonoBehaviour
{
    public enum OrbType
    {
        FIRE,
        ICE
    }

    public const float RotationSpeed = 100.0f; // Adjust this value to control the rotation speed.
    public OrbType Type { get; private set; }
    public float Xp { get; private set; }

    public static OrbController Create(
        OrbController prefab,
        OrbDropper orbDropper,
        OrbType type,
        float xp
    )
    {
        var createdOrb = Instantiate(prefab, orbDropper.transform);
        createdOrb.Type = type;
        createdOrb.Xp = xp;
        createdOrb.GetComponent<Rigidbody2D>().angularVelocity = RotationSpeed;
        return createdOrb;
    }
}
