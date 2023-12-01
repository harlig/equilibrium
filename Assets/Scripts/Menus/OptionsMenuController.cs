using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour
{
    [SerializeField]
    private Slider mainVolume;

    [SerializeField]
    private Slider musicVolume;

    [SerializeField]
    private Slider sfxVolume;

    void Start()
    {
        SetSliderInitialValue();
    }

    public void Show()
    {
        SetSliderInitialValue();
        gameObject.SetActive(true);
    }

    public void OnBackButtonClicked()
    {
        Hide();
        if (GetComponentInParent<GameManager>() != null)
        {
            GetComponent<GameManager>().AudioManager.PlayMenuClickSound();
        }
        else if (GetComponentInParent<MainMenuController>() != null)
        {
            GetComponent<MainMenuController>().AudioManager.PlayMenuClickSound();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void SetSliderInitialValue()
    {
        // TODO: change this wtf lol
        var audioPreferences = new AudioPreferences();
        mainVolume.value = audioPreferences.mainVolume;
        musicVolume.value = audioPreferences.musicVolume;
        sfxVolume.value = audioPreferences.sfxVolume;
    }
}
