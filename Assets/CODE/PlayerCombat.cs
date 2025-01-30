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

        if (Input.GetMouseButton(0) && Time.time >= lastShotTime + shootingCooldown)
        {
            Shoot();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            lineRenderer.enabled = false;

            // Stop spraying buildings when the player releases the fire button
            StopSprayingBuildings();
        }
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

        float playerY = transform.position.y;
        worldMousePosition.y = playerY;

        Vector3 direction = (worldMousePosition - transform.position).normalized;
        if (direction == Vector3.zero) direction = transform.forward;

        Vector3 endPoint = transform.position + (direction * lineRendererLength);
        endPoint.y = playerY;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint);
        lineRenderer.enabled = true;

        if (Physics.Raycast(transform.position, direction, out hit, lineRendererLength, raycastLayerMask))
        {
            endPoint = hit.point;
            endPoint.y = playerY;

            GameObject target = hit.collider.gameObject;

            if (target.CompareTag("PlayerRed") || target.CompareTag("PlayerGreen") || target.CompareTag("PlayerBlue"))
            {
                if (Random.Range(1, 101) <= hitChance)
                {
                    HandleHit(target);
                }
            }
            else if (target.CompareTag("CityBuildingRed") || target.CompareTag("CityBuildingGreen") ||
                     target.CompareTag("CityBuildingBlue") || target.CompareTag("CityBuildingGrey"))
            {
                CityBuildingTextureChange building = target.GetComponent<CityBuildingTextureChange>();
                if (building != null)
                {
                    string playerTag = gameObject.tag;
                    building.StartSpraying(playerTag); // Start spraying transition
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
