using UnityEngine;
using System.Collections;

public class CityBuildingTextureChange : MonoBehaviour
{
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;
    public Material greyMaterial;

    private Renderer buildingRenderer;
    private Coroutine transitionCoroutine;
    private bool isBeingSprayed = false; // Tracks if currently being sprayed

private void Start()
{
    buildingRenderer = GetComponent<Renderer>();

    if (buildingRenderer == null)
    {
        // Check child objects if Renderer is not on the parent
        buildingRenderer = GetComponentInChildren<Renderer>();
    }

    if (buildingRenderer == null)
    {
      //  Debug.LogError($"CityBuildingTextureChange: Renderer is missing on {gameObject.name} or its children.");
    }
}




    public void StartSpraying(string playerTag)
    {
        isBeingSprayed = true; // Player is actively spraying

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine); // Stop any previous transition
        }

        transitionCoroutine = StartCoroutine(SmoothBuildingTransition(playerTag));
    }

    public void StopSpraying()
    {
        isBeingSprayed = false; // Player stopped spraying
    }

    private IEnumerator SmoothBuildingTransition(string playerTag)
    {
        if (buildingRenderer == null)
        {
            Debug.LogWarning($"CityBuildingTextureChange: No Renderer found on {gameObject.name}.");
            yield break;
        }

        // Get the correct target material
        Material targetMaterial = GetMaterialFromTag(playerTag);
        if (targetMaterial == null)
        {
            Debug.LogWarning($"CityBuildingTextureChange: No valid material found for {playerTag}.");
            yield break;
        }

        Color startColor = buildingRenderer.material.color;
        Color targetColor = targetMaterial.color;

        float elapsedTime = 0f;
        float transitionDuration = 2f; // Time to transition fully

        while (elapsedTime < transitionDuration)
        {
            if (!isBeingSprayed) // Stop transition if player stops spraying
            {
                Debug.Log($"CityBuildingTextureChange: Transition stopped for {gameObject.name}.");
                yield break;
            }

            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;
            buildingRenderer.material.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        // Finalize transition
        buildingRenderer.material = targetMaterial;
        gameObject.tag = playerTag.Replace("Player", "CityBuilding");
        Debug.Log($"CityBuildingTextureChange: {gameObject.name} fully converted to {gameObject.tag}.");
    }

private Material GetMaterialFromTag(string tag)
{
    switch (tag)
    {
        case "PlayerRed": return redMaterial;
        case "PlayerGreen": return greenMaterial;
        case "PlayerBlue": return blueMaterial;
        case "CityBuildingGrey": return greyMaterial;
        case "CityBlockNone": return greyMaterial; // Default material or a special one
        default: return null;
    }
}




    public void SetBuildingColor(string newTag)
{
    if (buildingRenderer == null)
    {
        Debug.LogError($"CityBuildingTextureChange: Renderer is missing on {gameObject.name}");
        return;
    }

    // Assign the correct material based on the new tag
    switch (newTag)
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
            Debug.LogWarning($"CityBuildingTextureChange: Unknown tag {newTag}.");
            return;
    }

    // Update the building's tag
    gameObject.tag = newTag;

    Debug.Log($"CityBuildingTextureChange: {gameObject.name} updated to {newTag}.");
}

}
