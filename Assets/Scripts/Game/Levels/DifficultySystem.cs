using UnityEngine;

public class DifficultySystem : MonoBehaviour
{
    // TODO: system needs to pick these in order
    public Difficulty GetDifficulty()
    {
        return new Difficulty();
    }

    public class Difficulty
    {
        public float Modifier { get; set; }
    }
    //
}
