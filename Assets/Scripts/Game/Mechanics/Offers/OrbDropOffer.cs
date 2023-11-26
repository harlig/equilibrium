using UnityEngine;

public class OrbDropOffer : OfferData
{
    [SerializeField]
    private OrbController.OrbType orbType;

    [SerializeField]
    private OrbDropper orbDropperPrefab;

    public override string GetHelpText()
    {
        return $"Drop ({Value} * player level) {orbType} orbs";
    }

    public override string GetName()
    {
        return gameObject.name;
    }

    public override string GetValue()
    {
        return $"{Value}";
    }

    public void DropOrbs(PlayerController player)
    {
        var orbDropper = Instantiate(orbDropperPrefab, player.CurrentRoom.transform);
        orbDropper.transform.position = player.transform.position;
        orbDropper.DoOrbDrop(
            orbType,
            0,
            player.CurrentRoom,
            Mathf.RoundToInt(Value * player.PlayerLevel)
        );
    }
}
