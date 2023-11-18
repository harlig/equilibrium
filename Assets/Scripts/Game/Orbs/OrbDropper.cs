using TMPro;
using UnityEngine;

public class DamageTaken
{
    public float FireDamage { get; set; }
    public float IceDamage { get; set; }

    public float TotalDamage()
    {
        return FireDamage + IceDamage;
    }

    // TODO: this shouldn't be static I think
    public static void SetDamageTakenTextOnTextElement(
        float maxHp,
        DamageTaken damageTaken,
        TextMeshPro textElement
    )
    {
        // format to two decimal places
        textElement.text = string.Format("{0:N1}", maxHp - damageTaken.TotalDamage());
    }
}

public class OrbDropper : MonoBehaviour
{
    [SerializeField]
    private OrbController fireOrbPrefab;

    [SerializeField]
    private OrbController iceOrbPrefab;
    const float MAX_PROBABILITY = 0.80f;

    public float scatterRange = 1.0f;

    private static bool ShouldDropFireOrb(DamageTaken damageTaken)
    {
        float fireProbability = damageTaken.FireDamage / damageTaken.TotalDamage();

        return Random.Range(0.0f, 1.0f)
            < Mathf.Clamp(fireProbability, 1.0f - MAX_PROBABILITY, MAX_PROBABILITY);
    }

    public void DoOrbDrop(
        DamageTaken damageTaken,
        float totalXp,
        RoomManager containingRoom,
        int desiredNumToDrop = 10
    )
    {
        var shouldDropFireOrb = ShouldDropFireOrb(damageTaken);

        int minNumToDrop = Mathf.Max(Mathf.FloorToInt(desiredNumToDrop * 0.8f), 1);
        int maxNumToDrop = Mathf.FloorToInt(desiredNumToDrop * 1.2f);

        int numToDrop = Random.Range(minNumToDrop, maxNumToDrop + 1);

        int baseXp = (int)(totalXp / numToDrop);
        int remainingXp = (int)(totalXp % numToDrop);

        for (int ndx = 0; ndx < numToDrop; ndx++)
        {
            int xp = baseXp;
            if (remainingXp > 0)
            {
                xp++;
                remainingXp--;
            }

            Vector2? scatter = null;

            if (numToDrop > 1)
            {
                scatter = GetRandomWalkablePosition(containingRoom.Grid);
            }

            if (shouldDropFireOrb)
            {
                OrbController.Create(fireOrbPrefab, this, OrbController.OrbType.FIRE, xp, scatter);
            }
            else
            {
                OrbController.Create(iceOrbPrefab, this, OrbController.OrbType.ICE, xp, scatter);
            }
        }
    }

    private Vector2 GetRandomWalkablePosition(Grid grid)
    {
        int attempts = 0;
        int maxAttempts = 100; // Maximum number of attempts to find a walkable position

        while (attempts < maxAttempts)
        {
            float xOffset = Random.Range(-scatterRange, scatterRange);
            float yOffset = Random.Range(-scatterRange, scatterRange);

            Vector2 potentialPosition = new Vector2(xOffset, yOffset);

            // Convert potential position to Grid coordinates
            Vector2Int gridPosition = grid.WorldToGrid(
                potentialPosition + new Vector2(transform.position.x, transform.position.y)
            );

            if (
                gridPosition.x >= 0
                && gridPosition.x < grid.FloorWidth
                && gridPosition.y >= 0
                && gridPosition.y < grid.FloorHeight
            )
            {
                Node node = grid.nodes[gridPosition.x, gridPosition.y];
                if (node.Walkable)
                {
                    return potentialPosition;
                }
            }

            attempts++;
        }

        // Default to a safe position if no walkable position is found
        return Vector2.zero;
    }
}
