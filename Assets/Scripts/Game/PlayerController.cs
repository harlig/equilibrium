using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float MOVEMENT_SPEED = 0.1f;

    void FixedUpdate()
    {
        var movement = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.A))
        {
            movement.x -= 1.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement.x += 1.0f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            movement.y += 1.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement.y -= 1.0f;
        }

        var rigid_body = gameObject.GetComponent<Rigidbody2D>();
        var new_position = rigid_body.position + movement.normalized * MOVEMENT_SPEED;

        rigid_body.MovePosition(new_position);
    }
}
