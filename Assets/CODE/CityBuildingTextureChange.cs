using UnityEngine;

public class CityBuildingTextureChange : MonoBehaviour
{
    public Material redMaterial;   // Material for PlayerRed
    public Material greenMaterial; // Material for PlayerGreen
    public Material blueMaterial;  // Material for PlayerBlue
    public Material greyMaterial;  // Default grey material

    private Renderer buildingRenderer; // Renderer for the building

    private void Start()
    {
        // Initialize the building's renderer
        buildingRenderer = GetComponent<Renderer>();
        if (buildingRenderer == null)
        {
            Debug.LogError($"CityBuildingTextureChange: No Renderer found on {gameObject.name}.");
            return;
        }

        // Ensure the material matches the tag
        UpdateBuildingMaterial();
    }

    public void UpdateBuildingMaterial()
    {
        // Ensure the building's material matches its tag
        if (buildingRenderer == null) return;

        switch (gameObject.tag)
        {
            case "CityBuildingRed":
                buildingRenderer.material = redMaterial;
                break;
            case "CityBuildingGreen":
                buildingRenderer.material = greenMaterial;
                break;
            case "CityBuildingBlue":
                buildingRenderer.material = blueMaterial;
                break;
            case "CityBuildingGrey":
                buildingRenderer.material = greyMaterial;
                break;
            default:
                Debug.LogWarning($"CityBuildingTextureChange: Unknown tag {gameObject.tag} on {gameObject.name}.");
                break;
        }
    }

    public void ApplyPlayerColor(string playerTag)
    {
        switch (playerTag)
        {
            case "PlayerRed":
            case "CityBuildingRed":
                buildingRenderer.material = redMaterial;
                gameObject.tag = "CityBuildingRed";
                break;
            case "PlayerGreen":
            case "CityBuildingGreen":
                buildingRenderer.material = greenMaterial;
                gameObject.tag = "CityBuildingGreen";
                break;
            case "PlayerBlue":
            case "CityBuildingBlue":
                buildingRenderer.material = blueMaterial;
                gameObject.tag = "CityBuildingBlue";
                break;
            case "CityBuildingGrey":
                buildingRenderer.material = greyMaterial;
                gameObject.tag = "CityBuildingGrey";
                break;
            default:
                Debug.LogWarning($"CityBuildingTextureChange: Unknown player tag {playerTag}.");
                break;
        }
    }

}
