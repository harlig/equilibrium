using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class EquilibriumScaleBasketAnimator : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> sprites;
    private Image targetImage;
    private const float frameDuration = 0.1f;

    void Awake()
    {
        targetImage = GetComponent<Image>();
    }

    private void Start()
    {
        if (sprites.Count > 0)
        {
            StartCoroutine(AnimateSprites());
        }
    }

    private IEnumerator AnimateSprites()
    {
        while (true) // Loop indefinitely
        {
            foreach (Sprite sprite in sprites)
            {
                targetImage.sprite = sprite; // Set the current sprite
                yield return new WaitForSeconds(frameDuration); // Wait for the frame duration
            }
        }
    }
}
