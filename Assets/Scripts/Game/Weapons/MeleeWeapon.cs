using System;
using System.Collections;
using UnityEngine;

public class MeleeWeapon : WeaponController
{
    [SerializeField]
    DamageDealerEffect damageDealerEffectPrefab;

    private WeaponAnimator weaponAnimator;
    private BoxCollider2D boxCollider;
    public ElementalSystem elementalSystem = new();

    public override bool ShouldRotateToMousePosition => false;

    public override float BaseDamageAmount => 10f;

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

    public override void AttackAtPosition(Func<Vector2> getPosition)
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

    public override void StopAttacking()
    {
        weaponAnimator.StopAnimate();
        boxCollider.enabled = false;
    }

    private void ApplyCharacterDamage(GenericCharacterController character, float damageMultiplier)
    {
        character.TakeDamage(DamageType, BaseDamageAmount * damageMultiplier);

        if (elementalSystem.Chance > Chance.Get())
        {
            character.ApplyEffectsForDamageType(
                DamageType,
                elementalSystem.Duration,
                elementalSystem.Damage * damageMultiplier
            );
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // The weapon has collided with some other game object, do damage if character
        if (!other.TryGetComponent<GenericCharacterController>(out var otherChar))
        {
            return;
        }

        // Means we have collided with a character, apply damage, and no friendly fire on self
        if (!IsFriendlyFire(otherChar))
        {
            ApplyCharacterDamage(otherChar, GetDamageModifierOfParentCharacter());

            if (damageDealerEffectPrefab != null)
            {
                // Instantiate the damage dealer effect at the approximate collision point
                DamageDealerEffect damageDealerEffect = DamageDealerEffect
                    .Create(damageDealerEffectPrefab, other, otherChar.transform)
                    .GetComponent<DamageDealerEffect>();
                damageDealerEffect.OnHit();
            }
        }
    }

    bool IsFriendlyFire(GenericCharacterController otherChar)
    {
        if (otherChar is PlayerController)
        {
            return GetComponentInParent<PlayerController>() != null;
        }
        // if an enemy is hitting another enemy, it's friendly fire
        if (otherChar is EnemyController)
        {
            return GetComponentInParent<EnemyController>() != null;
        }

        throw new System.Exception(
            $"Melee weapon collided with something that's not a player or enemy! {otherChar.GetType()}"
        );
    }
}
