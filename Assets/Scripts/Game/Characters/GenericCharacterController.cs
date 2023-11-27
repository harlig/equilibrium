using System.Collections;
using UnityEngine;

public abstract class GenericCharacterController : MonoBehaviour
{
    // public api for move speed
    public float MovementSpeed
    {
        get => BaseMovementSpeed + movementSpeedModifier;
    }
    protected abstract float BaseMovementSpeed { get; }

    public abstract float MaxHp { get; }

    public abstract float HpRemaining { get; }

    private float movementSpeedModifier = 0;
    public float DamageDealtModifier { get; private set; } = 0;

    public void AddToMovementSpeedModifier(float speedToAdd)
    {
        movementSpeedModifier += speedToAdd;
    }

    public void AddToDamageDealtModifier(float extraDamageDealt)
    {
        DamageDealtModifier += extraDamageDealt;
    }

    public abstract bool IsDead();

    public abstract void DealDamage(DamageType damageType, float damageTaken);

    protected bool applyingStatusEffect = false;

    protected StatusEffectSystem elementalSystem;

    protected virtual void Start()
    {
        var gameManager = GetComponentInParent<GameManager>();
        var prefab = gameManager.ElementalDamageStatusEffectSystemPrefab;
        elementalSystem = Instantiate(prefab, transform).GetComponent<StatusEffectSystem>();
    }

    public void ApplyDamageOverTime(
        DamageType damageType,
        float damageOverTimeDuration,
        float totalDamage = 0
    )
    {
        if (applyingStatusEffect || IsDead())
        {
            return;
        }

        applyingStatusEffect = true;

        var state = damageType switch
        {
            DamageType.FIRE => EquilibriumManager.EquilibriumState.INFERNO,
            _
                => throw new System.Exception(
                    $"Couldn't handle damage type {damageType} for the elemental status effect system"
                ),
        };
        elementalSystem.SetStateAndAnimate(state);

        Debug.Log("applying DOT to character");

        StartCoroutine(DoFireDOT(damageType, damageOverTimeDuration, totalDamage));
    }

    // TODO: how to make better
    public void MakeEnemyFrozen(
        DamageType damageType,
        float freezingDuration,
        float totalDamage = 0
    )
    {
        if (applyingStatusEffect || IsDead())
        {
            return;
        }

        applyingStatusEffect = true;

        var state = damageType switch
        {
            DamageType.ICE => EquilibriumManager.EquilibriumState.FROZEN,
            _
                => throw new System.Exception(
                    $"Couldn't handle damage type {damageType} for the elemental status effect system"
                ),
        };
        elementalSystem.SetStateAndAnimate(state);

        Debug.Log("freezing enemy");

        StartCoroutine(DoFreezing(freezingDuration, totalDamage));
    }

    private const float DOT_INTERVAL = 0.5f;
    private const float DOT_BASE_DURATION = 5.0f;
    private const float DOT_DEFAULT_DAMAGE_PER_TICK = 0.5f;

    private IEnumerator DoFireDOT(DamageType damageType, float duration, float totalDamage)
    {
        float damagePerInterval = CalculateDamagePerInterval(
            // this allows an increased duration to get extra ticks of damage
            DOT_BASE_DURATION,
            DOT_INTERVAL,
            totalDamage
        );

        while (duration > 0 && applyingStatusEffect)
        {
            if (IsDead())
            {
                elementalSystem.StopAnimating();
                yield break;
            }
            yield return new WaitForSeconds(DOT_INTERVAL);
            if (IsDead())
            {
                elementalSystem.StopAnimating();
                yield break;
            }
            duration -= DOT_INTERVAL;
            DealDamage(damageType, damagePerInterval);
        }

        elementalSystem.StopAnimating();
        applyingStatusEffect = false;
    }

    private float CalculateDamagePerInterval(float duration, float interval, float? totalDamage)
    {
        if (totalDamage == null)
        {
            return DOT_DEFAULT_DAMAGE_PER_TICK;
        }
        return (float)(totalDamage / (duration / interval));
    }

    private const float FREEZING_INTERVAL = 0.1f;
    private const float FREEZING_BASE_DURATION = 5.0f;

    private IEnumerator DoFreezing(float duration, float totalDamage)
    {
        float damagePerInterval = CalculateDamagePerInterval(
            // this allows an increased duration to get extra ticks of damage
            FREEZING_BASE_DURATION,
            FREEZING_INTERVAL,
            totalDamage
        );

        while (duration > 0 && applyingStatusEffect)
        {
            if (IsDead())
            {
                elementalSystem.StopAnimating();
                yield break;
            }
            yield return new WaitForSeconds(FREEZING_INTERVAL);
            if (IsDead())
            {
                elementalSystem.StopAnimating();
                yield break;
            }
            duration -= FREEZING_INTERVAL;

            DealDamage(DamageType.ICE, damagePerInterval);
        }

        elementalSystem.StopAnimating();
        applyingStatusEffect = false;
    }
}
