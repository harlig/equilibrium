using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterAnimator : MonoBehaviour
{
    // TODO we probably need to define generic sets of sprite "groups" like "idle group", "moving group", "attacking group", and then each of those should have directional aspects (E/N/S/W)
    [SerializeField]
    private Sprite idleSpriteEast;

    [SerializeField]
    private Sprite idleSpriteNorth;

    [SerializeField]
    private Sprite idleSpriteSouth;

    [SerializeField]
    private Sprite idleSpriteWest;

    [SerializeField]
    private Sprite[] walkAnimationSpritesEast;

    [SerializeField]
    private Sprite[] walkAnimationSpritesNorth;

    [SerializeField]
    private Sprite[] walkAnimationSpritesSouth;

    [SerializeField]
    private Sprite[] walkAnimationSpritesWest;

    [SerializeField]
    private Sprite[] idleAnimationSpritesEast;

    [SerializeField]
    private Sprite[] idleAnimationSpritesNorth;

    [SerializeField]
    private Sprite[] idleAnimationSpritesSouth;

    [SerializeField]
    private Sprite[] idleAnimationSpritesWest;

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
            AnimateIdle();
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
                    "Unhandled move direction for walk animation: {0}",
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

    void AnimateIdle()
    {
        updatesSinceLastSpriteChange++;

        Sprite[] idleAnimationArray = null;
        switch (moveDirection)
        {
            case MoveDirection.Down:
                idleAnimationArray = idleAnimationSpritesSouth;
                break;
            case MoveDirection.Left:
                idleAnimationArray = idleAnimationSpritesWest;
                break;
            case MoveDirection.Up:
                idleAnimationArray = idleAnimationSpritesNorth;
                break;
            case MoveDirection.Right:
                idleAnimationArray = idleAnimationSpritesEast;
                break;
            // unset, just use south
            case null:
                idleAnimationArray = idleAnimationSpritesSouth;
                break;
            default:
                Debug.LogErrorFormat(
                    "Unhandled move direction for idle animation {0}",
                    moveDirection
                );
                break;
        }

        if (updatesSinceLastSpriteChange >= animationSpeed)
        {
            currentSpriteIndex = (currentSpriteIndex + 1) % idleAnimationArray.Length;
            spriteRenderer.sprite = idleAnimationArray[currentSpriteIndex];
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
