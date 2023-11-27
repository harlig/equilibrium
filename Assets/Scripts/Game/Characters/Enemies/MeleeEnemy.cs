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
            // if within 2 units of player, start swinging
            if (Vector2.Distance(player.transform.position, transform.position) < 2.0f)
            {
                weaponSlotController.AttackAtPosition(WeaponController.WeaponType.MELEE);
            }
        }
    }
}
