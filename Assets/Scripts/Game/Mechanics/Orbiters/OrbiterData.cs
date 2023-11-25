using UnityEngine;

public class OrbiterData : MonoBehaviour
{
    [SerializeField]
    private Color color;
    public OrbitSystem.OrbiterType OrbiterType;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<EnemyController>() != null)
        {
            Debug.Log("Orbiter hit an emey");
        }
    }
}
