using UnityEngine;

public class OrbController : MonoBehaviour
{
    public enum OrbType
    {
        FIRE,
        ICE
    }

    public const float RotationSpeed = 150.0f; // Adjust this value to control the rotation speed.
    public OrbType Type { get; private set; }
    public float Xp { get; private set; }

    public static OrbController Create(
        OrbController prefab,
        OrbDropper orbDropper,
        OrbType type,
        float xp,
        Vector2? scatterOffset = null
    )
    {
        var createdOrb = Instantiate(prefab, orbDropper.transform);
        createdOrb.Type = type;
        createdOrb.Xp = xp;

        // scatter orb around its position
        if (scatterOffset != null)
        {
            Vector3 offset = scatterOffset ?? Vector3.zero;
            createdOrb.transform.position += offset;
        }

        // apply random starting rotation
        Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        createdOrb.transform.rotation = randomRotation;

        float randomAngularVelocity = Random.Range(-RotationSpeed, RotationSpeed);
        // determine the direction of rotation (either clockwise or counterclockwise)
        if (Random.value > 0.5f)
        {
            randomAngularVelocity = Mathf.Abs(randomAngularVelocity);
        }
        else
        {
            randomAngularVelocity = -Mathf.Abs(randomAngularVelocity);
        }

        createdOrb.GetComponent<Rigidbody2D>().angularVelocity = randomAngularVelocity;
        return createdOrb;
    }
}
