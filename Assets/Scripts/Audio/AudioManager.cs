using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] hurtSounds;

    [SerializeField]
    private AudioClip[] weaponSwingSounds;

    [SerializeField]
    private AudioClip musicTrack;

    private AudioSource sfxAudioSource; // AudioSource for sound effects
    private AudioSource musicAudioSource; // Separate AudioSource for music
    private AudioPreferences audioPreferences;

    void Awake()
    {
        // Get components for both AudioSources
        AudioSource[] audioSources = GetComponents<AudioSource>();
        sfxAudioSource = audioSources[0];
        musicAudioSource = audioSources[1]; // Ensure you have two AudioSources attached

        audioPreferences = new();
        UpdateAudioLevels();
    }

    void OnEnable()
    {
        SettingsManager.OnSettingsUpdated += UpdateAudioLevels;
    }

    void OnDisable()
    {
        SettingsManager.OnSettingsUpdated -= UpdateAudioLevels;
    }

    void UpdateAudioLevels()
    {
        audioPreferences.LoadPreferences();
        sfxAudioSource.volume = audioPreferences.sfxVolume * audioPreferences.mainVolume;
        musicAudioSource.volume = audioPreferences.musicVolume * audioPreferences.mainVolume;
    }

    public void PlayEffect(AudioClip effectClip)
    {
        if (sfxAudioSource.isPlaying)
        {
            sfxAudioSource.Stop();
        }

        UpdateAudioLevels();
        sfxAudioSource.clip = effectClip;
        sfxAudioSource.Play();
    }

    public void PlayHurtSound()
    {
        PlayEffect(hurtSounds[Random.Range(0, hurtSounds.Length)]);
    }

    public void PlayMusic()
    {
        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
        }

        UpdateAudioLevels();

        musicAudioSource.clip = musicTrack;
        musicAudioSource.loop = true;
        musicAudioSource.Play();
    }
}
