using UnityEngine;

public abstract class WeaponController : MonoBehaviour
{
    protected CharacterController character;

    public abstract DamageType DamageType { get; }

    public abstract float BaseDamageAmount { get; }

    public abstract bool ShouldRotateToMousePosition { get; }

    public static WeaponController Create(
        WeaponController prefab,
        Vector2 position,
        CharacterController character
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
        var parentCharacter = GetComponentInParent<CharacterController>();
        float damageModifer = 0;
        if (parentCharacter != null)
        {
            damageModifer = GetComponentInParent<CharacterController>().DamageDealtModifier;
        }
        return damageModifer;
    }
}
