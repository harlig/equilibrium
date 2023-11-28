using UnityEngine;

public class ProjectileAnimator : MonoBehaviour
{
    [SerializeField]
    private Sprite[] projectileAnimationArray;

    private int updatesSinceLastSpriteChange = 0;

    private readonly float animationSpeed = 6;

    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        AnimateProjectile();
    }

    void AnimateProjectile()
    {
        updatesSinceLastSpriteChange++;
        if (updatesSinceLastSpriteChange >= animationSpeed)
        {
            currentSpriteIndex = (currentSpriteIndex + 1) % projectileAnimationArray.Length;
            spriteRenderer.sprite = projectileAnimationArray[currentSpriteIndex];
            updatesSinceLastSpriteChange = 0;
        }
    }
}
