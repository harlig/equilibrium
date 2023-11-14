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

    private float movementX,
        movementY;

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

    void FixedUpdate()
    {
        if (IsDead())
        {
            return;
        }

        if (startFollowing && !player.IsDead())
        {
            pathUpdateTimer += Time.fixedDeltaTime;
            if (pathUpdateTimer >= pathUpdateInterval || currentPathIndex == path.Count)
            {
                CalculatePath();
                pathUpdateTimer = 0;
            }

            if (path != null && currentPathIndex < path.Count)
            {
                var rigidBody = gameObject.GetComponent<Rigidbody2D>();
                Node nextNode = path[currentPathIndex];

                Vector2 nextPosition = new Vector2(nextNode.X, nextNode.Y);
                Vector2 direction = (nextPosition - rigidBody.position).normalized;

                // Calculate distance to move this frame
                // TODO: wtf is up with this move speed
                float step = 80 * MovementSpeed * Time.fixedDeltaTime;
                float distanceToNextNode = Vector2.Distance(rigidBody.position, nextPosition);

                // Move only as far as or closer to the next node
                Vector2 newPosition =
                    rigidBody.position + direction * Mathf.Min(step, distanceToNextNode);

                rigidBody.MovePosition(newPosition);

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
        Debug.LogFormat("start {0}, end {1}", start, end);

        // Implement or call your A* pathfinding method here
        path = AStarPathfinding.FindPath(containingRoom.Grid, start, end);
        if (path == null)
        {
            // don't even try
            startFollowing = false;
        }
        string pathStr = "";
        foreach (var node in path)
        {
            pathStr += $"[{node.X}, {node.Y}]; ";
        }
        Debug.LogFormat("Found path: {0}", pathStr);

        if (path != null && path.Count > 0)
        {
            currentPathIndex = 0;
        }
    }
}
