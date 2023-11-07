using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterAnimator : MonoBehaviour
// TODO we should have an AbstractAnimator
{
    [SerializeField]
    private Sprite idleSpriteEast;

    [SerializeField]
    private Sprite idleSpriteNorth;

    [SerializeField]
    private Sprite idleSpriteSouth;

    [SerializeField]
    private Sprite idleSpriteWest;

    [SerializeField]
    private Sprite[] walkAnimationSpritesEast; // Array of sprites for walking animation

    [SerializeField]
    private Sprite[] walkAnimationSpritesNorth; // Array of sprites for walking animation

    [SerializeField]
    private Sprite[] walkAnimationSpritesSouth; // Array of sprites for walking animation

    [SerializeField]
    private Sprite[] walkAnimationSpritesWest; // Array of sprites for walking animation
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
                    spriteRenderer.sprite = idleSpriteSouth;
                    break;
                case MoveDirection.Left:
                    spriteRenderer.sprite = idleSpriteWest;
                    break;
                case MoveDirection.Up:
                    spriteRenderer.sprite = idleSpriteNorth;
                    break;
                case MoveDirection.Right:
                    spriteRenderer.sprite = idleSpriteEast;
                    break;
                // unset, just use south
                case null:
                    spriteRenderer.sprite = idleSpriteSouth;
                    break;

                default:
                    Debug.LogErrorFormat("Unhandled move direction {0}", moveDirection);
                    spriteRenderer.sprite = idleSpriteSouth;
                    break;
            }
        }

        lastPosition = currentPosition;
    }

    void AnimateWalk()
    {
        updatesSinceLastSpriteChange++;

        Sprite[] walkAnimationArray = null;
        switch (moveDirection)
        {
            case MoveDirection.Down:
                walkAnimationArray = walkAnimationSpritesSouth;
                break;
            case MoveDirection.Left:
                walkAnimationArray = walkAnimationSpritesWest;
                break;
            case MoveDirection.Up:
                walkAnimationArray = walkAnimationSpritesNorth;
                break;
            case MoveDirection.Right:
                walkAnimationArray = walkAnimationSpritesEast;
                break;
            default:
                Debug.LogErrorFormat(
                    "Unhandled move direction for walk animation {0}",
                    moveDirection
                );
                break;
        }

        if (updatesSinceLastSpriteChange >= animationSpeed)
        {
            currentSpriteIndex = (currentSpriteIndex + 1) % walkAnimationArray.Length;
            spriteRenderer.sprite = walkAnimationArray[currentSpriteIndex];
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
