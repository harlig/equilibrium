using System.Collections;
using UnityEngine;

public class MeleeWeapon : WeaponController
{
    private WeaponAnimator weaponAnimator;
    private BoxCollider2D boxCollider;
    public ElementalSystem elementalSystem = new(5f);

    public override bool ShouldRotateToMousePosition => false;

    public override DamageType DamageType => DamageType.FIRE;

    public override float BaseDamageAmount => 20f;

    public override WeaponType Type => WeaponType.MELEE;

    private bool isSwinging = false;

    void Awake()
    {
        weaponAnimator = GetComponent<WeaponAnimator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        boxCollider.enabled = false;
    }

    public override void AttackAtPosition(Vector2 position)
    {
        if (isSwinging)
        {
            return;
        }

        isSwinging = true;
        boxCollider.enabled = true;
        weaponAnimator.DoAnimate(
            AttackSpeed,
            () =>
            {
                isSwinging = false;
                boxCollider.enabled = false;
            }
        );
    }

    private void ApplyCharacterDamage(GenericCharacterController character, float damageModifier)
    {
        character.DealDamage(DamageType, BaseDamageAmount + damageModifier);

        if (elementalSystem.Chance > Random.Range(0f, 1f))
        {
            character.ApplyDamageOverTime(
                DamageType.FIRE,
                elementalSystem.Duration,
                elementalSystem.Damage
            );
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // the weapon has collided with some other game object, do damage if character
        GenericCharacterController otherChar = other.GetComponent<GenericCharacterController>();

        // means we have collided with a character, apply damage, and no friendly fire on self
        if (otherChar != null && otherChar.transform != transform.parent)
        {
            ApplyCharacterDamage(otherChar, GetDamageModifierOfParentCharacter());
        }
    }
}
