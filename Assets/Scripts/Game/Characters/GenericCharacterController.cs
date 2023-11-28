using System.Collections;
using UnityEngine;
using UnityEngine.Video;

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

    public abstract void TakeDamage(DamageType damageType, float damageTaken);

    protected bool applyingStatusEffect = false;

    protected StatusEffectSystem elementalSystem;

    [SerializeField]
    private DamageReceiverEffect damageReceiverEffectPrefab;
    protected DamageReceiverEffect damageReceiverEffect;

    protected virtual void Start()
    {
        var gameManager = GetComponentInParent<GameManager>();
        var elementalDamageStatusEffectSystemPrefab =
            gameManager.ElementalDamageStatusEffectSystemPrefab;
        elementalSystem = Instantiate(elementalDamageStatusEffectSystemPrefab, transform)
            .GetComponent<StatusEffectSystem>();
        if (damageReceiverEffectPrefab != null)
        {
            damageReceiverEffect = Instantiate(damageReceiverEffectPrefab, transform);
        }
    }

    private float CalculateDamagePerInterval(float duration, float interval, float? totalDamage)
    {
        if (totalDamage == null)
        {
            return DamageOverTimeSystem.DOT_DEFAULT_DAMAGE_PER_TICK;
        }
        return (float)(totalDamage / (duration / interval));
    }

    public void ApplyDamageOverTime(DamageType damageType, float duration, float totalDamage = 0)
    {
        if (applyingStatusEffect || IsDead())
        {
            return;
        }

        applyingStatusEffect = true;

        var state = damageType switch
        {
            DamageType.FIRE => EquilibriumManager.EquilibriumState.INFERNO,
            DamageType.ICE => EquilibriumManager.EquilibriumState.FROZEN,
            _ => throw new System.Exception($"Unhandled damage type {damageType}")
        };
        elementalSystem.SetStateAndAnimate(state);

        if (state == EquilibriumManager.EquilibriumState.FROZEN)
        {
            StartCoroutine(SlowMovementSpeedForDuration(duration, 0.50f));
        }

        StartCoroutine(DoDamageOverTime(damageType, duration, totalDamage));
    }

    private IEnumerator DoDamageOverTime(DamageType damageType, float duration, float totalDamage)
    {
        float baseDuration = DamageOverTimeSystem.DOT_BASE_DURATION;
        float damagePerInterval = CalculateDamagePerInterval(
            baseDuration,
            DamageOverTimeSystem.DOT_INTERVAL,
            totalDamage
        );

        while (duration > 0 && applyingStatusEffect)
        {
            if (IsDead())
            {
                elementalSystem.StopAnimating();
                yield break;
            }

            yield return new WaitForSeconds(DamageOverTimeSystem.DOT_INTERVAL);

            if (IsDead())
            {
                elementalSystem.StopAnimating();
                yield break;
            }

            duration -= DamageOverTimeSystem.DOT_INTERVAL;
            TakeDamage(damageType, damagePerInterval);
        }

        elementalSystem.StopAnimating();
        applyingStatusEffect = false;
    }

    private IEnumerator SlowMovementSpeedForDuration(
        float durationSeconds,
        float speedToSlowPercentage
    )
    {
        // take speed down to 1/2 of what it is
        var newSpeed = MovementSpeed * speedToSlowPercentage;
        var speedToRemove = MovementSpeed - newSpeed;
        AddToMovementSpeedModifier(-speedToRemove);
        yield return new WaitForSeconds(durationSeconds);
        AddToMovementSpeedModifier(speedToRemove);
    }
}
