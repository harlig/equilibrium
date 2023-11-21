using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] hurtSounds;

    [SerializeField]
    private AudioClip[] weaponSwingSounds;

    private AudioSource audioSource;
    private AudioPreferences audioPreferences;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioPreferences = new();
    }

    public void DoPlaySound(AudioClip sound)
    {
        audioPreferences.LoadPreferences();
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.clip = sound;
        audioSource.volume = audioPreferences.mainVolume;
        audioSource.Play();
    }

    public void PlayHurtSound()
    {
        DoPlaySound(hurtSounds[Random.Range(0, hurtSounds.Length)]);
    }
}
