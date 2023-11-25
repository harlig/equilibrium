using UnityEngine;

public class OrbiterData : MonoBehaviour
{
    [SerializeField]
    private Color color;
    private DamageType damageType;
    public OrbitSystem.OrbiterType OrbiterType;
    private float damageAmount = 5.0f;

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
            enemy.OnDamageTaken(damageType, damageAmount);
        }
    }

    public void AddToDamage(float damage)
    {
        damageAmount += damage;
    }
}
