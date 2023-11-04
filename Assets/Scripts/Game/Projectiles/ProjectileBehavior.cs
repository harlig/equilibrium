using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float Speed = 8.5f;

    private Vector2 direction;
    private bool canMove = false;

    void FixedUpdate()
    {
        if (canMove)
        {
            transform.position += Speed * Time.deltaTime * (Vector3)direction;
        }
    }

    void OnCollisionEnter2D(Collision2D other) { }

    public void MoveInDirection(Vector2 directionToMove)
    {
        direction = directionToMove;
        canMove = true;
    }
}
