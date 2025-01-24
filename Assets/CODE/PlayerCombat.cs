using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float shootingCooldown = 1f; // Cooldown between shots
    public int hitChance = 50; // Chance to hit a target
    public float shootingRange = 50f; // Range of the player's raycast
    public LayerMask raycastLayerMask; // Layers the raycast can hit
    public float respawnDelay = 3f; // Delay before respawn if the player is hit

    private float lastShotTime = 0f; // Tracks time of last shot
    private bool isRespawning = false; // Tracks if the player is respawning
    private Vector3 initialPosition; // Player's spawn point
    private LineRenderer lineRenderer; // Reference to the Line Renderer

    private void Start()
    {
        // Cache the initial spawn position
        initialPosition = transform.position;

        // Get the existing Line Renderer attached to the player
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
        lineRenderer.enabled = false; // Initially disable the Line Renderer
    }

    private void Update()
    {
        if (isRespawning) return;

        // Handle shooting when the mouse button is held down
        if (Input.GetMouseButton(0) && Time.time >= lastShotTime + shootingCooldown)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        lastShotTime = Time.time; // Update the last shot time

        // Get the ray from the player's camera to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit, shootingRange, raycastLayerMask))
        {
            GameObject target = hit.collider.gameObject;

            // Check if the hit object is a building
            if (target.CompareTag("CityBuildingGrey") || target.CompareTag("CityBuildingRed") ||
                target.CompareTag("CityBuildingGreen") || target.CompareTag("CityBuildingBlue"))
            {
                string playerTag = gameObject.tag; // Get the player's tag (e.g., PlayerRed)
                CityBuildingTextureChange buildingTexture = target.GetComponent<CityBuildingTextureChange>();

                if (buildingTexture != null)
                {
                    buildingTexture.ApplyPlayerColor(playerTag);
                    Debug.Log($"PlayerCombat: {gameObject.name} sprayed {target.name}.");
                }
            }

            // Draw the ray to the hit point
            DrawRay(transform.position, hit.point);
        }
        else
        {
            // If no hit, draw the ray to the maximum range
            Vector3 targetPoint = ray.origin + ray.direction * shootingRange;
            DrawRay(transform.position, targetPoint);
        }
    }

    private void DrawRay(Vector3 start, Vector3 end)
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);

            // Show the line temporarily
            StartCoroutine(ShowRay());
        }
    }

    private System.Collections.IEnumerator ShowRay()
    {
        lineRenderer.enabled = true; // Enable the Line Renderer
        yield return new WaitForSeconds(0.1f); // Show the ray for 0.1 seconds
        lineRenderer.enabled = false; // Disable the Line Renderer
    }
}
