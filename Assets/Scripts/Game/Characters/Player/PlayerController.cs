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

    public override float MaxHp => BASE_MAX_HP * LocalMaxHpModifier;

    private const float BASE_MAX_HP = 3000;
    public float LocalMaxHpModifier { get; set; } = 1f;

    public override float BaseMovementSpeed => 0.11f;

    public override float HpRemaining
    {
        get { return hpRemaining; }
    }

    public float HpDropOnKillChance { get; set; } = 0.05f;

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
        int totalLevels = XpNeededForLevelUpAtIndex.Count;
        float targetMaxHp = 15000f;
        float finalModifier = targetMaxHp / BASE_MAX_HP;
        float powerFactor = 2f; // Adjust this to control the curve's steepness

        if (PlayerLevel < totalLevels)
        {
            var xpForLevelUp = XpNeededForLevelUpAtIndex[PlayerLevel];
            if (OrbCollector.XpCollected >= xpForLevelUp)
            {
                PlayerLevel++;

                // Apply power curve for HP scaling with consideration of existing HP modifier
                float newModifier =
                    Mathf.Pow(PlayerLevel / (float)(totalLevels - 1), powerFactor)
                        * (finalModifier - 1f)
                    + 1f;
                LocalMaxHpModifier *= newModifier; // Apply the new modifier to the existing one

                // heal 10% on level up
                Heal(MaxHp * 0.1f);

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

    public override bool IsDead()
    {
        return hpRemaining <= 0;
    }

    protected override void OnDeath()
    {
        DisablePlayer();
        GetComponentInParent<GameManager>().statisticsTracker.Increment(
            StatisticsTracker.StatisticType.NUM_DEATHS
        );
        GetComponentInParent<GameManager>().OnGameOver(GameOverStatus.FAIL);
        canTakeDmg = false;
    }

    bool canTakeDmg = true;

    public void Respawn()
    {
        // respawn with 50% hp
        LocalMaxHpModifier /= 2;
        hpRemaining = MaxHp;
        EnablePlayer();
        SetEquilibriumState(EquilibriumManager.ManageEquilibrium(OrbCollector));

        // Start invulnerability coroutine
        StartCoroutine(InvulnerabilityDelay());
    }

    private IEnumerator InvulnerabilityDelay()
    {
        // Set canTakeDmg to false to prevent damage
        canTakeDmg = false;

        // Wait for 3 seconds
        yield return new WaitForSeconds(3);

        // After 3 seconds, allow taking damage again
        canTakeDmg = true;
    }

    public void Heal(float amountToHeal)
    {
        hpRemaining += amountToHeal;
        hpRemaining = Mathf.Clamp(hpRemaining, 0, MaxHp);
        GetComponentInParent<GameManager>().HudController.SetPlayerHp(hpRemaining, MaxHp);
    }

    public override void TakeDamage(
        DamageType damageType,
        float damageTaken,
        bool isDamageFromDOT = false
    )
    {
        if (!canTakeDmg)
        {
            return;
        }
        hpRemaining -= damageTaken;

        if (IsDead())
        {
            OnDeath();
            return;
        }

        if (isDamageFromDOT && damageTaken != 0)
        {
            GetComponentInParent<GameManager>().AudioManager.PlayDOTSound();
        }
        else if (damageTaken != 0)
        {
            GetComponentInParent<GameManager>().AudioManager.PlayHurtSound();
        }
        GetComponentInParent<GameManager>().HudController.SetPlayerHp(hpRemaining, MaxHp);

        characterAnimator.AnimateHurt();
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
        else if (oldState == EquilibriumManager.EquilibriumState.FROZEN)
        {
            MultiplyToMovementSpeedModifier(1 / FROZEN_SPEED_MULTIPLIER);
        }
        EquilibriumState = newState;
    }

    public void DisablePlayer()
    {
        weaponSlotController.attackingEnabled = false;
        _canMove = false;

        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        // no longer collide with it
        GetComponent<BoxCollider2D>().enabled = false;
        applyingStatusEffect = false;
    }

    public void EnablePlayer()
    {
        weaponSlotController.attackingEnabled = true;
        _canMove = true;

        GetComponent<BoxCollider2D>().enabled = true;
    }
}
