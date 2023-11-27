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

    private float CalculateDamagePerInterval(float duration, float interval, float? totalDamage)
    {
        if (totalDamage == null)
        {
            return DOT_DEFAULT_DAMAGE_PER_TICK;
        }
        return (float)(totalDamage / (duration / interval));
    }

    private const float DOT_INTERVAL = 0.5f; // Common interval for DOT effects
    private const float DOT_BASE_DURATION = 5.0f; // Common base duration
    private const float DOT_DEFAULT_DAMAGE_PER_TICK = 0.5f;
    private const float FREEZING_INTERVAL = 0.1f;

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

        Debug.Log($"Applying {damageType} DOT to character");

        StartCoroutine(DoDamageOverTime(damageType, duration, totalDamage));
    }

    private IEnumerator DoDamageOverTime(DamageType damageType, float duration, float totalDamage)
    {
        float interval = damageType == DamageType.FIRE ? DOT_INTERVAL : FREEZING_INTERVAL;
        float baseDuration = DOT_BASE_DURATION;
        float damagePerInterval = CalculateDamagePerInterval(baseDuration, interval, totalDamage);

        while (duration > 0 && applyingStatusEffect)
        {
            if (IsDead())
            {
                elementalSystem.StopAnimating();
                yield break;
            }

            yield return new WaitForSeconds(interval);

            if (IsDead())
            {
                elementalSystem.StopAnimating();
                yield break;
            }

            duration -= interval;
            DealDamage(damageType, damagePerInterval);
        }

        elementalSystem.StopAnimating();
        applyingStatusEffect = false;
    }
}
