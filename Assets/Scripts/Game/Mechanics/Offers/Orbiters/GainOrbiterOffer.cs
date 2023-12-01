using System.Collections.Generic;

public class GainOrbiterOffer : SpecificTypeOfOrbiterOffer
{
    public override void ApplyToOrbitSystem(OrbitSystem orbitSystem)
    {
        orbitSystem.AddOrbiter(orbiterType);
    }

    public override string GetHelpText()
    {
        return $"Gain {Value} {orbiterType.ToString().ToLower()} orbiter{(Value > 1 ? "s" : "")} which deal damage to enemies";
    }

    // can always get a gain orbiter offer
    // TODO: maybe you should actually only be able to have like 8 of them?
    public override bool PrerequisitesMet(List<OfferData> offers)
    {
        return true;
    }
}
