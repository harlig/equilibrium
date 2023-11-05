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

    // TODO use this
    // [SerializeField]
    // private Transform launchOffset;

    private float movementSpeed = 0.05f;
    private const int MAX_HP = 10;

    private float hpRemaining;

    private float movementX,
        movementY;

    protected PlayerController player;

    private bool startFollowing = false;

    void Awake()
    {
        hpRemaining = MAX_HP;
        hpTextElement.text = $"{hpRemaining}";
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
}
