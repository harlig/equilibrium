using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class AcquisitionsDisplayController : MonoBehaviour
{
    public GameObject acquisitionPrefab;
    private GridLayoutGroup gridLayoutGroup;
    private RectTransform rectTransform;
    private const float GapPercentage = 0.05f; // 5% gap

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (!TryGetComponent<GridLayoutGroup>(out gridLayoutGroup))
        {
            throw new System.Exception("No grid layout on acqusitions display controller");
        }
    }

    public void UpdateDisplay(List<Acquisition> acquisitions)
    {
        // Clear existing displays
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Set GridLayoutGroup properties
        AdjustGridLayoutGroup(acquisitions.Count);

        // Create new displays
        for (int i = 0; i < acquisitions.Count; i++)
        {
            var acquisition = acquisitions[i];
            GameObject display = Instantiate(acquisitionPrefab, transform);

            // Set the data for this acquisition
            display.GetComponentInChildren<TextMeshProUGUI>().text =
                $"{acquisition.Name}\n+{acquisition.Value}";
            // TODO don't want color like this
            display.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;

            display.GetComponentInChildren<Image>().color = acquisition.Color;
        }
    }

    private void AdjustGridLayoutGroup(int itemCount)
    {
        int maxItemsPerRow = 4; // Maximum items per row, used for centering
        float totalHorizontalSpacing =
            (maxItemsPerRow - 1) * (rectTransform.rect.width * GapPercentage); // Total horizontal spacing
        float availableWidth = rectTransform.rect.width - totalHorizontalSpacing; // Width available for cells
        float cellWidth = availableWidth / maxItemsPerRow;
        float cellHeight = cellWidth; // or any other logic to determine cell height

        // Set the spacing based on the cell width
        float spacing = cellWidth * GapPercentage;
        gridLayoutGroup.spacing = new Vector2(spacing, spacing);

        gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);

        // Calculate rows and adjust for resizing if necessary
        int rowsNeeded = Mathf.CeilToInt((float)itemCount / maxItemsPerRow);
        float totalHeightNeeded = rowsNeeded * cellHeight + (rowsNeeded - 1) * spacing;

        if (totalHeightNeeded > rectTransform.rect.height)
        {
            // Increase maxItemsPerRow to fit within the vertical space
            maxItemsPerRow = CalculateMaxItemsPerRow(itemCount, cellHeight, spacing);
            cellWidth =
                (rectTransform.rect.width - (maxItemsPerRow - 1) * spacing) / maxItemsPerRow;
            gridLayoutGroup.cellSize = new Vector2(cellWidth, cellWidth); // Keeping aspect ratio
        }

        // Adjust padding for centering based on maxItemsPerRow
        CenterItems(itemCount, maxItemsPerRow, cellWidth, spacing);
    }

    private int CalculateMaxItemsPerRow(int itemCount, float cellHeight, float spacing)
    {
        int itemsPerRow = 4;
        while (true)
        {
            float totalHeightNeeded =
                (Mathf.Ceil(itemCount / (float)itemsPerRow)) * (cellHeight + spacing);
            if (totalHeightNeeded <= rectTransform.rect.height || itemsPerRow >= itemCount)
            {
                break;
            }
            itemsPerRow++;
        }
        return itemsPerRow;
    }

    private void CenterItems(int itemCount, int maxItemsPerRow, float cellWidth, float spacing)
    {
        int rows = Mathf.CeilToInt((float)itemCount / maxItemsPerRow);
        float totalRowWidth = maxItemsPerRow * cellWidth + (maxItemsPerRow - 1) * spacing;
        float paddingLeft = (rectTransform.rect.width - totalRowWidth) / 2;
        paddingLeft = Mathf.Max(paddingLeft, 0);

        gridLayoutGroup.padding.left = (int)paddingLeft;
    }
}
