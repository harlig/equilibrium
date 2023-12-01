using System;
using UnityEngine;

public class RangedWeapon : WeaponController
{
    [SerializeField]
    private ProjectileBehavior projectilePrefab;
    private WeaponAnimator weaponAnimator;
    public ElementalSystem elementalSystem = new();

    public override bool ShouldRotateToMousePosition => true;

    // this is just a modifier for the projectile damage
    public override float BaseDamageAmount => 0f;
    public override WeaponType Type => WeaponType.RANGED;

    private bool isShooting = false;

    public ProjectileBehavior OverrideProjectile { private get; set; } = null;

    void Awake()
    {
        weaponAnimator = GetComponent<WeaponAnimator>();
        character = GetComponentInParent<GenericCharacterController>();
    }

    public override void AttackAtPosition(Func<Vector2> getPosition)
    {
        if (isShooting)
        {
            return;
        }

        isShooting = true;

        weaponAnimator.DoAnimate(
            AttackSpeed,
            () =>
            {
                FireProjectile(getPosition());
                isShooting = false;
            }
        );
    }

    public override void StopAttacking()
    {
        weaponAnimator.StopAnimate();
    }

    void FireProjectile(Vector2 firePosition)
    {
        var directionX = firePosition.x - transform.position.x;
        var directionY = firePosition.y - transform.position.y;
        Vector2 launchDirection = new Vector2(directionX, directionY).normalized;

        ProjectileBehavior.Create(
            OverrideProjectile != null ? OverrideProjectile : projectilePrefab,
            transform.localToWorldMatrix.GetPosition(),
            launchDirection,
            character,
            elementalSystem
        );
    }
}
