using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponAnimator : MonoBehaviour
{
    [SerializeField]
    private Sprite originalSprite;

    [SerializeField]
    private Sprite[] attackSprites;

    private int updatesSinceLastSpriteChange = 0;

    // TODO: this should be based on WeaponController.WeaponSpeed
    [SerializeField]
    private float animationSpeed = 3;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastPosition;
    private Vector2 desiredPosition;
    private int currentSpriteIndex = 0;
    private bool isSwinging = false;
    private Action afterSwingAction;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        lastPosition = transform.position;
    }

    // TODO: I think this should be `Update` but it won't work for some reason
    void FixedUpdate()
    {
        if (!isSwinging)
        {
            return;
        }
        Vector2 currentPosition = transform.position;
        lastPosition = currentPosition;

        AnimateSwing();
    }

    public void DoSwing(Vector2 desiredPosition, Action afterSwingAction)
    {
        isSwinging = true;
        this.desiredPosition = desiredPosition;
        this.afterSwingAction = afterSwingAction;
    }

    void AnimateSwing()
    {
        updatesSinceLastSpriteChange++;

        if (updatesSinceLastSpriteChange >= animationSpeed)
        {
            if (currentSpriteIndex == attackSprites.Length - 1)
            {
                isSwinging = false;
                currentSpriteIndex = 0;
                spriteRenderer.sprite = originalSprite;
                afterSwingAction?.Invoke();
            }
            if (isSwinging)
            {
                currentSpriteIndex = (currentSpriteIndex + 1) % attackSprites.Length;
                spriteRenderer.sprite = attackSprites[currentSpriteIndex];
            }
            updatesSinceLastSpriteChange = 0;
        }
    }
}
