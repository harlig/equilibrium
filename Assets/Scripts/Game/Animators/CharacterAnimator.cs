using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterAnimator : MonoBehaviour
{
    // TODO: we probably need to define generic sets of sprite "groups" like "idle group", "moving group", "attacking group", and then each of those should have directional aspects (E/N/S/W)
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

    [SerializeField]
    private Sprite[] hurtSpritesEast;

    [SerializeField]
    private Sprite[] hurtSpritesNorth;

    [SerializeField]
    private Sprite[] hurtSpritesSouth;

    [SerializeField]
    private Sprite[] hurtSpritesWest;

    [SerializeField]
    private Sprite deathSprite;

    private int updatesSinceLastSpriteChange = 0;

    [SerializeField]
    private float animationSpeed = 3;
    private GenericCharacterController character;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastPosition;
    private MoveDirection? moveDirection = null;
    private int currentSpriteIndex = 0;
    private bool animatingHurt = false;
    private float oldAnimationSpeed;

    void Awake()
    {
        character = GetComponent<GenericCharacterController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastPosition = transform.position;
    }

    // TODO: I think this should be `Update` but it won't work for some reason
    void FixedUpdate()
    {
        if (character.IsDead())
        {
            spriteRenderer.sprite = deathSprite;
            return;
        }

        Vector2 currentPosition = transform.position;

        Sprite[] animationSprites;
        if (animatingHurt)
        {
            animationSprites = GetHurtAnimationSprites();
        }
        // Check if the character is moving
        else if (currentPosition != lastPosition)
        {
            SetMoveDirection(currentPosition, lastPosition);
            animationSprites = GetWalkAnimationSprites();
        }
        else
        {
            animationSprites = GetIdleAnimationSprites();
        }

        if (updatesSinceLastSpriteChange >= animationSpeed)
        {
            if (animatingHurt && currentSpriteIndex == animationSprites.Length - 1)
            {
                animatingHurt = false;
                animationSpeed = oldAnimationSpeed;
            }
            currentSpriteIndex = (currentSpriteIndex + 1) % animationSprites.Length;
            spriteRenderer.sprite = animationSprites[currentSpriteIndex];
            updatesSinceLastSpriteChange = 0;
        }
        updatesSinceLastSpriteChange++;

        lastPosition = currentPosition;
    }

    Sprite[] GetWalkAnimationSprites()
    {
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
            // unset, just use south
            case null:
                walkAnimationArray = walkAnimationSpritesSouth;
                break;
            default:
                Debug.LogErrorFormat(
                    "Unhandled move direction for walk animation: {0}",
                    moveDirection
                );
                break;
        }

        return walkAnimationArray;
    }

    Sprite[] GetIdleAnimationSprites()
    {
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
        return idleAnimationArray;
    }

    public void AnimateHurt()
    {
        animatingHurt = true;
        oldAnimationSpeed = animationSpeed;
        animationSpeed = 2;
        currentSpriteIndex = 0;
    }

    private Sprite[] GetHurtAnimationSprites()
    {
        Sprite[] hurtAnimationSprites = null;
        switch (moveDirection)
        {
            case MoveDirection.Down:
                hurtAnimationSprites = hurtSpritesSouth;
                break;
            case MoveDirection.Left:
                hurtAnimationSprites = hurtSpritesWest;
                break;
            case MoveDirection.Up:
                hurtAnimationSprites = hurtSpritesNorth;
                break;
            case MoveDirection.Right:
                hurtAnimationSprites = hurtSpritesEast;
                break;
            // unset, just use south
            case null:
                hurtAnimationSprites = hurtSpritesSouth;
                break;
            default:
                Debug.LogErrorFormat(
                    "Unhandled move direction for idle animation {0}",
                    moveDirection
                );
                break;
        }
        return hurtAnimationSprites;
    }

    public void SetMoveDirection(Vector2 currentPosition, Vector2 lastPosition)
    {
        // Calculate absolute differences in both x and y directions
        float xDifference = Mathf.Abs(currentPosition.x - lastPosition.x);
        float yDifference = Mathf.Abs(currentPosition.y - lastPosition.y);

        if (xDifference > yDifference)
        {
            // Movement is primarily horizontal
            moveDirection =
                currentPosition.x > lastPosition.x ? MoveDirection.Right : MoveDirection.Left;
        }
        else
        {
            // Movement is primarily vertical
            moveDirection =
                currentPosition.y > lastPosition.y ? MoveDirection.Up : MoveDirection.Down;
        }
    }
}
