using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class EnemyController : GenericCharacterController
{
    public enum EnemyType
    {
        MeleeFollowing,
        MeleePatrolling,
        Ranged
    }

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

    protected override float BaseMovementSpeed => 0.01f;

    public override float HpRemaining
    {
        get { return GetMaxHp() - damageTaken.TotalDamage(); }
    }

    protected WeaponSlotController weaponSlotController;

    [SerializeField]
    private MeleeWeapon meleeWeaponPrefab;

    [SerializeField]
    private RangedWeapon rangedWeaponPrefab;

    private MeleeWeapon meleeWeapon;

    private RangedWeapon rangedWeapon;

    void Awake()
    {
        containingRoom = GetComponentInParent<RoomManager>();
        damageTaken.TextElement = hpTextElement;
        damageTaken.SetDamageTakenTextOnTextElement(GetMaxHp());
        weaponSlotController = new(this, 0.4f);
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

        createdEnemy.transform.localPosition = position;
        createdEnemy.player = player;

        // instantiating this directly bc no need to pass additional data to it
        createdEnemy.orbDropper = Instantiate(prefab.orbDropper, createdEnemy.transform);
        return createdEnemy;
    }

    private readonly float pathUpdateInterval = 0.2f; // Time in seconds between path updates
    private float pathUpdateTimer;

    protected virtual void DoMovementActions()
    {
        var rigidBody = gameObject.GetComponent<Rigidbody2D>();

        if (startFollowing && !player.IsDead())
        {
            pathUpdateTimer += Time.fixedDeltaTime;
            if (
                pathUpdateTimer >= pathUpdateInterval
                || (path != null && currentPathIndex == path.Count)
            )
            {
                CalculateFollowPlayerPath();
                pathUpdateTimer = 0;
            }

            if (path != null && currentPathIndex < path.Count)
            {
                Node nextNode = path[currentPathIndex];

                Vector2 nextPosition = new(nextNode.WorldX, nextNode.WorldY);
                Vector2 direction = (nextPosition - rigidBody.position).normalized;

                // Set velocity in the direction of the next node
                float speed = MovementSpeed * 80; // Adjust this speed as needed
                rigidBody.velocity = direction * speed;

                float distanceToNextNode = Vector2.Distance(rigidBody.position, nextPosition);

                // Check if the node is reached or passed
                if (distanceToNextNode <= speed * Time.fixedDeltaTime)
                {
                    currentPathIndex++;
                }
            }
        }
        else if (isPatrolling)
        {
            if (path != null && currentPathIndex < path.Count)
            {
                Node nextNode = path[currentPathIndex];
                Vector2 nextPosition = new(nextNode.WorldX, nextNode.WorldY);
                Vector2 direction = (nextPosition - rigidBody.position).normalized;
                float speed = MovementSpeed * 80; // Adjust this speed as needed
                rigidBody.velocity = direction * speed;

                if (Vector2.Distance(rigidBody.position, nextPosition) < 0.2f)
                {
                    currentPathIndex++;
                    if (currentPathIndex >= path.Count)
                    {
                        // Switch target position and recalculate path
                        if (patrolDirection == MoveDirection.TOWARDS_PATROL_POSITION)
                        {
                            patrolDirection = MoveDirection.TOWARDS_START_POSITION;
                            CalculatePatrolPath(patrolStartWorldPosition); // Recalculate path back to the start
                        }
                        else
                        {
                            patrolDirection = MoveDirection.TOWARDS_PATROL_POSITION;
                            CalculatePatrolPath(patrolEndWorldPosition); // Recalculate path to the end position
                        }
                    }
                }
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        if (IsDead())
        {
            return;
        }

        DoMovementActions();
    }

    // TODO: this is mega broken, need to fix
    public void ApplyKnockback(Vector2 knockbackDirection, float knockbackStrength)
    {
        // Calculate the target position for the knockback
        // Vector2 targetPosition =
        //     new Vector2(transform.position.x, transform.position.y)
        //     + knockbackDirection * knockbackStrength;

        // var rigidBody = GetComponent<Rigidbody2D>();

        // // Calculate the velocity required to reach the target position
        // Vector2 velocity = targetPosition - rigidBody.position; // No division by time, implies immediate application

        // // Apply the calculated velocity
        // rigidBody.velocity = velocity;
    }

    public void FollowPlayer(PlayerController player)
    {
        this.player = player;
        startFollowing = true;
        isPatrolling = false;

        float randomSpeedBoost = Random.Range(0f, 0.05f);
        AddToMovementSpeedModifier(randomSpeedBoost);

        CalculateFollowPlayerPath();
    }

    private Vector2 patrolStartWorldPosition;
    private Vector2 patrolEndWorldPosition;
    private MoveDirection patrolDirection = MoveDirection.TOWARDS_PATROL_POSITION;
    private const float DEFAULT_DETECTION_RADIUS = 3f;
    private float detectionRadius = DEFAULT_DETECTION_RADIUS;
    private bool isPatrolling = false;

    public void PatrolArea(
        Vector2 endWorldPosition,
        float overrideDetectionRadius = DEFAULT_DETECTION_RADIUS
    )
    {
        patrolStartWorldPosition = transform.position;
        patrolEndWorldPosition = endWorldPosition;
        detectionRadius = overrideDetectionRadius;
        startFollowing = false;
        patrolDirection = MoveDirection.TOWARDS_PATROL_POSITION;
        isPatrolling = true;

        CalculatePatrolPath(patrolEndWorldPosition);
    }

    public override void TakeDamage(DamageType damageType, float damage)
    {
        if (isPatrolling)
        {
            FollowPlayer(player);
        }
        switch (damageType)
        {
            case DamageType.FIRE:
                damageTaken.FireDamage += damage;
                break;
            case DamageType.ICE:
                damageTaken.IceDamage += damage;
                break;
            case DamageType.NEUTRAL:
                damageTaken.NeutralDamage += damage;
                break;
        }

        damageTaken.SetDamageTakenTextOnTextElement(GetMaxHp());
        characterAnimator.AnimateHurt();

        if (IsDead())
        {
            OnDeath();
        }
    }

    protected override void OnDeath()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().freezeRotation = true;
        // drop orb
        // orb should have amount of XP based on what kind of enemy this is (derivative of MAX_HP? log(MAX_HP)?)
        // TODO: maybe we should drop a number of orbs depending on how many hits the enemy took?
        orbDropper.DoOrbDrop(damageTaken, GetMaxHp(), containingRoom);

        // no longer collide with it
        GetComponent<BoxCollider2D>().enabled = false;

        elementalSystem.StopAnimating();
        damageTaken.HideTextElement();

        if (meleeWeapon != null)
        {
            Destroy(meleeWeapon.gameObject);
        }
        if (rangedWeapon != null)
        {
            Destroy(rangedWeapon.gameObject);
        }
    }

    public override bool IsDead()
    {
        return damageTaken.TotalDamage() >= GetMaxHp();
    }

    void CalculateFollowPlayerPath()
    {
        Vector2Int start = containingRoom.Grid.WorldToGrid(transform.position);
        Vector2Int end = containingRoom.Grid.WorldToGrid(player.transform.position);

        path = AStarPathfinding.FindPath(containingRoom.Grid, start, end);
        if (path is null)
        {
            Debug.Log("Path is null!");
            // don't even try
            return;
        }

        if (path != null && path.Count > 0)
        {
            currentPathIndex = 0;
        }
    }

    void CalculatePatrolPath(Vector2 targetPosition)
    {
        Vector2Int start = containingRoom.Grid.WorldToGrid(transform.position);
        Vector2Int end = containingRoom.Grid.WorldToGrid(targetPosition);

        path = AStarPathfinding.FindPath(containingRoom.Grid, start, end);
        if (path != null && path.Count > 0)
        {
            currentPathIndex = 0;
        }
    }

    void Update()
    {
        if (!IsDead())
        {
            weaponSlotController.MoveWeaponsAtPosition(player.transform.position);

            // if we get too close to the player, get em!
            if (
                !startFollowing
                && Vector2.Distance(transform.position, player.transform.position)
                    <= detectionRadius
            )
            {
                FollowPlayer(player);
            }
        }
    }

    protected void CreateRangedWeapon()
    {
        var weapon = WeaponController.Create(rangedWeaponPrefab, transform.position, this);
        weaponSlotController.AssignWeaponSlot(weapon, 1);
        rangedWeapon = (RangedWeapon)weapon;
    }

    protected void CreateMeleeWeapon()
    {
        var weapon = WeaponController.Create(meleeWeaponPrefab, transform.position, this);
        weaponSlotController.AssignWeaponSlot(weapon, 0);
        meleeWeapon = (MeleeWeapon)weapon;
    }
}
