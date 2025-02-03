using UnityEngine;
using System.Collections;

public class CityBuildingTextureChange : MonoBehaviour
{
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;
    public Material greyMaterial;

    private Renderer buildingRenderer;
    private Coroutine currentSprayCoroutine;

    private void Start()
    {
        buildingRenderer = GetComponent<Renderer>();
        if (buildingRenderer == null)
        {
            Debug.LogError($"CityBuildingTextureChange: No Renderer found on {gameObject.name}.");
            return;
        }

        InitializeMaterialBasedOnTag();
    }

public void SetBuildingColor(Material material, string newTag)
{
    Renderer buildingRenderer = GetComponent<Renderer>();
    if (buildingRenderer != null)
    {
        buildingRenderer.material = material;
        gameObject.tag = newTag;
    }
    else
    {
        Debug.LogError($"CityBuildingTextureChange: Renderer is missing on {gameObject.name}.");
    }
}

    private void InitializeMaterialBasedOnTag()
    {
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

    public void StartSpraying(Material playerMaterial, string playerTag)
    {
        if (currentSprayCoroutine != null)
        {
            StopCoroutine(currentSprayCoroutine);
        }
        currentSprayCoroutine = StartCoroutine(SmoothTransition(playerMaterial, playerTag));
    }

    private IEnumerator SmoothTransition(Material targetMaterial, string newTag)
    {
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

        buildingRenderer.material.color = targetColor;
        gameObject.tag = newTag.Replace("Player", "CityBuilding");

        Debug.Log($"{gameObject.name} fully transitioned to {gameObject.tag}");
    }
}
