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
        Ranged,
        Boss
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

    protected RoomManager containingRoom;

    private List<Node> path;
    private int currentPathIndex;

    protected virtual float GetMaxHp()
    {
        return localMaxHp;
    }

    public const float BASE_HP = 15;

    protected float localMaxHp = BASE_HP;

    protected PlayerController player;

    private bool startFollowing = false;
    private readonly DamageTaken damageTaken = new();

    public override float MaxHp
    {
        get { return GetMaxHp(); }
    }

    public override float BaseMovementSpeed => 0.08f;

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

    const float pathUpdateInterval = 0.2f; // Time in seconds between path updates
    const float movementSpeedModifierWhenPatrolling = 0.5f;
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

                var newPosition = rigidBody.position + direction.normalized * MovementSpeed;
                rigidBody.MovePosition(newPosition);

                // Check if the node is reached or passed
                if (Vector2.Distance(rigidBody.position, nextPosition) <= 0.5f)
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

                var newPosition =
                    rigidBody.position
                    + MovementSpeed * movementSpeedModifierWhenPatrolling * direction.normalized;
                rigidBody.MovePosition(newPosition);

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
        patrolDirection = MoveDirection.TOWARDS_PATROL_POSITION;

        startFollowing = false;
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

        GetComponentInParent<GameManager>().statisticsTracker.Increment(
            StatisticsTracker.StatisticType.DAMAGE_DEALT,
            damage
        );
    }

    protected override void OnDeath()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().freezeRotation = true;
        // drop orb
        // orb should have amount of XP based on what kind of enemy this is (derivative of MAX_HP? log(MAX_HP)?)
        // TODO: maybe we should drop a number of orbs depending on how many hits the enemy took?
        orbDropper.DoOrbDrop(damageTaken, GetMaxHp(), containingRoom);

        if (player.HpDropOnKillChance > Random.Range(0, 1f))
        {
            // drop HP interactable
            HealthDropController.Create(
                GetComponentInParent<GameManager>().healthDropPrefab,
                transform,
                // TODO: derive this better
                player.MaxHp * 0.10f
            );
        }

        // no longer collide with it
        GetComponent<BoxCollider2D>().enabled = false;

        GetComponentInParent<GameManager>().statisticsTracker.Increment(
            StatisticsTracker.StatisticType.ENEMIES_DEFEATED
        );

        elementalStatusEffectSystem.StopAnimating();
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
            damageTaken.SetDamageTakenTextOnTextElement(GetMaxHp());

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

    public void IncreaseAllWeaponsAttackSpeedMultiplier(float modifierToAdd)
    {
        if (rangedWeapon != null)
        {
            rangedWeapon.IncreaseAttackSpeedMultiplier(modifierToAdd);
        }
        if (meleeWeapon != null)
        {
            meleeWeapon.IncreaseAttackSpeedMultiplier(modifierToAdd);
        }
    }

    public void IncreaseElementalSystemChance(float amountToIncrease)
    {
        if (rangedWeapon != null)
        {
            rangedWeapon.elementalSystem.Chance += amountToIncrease;
        }
        if (meleeWeapon != null)
        {
            meleeWeapon.elementalSystem.Chance += amountToIncrease;
        }
    }

    public void SetMaxHp(float newMaxHp)
    {
        localMaxHp = newMaxHp;
    }
}
