using UnityEngine;

public class OrbController : MonoBehaviour
{
    public const float RotationSpeed = 100.0f; // Adjust this value to control the rotation speed.
    public float Xp { get; private set; }

    public static OrbController Create(OrbController prefab, OrbDropper orbDropper, float xp)
    {
        var createdOrb = Instantiate(prefab, orbDropper.transform);
        createdOrb.Xp = xp;
        createdOrb.GetComponent<Rigidbody2D>().angularVelocity = RotationSpeed;
        return createdOrb;
    }
}
