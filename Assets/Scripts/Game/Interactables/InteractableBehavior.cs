using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBehavior : MonoBehaviour
{
    // This can be overridden if the interactable needs to do something when it's hit. However if the behavior is in the context of the level, the handling should be in LevelManager.OnInteractableHitPlayer
    protected virtual void OnPlayerHit(PlayerController player) { }

    //////////////////////////////////////////////////////////
    //////////////////////////events//////////////////////////
    //////////////////////////////////////////////////////////
    public delegate void OnPlayerHitAction(InteractableBehavior interactable);
    public event OnPlayerHitAction OnInteractableHitPlayer;

    //////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////

    // if an interactable is getting hit twice, does it have two rigidbodies?
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            OnInteractableHitPlayer?.Invoke(this);
            OnPlayerHit(other.GetComponent<PlayerController>());
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            OnInteractableHitPlayer?.Invoke(this);
            OnPlayerHit(other.gameObject.GetComponent<PlayerController>());
        }
    }
}
