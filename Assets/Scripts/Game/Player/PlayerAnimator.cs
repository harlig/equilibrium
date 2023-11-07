using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAnimator : MonoBehaviour
{
    public Sprite[] walkSprites; // Array of sprites for walking animation
    private int updatesSinceLastSpriteChange = 0;
    private readonly float animationSpeed = 3;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastPosition;
    private Sprite originalSprite;
    private MoveDirection moveDirection;
    private int currentSpriteIndex = 0;

    public static PlayerAnimator Create(PlayerAnimator animatorPrefab, PlayerController player)
    {
        var newAnimator = Instantiate(animatorPrefab, player.transform);
        newAnimator.spriteRenderer = player.GetComponent<SpriteRenderer>();
        newAnimator.lastPosition = player.transform.position;
        newAnimator.originalSprite = newAnimator.spriteRenderer.sprite;
        return newAnimator;
    }

    // TODO: this should be `Update` but it won't work for some reason
    void FixedUpdate()
    {
        Debug.Log("update getting called");
        Vector2 currentPosition = transform.position;

        // Check if the player is moving
        if (currentPosition != lastPosition)
        {
            Debug.Log("new location");
            AnimateWalk();
        }
        else
        {
            // If the player is not moving, set a default sprite (e.g., standing still)
            spriteRenderer.sprite = originalSprite;
        }

        lastPosition = currentPosition;
    }

    void AnimateWalk()
    {
        updatesSinceLastSpriteChange++;

        if (updatesSinceLastSpriteChange >= animationSpeed)
        {
            currentSpriteIndex = (currentSpriteIndex + 1) % walkSprites.Length;
            spriteRenderer.sprite = walkSprites[currentSpriteIndex];
            updatesSinceLastSpriteChange = 0;
        }
    }

    public void SetMoveDirection(MoveDirection moveDirection)
    {
        if (this.moveDirection != moveDirection)
        {
            this.moveDirection = moveDirection;
            currentSpriteIndex = 0;
        }
    }
}
