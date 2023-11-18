using UnityEngine;
using UnityEngine.Tilemaps;

public class ProjectileBehavior : MonoBehaviour
{
    public float Speed = 8.5f;

    private Vector2 direction;
    private bool canMove = false;

    public float DamageAmount = 7.0f;

    void FixedUpdate()
    {
        if (canMove)
        {
            transform.position += Speed * Time.fixedDeltaTime * (Vector3)direction;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            Destroy(gameObject);
        }
        else if (other.GetComponent<TilemapCollider2D>() != null)
        {
            Destroy(gameObject);
        }
    }

    public void MoveInDirection(Vector2 directionToMove)
    {
        direction = directionToMove;
        canMove = true;
    }
}
