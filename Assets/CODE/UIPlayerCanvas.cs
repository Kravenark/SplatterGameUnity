using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class UIPlayerCanvas : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Set the player number (1, 2, or 3)")]
    public int PlayerNumber;

    private GameObject playerUI;         // Player UI reference
    private Slider progressSlider;       // Slider reference for spraying
    private Image backgroundImage;       // Slider Background image
    private Image fillImage;             // Slider Fill image

    public TextMeshProUGUI  healthText;             // Text object to display health
    private GameObject playerDeathUI;    // Player Death UI reference

    private ColourTransitionManager currentCityBlock;  // Reference to the current City Block's script
    private PlayerManager playerManager;               // Reference to PlayerManager
    private GameObject player;                         // Reference to the player

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        player = GameObject.Find($"Player{PlayerNumber}");

        if (playerManager == null || player == null)
        {
            Debug.LogError("UIPlayerCanvas: PlayerManager or Player not found.");
            return;
        }

        // Find the UI for this player
        FindPlayerUI();

        // Initially hide the progress slider and death UI
        if (progressSlider != null)
        {
            progressSlider.gameObject.SetActive(false);
        }

        if (playerDeathUI != null)
        {
            playerDeathUI.SetActive(false); // Hide death UI initially
        }

        // Initialize the health text display
        UpdateHealthText();  // Initialize health display
    }

    private void Update()
    {
        // Update health display continuously
        UpdateHealthText();

        // Update health text position to follow the player
        UpdateHealthTextPosition();

        // Update the slider value if the player is inside a CityBlockAREA
        if (currentCityBlock != null)
        {
            UpdateSliderProgress();
        }

        // Check if the player is dead to show the death UI
        if (playerManager.GetPlayerHealth(player) <= 0)
        {
            ShowDeathUI();
        }
        else
        {
            HideDeathUI();
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

        // Construct the Player UI name (e.g., "Player01UI")
        string uiObjectName = $"Player0{PlayerNumber}UI";

        // Find the Player UI inside the Canvas
        playerUI = canvas.transform.Find(uiObjectName)?.gameObject;

        if (playerUI != null)
        {
            // Find the Slider and its components
            progressSlider = playerUI.GetComponentInChildren<Slider>();
            if (progressSlider != null)
            {
                backgroundImage = progressSlider.transform.Find("Background")?.GetComponent<Image>();
                fillImage = progressSlider.transform.Find("Fill Area/Fill")?.GetComponent<Image>();
            }

            // Find the Death UI
            playerDeathUI = playerUI.transform.Find($"Player0{PlayerNumber}DeathUI")?.gameObject;
            if (playerDeathUI == null)
            {
                Debug.LogWarning($"UIPlayerCanvas: Death UI not found under {uiObjectName}.");
            }

            // Find the Health Text
            healthText = playerUI.transform.Find("HealthText")?.GetComponent<TextMeshProUGUI >();
            if (healthText == null)
            {
                Debug.LogWarning("UIPlayerCanvas: No HealthText found under Player UI.");
            }
        }
        else
        {
            Debug.LogWarning($"UIPlayerCanvas: UI GameObject '{uiObjectName}' not found under the Canvas.");
        }
    }
private void UpdateHealthText()
{
    if (healthText != null && playerManager != null)
    {
        float currentHealth = gameObject.GetComponent<PlayerCombat >().health;
        currentHealth = (float)(Math.Floor(currentHealth * 10) / 10);
        float maxHealth = playerManager.maxHealth;
        healthText.text = $"{currentHealth} / {maxHealth}";
        Debug.Log($"Health Text Updated: {healthText.text}");  // Debug log
    }
}

    private void UpdateHealthTextPosition()
    {
        if (healthText != null && player != null)
        {
            // Position the health text above the player's head
            Vector3 worldPosition = player.transform.position + Vector3.up * 2.0f; // Adjust height as needed
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
            
            // Ensure the health text is visible on screen
            if (screenPosition.z > 0)
            {
                healthText.transform.position = screenPosition;
            }
        }
    }

    private void ShowDeathUI()
    {
        if (playerDeathUI != null)
        {
            playerDeathUI.SetActive(true);
            Debug.Log($"UIPlayerCanvas: Player {PlayerNumber} has died. Death UI activated.");
        }
    }

    private void HideDeathUI()
    {
        if (playerDeathUI != null)
        {
            playerDeathUI.SetActive(false);
            Debug.Log($"UIPlayerCanvas: Player {PlayerNumber} is alive. Death UI deactivated.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("CityBlockAREA"))
        {
            // Enable the progress slider when entering a CityBlockAREA
            if (progressSlider != null)
            {
                progressSlider.gameObject.SetActive(true);
                Debug.Log($"UIPlayerCanvas: Player {PlayerNumber} entered {other.gameObject.name}. Slider activated.");
            }

            // Get the ColourTransitionManager from the City Block
            currentCityBlock = other.GetComponent<ColourTransitionManager>();
            if (currentCityBlock == null)
            {
                Debug.LogWarning($"UIPlayerCanvas: No ColourTransitionManager found on {other.gameObject.name}.");
            }

            // Update colors based on city block and player tags
            UpdateUIColors(other.gameObject.tag, this.gameObject.tag);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("CityBlockAREA"))
        {
            // Disable the progress slider when leaving a CityBlockAREA
            if (progressSlider != null)
            {
                progressSlider.gameObject.SetActive(false);
                Debug.Log($"UIPlayerCanvas: Player {PlayerNumber} exited {other.gameObject.name}. Slider deactivated.");
            }

            // Clear the reference to the City Block
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
                Debug.LogWarning($"UIPlayerCanvas: Unknown tag '{tag}'. Defaulting to white.");
                return Color.white;
        }
    }

    private void UpdateSliderProgress()
    {
        if (progressSlider == null || currentCityBlock == null)
        {
            return;
        }

        // Determine which percentage to use based on the player's tag
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
                break;
        }

        // Normalize the percentage (assuming the slider's value goes from 0 to 1)
        progressSlider.value = playerPercentage / 100f;
    }
}