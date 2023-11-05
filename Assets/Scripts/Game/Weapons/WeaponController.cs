using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public abstract class WeaponController : MonoBehaviour
{
    protected CharacterController character;

    public abstract DamageType damageType { get; }

    public abstract float baseDamageAmount { get; }

    public static WeaponController Create(
        WeaponController prefab,
        Vector2 position,
        CharacterController character
    )
    {
        var createdWeapon = Instantiate(prefab, position, Quaternion.identity);
        createdWeapon.character = character;
        return createdWeapon;
    }

    public abstract void AttackAtPosition(Vector2 position);
}
