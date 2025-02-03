using UnityEngine;

public class PlayerTextureChange : MonoBehaviour
{
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;
    public Material noneMaterial;

    private Renderer playerRenderer;

    private void Start()
    {
        playerRenderer = GetComponent<Renderer>();
        if (playerRenderer == null)
        {
            Debug.LogError($"PlayerTextureChange: No Renderer found on {gameObject.name}.");
            return;
        }

        UpdatePlayerMaterial();
    }

public void UpdatePlayerTexture()
{
    Renderer playerRenderer = GetComponent<Renderer>();
    if (playerRenderer != null)
    {
        switch (gameObject.tag)
        {
            case "PlayerRed":
                playerRenderer.material = redMaterial;
                break;
            case "PlayerGreen":
                playerRenderer.material = greenMaterial;
                break;
            case "PlayerBlue":
                playerRenderer.material = blueMaterial;
                break;
            default:
                Debug.LogWarning($"PlayerTextureChange: Unknown player tag {gameObject.tag}.");
                break;
        }
    }
    else
    {
        Debug.LogError($"PlayerTextureChange: Renderer is missing on {gameObject.name}.");
    }
}


    // This method updates the player's material based on its tag
public void UpdatePlayerMaterial()
{
    Renderer playerRenderer = GetComponent<Renderer>();

    if (playerRenderer == null)
    {
        Debug.LogError($"PlayerTextureChange: Renderer not found on {gameObject.name}.");
        return;
    }

    // Check if materials are assigned
    if (redMaterial == null || greenMaterial == null || blueMaterial == null)
    {
        Debug.LogError($"PlayerTextureChange: One or more materials are not assigned on {gameObject.name}.");
        return;
    }

    // Apply material based on tag
    switch (gameObject.tag)
    {
        case "PlayerRed":
            playerRenderer.material = redMaterial;
            break;
        case "PlayerGreen":
            playerRenderer.material = greenMaterial;
            break;
        case "PlayerBlue":
            playerRenderer.material = blueMaterial;
            break;
        default:
            Debug.LogWarning($"PlayerTextureChange: Unknown tag {gameObject.tag} on {gameObject.name}.");
            break;
    }
}


    // Retrieve the current material for reference in shooting
    public Material GetCurrentMaterial()
    {
        return playerRenderer.material;
    }
}
