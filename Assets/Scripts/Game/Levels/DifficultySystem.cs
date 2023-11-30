using System.Collections.Generic;
using UnityEngine;

public class DifficultySystem
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
            // get a difficulty between 80% and 120% of this difficulty
            return Random.Range(OverallModifier * 0.8f, OverallModifier * 1.2f);
        }

        public void ApplyToEnemy(EnemyController enemy)
        {
            var originalMaxHp = enemy.MaxHp;
            // Adjusting Max HP based on difficulty
            enemy.SetMaxHp(enemy.MaxHp * GenerateDifficultyModifier());

            // Scaling Damage based on difficulty and new Max HP
            enemy.AddToDamageDealtModifier(
                enemy.MaxHp * GenerateDifficultyModifier() / originalMaxHp
            );

            // Scaling Movement Speed
            float movementSpeedModifier = GenerateDifficultyModifier();
            float newMovementSpeed = enemy.MovementSpeed * movementSpeedModifier;

            // Ensuring new movement speed does not exceed 2.5 times the original speed
            if (newMovementSpeed > enemy.MovementSpeed * 2.5f)
            {
                newMovementSpeed = enemy.MovementSpeed * 2.5f;
            }

            enemy.AddToMovementSpeedModifier(newMovementSpeed - enemy.MovementSpeed);
        }
    }
}
