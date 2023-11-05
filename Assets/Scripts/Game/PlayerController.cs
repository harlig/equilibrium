using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : CharacterController
{
    public int PlayerLevel { get; private set; } = 0;

    [SerializeField]
    private TextMeshProUGUI xpTextElement;

    [SerializeField]
    private TextMeshPro hpTextElement;

    [SerializeField]
    private TextMeshPro levelTextElement;

    // configure orbs types
    private OrbCollector orbCollector;

    [SerializeField]
    private TextMeshProUGUI fireOrbsTextElement;

    [SerializeField]
    private TextMeshProUGUI iceOrbsTextElement;

    [SerializeField]
    private MeleeWeapon meleeWeapon;
    private float hpRemaining;
    private bool _canMove = true;

    private const int MAX_HP = 30;
    private const float MOVEMENT_SPEED = 0.12f;

    //////////////////////////////////////////////////////////
    //////////////////////////events//////////////////////////
    //////////////////////////////////////////////////////////
    public delegate void LevelUpAction(int newPlayerLevel, Action afterLevelUpAction);
    public event LevelUpAction OnLevelUp;

    //////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////

    private const float WEAPON_OFFSET = 0.75F;

    public override float MaxHp
    {
        get { return MAX_HP; }
    }

    public override float MovementSpeed
    {
        get { return MOVEMENT_SPEED; }
    }

    public override float HpRemaining
    {
        get { return hpRemaining; }
    }

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
        levelTextElement.text = $"lvl {PlayerLevel}";
        xpTextElement.text = $"{orbCollector.XpCollected} xp collected";
        CreateMeleeWeapon();
    }

    void Update()
    {
        xpTextElement.text = $"{orbCollector.XpCollected} xp collected";
        // move weapon to left or right depending on where mouse is
        MoveWeaponRelativeToMouse();

        if (Input.GetMouseButtonDown(0))
        {
            meleeWeapon.AttackAtPosition(GetPositionAsVector2());
        }
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
            CollectOrb(orb);
        }
        else
        {
            RegisterDamage(other.gameObject, true);
        }
    }

    void CollectOrb(OrbController orb)
    {
        orbCollector.Collect(orb);
        TryLevelUp();
    }

    void TryLevelUp()
    {
        // you can only possibly level up if you aren't yet at the last level
        if (PlayerLevel < GameManager.XpNeededForLevelUpAtIndex.Count)
        {
            var xpForLevelUp = GameManager.XpNeededForLevelUpAtIndex[PlayerLevel];
            if (orbCollector.XpCollected >= xpForLevelUp)
            {
                // TODO celebrate that player leveled up, offer reward!
                PlayerLevel++;
                levelTextElement.text = $"lvl {PlayerLevel}";

                // recursively call in case we need to level up again!
                OnLevelUp?.Invoke(PlayerLevel, () => TryLevelUp());
            }
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

    public override bool IsDead()
    {
        return hpRemaining <= 0;
    }

    public override void OnDamageTaken(DamageType damageType, float damageTaken)
    {
        // TODO: add in effects for different damage types
        hpRemaining -= damageTaken;
    }

    private void CreateMeleeWeapon()
    {
        Vector2 offset = new(WEAPON_OFFSET, WEAPON_OFFSET);

        // calculate the position to the top right corner
        Vector2 spawnPos = (Vector2)transform.position + offset;

        var weapon = WeaponController.Create(meleeWeapon, spawnPos, this);
        weapon.transform.parent = transform;
        meleeWeapon = (MeleeWeapon)weapon;
    }

    private void MoveWeaponRelativeToMouse()
    {
        if (meleeWeapon == null)
        {
            return;
        }

        Vector2 mousePos = Input.mousePosition;
        Vector2 currentPos = transform.position;
        if (mousePos == null)
        {
            return;
        }

        Camera camera = GetComponentInChildren<Camera>();

        Vector2 worldMousePos = camera.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));

        Vector2 direction = worldMousePos - currentPos;

        bool isMovingRight = direction.x > 0;

        Vector2 topRight = new(WEAPON_OFFSET, WEAPON_OFFSET);
        Vector2 topLeft = new(-WEAPON_OFFSET, WEAPON_OFFSET);

        Vector2 offset = isMovingRight ? topRight : topLeft;

        meleeWeapon.transform.position = currentPos + offset;
    }
}
