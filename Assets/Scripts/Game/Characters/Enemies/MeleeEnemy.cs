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
            // if within 1.5 units of player, start swinging
            if (Vector2.Distance(player.transform.position, transform.position) < 1.5f)
            {
                weaponSlotController.AttackAtPosition(WeaponController.WeaponType.MELEE);
            }
        }
    }
}
