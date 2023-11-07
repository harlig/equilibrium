using System.Collections.Generic;

public class GameManager
{
    // if you are at level 0, you need 1 xp to level up. if you are at level 1, you need 10, etc.
    public static List<int> XpNeededForLevelUpAtIndex { get; } = new() { 1, 10, 30, 80, 150 };
}
