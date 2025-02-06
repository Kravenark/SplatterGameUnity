using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerCanvas : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Set the player number (1, 2, or 3)")]
    public int PlayerNumber;

    private GameObject playerUI;         // Player UI reference
    private Slider progressSlider;       // Slider reference
    private Image backgroundImage;       // Slider Background image
    private Image fillImage;             // Slider Fill image
private TMPro.TextMeshProUGUI healthText;  // Make sure this is TextMeshProUGUI

            // Health text reference

    private ColourTransitionManager currentCityBlock;  // Reference to the current City Block's script
    private Transform playerTransform;   // Reference to the player transform
    private PlayerManager playerManager; // Reference to PlayerManager to get health data

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        if (playerManager == null)
        {
            Debug.LogError("UIPlayerCanvas: PlayerManager not found in the scene!");
            return;
        }

        // Find the player object
        playerTransform = GameObject.Find($"Player{PlayerNumber}").transform;

        if (playerTransform == null)
        {
            Debug.LogError($"UIPlayerCanvas: Player{PlayerNumber} not found!");
            return;
        }

        // Find the UI for this player
        FindPlayerUI();

        // Initially hide the UI
        if (playerUI != null)
        {
            playerUI.SetActive(true);
        }
    }

private void Update()
{
    if (playerUI != null)
    {
        // Ensure Health Text follows the player
        UpdateHealthTextPosition();
    }

    if (currentCityBlock != null)
    {
        UpdateSliderProgress();
    }
}
private void UpdateHealthTextPosition()
{
    if (healthText == null) return;

    // Find the player's camera dynamically based on PlayerNumber
    Camera playerCamera = (PlayerNumber == 1) ? GameObject.Find("Player1Camera").GetComponent<Camera>()
                                              : GameObject.Find("Player2Camera").GetComponent<Camera>();

    if (playerCamera == null)
    {
        Debug.LogError($"UIPlayerCanvas: Camera for Player{PlayerNumber} not found!");
        return;
    }

    // Convert world position to screen position using the player's camera
    Vector3 screenPosition = playerCamera.WorldToScreenPoint(transform.position + Vector3.up * 2.0f);
    
    // Only update position if the object is in front of the camera
    if (screenPosition.z > 0)
    {
        healthText.transform.position = screenPosition;
    }
}

private void FindPlayerUI()
{
    // Find the Canvas
    GameObject canvas = GameObject.Find("Canvas");
    if (canvas == null)
    {
        Debug.LogError("UIPlayerCanvas: No Canvas found in the scene!");
        return;
    }

    // Construct the Player UI name dynamically (e.g., "Player01UI")
    string uiObjectName = $"Player0{PlayerNumber}UI";

    // Find the Player UI inside the Canvas
    Transform uiTransform = canvas.transform.Find(uiObjectName);

    if (uiTransform == null)
    {
        Debug.LogError($"UIPlayerCanvas: Could not find {uiObjectName} inside the Canvas!");
        return;
    }

    playerUI = uiTransform.gameObject;

    // **Find the Slider inside Player UI**
    Transform sliderTransform = playerUI.transform.Find("Slider");
    if (sliderTransform == null)
    {
        Debug.LogError($"UIPlayerCanvas: Slider not found inside {uiObjectName}.");
    }
    else
    {
        progressSlider = sliderTransform.GetComponent<Slider>();
        if (progressSlider == null)
        {
            Debug.LogError($"UIPlayerCanvas: Slider exists but is missing the Slider component in {uiObjectName}.");
        }
    }

    // **Find the Background inside the Slider**
    Transform backgroundTransform = sliderTransform.Find("Background");
    if (backgroundTransform == null)
    {
        Debug.LogError($"UIPlayerCanvas: Background not found inside Slider for {uiObjectName}.");
    }
    else
    {
        backgroundImage = backgroundTransform.GetComponent<UnityEngine.UI.Image>();
        if (backgroundImage == null)
        {
            Debug.LogError($"UIPlayerCanvas: Background exists but is missing Image component in {uiObjectName}.");
        }
    }

    // **Find the Fill inside the Slider**
    Transform fillTransform = sliderTransform.Find("Fill Area/Fill");
    if (fillTransform == null)
    {
        Debug.LogError($"UIPlayerCanvas: Fill not found inside Slider for {uiObjectName}.");
    }
    else
    {
        fillImage = fillTransform.GetComponent<UnityEngine.UI.Image>();
        if (fillImage == null)
        {
            Debug.LogError($"UIPlayerCanvas: Fill exists but is missing Image component in {uiObjectName}.");
        }
    }

    // **Find the HealthText inside Player UI**
    Transform healthTextTransform = playerUI.transform.Find("HealthText");
    if (healthTextTransform == null)
    {
        Debug.LogError($"UIPlayerCanvas: HealthText not found inside {uiObjectName}.");
    }
    else
    {
        healthText = healthTextTransform.GetComponent<TMPro.TextMeshProUGUI>();
        if (healthText == null)
        {
            Debug.LogError($"UIPlayerCanvas: HealthText exists but is missing the TextMeshProUGUI component in {uiObjectName}.");
        }
    }

    Debug.Log($"UIPlayerCanvas: Successfully found UI components for Player {PlayerNumber}");
}





    private void UpdateHealthText()
    {
        if (healthText != null && playerManager != null)
        {
            int currentHealth = playerManager.GetPlayerHealth(playerTransform.gameObject);
            int maxHealth = playerManager.maxHealth;

            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

private void OnTriggerEnter(Collider other)
{
    if (other.gameObject.layer == LayerMask.NameToLayer("CityBlockAREA"))
    {
        if (progressSlider != null)
        {
            progressSlider.gameObject.SetActive(true);
            Debug.Log($"UIPlayerCanvas: Player {PlayerNumber} entered {other.gameObject.name}. Slider activated.");
        }

        currentCityBlock = other.GetComponent<ColourTransitionManager>();
        if (currentCityBlock == null)
        {
            Debug.LogWarning($"PlayerUI: No ColourTransitionManager found on {other.gameObject.name}.");
        }

        // Update UI colors
        UpdateUIColors(other.gameObject.tag, this.gameObject.tag);
    }
}


private void OnTriggerExit(Collider other)
{
    if (other.gameObject.layer == LayerMask.NameToLayer("CityBlockAREA"))
    {
        Debug.Log("I touched a city block trigger");
        progressSlider.gameObject.SetActive(false);
        if (progressSlider != null)
        {
            progressSlider.gameObject.SetActive(false);
            Debug.Log($"UIPlayerCanvas: Player {PlayerNumber} exited {other.gameObject.name}. Slider deactivated.");
        }

        currentCityBlock = null;
    }
}



private void UpdateUIColors(string cityBlockTag, string playerTag)
{
    if (backgroundImage != null)
    {
        backgroundImage.color = GetColorFromTag(cityBlockTag);  // Background based on CityBlock tag
    }

    if (fillImage != null)
    {
        fillImage.color = GetColorFromTag(playerTag);  // Fill based on Player tag
    }
}

private Color GetColorFromTag(string tag)
{
    switch (tag)
    {
        case "CityBlockRed":
        case "PlayerRed":
            return Color.red;

        case "CityBlockGreen":
        case "PlayerGreen":
            return Color.green;

        case "CityBlockBlue":
        case "PlayerBlue":
            return Color.blue;

        case "CityBlockGrey":
            return Color.grey;

        default:
            Debug.LogWarning($"PlayerUI: Unknown tag '{tag}'. Defaulting to white.");
            return Color.white;
    }
}


private void UpdateSliderProgress()
{
    if (progressSlider == null || currentCityBlock == null)
    {
        Debug.LogWarning($"UIPlayerCanvas: Slider or CityBlock reference missing for Player {PlayerNumber}.");
        return;
    }

    float playerPercentage = 0f;

    switch (this.gameObject.tag)
    {
        case "PlayerRed":
            playerPercentage = currentCityBlock.percColoursRed;
            break;
        case "PlayerGreen":
            playerPercentage = currentCityBlock.percColoursGreen;
            break;
        case "PlayerBlue":
            playerPercentage = currentCityBlock.percColoursBlue;
            break;
        default:
            Debug.LogWarning($"UIPlayerCanvas: Unknown player tag '{this.gameObject.tag}'. Slider won't update.");
            return;
    }

    float newValue = Mathf.Clamp(playerPercentage / 100f, 0f, 1f);

    Debug.Log($"[Slider Debug] Player {PlayerNumber} - Tag: {this.gameObject.tag} - Assigned Percentage: {playerPercentage}% - New Slider Value: {newValue}");

    // Update slider
    progressSlider.value = newValue;
}


}
