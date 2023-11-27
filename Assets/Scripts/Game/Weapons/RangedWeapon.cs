using UnityEngine;

public class RangedWeapon : WeaponController
{
    [SerializeField]
    private ProjectileBehavior projectilePrefab;
    private WeaponAnimator weaponAnimator;
    public ElementalSystem elementalSystem = new();

    public override bool ShouldRotateToMousePosition => true;

    public override DamageType DamageType => DamageType.ICE;

    // this is just a modifier for the projectile damage
    public override float BaseDamageAmount => 0f;
    public override WeaponType Type => WeaponType.RANGED;

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

        weaponAnimator.DoAnimate(
            AttackSpeed,
            () =>
            {
                FireProjectile(position);
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
            projectilePrefab,
            transform.localToWorldMatrix.GetPosition(),
            launchDirection,
            character,
            elementalSystem,
            BaseDamageAmount
        );
    }
}
