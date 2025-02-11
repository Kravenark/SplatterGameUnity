using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerCanvas : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Set the player number (1, 2, or 3)")]
    public int PlayerNumber;

    private GameObject playerUI;
    private GameObject playerDeathUI;
    private GameObject deathHoverSpawnPos;
    private Slider progressSlider;
    private Image backgroundImage;
    private Image fillImage;
    private TextMeshProUGUI healthText;

    private Transform playerTransform;
    private PlayerCombat playerCombat;
    private Camera playerCamera;

    [Header("Respawn Settings")]
    public LayerMask spawnLayerMask; // Set this to the layer where spawn points exist

    private void Start()
    {
        GameObject playerObject = GameObject.Find($"Player{PlayerNumber}");
        if (playerObject == null)
        {
            Debug.LogError($"UIPlayerCanvas: Player{PlayerNumber} not found!");
            return;
        }

        playerCombat = playerObject.GetComponent<PlayerCombat>();
        if (playerCombat == null)
        {
            Debug.LogError($"UIPlayerCanvas: Player{PlayerNumber} missing PlayerCombat component!");
            return;
        }

        playerTransform = playerObject.transform;
        playerCamera = GameObject.Find($"Player{PlayerNumber}Camera")?.GetComponent<Camera>();

        if (playerCamera == null)
        {
            Debug.LogError($"UIPlayerCanvas: Camera for Player{PlayerNumber} not found!");
            return;
        }

        FindPlayerUI();
    }

    private void Update()
    {
        if (playerUI != null)
        {
            UpdateHealthText();
            UpdateHealthTextPosition();
            HandleDeathUI();
            
            if (playerCombat.health <= 0)
            {
                ChooseRespawnLocation();
            }
        }
    }

    private void FindPlayerUI()
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("UIPlayerCanvas: No Canvas found in the scene!");
            return;
        }

        string uiObjectName = $"Player0{PlayerNumber}UI";
        Transform uiTransform = canvas.transform.Find(uiObjectName);

        if (uiTransform == null)
        {
            Debug.LogError($"UIPlayerCanvas: Could not find {uiObjectName} inside the Canvas!");
            return;
        }

        playerUI = uiTransform.gameObject;
        Transform healthTextTransform = playerUI.transform.Find("HealthText");

        if (healthTextTransform == null)
        {
            Debug.LogError($"UIPlayerCanvas: HealthText not found inside {uiObjectName}.");
        }
        else
        {
            healthText = healthTextTransform.GetComponent<TextMeshProUGUI>();
            if (healthText == null)
            {
                Debug.LogError($"UIPlayerCanvas: HealthText exists but is missing the TextMeshProUGUI component in {uiObjectName}.");
            }
        }

        // Find PlayerDeathUI
        Transform deathUITransform = playerUI.transform.Find($"Player0{PlayerNumber}DeathUI");
        if (deathUITransform == null)
        {
            Debug.LogError($"UIPlayerCanvas: Player0{PlayerNumber}DeathUI not found inside {uiObjectName}.");
        }
        else
        {
            playerDeathUI = deathUITransform.gameObject;
            playerDeathUI.SetActive(false);
        }

        // Find Death Hover Spawn Position inside Death UI
        Transform hoverSpawnTransform = deathUITransform.Find("Death Hover SpawnPos");
        if (hoverSpawnTransform == null)
        {
            Debug.LogError($"UIPlayerCanvas: Death Hover SpawnPos not found inside {uiObjectName}.");
        }
        else
        {
            deathHoverSpawnPos = hoverSpawnTransform.gameObject;
        }
    }

    private void UpdateHealthText()
    {
        if (healthText != null && playerCombat != null)
        {
            healthText.text = $"{playerCombat.health} / 100";
        }
    }

    private void UpdateHealthTextPosition()
    {
        if (healthText == null || playerTransform == null || playerCamera == null) return;

        Vector3 worldPosition = playerTransform.position + Vector3.up * 2.0f;
        Vector3 screenPosition = playerCamera.WorldToScreenPoint(worldPosition);

        if (screenPosition.z > 0)
        {
            healthText.transform.position = screenPosition;
        }
    }

    private void HandleDeathUI()
    {
        if (playerCombat != null && playerDeathUI != null)
        {
            if (playerCombat.health <= 0)
            {
                playerDeathUI.SetActive(true);
            }
            else
            {
                playerDeathUI.SetActive(false);
            }
        }
    }

    private void ChooseRespawnLocation()
    {
        if (deathHoverSpawnPos == null || playerCamera == null) return;

        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, spawnLayerMask))
        {
            GameObject spawnPoint = hit.collider.gameObject;

            // **Only snap if the spawn point matches the player's tag**
            if (spawnPoint.CompareTag(gameObject.tag))
            {
                Debug.Log($"UIPlayerCanvas: Hovered over valid spawn point {spawnPoint.name}.");

                // Move Death Hover SpawnPos to the screen position of the hovered spawn point
                Vector3 screenPos = playerCamera.WorldToScreenPoint(spawnPoint.transform.position);
                if (screenPos.z > 0)
                {
                    deathHoverSpawnPos.transform.position = screenPos;
                }
            }
        }
    }
}
