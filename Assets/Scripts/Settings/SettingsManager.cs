using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public delegate void VolumeChangedAction();
    public static event VolumeChangedAction OnSettingsUpdated;

    public void OnMainVolumeChanged(float value)
    {
        var audioPreferences = new AudioPreferences { mainVolume = value };
        audioPreferences.SavePreferences();
        OnSettingsUpdated?.Invoke();
    }

    public void OnSFXVolumeChanged(float value)
    {
        var audioPreferences = new AudioPreferences { sfxVolume = value };
        audioPreferences.SavePreferences();
        OnSettingsUpdated?.Invoke();
    }

    public void OnMusicVolumeChanged(float value)
    {
        var audioPreferences = new AudioPreferences { musicVolume = value };
        audioPreferences.SavePreferences();
        OnSettingsUpdated?.Invoke();
    }
}
