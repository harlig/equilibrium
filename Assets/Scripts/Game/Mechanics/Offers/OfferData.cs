using UnityEngine;

public abstract class OfferData : MonoBehaviour
{
    public EquilibriumManager.EquilibriumState CorrespondingState;
    public int OfferPool;

    public Color color;

    public abstract float GetOfferValue();

    public enum EffectType
    {
        DAMAGE,
        SPEED
    }

    public static OfferData Create(OfferData prefab, Transform parent)
    {
        var instance = Instantiate(prefab, parent);
        instance.name = prefab.name;
        return instance;
    }

    public string GetName()
    {
        // TODO
        return gameObject.name;
    }
}
