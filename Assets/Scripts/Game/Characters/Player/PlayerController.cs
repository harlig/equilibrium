using System;
using System.Collections;
using UnityEngine;

public class PlayerController : CharacterController
{
    // prefabs
    [SerializeField]
    private MeleeWeapon meleeWeapon;

    [SerializeField]
    private RangedWeapon rangedWeapon;

    // non-prefabs
    public int PlayerLevel { get; private set; } = 0;
    public Camera MainCamera { get; set; }

    private CharacterAnimator characterAnimator;

    private WeaponSlotController weaponSlotController;

    public EquilibriumManager.EquilibriumState EquilibriumState { get; private set; } =
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

    public delegate void OrbCollectedAction(OrbController orbCollected, float newXp);
    public event OrbCollectedAction OnOrbCollectedAction;

    //////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////

    private const float WEAPON_OFFSET = 0.40f;

    public override float MaxHp => 3000;

    protected override float BaseMovementSpeed => 0.15f;

    public override float HpRemaining
    {
        get { return hpRemaining; }
    }

    private Vector2? AutomoveLocation = null;
    private Rigidbody2D rigidBody;
    public OrbitSystem OrbitSystem { get; private set; }
    public RoomManager CurrentRoom { get; set; }

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
        OrbitSystem = GetComponentInChildren<OrbitSystem>();
        hpRemaining = MaxHp;

        weaponSlotController = new(this);
        CreateMeleeWeapon();
        CreateRangedWeapon();
    }

    public float XpCollected()
    {
        return OrbCollector.XpCollected;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (UnityEngine.Random.Range(0.0f, 1.0f) < 0.5)
            {
                OrbitSystem.AddOrbiter(OrbitSystem.OrbiterType.FIRE);
            }
            else
            {
                OrbitSystem.AddOrbiter(OrbitSystem.OrbiterType.ICE);
            }
        }

        weaponSlotController.ManageWeaponSlots();
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
            movement.x -= 1.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement.x += 1.0f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            movement.y += 1.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
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

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<OrbController>() != null)
        {
            var orb = other.GetComponent<OrbController>();
            CollectOrb(orb);
        }
    }

    void CollectOrb(OrbController orb)
    {
        if (!canCollectOrbs)
        {
            return;
        }
        OrbCollector.Collect(orb);
        OnOrbCollectedAction?.Invoke(orb, OrbCollector.XpCollected);

        TryLevelUp();
    }

    bool canCollectOrbs = true;

    void TryLevelUp()
    {
        // you can only possibly level up if you aren't yet at the last level
        if (PlayerLevel < GameManager.XpNeededForLevelUpAtIndex.Count)
        {
            var xpForLevelUp = GameManager.XpNeededForLevelUpAtIndex[PlayerLevel];
            if (OrbCollector.XpCollected >= xpForLevelUp)
            {
                PlayerLevel++;

                // can't collect more orbs until we finish the level up
                canCollectOrbs = false;
                OnLevelUpAction?.Invoke(
                    PlayerLevel,
                    () =>
                    {
                        // recursively call in case we need to level up again!
                        TryLevelUp();
                        canCollectOrbs = true;
                    }
                );
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
        else
        {
            return;
        }

        canTakeDmg = false;

        DealDamage(DamageType.FIRE, dmgAmount);
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

    public override void DealDamage(DamageType damageType, float damageTaken)
    {
        hpRemaining -= damageTaken;

        if (IsDead())
        {
            // TODO: game over
            _canMove = false;
            // don't want to call WaitBeforeTakingDmg so we don't take more dmg
            GetComponent<BoxCollider2D>().enabled = false;
            return;
        }

        GetComponentInParent<GameManager>().AudioManager.PlayHurtSound();
        GetComponentInParent<GameManager>().HudController.SetPlayerHp(hpRemaining);

        StartCoroutine(WaitBeforeTakingDmg(DMG_FREQUENCY_INTERVAL));
    }

    private void CreateRangedWeapon()
    {
        var weapon = WeaponController.Create(rangedWeapon, transform.position, this);
        weaponSlotController.AssignWeaponSlot(weapon, 1);
    }

    private void CreateMeleeWeapon()
    {
        var weapon = WeaponController.Create(meleeWeapon, transform.position, this);
        weaponSlotController.AssignWeaponSlot(weapon, 0);
    }

    // TODO: I think there's a better generic way of this? should I just expose meleeWeapon?
    public void AddFirestarterChance(float firestarterModifier)
    {
        meleeWeapon.firestarterSystem.Chance += firestarterModifier;
    }

    public void AddFirestarterDuration(float firestarterModifier)
    {
        meleeWeapon.firestarterSystem.Duration += firestarterModifier;
    }

    public void AddFirestarterDamage(float firestarterModifier)
    {
        meleeWeapon.firestarterSystem.Damage += firestarterModifier;
    }

    private const float FROZEN_SPEED_MULTIPLICATIVE_MODIFIER = 0.5f;
    private float speedRemovedFromFrozenState = 0f;

    public void SetEquilibriumState(EquilibriumManager.EquilibriumState newState)
    {
        var oldState = EquilibriumState;
        if (newState == EquilibriumManager.EquilibriumState.INFERNO)
        {
            ApplyDamageOverTime(DamageType.FIRE, int.MaxValue);
        }
        else if (oldState == EquilibriumManager.EquilibriumState.INFERNO)
        {
            // transitioning out of inferno should stop DOT
            // TODO: we should remove this if the player can take DOT from sources other than this
            applyingDamageOverTime = false;
        }

        if (newState == EquilibriumManager.EquilibriumState.FROZEN)
        {
            // take speed down to 1/2 of what it is
            var newSpeed = MovementSpeed * FROZEN_SPEED_MULTIPLICATIVE_MODIFIER;
            var speedToRemove = MovementSpeed - newSpeed;
            speedRemovedFromFrozenState = speedToRemove;
            AddToMovementSpeedModifier(-speedToRemove);
        }
        else if (oldState == EquilibriumManager.EquilibriumState.INFERNO)
        {
            AddToMovementSpeedModifier(speedRemovedFromFrozenState);
            speedRemovedFromFrozenState = 0f;
        }
        EquilibriumState = newState;
    }
}
