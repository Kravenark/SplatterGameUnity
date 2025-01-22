using UnityEngine;

public class PlayerTextureChange : MonoBehaviour
{
    public Material redMaterial;   // Material for red player
    public Material greenMaterial; // Material for green player
    public Material blueMaterial;  // Material for blue player
    public Material defaultMaterial; // Default material for "Player" (neutral)

    // Function to update the player's material based on their tag
    public void UpdatePlayerTexture()
    {
        Renderer playerRenderer = GetComponent<Renderer>();
        if (playerRenderer == null)
        {
            Debug.LogWarning("PlayerTextureChange: No Renderer component found on this object.");
            return;
        }

        // Assign the material based on the GameObject's tag
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
            case "Player":
                playerRenderer.material = defaultMaterial;
                break;
            default:
                Debug.LogWarning("PlayerTextureChange: Unknown tag on this object.");
                break;
        }
    }
}
