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

    public static OrbiterData Create(OrbiterData prefab, Transform parent, PlayerController player)
    {
        var instance = Instantiate(prefab.gameObject, parent).GetComponent<OrbiterData>();
        instance.player = player;
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
            enemy.OnDamageTaken(damageType, damageAmount);

            // Apply knockback
            if (!enemy.IsDead() && other.attachedRigidbody != null)
            {
                Debug.Log("applying knockback to enemy");
                Vector2 knockbackDirection = (
                    other.transform.position - player.transform.position
                ).normalized;
                Debug.LogFormat("knockback direction {0}", knockbackDirection);
                other.attachedRigidbody.AddForce(
                    knockbackDirection * knockbackStrength,
                    ForceMode2D.Impulse
                );
            }
        }
    }

    public void AddToDamage(float damage)
    {
        damageAmount += damage;
    }
}
