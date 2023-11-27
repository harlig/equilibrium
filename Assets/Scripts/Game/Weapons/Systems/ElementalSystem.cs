public class ElementalSystem
{
    public ElementalSystem() { }

    public ElementalSystem(float baseDamage)
    {
        Damage = baseDamage;
    }

    public float Chance { get; set; } = 0f;
    public float Damage { get; set; } = 0f;
    public float Duration { get; set; } = 5f;
}
