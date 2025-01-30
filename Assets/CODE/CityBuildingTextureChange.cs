using UnityEngine;
using System.Collections;

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
            Debug.LogError($"CityBuildingTextureChange: No Renderer found on {gameObject.name}.");
            return;
        }

        // Get the parent city block
        Transform parentCityBlock = transform.parent;
        if (parentCityBlock == null)
        {
            Debug.LogWarning($"CityBuildingTextureChange: {gameObject.name} does not have a parent city block.");
            return;
        }

        // Get the initial color from the city block
        string cityBlockTag = parentCityBlock.tag.Replace("CityBlock", "CityBuilding");
        StartCoroutine(DelayedInitializeMaterial(cityBlockTag));
    }

    private IEnumerator DelayedInitializeMaterial(string newTag)
    {
        yield return new WaitForSeconds(0.5f); // Small delay to ensure city block initializes first
        ApplyMaterialBasedOnTag(newTag);
    }

    public void SetBuildingColor(string newTag, Material newMaterial)
    {
        gameObject.tag = newTag;
        ApplyMaterialBasedOnTag(newTag);
    }

    private void ApplyMaterialBasedOnTag(string buildingTag)
    {
        if (buildingRenderer == null) return;

        switch (buildingTag)
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
                Debug.LogWarning($"CityBuildingTextureChange: Unknown tag {buildingTag}.");
                break;
        }

        Debug.Log($"{gameObject.name} assigned material based on {buildingTag}.");
    }

    public IEnumerator SmoothBuildingTransition(string playerTag)
{
    if (buildingRenderer == null)
    {
        Debug.LogWarning($"CityBuildingTextureChange: No Renderer found on {gameObject.name}.");
        yield break;
    }

    Material targetMaterial = GetMaterialFromTag(playerTag);
    if (targetMaterial == null)
    {
        Debug.LogWarning($"CityBuildingTextureChange: No matching material for tag {playerTag}.");
        yield break;
    }

    Color startColor = buildingRenderer.material.color;
    Color targetColor = targetMaterial.color;
    float transitionDuration = 2f;
    float elapsedTime = 0f;

    while (elapsedTime < transitionDuration)
    {
        elapsedTime += Time.deltaTime;
        float t = elapsedTime / transitionDuration;
        buildingRenderer.material.color = Color.Lerp(startColor, targetColor, t);
        yield return null;
    }

    // Finalize transition and update tag
    buildingRenderer.material = targetMaterial;
    gameObject.tag = ConvertPlayerTagToBuildingTag(playerTag);
    Debug.Log($"CityBuildingTextureChange: {gameObject.name} fully transitioned to {gameObject.tag}.");
}

// **Helper: Convert Player Tag (PlayerRed) to Building Tag (CityBuildingRed)**
private string ConvertPlayerTagToBuildingTag(string playerTag)
{
    return playerTag.Replace("Player", "CityBuilding");
}

// **Helper: Get Material from Tag**
private Material GetMaterialFromTag(string tag)
{
    switch (tag)
    {
        case "PlayerRed": return redMaterial;
        case "PlayerGreen": return greenMaterial;
        case "PlayerBlue": return blueMaterial;
        default: return null;
    }
}

}
