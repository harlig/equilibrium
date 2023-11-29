using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class ElementalEnemy : MonoBehaviour
{
    [SerializeField]
    private Sprite[] elementalSprites;

    [SerializeField]
    private ProjectileBehavior elementalProjectilePrefab;
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
        GetComponentInChildren<RangedWeapon>().OverrideProjectile = elementalProjectilePrefab;
    }

    private void DectivateElementalEffect()
    {
        elementalActivated = false;
        characterAnimator.StopUsingCustomSprites();
        GetComponentInChildren<RangedWeapon>().OverrideProjectile = null;
    }
}
