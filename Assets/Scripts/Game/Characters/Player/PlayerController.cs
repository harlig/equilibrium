using System;
using System.Collections;
using UnityEngine;

public class PlayerController : CharacterController
{
    // prefabs
    [SerializeField]
    private MeleeWeapon meleeWeapon;

    // non-prefabs
    public int PlayerLevel { get; private set; } = 0;
    public Camera MainCamera { private get; set; }

    private CharacterAnimator characterAnimator;
    public EquilibriumManager.EquilibriumState EquilibriumState { get; set; } =
        EquilibriumManager.DefaultState();

    public OrbCollector OrbCollector { get; private set; }
    public StatusEffectSystem StatusEffectSystem { get; private set; }
    private float hpRemaining;
    private bool _canMove = true;

    //////////////////////////////////////////////////////////
    //////////////////////////events//////////////////////////
    //////////////////////////////////////////////////////////
    public delegate void LevelUpAction(int newPlayerLevel, Action afterLevelUpAction);
    public event LevelUpAction OnLevelUpAction;

    public delegate void DamageTakenAction(float newPlayerHp);
    public event DamageTakenAction OnDamageTakenAction;
    public delegate void OrbCollectedAction(OrbController orbCollected, float newXp);
    public event OrbCollectedAction OnOrbCollectedAction;

    //////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////

    private const float WEAPON_OFFSET = 0.75F;

    public override float MaxHp => 3000;

    protected override float BaseMovementSpeed => 0.22f;

    public void IncreaseMovementSpeed(float amountToIncrease)
    {
        // MovementSpeedModifier += amountToIncrease;
    }

    public override float HpRemaining
    {
        get { return hpRemaining; }
    }

    private Vector2? AutomoveLocation = null;
    private Rigidbody2D rigidBody;

    void Awake()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        characterAnimator = GetComponent<CharacterAnimator>();
        OrbController.OrbType[] orbsToSupport =
        {
            OrbController.OrbType.FIRE,
            OrbController.OrbType.ICE
        };
        OrbCollector = new OrbCollector(orbsToSupport);
        StatusEffectSystem = GetComponentInChildren<StatusEffectSystem>();
        hpRemaining = MaxHp;
        CreateMeleeWeapon();
    }

    public float XpCollected()
    {
        return OrbCollector.XpCollected;
    }

    void Update()
    {
        // move weapon to left or right depending on where mouse is
        MoveWeaponRelativeToMouse();

        if (Input.GetMouseButtonDown(0))
        {
            meleeWeapon.AttackAtPosition(GetPositionAsVector2());
        }
    }

    int automoveInterval = 1;

    void FixedUpdate()
    {
        if (AutomoveLocation != null)
        {
            GetComponent<BoxCollider2D>().enabled = false;
            var x = transform.position.x;
            var y = transform.position.y;

            if (
                Mathf.Approximately(x, AutomoveLocation.Value.x)
                && Mathf.Approximately(y, AutomoveLocation.Value.y)
            )
            {
                GetComponent<BoxCollider2D>().enabled = true;
                AutomoveLocation = null;
                automoveInterval = 0;
                return;
            }

            x = Mathf.Lerp(x, AutomoveLocation.Value.x, automoveInterval * Time.fixedDeltaTime);
            y = Mathf.Lerp(y, AutomoveLocation.Value.y, automoveInterval * Time.fixedDeltaTime);
            rigidBody.MovePosition(new Vector2(x, y));
            automoveInterval++;
            return;
        }
        if (!_canMove)
        {
            return;
        }
        var movement = SetMoveDirection();
        var newPosition = rigidBody.position + movement.normalized * MovementSpeed;

        rigidBody.MovePosition(newPosition);
    }

    private Vector2 SetMoveDirection()
    {
        var movement = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.A))
        {
            characterAnimator.SetMoveDirection(UnityEngine.EventSystems.MoveDirection.Left);
            movement.x -= 1.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            characterAnimator.SetMoveDirection(UnityEngine.EventSystems.MoveDirection.Right);
            movement.x += 1.0f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            characterAnimator.SetMoveDirection(UnityEngine.EventSystems.MoveDirection.Up);
            movement.y += 1.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            characterAnimator.SetMoveDirection(UnityEngine.EventSystems.MoveDirection.Down);
            movement.y -= 1.0f;
        }

        return movement;
    }

    public Vector2 LocationAsVector2()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    public void MovePlayerToLocation(Vector2 position)
    {
        AutomoveLocation = position;
    }

    // player can only take damage every DMG_FREQUENCY_INTERVAL seconds
    private const float DMG_FREQUENCY_INTERVAL = 0.5f;

    private bool canTakeDmg = true;

    void OnCollisionEnter2D(Collision2D other)
    {
        RegisterDamage(other.gameObject);
    }

    void OnCollisionStay2D(Collision2D other)
    {
        RegisterDamage(other.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
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
        OrbCollector.Collect(orb);
        OnOrbCollectedAction?.Invoke(orb, OrbCollector.XpCollected);

        TryLevelUp();
    }

    void TryLevelUp()
    {
        // you can only possibly level up if you aren't yet at the last level
        if (PlayerLevel < GameManager.XpNeededForLevelUpAtIndex.Count)
        {
            var xpForLevelUp = GameManager.XpNeededForLevelUpAtIndex[PlayerLevel];
            if (OrbCollector.XpCollected >= xpForLevelUp)
            {
                // TODO: celebrate that player leveled up, offer reward!
                PlayerLevel++;

                // recursively call in case we need to level up again!
                OnLevelUpAction?.Invoke(PlayerLevel, () => TryLevelUp());
            }
        }
    }

    // TODO: should we remove this?
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

        OnDamageTaken(DamageType.FIRE, dmgAmount);
    }

    private IEnumerator WaitBeforeTakingDmg(float waitTime, bool forceDmg = false)
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
        OnDamageTakenAction?.Invoke(hpRemaining);

        if (IsDead())
        {
            // TODO: game over
            _canMove = false;
            // don't want to call WaitBeforeTakingDmg so we don't take more dmg
            GetComponent<BoxCollider2D>().enabled = false;
            return;
        }
        // TODO: how to force damage sometimes?
        StartCoroutine(WaitBeforeTakingDmg(DMG_FREQUENCY_INTERVAL));
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
        if (meleeWeapon == null || MainCamera == null)
        {
            return;
        }

        Vector2 mousePos = Input.mousePosition;
        Vector2 currentPos = transform.position;
        if (mousePos == null)
        {
            return;
        }

        Vector2 worldMousePos = MainCamera.ScreenToWorldPoint(new Vector2(mousePos.x, mousePos.y));

        Vector2 direction = worldMousePos - currentPos;

        bool isMovingRight = direction.x > 0;

        Vector2 topRight = new(WEAPON_OFFSET, WEAPON_OFFSET);
        Vector2 topLeft = new(-WEAPON_OFFSET, WEAPON_OFFSET);

        Vector2 offset = isMovingRight ? topRight : topLeft;

        meleeWeapon.transform.position = currentPos + offset;
    }
}
