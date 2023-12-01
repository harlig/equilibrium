public class ElementalSystem
{
    public ElementalSystem(float baseDamage = 10f)
    {
        this.baseDamage = baseDamage;
    }

    public float Chance { get; set; } = 0f;
    private readonly float baseDamage;
    public float Damage
    {
        get => baseDamage * DamageModifier;
    }

    public float DamageModifier { get; set; } = 1f;
    public float Duration { get; set; } = 5f;
}
