using System;
using UnityEngine;

public class RangedEnemy : EnemyController
{
    [SerializeField]
    private ProjectileBehavior projectilePrefab;

    new void Start()
    {
        base.Start();
        StayAtSpawn();
        CreateRangedWeapon();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!IsDead())
        {
            // we can just constantly do this, the weapon will limit how fast it can be done
            FireProjectile();
        }
    }

    // protected override void DoMovementActions()
    // {
    //     var rigidBody = gameObject.GetComponent<Rigidbody2D>();

    //     Vector2 targetPosition = new(spawnPosition.x, spawnPosition.y);
    //     Vector2 currentPosition = rigidBody.position;
    //     Vector2 directionToSpawn = (targetPosition - currentPosition).normalized;

    //     // Calculate the remaining distance to the target
    //     float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);

    //     // Check if the enemy is at or has passed the target position
    //     if (distanceToTarget < 0.1f) // Threshold distance to consider as reached the target
    //     {
    //         // Stop the movement
    //         rigidBody.velocity = Vector2.zero;
    //     }
    //     else
    //     {
    //         // Apply force towards the target
    //         rigidBody.AddForce(100 * MovementSpeed * directionToSpawn, ForceMode2D.Force);
    //     }
    // }

    void FireProjectile()
    {
        weaponSlotController.AttackAtPosition(
            WeaponController.WeaponType.RANGED,
            player.transform.position
        );
    }

    protected override int GetMaxHp()
    {
        return 200;
    }
}
