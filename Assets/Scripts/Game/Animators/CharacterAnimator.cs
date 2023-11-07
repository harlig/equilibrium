using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterAnimator : MonoBehaviour
// TODO we should have an AbstractAnimator
{
    [SerializeField]
    private Sprite idleSouthFacingSprite;

    [SerializeField]
    private Sprite idleWestFacingSprite;

    [SerializeField]
    private Sprite idleNorthFacingSprite;

    [SerializeField]
    private Sprite idleEastFacingSprite;
    public Sprite[] walkSprites; // Array of sprites for walking animation
    private int updatesSinceLastSpriteChange = 0;

    // TODO: this should be based on CharacterController.MovementSpeed
    [SerializeField]
    private float animationSpeed = 3;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastPosition;
    private MoveDirection? moveDirection = null;
    private int currentSpriteIndex = 0;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastPosition = transform.position;
    }

    // TODO: I think this should be `Update` but it won't work for some reason
    void FixedUpdate()
    {
        Vector2 currentPosition = transform.position;

        // Check if the character is moving
        if (!currentPosition.Equals(lastPosition))
        {
            AnimateWalk();
        }
        else
        {
            // If the character is not moving, set a default sprite (e.g., standing still)
            switch (moveDirection)
            {
                case MoveDirection.Down:
                    spriteRenderer.sprite = idleSouthFacingSprite;
                    break;
                case MoveDirection.Left:
                    spriteRenderer.sprite = idleWestFacingSprite;
                    break;
                case MoveDirection.Up:
                    spriteRenderer.sprite = idleNorthFacingSprite;
                    break;
                case MoveDirection.Right:
                    spriteRenderer.sprite = idleEastFacingSprite;
                    break;
                // unset, just use south
                case null:
                    spriteRenderer.sprite = idleSouthFacingSprite;
                    break;

                default:
                    Debug.LogErrorFormat("Unhandled move direction {0}", moveDirection);
                    spriteRenderer.sprite = idleSouthFacingSprite;
                    break;
            }
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
