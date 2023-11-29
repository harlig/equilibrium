using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class OfferData : MonoBehaviour
{
    public EquilibriumManager.EquilibriumState CorrespondingState;
    public int OfferPool;

    public Color Color;

    public float Value;

    public Sprite Sprite;

    // Delegate type for prerequisites
    public delegate bool PrerequisitesMet(List<OfferData> offer);

    protected PrerequisitesMet PrerequisiteMetFunction;

    public static OfferData Create(OfferData prefab, Transform parent)
    {
        var instance = Instantiate(prefab, parent);
        instance.name = prefab.name;
        return instance;
    }

    public abstract string GetName();
    public abstract string GetValue();
    public abstract string GetHelpText();

    public abstract void ApplyToPlayer(PlayerController player);

    public bool AreAllPrerequisitesMet(AcquisitionManager manager)
    {
        if (PrerequisiteMetFunction == null)
        {
            return true;
        }
        return PrerequisiteMetFunction(manager.OfferAcquisitions);
    }
}
