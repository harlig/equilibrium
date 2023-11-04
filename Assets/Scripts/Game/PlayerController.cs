using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro hpTextElement;
    private float hpRemaining;
    private const int MAX_HP = 30;
    private const float MOVEMENT_SPEED = 0.1f;
    private bool canMove = true;

    void Awake()
    {
        hpRemaining = MAX_HP;
        hpTextElement.text = $"{hpRemaining}";
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            return;
        }

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

    // player can only take damage every DMG_FREQUENCY_INTERVAL seconds
    private const float DMG_FREQUENCY_INTERVAL = 0.5f;

    private bool canTakeDmg = true;

    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Something hit me!");
        RegisterDamage(other);
    }

    void OnCollisionStay2D(Collision2D other)
    {
        Debug.Log("Something is constantly hitting me!");
        RegisterDamage(other);
    }

    void RegisterDamage(Collision2D other)
    {
        if (!canTakeDmg)
        {
            return;
        }
        if (other.gameObject.CompareTag("Enemy"))
        {
            canTakeDmg = false;
            Debug.Log("Something that hit me is dealing dmg");
            // get amount of dmg dealt by this enemy, then apply it to the player

            hpRemaining -= 5;
            hpTextElement.text = $"{hpRemaining}";

            if (IsDead())
            {
                // TODO game over
                canMove = false;
                hpTextElement.text = $"{hpRemaining}\nGame Over!";
                // don't want to call WaitBeforeTakingDmg so we don't take more dmg
                return;
            }
            StartCoroutine(WaitBeforeTakingDmg(DMG_FREQUENCY_INTERVAL));
        }
    }

    private IEnumerator WaitBeforeTakingDmg(float waitTime)
    {
        // weird edge case?
        if (canTakeDmg)
            yield break;

        yield return new WaitForSeconds(waitTime);
        canTakeDmg = true;
    }

    public bool IsDead()
    {
        return hpRemaining <= 0;
    }
}
