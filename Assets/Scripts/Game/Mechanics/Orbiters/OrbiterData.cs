using UnityEngine;

public class OrbiterData : MonoBehaviour
{
    [SerializeField]
    private Color color;
    public OrbitSystem.OrbiterType OrbiterType;
    private readonly float damageAmount = 5.0f;

    void Awake()
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<EnemyController>(out var enemy))
        {
            // TODO: this should be replaced with something specific to the orbiter and probably a system per-type of orbiter
            enemy.OnDamageTaken(DamageType.FIRE, damageAmount);
            Debug.Log("Orbiter hit an emey");
        }
    }
}
