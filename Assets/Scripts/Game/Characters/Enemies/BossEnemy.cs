using UnityEngine;

public class BossEnemy : EnemyController
{
    private float switchBehaviorTimer = 0f;
    private bool isFollowingPlayer;
    private const float SwitchBehaviorIntervalSeconds = 8f;

    // TODO: add something that is like ExtraActions that can be called, so the boss can spawn other enemies in the room but it would be set by the room manager

    protected override void Start()
    {
        base.Start();
        CreateMeleeWeapon();
        CreateRangedWeapon();

        PatrolArea(containingRoom.GenerateRandomEnemyWalkableNode(player).WorldPosition);
        isFollowingPlayer = false;

        // boss is strong af
        SetMaxHp(MaxHp * 10);
        AddToDamageDealtModifier(10f);
        AddToMovementSpeedModifier(1.5f);

        if (GetComponent<ElementalEnemy>() != null)
        {
            GetComponent<ElementalEnemy>().ElementalAlwaysActivated = true;
        }
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
            if (isFollowingPlayer)
            {
                FollowPlayer(player);
            }
            else
            {
                Debug.LogFormat("Patrol time {0}", containingRoom);
                PatrolArea(
                    containingRoom.GenerateRandomEnemyWalkableNode(player).WorldPosition,
                    // cannot detect player
                    0f
                );
            }
            switchBehaviorTimer = 0f;
        }

        // Combat behavior
        if (!IsDead())
        {
            if (isFollowingPlayer)
            {
                MeleeEnemy.TryAttack(transform.position, weaponSlotController, player);
            }
            else
            {
                RangedEnemy.TryFireProjectile(weaponSlotController, player);
            }
        }
    }
}
