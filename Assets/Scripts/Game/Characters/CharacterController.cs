using System.Collections;
using UnityEngine;

public abstract class CharacterController : MonoBehaviour
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

    protected bool applyingDamageOverTime = false;

    protected StatusEffectSystem elementalDamageSystem;

    protected virtual void Start()
    {
        var gameManager = GetComponentInParent<GameManager>();
        var prefab = gameManager.ElementalDamageStatusEffectSystemPrefab;
        elementalDamageSystem = Instantiate(prefab, transform).GetComponent<StatusEffectSystem>();
    }

    public void ApplyDamageOverTime(
        DamageType damageType,
        float damageOverTimeDuration,
        float? totalDamage = null
    )
    {
        if (applyingDamageOverTime || IsDead())
        {
            return;
        }

        applyingDamageOverTime = true;

        var state = damageType switch
        {
            DamageType.FIRE => EquilibriumManager.EquilibriumState.INFERNO,
            DamageType.ICE => EquilibriumManager.EquilibriumState.FROZEN,
            _
                => throw new System.Exception(
                    $"Couldn't handle damage type {damageType} for the elemental status effect system"
                ),
        };
        elementalDamageSystem.SetStateAndAnimate(state);

        Debug.Log("applying DOT to character");

        StartCoroutine(ApplyDOT(damageType, damageOverTimeDuration, totalDamage));
    }

    private const float DOT_INTERVAL = 0.5f;
    private const float DOT_BASE_DURATION = 5.0f;
    private const float DOT_DEFAULT_DAMAGE_PER_TICK = 0.5f;

    private IEnumerator ApplyDOT(DamageType damageType, float duration, float? totalDamage)
    {
        float damagePerInterval = CalculateDamagePerInterval(
            // this allows an increased duration to get extra ticks of damage
            DOT_BASE_DURATION,
            DOT_INTERVAL,
            totalDamage
        );

        while (duration > 0 && applyingDamageOverTime)
        {
            if (IsDead())
            {
                elementalDamageSystem.StopAnimating();
                yield break;
            }
            yield return new WaitForSeconds(DOT_INTERVAL);
            if (IsDead())
            {
                elementalDamageSystem.StopAnimating();
                yield break;
            }
            duration -= DOT_INTERVAL;
            DealDamage(damageType, damagePerInterval);
        }

        elementalDamageSystem.StopAnimating();
        applyingDamageOverTime = false;
    }

    private float CalculateDamagePerInterval(float duration, float interval, float? totalDamage)
    {
        if (totalDamage == null)
        {
            return DOT_DEFAULT_DAMAGE_PER_TICK;
        }
        return (float)(totalDamage / (duration / interval));
    }
}
