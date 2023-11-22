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

    public void ApplyDamageOverTime(DamageType damageType, float damageOverTimeDuration)
    {
        // spawn a status effect animator, set to either INFERNO or FROZEN
        // set something saying "applying DOT"
        // start coroutine to deal damage over time
        // every like 0.5 seconds we should deal some damage value to the character's HpRemaining (call a method?)
        // unset applyingDOT at end of coroutine
    }

    public Vector2 GetPositionAsVector2()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }
}
