using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class ElementalEnemy : MonoBehaviour
{
    [SerializeField]
    private Sprite[] elementalSprites;
    private CharacterAnimator characterAnimator;
    private bool elementalActivated = false;

    void Awake()
    {
        // this uses a weird access pattern because by requiring an enemy controller we require a character animator
        characterAnimator = GetComponent<EnemyController>().GetComponent<CharacterAnimator>();
    }

    public void ToggleElementalEffect()
    {
        elementalActivated = !elementalActivated;
        if (elementalActivated)
        {
            ActivateElementalEffect();
        }
        else
        {
            DectivateElementalEffect();
        }
    }

    private void ActivateElementalEffect()
    {
        elementalActivated = true;
        characterAnimator.UseCustomSpritesForWalkAndIdle(elementalSprites);
    }

    private void DectivateElementalEffect()
    {
        elementalActivated = false;
        characterAnimator.StopUsingCustomSprites();
    }
}
