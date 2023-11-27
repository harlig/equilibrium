using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpeedOffer : OfferData
{
    [SerializeField]
    private WeaponController.WeaponType affectedWeaponType;

    public override string GetHelpText()
    {
        return $"Increases the attack speed of your {affectedWeaponType.ToString().ToLower()} weapon by {Value}";
    }

    public override string GetName()
    {
        return gameObject.name;
    }

    public override string GetValue()
    {
        return $"{Value}";
    }
}
