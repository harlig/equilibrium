using UnityEngine;

public class RangedEnemy : EnemyController
{
    [SerializeField]
    private ProjectileBehavior projectilePrefab;

    public const float fireInterval = 60; // Number of FixedUpdate calls before firing

    private int currentFireInterval = 0;
    private Vector3 spawnPosition;

    void Start()
    {
        spawnPosition = transform.position;
    }

    void FixedUpdate()
    {
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

            var movementX = spawnPosition.x - transform.position.x;
            var movementY = spawnPosition.y - transform.position.y;
            var rigidBody = gameObject.GetComponent<Rigidbody2D>();

            var newPosition =
                rigidBody.position + new Vector2(movementX, movementY).normalized * MovementSpeed;

            rigidBody.MovePosition(newPosition);
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
        return 20;
    }
}
