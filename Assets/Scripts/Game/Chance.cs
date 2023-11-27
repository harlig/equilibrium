using UnityEngine;

public static class Chance
{
    private static readonly float MAX_CHANCE = 1.0f;

    public static float Get()
    {
        return Random.Range(0, MAX_CHANCE);
    }
}
