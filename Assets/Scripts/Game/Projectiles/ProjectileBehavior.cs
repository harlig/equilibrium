using UnityEngine;
using UnityEngine.Tilemaps;

public class ProjectileBehavior : MonoBehaviour
{
    public float Speed = 8.5f;

    private Vector2 direction;
    private bool canMove = false;

    public float DamageAmount;
    private GenericCharacterController CharacterFiredFrom { get; set; }

    public static ProjectileBehavior Create(
        ProjectileBehavior prefab,
        Vector3 position,
        Vector2 launchDirection,
        GenericCharacterController firedFrom,
        float extraDamageAmount = 0f
    )
    {
        float angle = Mathf.Atan2(launchDirection.y, launchDirection.x) * Mathf.Rad2Deg - 90;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        var projectile = Instantiate(prefab, position, rotation);

        projectile.MoveInDirection(launchDirection);
        projectile.CharacterFiredFrom = firedFrom;
        projectile.DamageAmount += extraDamageAmount;

        return projectile;
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            transform.position += Speed * Time.fixedDeltaTime * (Vector3)direction;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (
            other.GetComponent<PlayerController>() != null
            && CharacterFiredFrom is not PlayerController
        )
        {
            other.GetComponent<PlayerController>().DealDamage(DamageType.ICE, DamageAmount);
            Destroy(gameObject);
        }
        else if (
            other.GetComponent<EnemyController>() != null
            && CharacterFiredFrom is not EnemyController
        )
        {
            other.GetComponent<EnemyController>().DealDamage(DamageType.ICE, DamageAmount);
            Destroy(gameObject);
        }
        else if (other.GetComponent<TilemapCollider2D>() != null)
        {
            Destroy(gameObject);
        }
        else if (other.GetComponent<OrbiterData>() != null)
        {
            if (other.GetComponentInParent<OrbitSystem>().ShouldDeflectProjectile())
            {
                Destroy(gameObject);
            }
        }
    }

    public void MoveInDirection(Vector2 directionToMove)
    {
        direction = directionToMove;
        canMove = true;
    }
}
