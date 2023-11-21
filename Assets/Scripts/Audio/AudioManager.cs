using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] hurtSounds;

    [SerializeField]
    private AudioClip[] weaponSwingSounds;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void DoPlaySound(AudioClip sound)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = sound;
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
            audioSource.clip = sound;
            audioSource.Play();
        }
    }

    public void PlayHurtSound()
    {
        DoPlaySound(hurtSounds[Random.Range(0, hurtSounds.Length)]);
    }
}
