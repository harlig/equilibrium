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
    public bool ElementalAlwaysActivated { private get; set; } = false;

    void Awake()
    {
        // this uses a weird access pattern because by requiring an enemy controller we require a character animator
        characterAnimator = GetComponent<EnemyController>().GetComponent<CharacterAnimator>();
    }

    void Start()
    {
        ToggleElementalEffect(elementalActivated);
    }

    public void ToggleElementalEffect(bool? elementalActivatedOverride = null)
    {
        elementalActivated = !elementalActivated;
        if (elementalActivatedOverride != null)
        {
            elementalActivated = elementalActivatedOverride.Value;
        }

        if (elementalActivated || ElementalAlwaysActivated)
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
