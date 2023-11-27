using System;
using UnityEngine;

public class RangedEnemy : EnemyController
{
    [SerializeField]
    private ProjectileBehavior projectilePrefab;

    public const float fireInterval = 60; // Number of FixedUpdate calls before firing

    private int currentFireInterval = 0;
    private Vector2 spawnPosition;

    new void Start()
    {
        base.Start();
        spawnPosition = transform.position;
        CreateRangedWeapon();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!IsDead())
        {
            // Increment the current interval count
            currentFireInterval++;

            // Check if it's time to fire
            if (currentFireInterval >= fireInterval)
            {
                FireProjectile();
                currentFireInterval = 0; // Reset the interval count
            }
        }
    }

    protected override void DoMovementActions()
    {
        var rigidBody = gameObject.GetComponent<Rigidbody2D>();

        Vector2 targetPosition = new(spawnPosition.x, spawnPosition.y);
        Vector2 currentPosition = rigidBody.position;
        Vector2 directionToSpawn = (targetPosition - currentPosition).normalized;

        // Calculate the remaining distance to the target
        float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);

        // Check if the enemy is at or has passed the target position
        if (distanceToTarget < 0.1f) // Threshold distance to consider as reached the target
        {
            // Stop the movement
            rigidBody.velocity = Vector2.zero;
        }
        else
        {
            // Apply force towards the target
            rigidBody.AddForce(100 * MovementSpeed * directionToSpawn, ForceMode2D.Force);
        }
    }

    void FireProjectile()
    {
        weaponSlotController.AttackAtPosition(typeof(RangedWeapon), player.transform.position);
    }

    protected override int GetMaxHp()
    {
        return 200;
    }
}
