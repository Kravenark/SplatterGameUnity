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
        buildingRenderer = GetComponent<Renderer>();
        if (buildingRenderer == null)
        {
            Debug.LogError($"CityBuildingTextureChange: No Renderer found on {gameObject.name}.");
            return;
        }

        if (redMaterial == null || greenMaterial == null || blueMaterial == null || greyMaterial == null)
        {
            Debug.LogError($"CityBuildingTextureChange: One or more materials are not assigned on {gameObject.name}.");
            return;
        }
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
        if (buildingRenderer == null)
        {
            Debug.LogError($"CityBuildingTextureChange: Renderer is missing on {gameObject.name}");
            return;
        }

        switch (playerTag)
        {
            case "PlayerRed":
                buildingRenderer.material = redMaterial;
                gameObject.tag = "CityBuildingRed";
                break;
            case "PlayerGreen":
                buildingRenderer.material = greenMaterial;
                gameObject.tag = "CityBuildingGreen";
                break;
            case "PlayerBlue":
                buildingRenderer.material = blueMaterial;
                gameObject.tag = "CityBuildingBlue";
                break;
            default:
                Debug.LogWarning($"CityBuildingTextureChange: Unknown player tag {playerTag} on {gameObject.name}");
                break;
        }
    }

}
