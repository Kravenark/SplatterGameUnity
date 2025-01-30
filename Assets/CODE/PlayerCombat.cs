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

        // Start firing when holding the fire button
        if (Input.GetMouseButton(0) && Time.time >= lastShotTime + shootingCooldown)
        {
            Shoot();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            lineRenderer.enabled = false; // Hide the line when the button is released
        }
    }

    private void Shoot()
    {
        lastShotTime = Time.time;

        // Get the ray from camera to mouse
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 worldMousePosition;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, raycastLayerMask))
        {
            worldMousePosition = hit.point;
        }
        else
        {
            worldMousePosition = ray.GetPoint(10f);
        }

        // Ensure the Y level stays the same
        float playerY = transform.position.y;
        worldMousePosition.y = playerY;

        // Get direction from player to mouse
        Vector3 direction = (worldMousePosition - transform.position).normalized;
        if (direction == Vector3.zero) direction = transform.forward; // Prevent zero vector issues

        // Calculate the end point
        Vector3 endPoint = transform.position + (direction * lineRendererLength);
        endPoint.y = playerY;

        // Set line renderer positions
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint);
        lineRenderer.enabled = true;

        Debug.Log($"Shooting in direction: {direction}, End Point: {endPoint}");

        // Perform the raycast
        if (Physics.Raycast(transform.position, direction, out hit, lineRendererLength, raycastLayerMask))
        {
            endPoint = hit.point;
            endPoint.y = playerY;

            GameObject target = hit.collider.gameObject;

            // Handle hitting players
            if (target.CompareTag("PlayerRed") || target.CompareTag("PlayerGreen") || target.CompareTag("PlayerBlue"))
            {
                if (Random.Range(1, 101) <= hitChance)
                {
                    Debug.Log($"HIT! {gameObject.name} hit {target.name}");
                    HandleHit(target);
                }
                else
                {
                    Debug.Log($"MISS! {gameObject.name} missed {target.name}");
                }
            }
            // Handle hitting buildings
            else if (target.CompareTag("CityBuildingRed") || target.CompareTag("CityBuildingGreen") ||
                     target.CompareTag("CityBuildingBlue") || target.CompareTag("CityBuildingGrey"))
            {
                CityBuildingTextureChange building = target.GetComponent<CityBuildingTextureChange>();
                if (building != null)
                {
                    string playerTag = gameObject.tag;
                    building.StartCoroutine(building.SmoothBuildingTransition(playerTag));
                }
            }
        }

        DrawRay(transform.position, endPoint);
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
            // targetAI.StartRespawn(); (Uncomment when needed)
        }

        Debug.Log($"{target.name} was hit by {gameObject.name}!");
    }
}
