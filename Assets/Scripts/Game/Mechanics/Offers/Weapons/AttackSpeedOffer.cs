using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpeedOffer : OfferData
{
    [SerializeField]
    private WeaponController.WeaponType affectedWeaponType;

    public override void ApplyToPlayer(PlayerController player)
    {
        var weapon = player.GetWeaponOfType(affectedWeaponType);
        if (weapon == null)
        {
            Debug.LogErrorFormat(
                "We offered an attack speed reward for weapon type {0} when one doesn't exist on the player",
                affectedWeaponType
            );
            return;
        }
        weapon.IncreaseAttackSpeedMultiplier(Value);
    }

    public override string GetHelpText()
    {
        return $"Increases the base attack speed of your {affectedWeaponType.ToString().ToLower()} weapon by {GetValue()}%";
    }

    public override string GetName()
    {
        return $"{affectedWeaponType.ToString().ToLower()} {gameObject.name} increase";
    }

    public override string GetValue()
    {
        return $"{string.Format("{0:N0}", Value * 100)}";
    }
}
