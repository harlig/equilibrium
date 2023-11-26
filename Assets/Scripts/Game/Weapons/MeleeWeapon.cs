using System.Collections;
using UnityEngine;

public class MeleeWeapon : WeaponController
{
    private WeaponAnimator weaponAnimator;
    private BoxCollider2D boxCollider;
    public FirestarterSystem firestarterSystem = new();

    public override bool ShouldRotateToMousePosition
    {
        get { return false; }
    }

    public override DamageType DamageType
    {
        get { return DamageType.FIRE; }
    }

    public override float BaseDamageAmount
    {
        get { return 20.0f; }
    }

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
        weaponAnimator.DoSwing(
            position,
            () =>
            {
                isSwinging = false;
                boxCollider.enabled = false;
            }
        );
    }

    private void ApplyCharacterDamage(CharacterController character, float damageModifier)
    {
        character.DealDamage(DamageType, BaseDamageAmount + damageModifier);

        if (firestarterSystem.Chance > Random.Range(0f, 1f))
        {
            character.ApplyDamageOverTime(
                DamageType.FIRE,
                firestarterSystem.Duration,
                firestarterSystem.Damage
            );
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // the weapon has collided with some other game object, do damage if character
        CharacterController otherChar = other.GetComponent<CharacterController>();

        // means we have collided with a character, apply damage, and no friendly fire on self
        if (otherChar != null && otherChar.transform != transform.parent)
        {
            ApplyCharacterDamage(otherChar, GetDamageModifierOfParentCharacter());
        }
    }
}
