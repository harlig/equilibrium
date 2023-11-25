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
        var newProjectile = Instantiate(
            projectilePrefab,
            transform.localToWorldMatrix.GetPosition(),
            Quaternion.identity
        );
        var directionX = player.transform.position.x - transform.position.x;
        var directionY = player.transform.position.y - transform.position.y;

        Vector2 launchDirection = new Vector2(directionX, directionY).normalized;

        // Calculate the rotation in 2D space to align with the launch direction and adjust by 90 degrees to handle long projectile
        float angle = Mathf.Atan2(launchDirection.y, launchDirection.x) * Mathf.Rad2Deg - 90;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        newProjectile.transform.rotation = rotation;

        newProjectile.MoveInDirection(launchDirection);
    }

    protected override int GetMaxHp()
    {
        return 200;
    }
}
