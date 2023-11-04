using UnityEngine;

public class RangedEnemy : EnemyController
{
    [SerializeField]
    private ProjectileBehavior projectilePrefab;

    public const float fireInterval = 60; // Number of FixedUpdate calls before firing

    private int currentInterval = 0;

    void FixedUpdate()
    {
        // Increment the current interval count
        currentInterval++;

        // Check if it's time to fire
        if (currentInterval >= fireInterval)
        {
            FireProjectile();
            currentInterval = 0; // Reset the interval count
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
}
