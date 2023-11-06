using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBehavior : MonoBehaviour
{
    protected abstract void OnPlayerHit();

    //////////////////////////////////////////////////////////
    //////////////////////////events//////////////////////////
    //////////////////////////////////////////////////////////
    public delegate void OnPlayerHitAction(InteractableBehavior interactable);
    public event OnPlayerHitAction OnInteractableHitPlayer;

    //////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            Debug.Log("a player hit me, an interactable object!");

            OnInteractableHitPlayer?.Invoke(this);
            OnPlayerHit();
        }
    }

    void Start() { }

    void OnDestroy() { }
}
