using UnityEngine;

public class CityBlockTextureChange : MonoBehaviour
{
    public Material redMaterial;  // Material for red blocks
    public Material greenMaterial; // Material for green blocks
    public Material blueMaterial; // Material for blue blocks
    public Material greyMaterial; // Material for grey blocks
    public Material noneMaterial; // Default material for "none"

    // Function to update the texture based on the GameObject's tag
    public void UpdateBlockTexture()
    {
        Renderer blockRenderer = GetComponent<Renderer>();
        if (blockRenderer == null)
        {
            Debug.LogWarning($"CityBlockTextureChange: No Renderer found on {gameObject.name}.");
            return;
        }

        // Assign the material based on the GameObject's tag
        switch (gameObject.tag)
        {
            case "CityBlockRed":
                blockRenderer.material = redMaterial;
                UpdateChildrenTags("CityBuildingRed");
                break;
            case "CityBlockGreen":
                blockRenderer.material = greenMaterial;
                UpdateChildrenTags("CityBuildingGreen");
                break;
            case "CityBlockBlue":
                blockRenderer.material = blueMaterial;
                UpdateChildrenTags("CityBuildingBlue");
                break;
            case "CityBlockGrey":
                blockRenderer.material = greyMaterial;
                UpdateChildrenTags("CityBuildingGrey");
                break;
            case "CityBlockNone":
                blockRenderer.material = noneMaterial;
                UpdateChildrenTags("CityBuildingNone");
                break;
            default:
                Debug.LogWarning("CityBlockTextureChange: Unknown tag on this object.");
                break;
        }
    }

    // Function to update tags and materials for all child objects with the "Buildings" layer
    private void UpdateChildrenTags(string newTag)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.layer == LayerMask.NameToLayer("Buildings"))
            {
                CityBuildingTextureChange buildingTexture = child.gameObject.GetComponent<CityBuildingTextureChange>();
                if (buildingTexture != null)
                {
                    buildingTexture.ApplyPlayerColor(newTag);
                }

                // Debug log for tag update
                Debug.Log($"Updated {child.gameObject.name} to tag {newTag}.");
            }
        }
    }
}
