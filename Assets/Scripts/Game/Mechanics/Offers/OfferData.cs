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
}
