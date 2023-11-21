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
        if (!audioSource.isPlaying)
        {
            audioSource.clip = hurtSounds[Random.Range(0, hurtSounds.Length)];
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
            audioSource.clip = hurtSounds[Random.Range(0, hurtSounds.Length)];
            audioSource.Play();
        }
    }
}
