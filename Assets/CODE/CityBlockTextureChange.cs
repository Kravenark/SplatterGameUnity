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

        UpdateBlockTexture(); // Set the city block's color
    }

    public void UpdateBlockTexture()
    {
        if (blockRenderer == null) return;

        switch (gameObject.tag)
        {
            case "CityBlockRed":
                blockRenderer.material = redMaterial;
                UpdateChildrenBuildings("CityBuildingRed", redMaterial);
                break;
            case "CityBlockGreen":
                blockRenderer.material = greenMaterial;
                UpdateChildrenBuildings("CityBuildingGreen", greenMaterial);
                break;
            case "CityBlockBlue":
                blockRenderer.material = blueMaterial;
                UpdateChildrenBuildings("CityBuildingBlue", blueMaterial);
                break;
            case "CityBlockGrey":
                blockRenderer.material = greyMaterial;
                UpdateChildrenBuildings("CityBuildingGrey", greyMaterial);
                break;
            default:
                blockRenderer.material = noneMaterial;
                Debug.LogWarning($"CityBlockTextureChange: Unknown tag {gameObject.tag}.");
                break;
        }
    }

    private void UpdateChildrenBuildings(string newTag, Material newMaterial)
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.layer == LayerMask.NameToLayer("Buildings"))
            {
                CityBuildingTextureChange buildingScript = child.GetComponent<CityBuildingTextureChange>();
                if (buildingScript != null)
                {
                    buildingScript.SetBuildingColor(newTag, newMaterial);
                }
            }
        }
    }
}
