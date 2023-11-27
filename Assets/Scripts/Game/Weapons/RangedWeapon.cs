using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : WeaponController
{
    [SerializeField]
    private ProjectileBehavior projectilePrefab;
    private WeaponAnimator weaponAnimator;

    public override bool ShouldRotateToMousePosition
    {
        get { return true; }
    }

    public override DamageType DamageType
    {
        get { return DamageType.ICE; }
    }

    // this is just a modifier for the projectile damage
    public override float BaseDamageAmount
    {
        get { return 0.0f; }
    }

    private bool isShooting = false;

    void Awake()
    {
        weaponAnimator = GetComponent<WeaponAnimator>();
        character = GetComponentInParent<GenericCharacterController>();
    }

    public override void AttackAtPosition(Vector2 position)
    {
        if (isShooting)
        {
            return;
        }

        isShooting = true;

        weaponAnimator.DoAnimate(() =>
        {
            isShooting = false;
        });

        FireProjectile(position);
    }

    void FireProjectile(Vector2 firePosition)
    {
        var directionX = firePosition.x - transform.position.x;
        var directionY = firePosition.y - transform.position.y;
        Vector2 launchDirection = new Vector2(directionX, directionY).normalized;

        ProjectileBehavior.Create(
            projectilePrefab,
            transform.localToWorldMatrix.GetPosition(),
            launchDirection,
            character,
            BaseDamageAmount
        );
    }
}
