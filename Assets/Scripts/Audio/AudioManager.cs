using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] hurtSounds;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayHurtSound()
    {
        audioSource.clip = hurtSounds[Random.Range(0, hurtSounds.Length)];
        audioSource.Play();
    }
}
