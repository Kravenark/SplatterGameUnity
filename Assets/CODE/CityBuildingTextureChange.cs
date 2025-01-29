using UnityEngine;

public class CityBuildingTextureChange : MonoBehaviour
{
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;
    public Material greyMaterial;

    private Renderer buildingRenderer;

    private void Start()
    {
        buildingRenderer = GetComponent<Renderer>();

        if (buildingRenderer == null)
        {
            Debug.LogError($"CityBuildingTextureChange: Renderer is missing on {gameObject.name}");
            return;
        }

        // Ensure it starts with the correct material
        UpdateBuildingMaterialBasedOnTag();
    }

    public void ApplyPlayerColor(string playerTag)
    {
        if (buildingRenderer == null) return;

        switch (playerTag)
        {
            case "PlayerRed":
                gameObject.tag = "CityBuildingRed";
                buildingRenderer.material = redMaterial;
                break;
            case "PlayerGreen":
                gameObject.tag = "CityBuildingGreen";
                buildingRenderer.material = greenMaterial;
                break;
            case "PlayerBlue":
                gameObject.tag = "CityBuildingBlue";
                buildingRenderer.material = blueMaterial;
                break;
            default:
                Debug.LogWarning($"CityBuildingTextureChange: Unknown player tag {playerTag}.");
                break;
        }
    }



    public void BuildingColourUpdate()
    {
        if (buildingRenderer == null) return;
        UpdateBuildingMaterialBasedOnTag();
    }

    private void UpdateBuildingMaterialBasedOnTag()
    {
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
}
