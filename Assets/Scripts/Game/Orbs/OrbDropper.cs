using TMPro;
using UnityEngine;

public class DamageTaken
{
    public float FireDamage { get; set; }
    public float IceDamage { get; set; }
    public float NeutralDamage { get; set; }

    public TextMeshPro TextElement { get; set; }

    public float TotalDamage()
    {
        return FireDamage + IceDamage + NeutralDamage;
    }

    public float TotalElementalDamage()
    {
        return FireDamage + IceDamage;
    }

    public void SetDamageTakenTextOnTextElement(float maxHp)
    {
        TextElement.text = $"{Mathf.CeilToInt(maxHp - TotalDamage())}";
    }

    public void HideTextElement()
    {
        TextElement.gameObject.SetActive(false);
    }
}

public class OrbDropper : MonoBehaviour
{
    [SerializeField]
    private OrbController fireOrbPrefab;

    [SerializeField]
    private OrbController iceOrbPrefab;
    private const float MAX_PROBABILITY = 0.90f;

    public float scatterRange = 1.0f;
    private bool didDropOrbs = false;

    private static bool ShouldDropFireOrb(DamageTaken damageTaken)
    {
        float fireProbability = damageTaken.FireDamage / damageTaken.TotalElementalDamage();

        return Random.Range(0.0f, 1.0f)
            < Mathf.Clamp(fireProbability, 1.0f - MAX_PROBABILITY, MAX_PROBABILITY);
    }

    public void DoOrbDrop(
        OrbController.OrbType orbTypeToDrop,
        float totalXp,
        RoomManager containingRoom,
        int desiredNumToDrop = 10
    )
    {
        if (didDropOrbs)
        {
            return;
        }
        didDropOrbs = true;

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

            if (orbTypeToDrop == OrbController.OrbType.FIRE)
            {
                OrbController.Create(fireOrbPrefab, this, orbTypeToDrop, xp, scatter);
            }
            else
            {
                OrbController.Create(iceOrbPrefab, this, orbTypeToDrop, xp, scatter);
            }
        }
    }

    public void DoOrbDrop(
        DamageTaken damageTaken,
        float totalXp,
        RoomManager containingRoom,
        int desiredNumToDrop = 10
    )
    {
        var shouldDropFireOrb = ShouldDropFireOrb(damageTaken);
        if (shouldDropFireOrb)
        {
            DoOrbDrop(OrbController.OrbType.FIRE, totalXp, containingRoom, desiredNumToDrop);
        }
        else
        {
            DoOrbDrop(OrbController.OrbType.ICE, totalXp, containingRoom, desiredNumToDrop);
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

            Vector2 potentialPosition = new(xOffset, yOffset);

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
