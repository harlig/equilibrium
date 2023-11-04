using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro hpTextElement;
    private float hpRemaining;
    private const int MAX_HP = 100;
    private const float MOVEMENT_SPEED = 0.1f;

    void Awake()
    {
        hpRemaining = MAX_HP;
        hpTextElement.text = $"{hpRemaining}";
    }

    void FixedUpdate()
    {
        var Movement = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.A))
        {
            Movement.x -= 1.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Movement.x += 1.0f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            Movement.y += 1.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Movement.y -= 1.0f;
        }

        var RigidBody = gameObject.GetComponent<Rigidbody2D>();
        var NewPosition = RigidBody.position + Movement.normalized * MOVEMENT_SPEED;

        RigidBody.MovePosition(NewPosition);
    }
}
