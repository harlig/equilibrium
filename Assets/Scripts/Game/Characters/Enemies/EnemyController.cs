using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class EnemyController : CharacterController
{
    private enum MoveDirection
    {
        TOWARDS_START_POSITION,
        TOWARDS_PATROL_POSITION
    }

    [SerializeField]
    private TextMeshPro hpTextElement;

    [SerializeField]
    private OrbDropper orbDropper;

    private RoomManager containingRoom;

    // TODO: use this
    // [SerializeField]
    // private Transform launchOffset;

    private List<Node> path;
    private int currentPathIndex;

    protected virtual int GetMaxHp()
    {
        return 10;
    }

    protected PlayerController player;

    private bool startFollowing = false;
    private readonly DamageTaken damageTaken = new();

    public override float MaxHp
    {
        get { return GetMaxHp(); }
    }

    protected override float BaseMovementSpeed => 0.03f;

    public override float HpRemaining
    {
        get { return GetMaxHp() - damageTaken.TotalDamage(); }
    }

    void Awake()
    {
        containingRoom = GetComponentInParent<RoomManager>();
        DamageTaken.SetDamageTakenTextOnTextElement(GetMaxHp(), damageTaken, hpTextElement);
        lastPosition = transform.position;
    }

    public static EnemyController Create(
        EnemyController prefab,
        Vector2 position,
        PlayerController player,
        Transform parent = null
    )
    {
        EnemyController createdEnemy;
        if (parent != null)
        {
            createdEnemy = Instantiate(prefab, parent);
        }
        else
        {
            createdEnemy = Instantiate(prefab);
        }
        // is this okay? I want it to be relative to its parent's position so I think yes?
        createdEnemy.transform.localPosition = position;
        createdEnemy.player = player;
        // instantiating this directly bc no need to pass additional data to it
        createdEnemy.orbDropper = Instantiate(prefab.orbDropper, createdEnemy.transform);
        return createdEnemy;
    }

    private float pathUpdateInterval = 0.5f; // Time in seconds between path updates
    private float pathUpdateTimer;
    private Vector3 lastPosition;
    bool tryUnstuck = false;
    UnstuckAttempt currentUnstuckAttempt;
    private float stuckThreshold = 0.05f; // Threshold to determine if stuck
    private float stuckTime = 0f; // Time since the enemy is stuck

    enum UnstuckAttempt
    {
        NOT_SET,
        POSITIVE_X,
        POSITIVE_Y,
        NEGATIVE_X,
        NEGATIVE_Y,
    }

    static Vector2 SetDirectionForUnstuckAttempt(UnstuckAttempt unstuckAttempt, Vector2 direction)
    {
        Vector2 modifiedDirection = direction;
        switch (unstuckAttempt)
        {
            case UnstuckAttempt.POSITIVE_X:
                Debug.Log("trying positive unstuck in X");
                modifiedDirection.y = 0;
                break;
            case UnstuckAttempt.POSITIVE_Y:
                Debug.Log("trying positive unstuck in Y");
                modifiedDirection.x = 0;
                break;
            case UnstuckAttempt.NEGATIVE_X:
                Debug.Log("trying negative unstuck in X");
                modifiedDirection.y = 0;
                modifiedDirection.x = -modifiedDirection.x;
                break;
            case UnstuckAttempt.NEGATIVE_Y:
                Debug.Log("trying negative unstuck in Y");
                modifiedDirection.x = 0;
                modifiedDirection.y = -modifiedDirection.y;
                break;
            case UnstuckAttempt.NOT_SET:
                return SetDirectionForUnstuckAttempt(UnstuckAttempt.POSITIVE_X, direction);
            default:
                Debug.LogErrorFormat("Unhandled unstuck attempt {0}", unstuckAttempt);
                break;
        }
        return modifiedDirection;
    }

    void FixedUpdate()
    {
        if (IsDead())
        {
            return;
        }

        if (startFollowing && !player.IsDead())
        {
            pathUpdateTimer += Time.fixedDeltaTime;
            if (
                pathUpdateTimer >= pathUpdateInterval
                || (path != null && currentPathIndex == path.Count)
            )
            {
                CalculatePath();
                pathUpdateTimer = 0;
            }

            if (path != null && currentPathIndex < path.Count)
            {
                var rigidBody = gameObject.GetComponent<Rigidbody2D>();
                Node nextNode = path[currentPathIndex];

                Vector2 nextPosition = new(nextNode.WorldX, nextNode.WorldY);
                Vector2 direction = (nextPosition - rigidBody.position).normalized;

                // TODO: what did I break here?
                if (tryUnstuck)
                {
                    direction = SetDirectionForUnstuckAttempt(currentUnstuckAttempt, direction);
                    currentUnstuckAttempt = (UnstuckAttempt)(
                        ((int)currentUnstuckAttempt + 1)
                        % System.Enum.GetNames(typeof(UnstuckAttempt)).Length
                    );
                }

                // Calculate distance to move this frame
                // TODO: wtf is up with this move speed
                float step = 80 * MovementSpeed * Time.fixedDeltaTime;
                float distanceToNextNode = Vector2.Distance(rigidBody.position, nextPosition);

                // Move only as far as or closer to the next node
                Vector2 newPosition = rigidBody.position + direction * step;

                rigidBody.MovePosition(newPosition);

                // Inside FixedUpdate
                if (Vector2.Distance(transform.position, lastPosition) < stuckThreshold)
                {
                    stuckTime += Time.fixedDeltaTime;
                    if (stuckTime > 1f) // Consider stuck if it hasn't moved significantly for more than 1 second
                    {
                        Debug.Log("Let's try to unstuck");
                        tryUnstuck = true;
                    }
                }
                else
                {
                    stuckTime = 0f;
                    tryUnstuck = false;
                }

                lastPosition = transform.position;

                // Check if the node is reached or passed
                if (distanceToNextNode <= step)
                {
                    currentPathIndex++;
                }
            }
        }
    }

    public void FollowPlayer(PlayerController player)
    {
        this.player = player;
        startFollowing = true;

        float randomSpeedBoost = Random.Range(0f, 0.05f);
        AddToMovementSpeedModifier(randomSpeedBoost);

        CalculatePath();
    }

    public override void OnDamageTaken(DamageType damageType, float damage)
    {
        // use DamageType enum here
        // maintain amount of damage dealt with certain types of orb
        switch (damageType)
        {
            case DamageType.FIRE:
                damageTaken.FireDamage += damage;
                break;
            case DamageType.ICE:
                damageTaken.IceDamage += damage;
                break;
        }

        // TODO: this should be an instance method and then automatically set the text when FireDamage or IceDamage are modified
        DamageTaken.SetDamageTakenTextOnTextElement(GetMaxHp(), damageTaken, hpTextElement);

        if (IsDead())
        {
            OnDeath();
        }
    }

    void OnDeath()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().freezeRotation = true;
        // drop orb
        // orb should have amount of XP based on what kind of enemy this is (derivative of MAX_HP? log(MAX_HP)?)
        // TODO: maybe we should drop a number of orbs depending on how many hits the enemy took?
        orbDropper.DoOrbDrop(damageTaken, GetMaxHp());

        // no longer collide with it
        GetComponent<BoxCollider2D>().enabled = false;
    }

    public override bool IsDead()
    {
        return damageTaken.TotalDamage() >= GetMaxHp();
    }

    void CalculatePath()
    {
        // Convert world position to grid position
        Vector2Int start = containingRoom.Grid.WorldToGrid(transform.position);
        Vector2Int end = containingRoom.Grid.WorldToGrid(player.transform.position);

        // Implement or call your A* pathfinding method here
        path = AStarPathfinding.FindPath(containingRoom.Grid, start, end);
        if (path is null)
        {
            // don't even try
            return;
        }

        if (path != null && path.Count > 0)
        {
            currentPathIndex = 0;
        }
    }
}
