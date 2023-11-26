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

    [SerializeField]
    private Sprite[] orbAnimationArray;

    private int updatesSinceLastSpriteChange = 0;

    private readonly float animationSpeed = 3;

    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        AnimateOrb();
    }

    void AnimateOrb()
    {
        updatesSinceLastSpriteChange++;
        if (updatesSinceLastSpriteChange >= animationSpeed)
        {
            currentSpriteIndex = (currentSpriteIndex + 1) % orbAnimationArray.Length;
            spriteRenderer.sprite = orbAnimationArray[currentSpriteIndex];
            updatesSinceLastSpriteChange = 0;
        }
    }

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

        return createdOrb;
    }
}
