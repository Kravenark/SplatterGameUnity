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

        // Set initial material to grey
        buildingRenderer.material = greyMaterial;
    }

    public void BuildingColourUpdate()
    {
        // Continuously check and align the tag and material
        EnsureTagAndMaterialMatch();
    }

    // Ensure that the tag and material are always aligned
    private void EnsureTagAndMaterialMatch()
    {
        if (buildingRenderer == null) return;

        switch (gameObject.tag)
        {
            case "CityBuildingRed":
                if (buildingRenderer.material != redMaterial)
                {
                    buildingRenderer.material = redMaterial;
                }
                break;
            case "CityBuildingGreen":
                if (buildingRenderer.material != greenMaterial)
                {
                    buildingRenderer.material = greenMaterial;
                }
                break;
            case "CityBuildingBlue":
                if (buildingRenderer.material != blueMaterial)
                {
                    buildingRenderer.material = blueMaterial;
                }
                break;
            case "CityBuildingGrey":
                if (buildingRenderer.material != greyMaterial)
                {
                    buildingRenderer.material = greyMaterial;
                }
                break;
            default:
                Debug.LogWarning($"CityBuildingTextureChange: Unknown tag {gameObject.tag} on {gameObject.name}.");
                break;
        }

        // Additionally ensure that the tag is updated if the material changes
        if (buildingRenderer.material == redMaterial && gameObject.tag != "CityBuildingRed")
        {
            gameObject.tag = "CityBuildingRed";
        }
        else if (buildingRenderer.material == greenMaterial && gameObject.tag != "CityBuildingGreen")
        {
            gameObject.tag = "CityBuildingGreen";
        }
        else if (buildingRenderer.material == blueMaterial && gameObject.tag != "CityBuildingBlue")
        {
            gameObject.tag = "CityBuildingBlue";
        }
        else if (buildingRenderer.material == greyMaterial && gameObject.tag != "CityBuildingGrey")
        {
            gameObject.tag = "CityBuildingGrey";
        }
    }

    // Function to change building color when hit by a player's raycast
    public void ChangeBuildingColor(string playerTag)
    {
        if (buildingRenderer == null) return;

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
                Debug.LogWarning($"CityBuildingTextureChange: Unknown player tag {playerTag}.");
                break;
        }
    }
}
