using System.Collections.Generic;
using UnityEngine;

public class DifficultySystem : MonoBehaviour
{
    public FloorDifficulty GetFloorDifficulty()
    {
        var floorDifficulty = new FloorDifficulty();
        // TODO: how to register
        floorDifficulty.RoomDifficulties.Add(new Difficulty { OverallModifier = 1 });

        return floorDifficulty;
    }

    public class FloorDifficulty
    {
        public List<Difficulty> RoomDifficulties { get; private set; } = new();

        // TODO: system needs to pick these in order
        public Difficulty GetRoomDifficulty()
        {
            return RoomDifficulties[0];
        }
    }

    public class Difficulty
    {
        public float OverallModifier { get; set; }

        public float GenerateDifficultyModifier()
        {
            // get a difficulty between 90% and 110% of this difficulty
            return Random.Range(OverallModifier * 0.9f, OverallModifier * 1.1f);
        }

        public void ApplyToEnemy(EnemyController enemy)
        {
            enemy.SetMaxHp(enemy.MaxHp * GenerateDifficultyModifier());
            enemy.AddToMovementSpeedModifier(
                (GenerateDifficultyModifier() * enemy.MovementSpeed) - enemy.MovementSpeed
            );
            enemy.AddToDamageDealtModifier(
                (GenerateDifficultyModifier() * enemy.DamageDealtModifier)
                    - enemy.DamageDealtModifier
            );
        }
    }
    //
}
