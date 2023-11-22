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

    public abstract void OnDamageTaken(DamageType damageType, float damageTaken);

    bool applyingDamageOverTime = false;

    public void ApplyDamageOverTime(DamageType damageType, float damageOverTimeDuration)
    {
        if (applyingDamageOverTime)
        {
            return;
        }

        applyingDamageOverTime = true;

        var elementalDamageSystem = Instantiate(
                GetComponentInParent<GameManager>().ElementalDamageStatusEffectSystemPrefab,
                transform
            )
            .GetComponent<StatusEffectSystem>();
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

        StartCoroutine(ApplyDOT(damageType, damageOverTimeDuration));
    }

    private IEnumerator ApplyDOT(DamageType damageType, float duration)
    {
        float interval = 0.5f;
        float damagePerInterval = CalculateDamagePerInterval(duration, interval);

        while (duration > 0)
        {
            duration -= interval;
            ApplyDamageOverTime(damageType, damagePerInterval);
            yield return new WaitForSeconds(interval);
        }

        applyingDamageOverTime = false;
    }

    private float CalculateDamagePerInterval(float duration, float interval)
    {
        float totalDamage = 10;
        return totalDamage / (duration / interval);
    }

    public Vector2 GetPositionAsVector2()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }
}
