using UnityEngine;

public abstract class WeaponController : MonoBehaviour
{
    protected GenericCharacterController character;

    public abstract DamageType DamageType { get; }

    public abstract float BaseDamageAmount { get; }

    public abstract bool ShouldRotateToMousePosition { get; }

    public static WeaponController Create(
        WeaponController prefab,
        Vector2 position,
        GenericCharacterController character
    )
    {
        var createdWeapon = Instantiate(
            prefab,
            position,
            prefab.transform.rotation,
            character.transform
        );
        createdWeapon.character = character;
        return createdWeapon;
    }

    public abstract void AttackAtPosition(Vector2 position);

    protected float GetDamageModifierOfParentCharacter()
    {
        // get damage modifier from the parent of this weapon if there is one
        var parentCharacter = GetComponentInParent<GenericCharacterController>();
        float damageModifer = 0;
        if (parentCharacter != null)
        {
            damageModifer = GetComponentInParent<GenericCharacterController>().DamageDealtModifier;
        }
        return damageModifer;
    }
}
