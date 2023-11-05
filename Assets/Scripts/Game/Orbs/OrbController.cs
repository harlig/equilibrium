using UnityEngine;

public class OrbController : MonoBehaviour
{
    public float rotationSpeed = 100.0f; // Adjust this value to control the rotation speed.
    public float Xp { get; private set; }

    private void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().AddTorque(rotationSpeed * Time.deltaTime);
    }

    public static OrbController Create(OrbController prefab, OrbDropper orbDropper, float xp)
    {
        var createdOrb = Instantiate(prefab, orbDropper.transform);
        createdOrb.Xp = xp;
        return createdOrb;
    }
}
