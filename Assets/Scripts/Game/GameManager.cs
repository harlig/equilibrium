using System.Collections.Generic;

public class GameManager
{
    // if you are at level 0, you need 1 xp to level up. if you are at level 1, you need 10, etc.
    public static List<int> XpNeededForLevelUpAtIndex { get; } =
        new() { 1, 2, 3, 4, 5, 11, 21, 31, 41, 51, 61, 71, 81, 91, 101 };
}
