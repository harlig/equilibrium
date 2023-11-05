using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int Playerlevel { get; private set; } = 0;

    [SerializeField]
    private TextMeshProUGUI xpTextElement;

    [SerializeField]
    private TextMeshPro hpTextElement;

    // configure orbs types
    private OrbCollector orbCollector;

    [SerializeField]
    private TextMeshProUGUI fireOrbsTextElement;

    [SerializeField]
    private TextMeshProUGUI iceOrbsTextElement;

    // TODO use damageTaken
    private float hpRemaining;
    private bool _canMove = true;

    private const int MAX_HP = 30;
    private const float MOVEMENT_SPEED = 0.12f;

    void Awake()
    {
        var orbsToSupport = new Dictionary<OrbController.OrbType, TextMeshProUGUI>
        {
            [OrbController.OrbType.FIRE] = fireOrbsTextElement,
            [OrbController.OrbType.ICE] = iceOrbsTextElement
        };

        orbCollector = new(xpTextElement, orbsToSupport);

        hpRemaining = MAX_HP;
        hpTextElement.text = $"{hpRemaining}";
    }

    void FixedUpdate()
    {
        if (!_canMove)
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
        RegisterDamage(other.gameObject);
    }

    void OnCollisionStay2D(Collision2D other)
    {
        Debug.Log("Something is constantly hitting me!");
        RegisterDamage(other.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Some trigger hit me: {other.name}");
        if (other.GetComponent<OrbController>() != null)
        {
            var orb = other.GetComponent<OrbController>();
            orbCollector.Collect(orb);
        }
        else
        {
            RegisterDamage(other.gameObject, true);
        }
    }

    void RegisterDamage(GameObject other, bool forceDmg = false)
    {
        float dmgAmount;
        if (!canTakeDmg && !forceDmg)
        {
            return;
        }
        if (other.GetComponent<EnemyController>() != null)
        {
            // melee dmg is fixed
            dmgAmount = 5;
        }
        else if (other.GetComponent<ProjectileBehavior>() != null)
        {
            var projectile = other.GetComponent<ProjectileBehavior>();
            dmgAmount = projectile.DamageAmount;
        }
        else
        {
            return;
        }

        canTakeDmg = false;
        Debug.Log("Something that hit me is dealing dmg");

        // TODO use damageTaken
        hpRemaining -= dmgAmount;
        hpTextElement.text = $"{hpRemaining}";

        if (IsDead())
        {
            // TODO game over
            _canMove = false;
            hpTextElement.text = $"{hpRemaining}\nGame Over!";
            // don't want to call WaitBeforeTakingDmg so we don't take more dmg
            GetComponent<BoxCollider2D>().enabled = false;
            return;
        }
        StartCoroutine(WaitBeforeTakingDmg(DMG_FREQUENCY_INTERVAL, forceDmg));
    }

    private IEnumerator WaitBeforeTakingDmg(float waitTime, bool forceDmg)
    {
        // weird edge case?
        if (canTakeDmg && !forceDmg)
            yield break;

        yield return new WaitForSeconds(waitTime);
        canTakeDmg = true;
    }

    public bool IsDead()
    {
        return hpRemaining <= 0;
    }
}
