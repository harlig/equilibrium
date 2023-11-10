using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public abstract class CharacterController : MonoBehaviour
{
    // public api for move speed
    public float MovementSpeed
    {
        get => BaseMovementSpeed + MovementSpeedModifier;
    }
    protected abstract float BaseMovementSpeed { get; }

    public abstract float MaxHp { get; }

    public abstract float HpRemaining { get; }

    protected virtual float MovementSpeedModifier
    {
        get => 0;
        // set => MovementSpeedModifier = value;
    }

    public abstract bool IsDead();

    public abstract void OnDamageTaken(DamageType damageType, float damageTaken);

    public Vector2 GetPositionAsVector2()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }
}
