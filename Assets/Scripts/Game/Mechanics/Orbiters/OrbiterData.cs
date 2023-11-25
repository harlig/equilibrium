using UnityEngine;

public class OrbiterData : MonoBehaviour
{
    [SerializeField]
    private Color color;

    // just support fire orbiter
    // this gameobject will have a trigger collider so it'll implement OnTriggerEnter2D where it can deal damage or apply its effect to enemies

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<EnemyController>() != null)
        {
            Debug.Log("Orbiter hit an emey");
        }
    }
}
