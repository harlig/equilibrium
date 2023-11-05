using TMPro;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour
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

    // TODO use this
    // [SerializeField]
    // private Transform launchOffset;

    private float movementSpeed = 0.05f;
    private const int MAX_HP = 10;

    private float movementX,
        movementY;

    protected PlayerController player;

    private bool startFollowing = false;
    private readonly DamageTaken damageTaken = new();

    void Awake()
    {
        DamageTaken.SetDamageTakenTextOnTextElement(MAX_HP, damageTaken, hpTextElement);
        Color randomColor = new(Random.value, Random.value, Random.value);
        GetComponent<SpriteRenderer>().color = randomColor;
    }

    public static EnemyController Create(
        EnemyController prefab,
        Vector2 position,
        PlayerController player
    )
    {
        var createdEnemy = Instantiate(prefab, position, Quaternion.identity);
        createdEnemy.player = player;
        // var orbDropper = Instantiate(orbDropper, createdEnemy.transform, Quaternion.identity);
        return createdEnemy;
    }

    public Vector2 GetPositionAsVector2()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    void FixedUpdate()
    {
        if (startFollowing && !player.IsDead())
        {
            movementX = player.transform.position.x - transform.position.x;
            movementY = player.transform.position.y - transform.position.y;
            var rigidBody = gameObject.GetComponent<Rigidbody2D>();

            var newPosition =
                rigidBody.position + new Vector2(movementX, movementY).normalized * movementSpeed;

            rigidBody.MovePosition(newPosition);
        }
    }

    public void FollowPlayer(PlayerController player)
    {
        this.player = player;
        startFollowing = true;

        movementSpeed = Random.Range(0.03f, 0.08f);
    }

    // TODO this should go to character controller
    void OnDamageTaken()
    {
        // use DamageType enum here
        // maintain amount of damage dealt with certain types of orb
        damageTaken.FireDamage += 2;

        DamageTaken.SetDamageTakenTextOnTextElement(MAX_HP, damageTaken, hpTextElement);
    }

    void OnDeath()
    {
        // drop orb
        // orb should have amount of XP based on what kind of enemy this is (derivative of MAX_HP? log(MAX_HP)?)

        if (OrbDropper.ShouldDropFireOrb(damageTaken))
        {
            // TODO XP
            orbDropper.DropFireOrb(10);
        }
        else
        {
            // TODO XP
            orbDropper.DropIceOrb(10);
        }
    }
}
