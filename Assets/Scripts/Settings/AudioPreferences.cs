using UnityEngine;

public class AudioPreferences
{
    public float mainVolume = 1.0f;
    public float musicVolume = 1.0f;
    public float sfxVolume = 1.0f;

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
        mainVolume = PlayerPrefs.GetFloat("MainVolume", 1.0f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
    }
}
