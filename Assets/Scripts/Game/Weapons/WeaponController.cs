using UnityEngine;

public abstract class WeaponController : MonoBehaviour
{
    protected GenericCharacterController character;

    public DamageType DamageType;

    public abstract float BaseDamageAmount { get; }
    public abstract WeaponType Type { get; }

    public abstract bool ShouldRotateToMousePosition { get; }
    public float AttackSpeed
    {
        get => BaseAttackSpeed + attackSpeedModifier;
    }

    [SerializeField]
    protected float BaseAttackSpeed = 3f;
    private float attackSpeedModifier = 0;

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
    public abstract void StopAttacking();

    protected float GetDamageModifierOfParentCharacter()
    {
        float damageModifer = GenericCharacterController.BASE_DAMAGE_DEALT_MULTIPLIER;

        // TODO: why would there ever not be a character parent?
        // get damage modifier from the parent of this weapon if there is one
        var parentCharacter = GetComponentInParent<GenericCharacterController>();
        if (parentCharacter != null)
        {
            damageModifer =
                GetComponentInParent<GenericCharacterController>().DamageDealtMultiplier;
        }
        return damageModifer;
    }

    public void IncreaseAttackSpeed(float amountToIncrease)
    {
        attackSpeedModifier += amountToIncrease;
    }

    public enum WeaponType
    {
        MELEE,
        RANGED
    }
}
