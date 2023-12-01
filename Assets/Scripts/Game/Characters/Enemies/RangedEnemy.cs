using System;
using UnityEngine;

public class RangedEnemy : EnemyController
{
    private Vector2 spawnPosition;

    new void Start()
    {
        base.Start();
        spawnPosition = transform.position;
        CreateRangedWeapon();
    }

    private float elementalEffectTimer = 0f;
    private const float ElementalEffectInterval = 5f; // 5 seconds

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!IsDead())
        {
            TryFireProjectile(weaponSlotController, player);
        }

        // Update the timer
        elementalEffectTimer += Time.fixedDeltaTime;

        // Check if 5 seconds have passed
        if (elementalEffectTimer >= ElementalEffectInterval)
        {
            if (!IsDead() && TryGetComponent<ElementalEnemy>(out var elementalEnemy))
            {
                elementalEnemy.ToggleElementalEffect();
            }

            // Reset the timer
            elementalEffectTimer = 0f;
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

    public static void TryFireProjectile(
        WeaponSlotController weaponSlotController,
        PlayerController player
    )
    {
        weaponSlotController.AttackAtPosition(
            WeaponController.WeaponType.RANGED,
            () => player.transform.position
        );
    }
}
