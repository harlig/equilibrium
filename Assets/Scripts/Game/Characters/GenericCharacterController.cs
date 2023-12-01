using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterAnimator))]
public abstract class GenericCharacterController : MonoBehaviour
{
    private const float MIN_MOVEMENT_SPEED = 0.01f;
    private const float BASE_MOVEMENT_SPEED_MULTIPLIER = 1f;
    public const float BASE_DAMAGE_DEALT_MULTIPLIER = 1f;
    protected const float FROZEN_SPEED_MULTIPLIER = 0.5f;

    // public api for move speed
    public float MovementSpeed
    {
        get
        {
            float totalMovementSpeed = BaseMovementSpeed * movementSpeedMultiplier;
            return totalMovementSpeed < MIN_MOVEMENT_SPEED
                ? MIN_MOVEMENT_SPEED
                : totalMovementSpeed;
        }
    }
    public abstract float BaseMovementSpeed { get; }

    public abstract float MaxHp { get; }

    public abstract float HpRemaining { get; }

    private float movementSpeedMultiplier = BASE_MOVEMENT_SPEED_MULTIPLIER;
    public float DamageDealtMultiplier { get; private set; } = BASE_DAMAGE_DEALT_MULTIPLIER;

    // TODO: update what uses this
    public void AddToMovementSpeedModifier(float moveSpeedModifierToAdd)
    {
        movementSpeedMultiplier += moveSpeedModifierToAdd;
    }

    public void MultiplyToMovementSpeedModifier(float moveSpeedModifierToMultiply)
    {
        movementSpeedMultiplier *= moveSpeedModifierToMultiply;
    }

    // TODO: update what uses this
    public void AddToDamageDealtModifier(float extraDamageDealtModifier)
    {
        DamageDealtMultiplier += extraDamageDealtModifier;
    }

    public abstract bool IsDead();

    public abstract void TakeDamage(DamageType damageType, float damageTaken);
    protected abstract void OnDeath();

    protected bool applyingStatusEffect = false;

    protected StatusEffectSystem elementalStatusEffectSystem;
    protected CharacterAnimator characterAnimator;

    protected virtual void Start()
    {
        var gameManager = GetComponentInParent<GameManager>();
        var elementalDamageStatusEffectSystemPrefab =
            gameManager.ElementalDamageStatusEffectSystemPrefab;
        elementalStatusEffectSystem = Instantiate(
                elementalDamageStatusEffectSystemPrefab,
                transform
            )
            .GetComponent<StatusEffectSystem>();
        characterAnimator = GetComponent<CharacterAnimator>();
    }

    private float CalculateDamagePerInterval(float duration, float interval, float totalDamage)
    {
        if (totalDamage == 0)
        {
            // damage .3% of player max hp every tick
            return (float)(
                GetComponentInParent<GameManager>()
                    .GetComponentInChildren<PlayerController>()
                    .LocalMaxHp * 0.003
            );
        }
        return (float)(totalDamage / (duration / interval));
    }

    public void ApplyEffectsForDamageType(
        DamageType damageType,
        float duration,
        float totalDamage = 0
    )
    {
        if (applyingStatusEffect || IsDead() || damageType == DamageType.NEUTRAL)
        {
            return;
        }

        applyingStatusEffect = true;

        var state = damageType switch
        {
            DamageType.FIRE => EquilibriumManager.EquilibriumState.INFERNO,
            DamageType.ICE => EquilibriumManager.EquilibriumState.FROZEN,
            _ => throw new System.Exception($"Unhandled damage type for DOT {damageType}")
        };
        elementalStatusEffectSystem.SetStateAndAnimate(state);

        if (state == EquilibriumManager.EquilibriumState.FROZEN)
        {
            StartCoroutine(SlowMovementSpeedForDuration(duration, FROZEN_SPEED_MULTIPLIER));
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
                elementalStatusEffectSystem.StopAnimating();
                yield break;
            }

            yield return new WaitForSeconds(DamageOverTimeSystem.DOT_INTERVAL);

            if (IsDead())
            {
                elementalStatusEffectSystem.StopAnimating();
                yield break;
            }

            duration -= DamageOverTimeSystem.DOT_INTERVAL;
            TakeDamage(damageType, damagePerInterval);
        }

        elementalStatusEffectSystem.StopAnimating();
        applyingStatusEffect = false;
    }

    private IEnumerator SlowMovementSpeedForDuration(
        float durationSeconds,
        float speedToSlowPercentage
    )
    {
        MultiplyToMovementSpeedModifier(speedToSlowPercentage);
        yield return new WaitForSeconds(durationSeconds);
        MultiplyToMovementSpeedModifier(1 / speedToSlowPercentage);
    }
}
