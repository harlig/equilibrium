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
        var Movement = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.A))
        {
            Movement.x -= 1.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Movement.x += 1.0f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            Movement.y += 1.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Movement.y -= 1.0f;
        }

        var RigidBody = gameObject.GetComponent<Rigidbody2D>();
        var NewPosition = RigidBody.position + Movement.normalized * MOVEMENT_SPEED;

        RigidBody.MovePosition(NewPosition);
    }
}
