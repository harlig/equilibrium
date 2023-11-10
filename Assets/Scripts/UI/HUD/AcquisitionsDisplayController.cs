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
        int minItemsPerRow = 4;
        float totalSpacing = (minItemsPerRow - 1) * (rectTransform.rect.width * GapPercentage); // Total horizontal spacing
        float availableWidth = rectTransform.rect.width - totalSpacing; // Width available for cells
        float cellWidth = availableWidth / minItemsPerRow;
        float cellHeight = cellWidth; // or any other logic to determine cell height

        // Set the spacing based on the cell width
        float spacing = cellWidth * GapPercentage;
        gridLayoutGroup.spacing = new Vector2(spacing, spacing);

        gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);

        // Additional logic to resize cells if the y space gets taken up
        float totalHeightNeeded =
            (Mathf.Ceil(itemCount / (float)minItemsPerRow)) * (cellHeight + spacing);
        if (totalHeightNeeded > rectTransform.rect.height)
        {
            // Adjust cell size based on available height
            cellHeight =
                (
                    rectTransform.rect.height
                    - (Mathf.Ceil(itemCount / (float)minItemsPerRow) - 1) * spacing
                ) / Mathf.Ceil(itemCount / (float)minItemsPerRow);
            cellWidth = cellHeight; // Keep aspect ratio
            gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);

            // Adjust the spacing proportionally
            spacing = cellWidth * GapPercentage;
            gridLayoutGroup.spacing = new Vector2(spacing, spacing);
        }
    }
}
