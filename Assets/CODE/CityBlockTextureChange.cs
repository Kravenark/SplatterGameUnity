using UnityEngine;

public class CityBlockTextureChange : MonoBehaviour
{
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;
    public Material greyMaterial;
    public Material noneMaterial;

    private Renderer blockRenderer;

    private void Start()
    {
        blockRenderer = GetComponent<Renderer>();
        if (blockRenderer == null)
        {
            Debug.LogError($"CityBlockTextureChange: No Renderer found on {gameObject.name}.");
            return;
        }

        UpdateBlockMaterial();
    }




    // Updates the block material based on the block's tag
public void UpdateBlockMaterial()
{
    Renderer blockRenderer = GetComponent<Renderer>();
    
    if (blockRenderer == null)
    {
        Debug.LogError($"CityBlockTextureChange: Renderer not found on {gameObject.name}.");
        return;
    }

    // Ensure materials are assigned in the inspector
    if (redMaterial == null || greenMaterial == null || blueMaterial == null || greyMaterial == null)
    {
        Debug.LogError($"CityBlockTextureChange: One or more materials are not assigned on {gameObject.name}.");
        return;
    }

    // Apply material based on tag
    switch (gameObject.tag)
    {
        case "CityBlockRed":
            blockRenderer.material = redMaterial;
            break;
        case "CityBlockGreen":
            blockRenderer.material = greenMaterial;
            break;
        case "CityBlockBlue":
            blockRenderer.material = blueMaterial;
            break;
        case "CityBlockGrey":
            blockRenderer.material = greyMaterial;
            break;
        default:
            Debug.LogWarning($"CityBlockTextureChange: Unknown tag {gameObject.tag} on {gameObject.name}.");
            break;
    }
}


private Material GetMaterialForTag(string tag)
{
    switch (tag)
    {
        case "CityBuildingRed":
            return redMaterial;
        case "CityBuildingGreen":
            return greenMaterial;
        case "CityBuildingBlue":
            return blueMaterial;
        case "CityBuildingGrey":
            return greyMaterial;
        default:
            Debug.LogWarning($"Unknown tag: {tag}");
            return greyMaterial; // Default fallback
    }
}


private void UpdateChildrenBuildings(string newTag)
{
    Material targetMaterial = GetMaterialForTag(newTag); // Get the correct material

    foreach (Transform child in transform)
    {
        if (child.gameObject.layer == LayerMask.NameToLayer("Buildings"))
        {
            CityBuildingTextureChange building = child.GetComponent<CityBuildingTextureChange>();
            if (building != null)
            {
                building.SetBuildingColor(targetMaterial, newTag);  // Now 'material' is defined
            }
            else
            {
                Debug.LogError($"CityBlockTextureChange: CityBuildingTextureChange not found on {child.gameObject.name}.");
            }
        }
    }
}

}
