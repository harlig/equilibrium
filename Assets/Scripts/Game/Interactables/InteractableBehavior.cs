using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBehavior : MonoBehaviour
{
    [SerializeField]
    private AudioClip onPlayerHitAudio;

    // This can be overridden if the interactable needs to do something when it's hit. However if the behavior is in the context of the floor, the handling should be in FloorManager.OnInteractableHitPlayer
    protected abstract void OnPlayerHit(PlayerController player);
    protected abstract string GetHelpText();

    protected virtual bool PlayerCanInteractWithThis { get; set; } = false;
    PlayerController playerController;
    HeadsUpDisplayController hudController;
    AudioManager audioManager;

    void Start()
    {
        hudController = GetComponentInParent<GameManager>().HudController;
        audioManager = GetComponentInParent<GameManager>().AudioManager;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            if (PlayerCanInteractWithThis)
            {
                OnPlayerHit(playerController);
                hudController.DisableInteractableHelpText();
                PlayerCanInteractWithThis = false;
                audioManager.PlayEffect(onPlayerHitAudio);
            }
        }
    }

    void PlayerHitInteractable(GameObject other)
    {
        PlayerCanInteractWithThis = true;
        playerController = other.GetComponent<PlayerController>();
        hudController.SetInteractableHelpText(GetHelpText());
    }

    void PlayerLeftInteractable()
    {
        PlayerCanInteractWithThis = false;
        playerController = null;
        hudController.DisableInteractableHelpText();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            PlayerHitInteractable(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            PlayerLeftInteractable();
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            PlayerHitInteractable(other.gameObject);
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            PlayerLeftInteractable();
        }
    }

    public class PlayerAndCameraLocation
    {
        public Vector2 PlayerLocation { get; set; }
        public Tuple<Vector2, Vector2> CameraBounds { get; set; }
    }
}
