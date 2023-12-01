using System;
using System.Collections;
using UnityEngine;
using static GameManager;

public class PlayerController : GenericCharacterController
{
    // prefabs
    [SerializeField]
    private MeleeWeapon meleeWeaponPrefab;

    [SerializeField]
    private RangedWeapon rangedWeaponPrefab;

    // non-prefabs
    public int PlayerLevel { get; private set; } = 0;
    public Camera MainCamera { get; set; }

    private WeaponSlotController weaponSlotController;

    public EquilibriumManager.EquilibriumState EquilibriumState { get; private set; } =
        EquilibriumManager.DefaultState();

    public OrbCollector OrbCollector { get; private set; }
    public StatusEffectSystem StatusEffectSystem { get; private set; }
    private float hpRemaining;
    private bool _canMove = true;

    public MeleeWeapon MeleeWeapon { get; private set; } = null;

    public RangedWeapon RangedWeapon { get; private set; } = null;

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
    public const float MAX_HP = 20000;

    public override float MaxHp => MAX_HP;

    public override float BaseMovementSpeed => 0.11f;

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
        weaponSlotController.ManageWeapons();
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<OrbController>() != null)
        {
            var orb = other.GetComponent<OrbController>();
            CollectOrb(orb);
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

        GetComponentInParent<GameManager>().statisticsTracker.Increment(
            StatisticsTracker.StatisticType.ORBS_COLLECTED
        );
        GetComponentInParent<GameManager>().statisticsTracker.Increment(
            StatisticsTracker.StatisticType.XP_COLLECTED,
            orb.Xp
        );

        TryLevelUp();
    }

    bool canCollectOrbs = true;

    void TryLevelUp()
    {
        // you can only possibly level up if you aren't yet at the last level
        if (PlayerLevel < XpNeededForLevelUpAtIndex.Count)
        {
            var xpForLevelUp = XpNeededForLevelUpAtIndex[PlayerLevel];
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

    protected override void OnDeath()
    {
        _canMove = false;
        weaponSlotController.DisableAttacking();
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().freezeRotation = true;

        // no longer collide with it
        GetComponent<BoxCollider2D>().enabled = false;

        GetComponentInParent<GameManager>().OnGameOver(GameOverStatus.FAIL);
    }

    public override void TakeDamage(DamageType damageType, float damageTaken)
    {
        Debug.LogFormat("took damage {0}", damageTaken);
        hpRemaining -= damageTaken;

        if (IsDead())
        {
            OnDeath();
            return;
        }

        GetComponentInParent<GameManager>().AudioManager.PlayHurtSound();
        GetComponentInParent<GameManager>().HudController.SetPlayerHp(hpRemaining, MaxHp);

        characterAnimator.AnimateHurt();

        StartCoroutine(WaitBeforeTakingDmg(DMG_FREQUENCY_INTERVAL));
    }

    private void CreateRangedWeapon()
    {
        var weapon = WeaponController.Create(rangedWeaponPrefab, transform.position, this);
        weaponSlotController.AssignWeaponSlot(weapon, 1);
        RangedWeapon = (RangedWeapon)weapon;
    }

    private void CreateMeleeWeapon()
    {
        var weapon = WeaponController.Create(meleeWeaponPrefab, transform.position, this);
        weaponSlotController.AssignWeaponSlot(weapon, 0);
        MeleeWeapon = (MeleeWeapon)weapon;
    }

    public WeaponController GetWeaponOfType(WeaponController.WeaponType weaponType)
    {
        return weaponType switch
        {
            WeaponController.WeaponType.MELEE => MeleeWeapon,
            WeaponController.WeaponType.RANGED => RangedWeapon,
            _ => throw new Exception($"Weapon type {weaponType} not supported"),
        };
    }

    public void SetEquilibriumState(EquilibriumManager.EquilibriumState newState)
    {
        var oldState = EquilibriumState;
        if (newState == EquilibriumManager.EquilibriumState.INFERNO)
        {
            ApplyEffectsForDamageType(DamageType.FIRE, int.MaxValue);
        }
        else if (oldState == EquilibriumManager.EquilibriumState.INFERNO)
        {
            // transitioning out of inferno should stop DOT
            // TODO: we should remove this if the player can take DOT from sources other than this
            applyingStatusEffect = false;
        }

        if (newState == EquilibriumManager.EquilibriumState.FROZEN)
        {
            MultiplyToMovementSpeedModifier(FROZEN_SPEED_MULTIPLIER);
        }
        else if (oldState == EquilibriumManager.EquilibriumState.INFERNO)
        {
            MultiplyToMovementSpeedModifier(1 / FROZEN_SPEED_MULTIPLIER);
        }
        EquilibriumState = newState;
    }
}
