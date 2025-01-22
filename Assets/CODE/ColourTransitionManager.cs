using UnityEngine;

public class ColourTransitionManager : MonoBehaviour
{
    public Material redMaterial;   // Material for red blocks
    public Material greenMaterial; // Material for green blocks
    public Material blueMaterial;  // Material for blue blocks
    public Material greyMaterial;  // Material for grey blocks

    private Renderer blockRenderer; // Renderer for the block

    private void Start()
    {
        blockRenderer = GetComponent<Renderer>();
        if (blockRenderer == null)
        {
            Debug.LogError($"ColourTransitionManager: No Renderer found on {gameObject.name}.");
        }
    }

    private void Update()
    {
        UpdateBlockColorBasedOnChildren();
    }

    private void UpdateBlockColorBasedOnChildren()
    {
        if (blockRenderer == null)
        {
            Debug.LogError("ColourTransitionManager: Block Renderer is missing!");
            return;
        }

        int totalBuildings = 0;
        int redCount = 0;
        int greenCount = 0;
        int blueCount = 0;

        // Count the buildings with specific tags
        foreach (Transform child in transform)
        {
            if (child.gameObject.layer == LayerMask.NameToLayer("Buildings"))
            {
                totalBuildings++;

                switch (child.tag)
                {
                    case "CityBuildingRed":
                        redCount++;
                        break;
                    case "CityBuildingGreen":
                        greenCount++;
                        break;
                    case "CityBuildingBlue":
                        blueCount++;
                        break;
                }
            }
        }

        if (totalBuildings == 0)
        {
            Debug.LogWarning($"ColourTransitionManager: No buildings found in {gameObject.name}.");
            return;
        }

        // Calculate percentages
        float redPercentage = (float)redCount / totalBuildings;
        float greenPercentage = (float)greenCount / totalBuildings;
        float bluePercentage = (float)blueCount / totalBuildings;

        // Debug the percentages
        Debug.Log($"Red: {redPercentage * 100}% | Green: {greenPercentage * 100}% | Blue: {bluePercentage * 100}% in {gameObject.name}");

        // Determine the dominant color and lerp the material color
        Color targetColor = LerpMaterialColors(redPercentage, greenPercentage, bluePercentage);
        blockRenderer.material.color = targetColor;

        // If 100% of a single color, update the tag and material
        if (redPercentage == 1f)
        {
            UpdateBlockToColor("CityBlockRed", redMaterial);
        }
        else if (greenPercentage == 1f)
        {
            UpdateBlockToColor("CityBlockGreen", greenMaterial);
        }
        else if (bluePercentage == 1f)
        {
            UpdateBlockToColor("CityBlockBlue", blueMaterial);
        }
    }

    private Color LerpMaterialColors(float redPercentage, float greenPercentage, float bluePercentage)
    {
        Color redColor = redMaterial.color;
        Color greenColor = greenMaterial.color;
        Color blueColor = blueMaterial.color;

        // Blend the colors based on percentages
        Color mixedColor = (redColor * redPercentage) +
                           (greenColor * greenPercentage) +
                           (blueColor * bluePercentage);

        return mixedColor;
    }

    private void UpdateBlockToColor(string newTag, Material newMaterial)
    {
        gameObject.tag = newTag;
        blockRenderer.material = newMaterial;

        Debug.Log($"ColourTransitionManager: {gameObject.name} has fully converted to {newTag}.");
    }
}
