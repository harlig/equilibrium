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

    // TODO: use this
    // [SerializeField]
    // private Transform launchOffset;


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

    void FixedUpdate()
    {
        if (IsDead())
        {
            return;
        }

        if (startFollowing && !player.IsDead())
        {
            movementX = player.transform.position.x - transform.position.x;
            movementY = player.transform.position.y - transform.position.y;
            var rigidBody = gameObject.GetComponent<Rigidbody2D>();

            var newPosition =
                rigidBody.position + new Vector2(movementX, movementY).normalized * MovementSpeed;

            rigidBody.MovePosition(newPosition);
        }
    }

    public void FollowPlayer(PlayerController player)
    {
        this.player = player;
        startFollowing = true;

        float randomSpeedBoost = Random.Range(0f, 0.05f);
        // MovementSpeedModifier += randomSpeedBoost;
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
}
