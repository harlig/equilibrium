using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public AudioPreferences audioPreferences;

    public delegate void VolumeChangedAction();
    public static event VolumeChangedAction OnSettingsUpdated;

    void Awake()
    {
        audioPreferences = new();
    }

    public void OnMainVolumeChanged(float value)
    {
        audioPreferences.mainVolume = value;
        audioPreferences.SavePreferences();
        OnSettingsUpdated?.Invoke();
    }

    public void OnSFXVolumeChanged(float value)
    {
        audioPreferences.sfxVolume = value;
        audioPreferences.SavePreferences();
        OnSettingsUpdated?.Invoke();
    }

    public void OnMusicVolumeChanged(float value)
    {
        audioPreferences.musicVolume = value;
        audioPreferences.SavePreferences();
        OnSettingsUpdated?.Invoke();
    }
}
