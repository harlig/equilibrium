using UnityEngine;

public class OfferData : MonoBehaviour
{
    public EquilibriumManager.EquilibriumState CorrespondingState;
    public int OfferPool;

    public OfferData(EquilibriumManager.EquilibriumState state, int pool)
    {
        CorrespondingState = state;
        OfferPool = pool;
    }

    public static OfferData Create(OfferData prefab)
    {
        var instance = Instantiate(prefab);
        instance.name = prefab.name;
        return instance;
    }

    public string GetName()
    {
        // TODO
        return gameObject.name;
    }
}
