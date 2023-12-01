using UnityEngine;

public class BossEnemy : EnemyController
{
    private float switchBehaviorTimer = 0f;
    private bool isFollowingPlayer = true;
    private const float SwitchBehaviorIntervalSeconds = 10f;

    protected override void Start()
    {
        base.Start();
        CreateMeleeWeapon();
        CreateRangedWeapon();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Update the timer
        switchBehaviorTimer += Time.deltaTime;

        // Check if it's time to switch behavior
        if (switchBehaviorTimer >= SwitchBehaviorIntervalSeconds)
        {
            isFollowingPlayer = !isFollowingPlayer;
            switchBehaviorTimer = 0f;
        }

        // Execute the appropriate behavior
        if (isFollowingPlayer)
        {
            FollowPlayer(player);
        }
        else
        {
            PatrolArea(containingRoom.GenerateRandomEnemyWalkableNode().WorldPosition);
        }

        // Combat behavior
        if (!IsDead())
        {
            MeleeEnemy.TryAttack(transform.position, weaponSlotController, player);
            RangedEnemy.TryFireProjectile(weaponSlotController, player);
        }
    }
}
