using UnityEngine;

public abstract class OfferData : MonoBehaviour
{
    public EquilibriumManager.EquilibriumState CorrespondingState;
    public int OfferPool;

    public Color color;

    public abstract float GetValue();

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
        // TODO: maybe a fixed name per instance that's forced to be defined in editor? I think that's ok
        return gameObject.name;
    }
}
