using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public AudioPreferences audioPreferences;

    void Start()
    {
        audioPreferences = new();
    }

    public void OnVolumeChanged(float value)
    {
        audioPreferences.mainVolume = value;
        audioPreferences.SavePreferences();
    }
}
