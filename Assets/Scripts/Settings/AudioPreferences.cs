using UnityEngine;

public class AudioPreferences
{
    public float mainVolume = 1.0f;

    public AudioPreferences()
    {
        LoadPreferences();
    }

    public void SavePreferences()
    {
        PlayerPrefs.SetFloat("MainVolume", mainVolume);
        PlayerPrefs.Save();
    }

    public void LoadPreferences()
    {
        mainVolume = PlayerPrefs.GetFloat("MainVolume", 1.0f);
    }
}
