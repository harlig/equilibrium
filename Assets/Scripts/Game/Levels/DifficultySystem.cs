using System.Collections.Generic;
using UnityEngine;

public class DifficultySystem
{
    public List<FloorDifficulty> floorDifficulties = new();

    const float NEXT_FLOOR_DIFFICULTY_MULTIPLIER = 1.3f;
    const int NUM_ROOM_DIFFICULTIES_TO_GENERATE = 5;
    const float ROOM_DIFFICULTY_INCREMENT = 0.04f; // Increment factor for each subsequent room

    public FloorDifficulty GenerateNextFloorDifficulty()
    {
        // Calculate the base difficulty for the new floor
        float lastFloorModifier =
            floorDifficulties.Count > 0 ? floorDifficulties[^1].OverallModifier : 1;
        float newFloorModifier = lastFloorModifier * NEXT_FLOOR_DIFFICULTY_MULTIPLIER;

        // Ensuring the first room of the new floor is at least as difficult as the last room of the prior floor
        if (floorDifficulties.Count > 0)
        {
            FloorDifficulty lastFloor = floorDifficulties[^1];
            Difficulty lastRoomOfLastFloor = lastFloor.RoomDifficulties[^1];
            newFloorModifier = Mathf.Max(newFloorModifier, lastRoomOfLastFloor.OverallModifier);
        }

        var floorDifficulty = new FloorDifficulty() { OverallModifier = newFloorModifier };

        // Adding progressively harder rooms
        for (int ndx = 0; ndx < NUM_ROOM_DIFFICULTIES_TO_GENERATE; ndx++)
        {
            float roomDifficultyModifier = newFloorModifier * (1 + ndx * ROOM_DIFFICULTY_INCREMENT);
            floorDifficulty.RoomDifficulties.Add(
                new Difficulty { OverallModifier = roomDifficultyModifier }
            );
        }

        floorDifficulties.Add(floorDifficulty);

        return floorDifficulty;
    }

    public class FloorDifficulty
    {
        public float OverallModifier { get; set; }
        public List<Difficulty> RoomDifficulties { get; private set; } = new List<Difficulty>();

        private int nextRoomIndex = 0;

        public Difficulty GetRoomDifficulty()
        {
            // Ensure there is at least one room difficulty to return
            if (RoomDifficulties.Count == 0)
            {
                throw new System.Exception(
                    "Tried to get room difficulty but none were registered. What the heck?"
                );
            }

            Difficulty difficulty = RoomDifficulties[nextRoomIndex];
            nextRoomIndex = (nextRoomIndex + 1) % RoomDifficulties.Count; // Loop back to the start

            return difficulty;
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
            if (newMovementSpeed > enemy.BaseMovementSpeed * 2.5f)
            {
                newMovementSpeed = enemy.BaseMovementSpeed * 2.5f;
            }

            enemy.AddToMovementSpeedModifier(newMovementSpeed - enemy.MovementSpeed);

            float attackSpeedMultiplier = CalculateAttackSpeedMultiplier(OverallModifier);
            enemy.IncreaseAllWeaponsAttackSpeedMultiplier(attackSpeedMultiplier);
        }

        private float CalculateAttackSpeedMultiplier(float difficultyModifier)
        {
            // Adjusting the base of the logarithm to scale the value appropriately
            const float logBase = 10; // You can tweak this base to change the scaling
            float normalizedDifficulty = Mathf.Clamp(difficultyModifier, 1, float.MaxValue);
            float logValue = Mathf.Log(normalizedDifficulty, logBase);

            // Normalize to range [0, 1]
            return Mathf.Clamp(logValue / logBase, 0, 1);
        }
    }
}
