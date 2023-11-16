using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBehavior : MonoBehaviour
{
    // This can be overridden if the interactable needs to do something when it's hit. However if the behavior is in the context of the floor, the handling should be in FloorManager.OnInteractableHitPlayer
    protected abstract void OnPlayerHit(PlayerController player);

    // if an interactable is getting hit twice, does it have two rigidbodies?
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            OnPlayerHit(other.GetComponent<PlayerController>());
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            OnPlayerHit(other.gameObject.GetComponent<PlayerController>());
        }
    }

    // TODO need to add OnTriggerExit and OnCollisionExit to clear the HUD

    public class PlayerAndCameraLocation
    {
        public Vector2 PlayerLocation { get; set; }
        public Tuple<Vector2, Vector2> CameraBounds { get; set; }
    }
}
