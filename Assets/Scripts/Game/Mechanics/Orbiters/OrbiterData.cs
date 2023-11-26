using UnityEngine;

public class OrbiterData : MonoBehaviour
{
    [SerializeField]
    private Color color;
    private DamageType damageType;
    public OrbitSystem.OrbiterType OrbiterType;
    private float damageAmount = 5.0f;
    private readonly float knockbackStrength = 10.0f;
    private PlayerController player;
    private OrbitSystem orbitSystem;

    public static OrbiterData Create(
        OrbiterData prefab,
        OrbitSystem orbitSystem,
        PlayerController player
    )
    {
        var instance = Instantiate(prefab.gameObject, orbitSystem.transform)
            .GetComponent<OrbiterData>();
        instance.player = player;
        instance.orbitSystem = orbitSystem;
        return instance;
    }

    void Awake()
    {
        GetComponent<SpriteRenderer>().color = color;
        damageType = OrbiterType switch
        {
            OrbitSystem.OrbiterType.FIRE => DamageType.FIRE,
            OrbitSystem.OrbiterType.ICE => DamageType.ICE,
            _
                => throw new System.Exception(
                    $"Unhandled damage type for orbiter type {OrbiterType}"
                ),
        };
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<EnemyController>(out var enemy))
        {
            // TODO: this should be replaced with something specific to the orbiter and probably a system per-type of orbiter
            enemy.DealDamage(damageType, damageAmount);

            // TODO: knockback is broken
            if (!enemy.IsDead() && other.attachedRigidbody != null)
            {
                Vector2 knockbackDirection = (
                    other.transform.position - player.transform.position
                ).normalized;
                enemy.ApplyKnockback(knockbackDirection, knockbackStrength);
            }

            if (
                orbitSystem.ChanceOfOrbiterTypeDoingElementalEffect[OrbiterType]
                > Random.Range(0, 1.0f)
            )
            {
                // chance for DOT
                enemy.ApplyDamageOverTime(damageType, 3.0f, 3.0f);
            }
        }
    }

    public void AddToDamage(float damage)
    {
        damageAmount += damage;
    }
}
