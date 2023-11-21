using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] soundClips; // Array of sound clips
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(int clipIndex)
    {
        if (clipIndex >= 0 && clipIndex < soundClips.Length)
        {
            audioSource.clip = soundClips[clipIndex];
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Clip index out of range");
        }
    }
}
