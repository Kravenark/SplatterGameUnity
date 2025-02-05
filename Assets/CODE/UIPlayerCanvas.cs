using UnityEngine;
using UnityEngine.UI;

public class UIPlayerCanvas : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Set the player number (1, 2, or 3)")]
    public int PlayerNumber;

    private GameObject playerUI;     // Player UI reference
    private Slider progressSlider;   // Slider reference
    private Image backgroundImage;   // Slider Background image
    private Image fillImage;         // Slider Fill image

    private ColourTransitionManager currentCityBlock;  // Reference to the current City Block's script

    private void Start()
    {
        // Find the UI for this player
        FindPlayerUI();

        // Initially hide the UI
        if (playerUI != null)
        {
            playerUI.SetActive(false);
        }
    }

    private void Update()
    {
        // Update the slider value if the player is inside a CityBlockAREA
        if (currentCityBlock != null)
        {
            UpdateSliderProgress();
        }
    }

    private void FindPlayerUI()
    {
        // Find the Canvas
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("PlayerUI: No Canvas found in the scene!");
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
        }
        else
        {
            Debug.LogWarning($"PlayerUI: UI GameObject '{uiObjectName}' not found under the Canvas.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("CityBlockAREA"))
        {
            // Enable the UI when entering a CityBlockAREA
            if (playerUI != null)
            {
                playerUI.SetActive(true);
                Debug.Log($"PlayerUI: Player {PlayerNumber} entered {other.gameObject.name}. UI activated.");
            }

            // Get the ColourTransitionManager from the City Block
            currentCityBlock = other.GetComponent<ColourTransitionManager>();
            if (currentCityBlock == null)
            {
                Debug.LogWarning($"PlayerUI: No ColourTransitionManager found on {other.gameObject.name}.");
            }

            // Update colors based on city block and player tags
            UpdateUIColors(other.gameObject.tag, this.gameObject.tag);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("CityBlockAREA"))
        {
            // Disable the UI when leaving a CityBlockAREA
            if (playerUI != null)
            {
                playerUI.SetActive(false);
                Debug.Log($"PlayerUI: Player {PlayerNumber} exited {other.gameObject.name}. UI deactivated.");
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
                Debug.LogWarning($"PlayerUI: Unknown tag '{tag}'. Defaulting to white.");
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
                Debug.LogWarning($"PlayerUI: Unknown player tag '{this.gameObject.tag}'. Slider won't update.");
                break;
        }

        // Normalize the percentage (assuming the slider's value goes from 0 to 1)
        progressSlider.value = playerPercentage / 100f;
    }
}
