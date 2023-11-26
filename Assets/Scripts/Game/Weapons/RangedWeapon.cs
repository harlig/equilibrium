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

    public override float BaseDamageAmount
    {
        get { return 20.0f; }
    }

    void Awake()
    {
        weaponAnimator = GetComponent<WeaponAnimator>();
        character = GetComponentInParent<CharacterController>();
    }

    public override void AttackAtPosition(Vector2 position)
    {
        FireProjectile(position);
    }

    void FireProjectile(Vector2 firePosition)
    {
        var directionX = firePosition.x - transform.position.x;
        var directionY = firePosition.y - transform.position.y;
        Vector2 launchDirection = new Vector2(directionX, directionY).normalized;

        ProjectileBehavior.Create(
            projectilePrefab,
            BaseDamageAmount,
            transform.localToWorldMatrix.GetPosition(),
            launchDirection,
            character
        );
    }
}
