using UnityEngine;
using System.Collections;
public class CityBlockTextureChange : MonoBehaviour
{
    public Material redMaterial;  // Material for red blocks
    public Material greenMaterial; // Material for green blocks
    public Material blueMaterial; // Material for blue blocks
    public Material greyMaterial; // Material for grey blocks
    public Material noneMaterial; // Default material for "none"

    // Function to update the texture based on the GameObject's tag

    private void Start()
    {
        StartCoroutine(DelayedBuildingUpdate());
    }

    private IEnumerator DelayedBuildingUpdate()
    {
        yield return new WaitForSeconds(0.2f); // Small delay
        UpdateChildrenTags(gameObject.tag);
    }



    public void UpdateBlockTexture()
    {
        Renderer blockRenderer = GetComponent<Renderer>();
        if (blockRenderer == null)
        {
            Debug.LogWarning($"CityBlockTextureChange: No Renderer found on {gameObject.name}.");
            return;
        }

        // Update the city block's material based on its tag
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
            default:
                Debug.LogWarning($"CityBlockTextureChange: Unknown tag {gameObject.tag}.");
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
                child.gameObject.tag = newTag;

                CityBuildingTextureChange buildingScript = child.gameObject.GetComponent<CityBuildingTextureChange>();
                if (buildingScript != null)
                {
                    buildingScript.BuildingColourUpdate(); // Ensure it updates the material
                    Debug.Log($"Updated {child.gameObject.name} to {newTag}"); // Debug log
                }
                else
                {
                    Debug.LogError($"CityBlockTextureChange: {child.gameObject.name} does not have CityBuildingTextureChange script!");
                }
            }
        }
    }

}
