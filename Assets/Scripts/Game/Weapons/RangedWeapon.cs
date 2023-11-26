using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : WeaponController
{
    [SerializeField]
    private ProjectileBehavior projectilePrefab;
    private WeaponAnimator weaponAnimator;

    public override bool shouldRotateToMousePosition
    {
        get { return true; }
    }

    public override DamageType damageType
    {
        get { return DamageType.ICE; }
    }

    public override float baseDamageAmount
    {
        get
        {
            // TODO: change to be specific to actual weapon
            return 2.0f;
        }
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
        var projectile = Instantiate(
            projectilePrefab,
            transform.localToWorldMatrix.GetPosition(),
            Quaternion.identity
        );

        var directionX = firePosition.x - character.transform.position.x;
        var directionY = firePosition.y - character.transform.position.y;

        Vector2 launchDirection = new Vector2(directionX, directionY).normalized;

        float angle = Mathf.Atan2(launchDirection.y, launchDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        projectile.transform.rotation = rotation;
        projectile.MoveInDirection(launchDirection);
    }
}
