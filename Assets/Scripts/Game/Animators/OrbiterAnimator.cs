using UnityEngine;

public class OrbiterAnimator : MonoBehaviour
{
    [SerializeField]
    private Sprite[] orbiterAnimationArray;

    private int updatesSinceLastSpriteChange = 0;

    private readonly float animationSpeed = 6;

    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        AnimateOrbiter();
    }

    void AnimateOrbiter()
    {
        updatesSinceLastSpriteChange++;
        if (updatesSinceLastSpriteChange >= animationSpeed)
        {
            currentSpriteIndex = (currentSpriteIndex + 1) % orbiterAnimationArray.Length;
            spriteRenderer.sprite = orbiterAnimationArray[currentSpriteIndex];
            updatesSinceLastSpriteChange = 0;
        }
    }
}
