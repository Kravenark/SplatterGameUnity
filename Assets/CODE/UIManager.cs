using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Transform cityBlocksParent;        // Parent object of city blocks in the scene
    public Transform blockLabelsParent;       // Parent object of TextMesh Pro UI elements
    public Camera mainCamera;                 // Reference to the main camera

    private Transform[] cityBlocks;           // Array of city block objects
    private TextMeshProUGUI[] blockLabels;    // Array of TextMesh Pro UI elements

    void Start()
    {
        // Automatically find all city block children
        cityBlocks = new Transform[cityBlocksParent.childCount];
        for (int i = 0; i < cityBlocksParent.childCount; i++)
        {
            cityBlocks[i] = cityBlocksParent.GetChild(i);
        }

        // Automatically find all TextMesh Pro UI children
        blockLabels = new TextMeshProUGUI[blockLabelsParent.childCount];
        for (int i = 0; i < blockLabelsParent.childCount; i++)
        {
            blockLabels[i] = blockLabelsParent.GetChild(i).GetComponent<TextMeshProUGUI>();
        }

        // Check if both arrays match in size
        if (cityBlocks.Length != blockLabels.Length)
        {
            Debug.LogError("Mismatch: Number of city blocks and UI labels do not match!");
        }
    }

    void Update()
    {
        for (int i = 0; i < Mathf.Min(cityBlocks.Length, blockLabels.Length); i++)
        {
            // Convert world position of the city block to screen space
            Vector3 screenPos = mainCamera.WorldToScreenPoint(cityBlocks[i].position);

            // Set the position of the corresponding UI text
            blockLabels[i].rectTransform.position = screenPos;
        }
    }
}
