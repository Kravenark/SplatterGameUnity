using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    public float shootingCooldown = 1f;
    public int hitChance = 50;
    public float shootingRange = 50f;
    public LayerMask raycastLayerMask;
    public float lineRendererLength = 10f;
    public float respawnDelay = 3f;

    private float lastShotTime = 0f;
    private bool isRespawning = false;
    private Vector3 initialPosition;
    private LineRenderer lineRenderer;

    public float health = 100;

    [Header("Player Settings")]
    public int playerNumber = 1; // 1 for Player 1, 2 for Player 2

    private void Start()
    {
        initialPosition = transform.position;

        lineRenderer = gameObject.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("PlayerCombat: No Line Renderer component found on this object.");
            return;
        }

        // Configure the Line Renderer
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (isRespawning) return;

        // Handle shooting based on player number
        if (ShouldShoot() && Time.time >= lastShotTime + shootingCooldown)
        {
            Shoot();
        }
        else if (ShouldStopShooting())
        {
            lineRenderer.enabled = false;
            StopSprayingBuildings();
        }
    }

    private bool ShouldShoot()
    {
        if (playerNumber == 1)
        {
            return Input.GetKey(KeyCode.E);
        }
        else if (playerNumber == 2)
        {
            return Input.GetKey(KeyCode.O);
        }

        return false;
    }

    private bool ShouldStopShooting()
    {
        if (playerNumber == 1)
        {
            return Input.GetKeyUp(KeyCode.E);
        }
        else if (playerNumber == 2)
        {
            return Input.GetKeyUp(KeyCode.O);
        }

        return false;
    }

    private void StopSprayingBuildings()
    {
        CityBuildingTextureChange[] allBuildings = FindObjectsOfType<CityBuildingTextureChange>();
        foreach (CityBuildingTextureChange building in allBuildings)
        {
            building.StopSpraying();
        }
    }

    private void Shoot()
    {
        lastShotTime = Time.time;

        // Shoot directly forward from the player
        Vector3 shootingDirection = transform.forward;

        // Set the line renderer positions
        Vector3 startPoint = transform.position;
        Vector3 endPoint = startPoint + (shootingDirection * lineRendererLength);

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
        lineRenderer.enabled = true;

        // Perform the raycast to detect hits
        if (Physics.Raycast(startPoint, shootingDirection, out RaycastHit hit, lineRendererLength, raycastLayerMask))
        {
            endPoint = hit.point;
            GameObject target = hit.collider.gameObject;

            // Handle player hit
            if (target.CompareTag("PlayerRed") || target.CompareTag("PlayerGreen") || target.CompareTag("PlayerBlue"))
            {
                if (Random.Range(1, 101) <= hitChance)
                {
                    HandleHit(target);
                }
            }
            // Handle city building spray
            else if (target.CompareTag("CityBuildingRed") || target.CompareTag("CityBuildingGreen") ||
                     target.CompareTag("CityBuildingBlue") || target.CompareTag("CityBuildingGrey"))
            {
                CityBuildingTextureChange building = target.GetComponent<CityBuildingTextureChange>();
                if (building != null)
                {
                    string playerTag = gameObject.tag;
                    building.StartSpraying(playerTag);
                }
            }
        }

        DrawRay(startPoint, endPoint);
    }

    private void DrawRay(Vector3 start, Vector3 end)
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
            lineRenderer.enabled = true;
        }
    }

    private void HandleHit(GameObject target)
    {
        AICombat targetAI = target.GetComponent<AICombat>();
        if (targetAI != null)
        {
            // targetAI.StartRespawn(); // Uncomment when ready
        }

        Debug.Log($"{target.name} was hit by {gameObject.name}!");
    }
}
