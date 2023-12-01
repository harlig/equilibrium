using UnityEngine;

public class MeleeEnemy : EnemyController
{
    protected override void Start()
    {
        base.Start();
        CreateMeleeWeapon();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!IsDead())
        {
            TryAttack(transform.position, weaponSlotController, player);
        }
    }

    public static void TryAttack(
        Vector3 attackingEnemyPosition,
        WeaponSlotController weaponSlotController,
        PlayerController player,
        float detectionDistance = 1.5f
    )
    {
        // if within 1.5 units of player, start swinging
        if (Vector2.Distance(player.transform.position, attackingEnemyPosition) < detectionDistance)
        {
            weaponSlotController.AttackAtPosition(WeaponController.WeaponType.MELEE);
        }
    }
}
