using UnityEngine;

public class OrbiterData : MonoBehaviour
{
    [SerializeField]
    private Color color;
    private DamageType damageType;
    public OrbitSystem.OrbiterType OrbiterType;
    private float damageMultiplier = 1f;
    private PlayerController player;
    private OrbitSystem orbitSystem;
    private const float BASE_DAMAGE_AMOUNT = 5.0f;
    private const float KNOCKBACK_STRENGTH = 10.0f;

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
            enemy.TakeDamage(damageType, BASE_DAMAGE_AMOUNT * damageMultiplier);

            // TODO: knockback is broken
            if (!enemy.IsDead() && other.attachedRigidbody != null)
            {
                Vector2 knockbackDirection = (
                    other.transform.position - player.transform.position
                ).normalized;
                enemy.ApplyKnockback(knockbackDirection, KNOCKBACK_STRENGTH);
            }

            if (orbitSystem.ChanceOfOrbiterTypeDoingElementalEffect[OrbiterType] > Chance.Get())
            {
                float damage = 0;
                if (damageType == DamageType.FIRE)
                {
                    damage = 3f;
                }
                enemy.ApplyEffectsForDamageType(damageType, 3.0f, damage);
            }
        }
    }

    public void AddToDamage(float damageMultiplierAddition)
    {
        damageMultiplier += damageMultiplierAddition;
    }
}
