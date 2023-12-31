using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProjectileBehavior : MonoBehaviour
{
    [SerializeField]
    DamageDealerEffect damageDealerEffectPrefab;
    public DamageType DamageType;
    public float Speed = 8.5f;

    private Vector2 direction;
    private bool canMove = false;

    public float DamageAmount;
    private GenericCharacterController CharacterFiredFrom { get; set; }
    private ElementalSystem elementalSystem;

    public static ProjectileBehavior Create(
        ProjectileBehavior prefab,
        Vector3 position,
        Vector2 launchDirection,
        GenericCharacterController firedFrom,
        ElementalSystem elementalSystem
    )
    {
        float angle = Mathf.Atan2(launchDirection.y, launchDirection.x) * Mathf.Rad2Deg - 90;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        var projectile = Instantiate(prefab, position, rotation);

        projectile.MoveInDirection(launchDirection);
        projectile.CharacterFiredFrom = firedFrom;
        projectile.elementalSystem = elementalSystem;

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
        // Check for collision with Player or Enemy and ensure the projectile was not fired by the same type of character
        var playerController = other.GetComponent<PlayerController>();
        var enemyController = other.GetComponent<EnemyController>();

        if (playerController != null && CharacterFiredFrom is not PlayerController)
        {
            DealDamage(playerController, other);
            Destroy(gameObject);
        }
        else if (enemyController != null && CharacterFiredFrom is not EnemyController)
        {
            DealDamage(enemyController, other);
            Destroy(gameObject);
        }
        else if (other.GetComponent<TilemapCollider2D>() != null || ShouldDestroyByOrbiter(other))
        {
            // Destroy the game object if it hits a tilemap or is deflected by an orbiter
            Destroy(gameObject);
        }
    }

    private bool ShouldDestroyByOrbiter(Collider2D collider)
    {
        if (collider.TryGetComponent<OrbiterData>(out _))
        {
            return collider.GetComponentInParent<OrbitSystem>().ShouldDeflectProjectile();
        }
        return false;
    }

    private void DealDamage(GenericCharacterController character, Collider2D otherCollider)
    {
        character.TakeDamage(DamageType, DamageAmount * CharacterFiredFrom.DamageDealtMultiplier);
        if (elementalSystem.Chance > Chance.Get())
        {
            character.ApplyEffectsForDamageType(
                DamageType,
                elementalSystem.Duration,
                elementalSystem.Damage * CharacterFiredFrom.DamageDealtMultiplier
            );
        }

        if (damageDealerEffectPrefab != null)
        {
            DamageDealerEffect damageDealerEffect = DamageDealerEffect
                .Create(damageDealerEffectPrefab, otherCollider, character.transform)
                .GetComponent<DamageDealerEffect>();
            damageDealerEffect.OnHit();
        }
    }

    public void MoveInDirection(Vector2 directionToMove)
    {
        direction = directionToMove;
        canMove = true;
    }
}
