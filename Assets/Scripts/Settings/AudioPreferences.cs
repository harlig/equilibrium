using UnityEngine;

public class AudioPreferences
{
    const float MAIN_VOLUME_DEFAULT = 0.5f;
    const float MUSIC_VOLUME_DEFAULT = 1.0f;
    const float SFX_VOLUME_DEFAULT = 0.2f;
    public float mainVolume = MAIN_VOLUME_DEFAULT;
    public float musicVolume = MUSIC_VOLUME_DEFAULT;
    public float sfxVolume = SFX_VOLUME_DEFAULT;

    public AudioPreferences()
    {
        LoadPreferences();
    }

    public void SavePreferences()
    {
        PlayerPrefs.SetFloat("MainVolume", mainVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    public void LoadPreferences()
    {
        mainVolume = PlayerPrefs.GetFloat("MainVolume", MAIN_VOLUME_DEFAULT);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", MUSIC_VOLUME_DEFAULT);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", SFX_VOLUME_DEFAULT);
    }
}
