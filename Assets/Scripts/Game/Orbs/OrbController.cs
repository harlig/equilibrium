using UnityEngine;

public class OrbController : MonoBehaviour
{
    public float Xp { get; private set; }

    public static OrbController Create(OrbController prefab, Vector2 position, float xp)
    {
        var createdOrb = Instantiate(prefab, position, Quaternion.identity);
        createdOrb.Xp = xp;
        return createdOrb;
    }
}
