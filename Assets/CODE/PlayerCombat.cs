using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public float shootingCooldown = 1f;
    public float shootingRange = 50f;
    public float lineRendererLength = 10f;
    public LayerMask raycastLayerMask;
    public LineRenderer lineRenderer;

    private float lastShotTime = 0f;
    private bool isShooting = false;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("PlayerCombat: No Line Renderer attached.");
            return;
        }

        ConfigureLineRenderer();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isShooting = true;
            lineRenderer.enabled = true;
        }

        if (Input.GetMouseButton(0) && Time.time >= lastShotTime + shootingCooldown)
        {
            Shoot();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isShooting = false;
            lineRenderer.enabled = false;
        }

        if (isShooting)
        {
            UpdateSprayDirection();
        }
    }

    private void ConfigureLineRenderer()
    {
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.enabled = false;
    }

    private void Shoot()
    {
        lastShotTime = Time.time;

        Vector3 direction = GetShootDirection();
        Vector3 start = transform.position;
        Vector3 endPoint = start + (direction * lineRendererLength);

        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, shootingRange, raycastLayerMask))
        {
            GameObject target = hit.collider.gameObject;
            CityBuildingTextureChange building = target.GetComponent<CityBuildingTextureChange>();

            if (building != null)
            {
                Material playerMaterial = GetComponent<PlayerTextureChange>().GetCurrentMaterial();
                building.StartSpraying(playerMaterial, gameObject.tag);
            }

            endPoint = hit.point;
        }

        DrawRay(start, endPoint);
    }

    private Vector3 GetShootDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 direction = ray.direction;
        direction.y = 0; // Keep the spray at player's Y level
        return direction.normalized;
    }

    private void UpdateSprayDirection()
    {
        Vector3 direction = GetShootDirection();
        Vector3 endPoint = transform.position + (direction * lineRendererLength);
        DrawRay(transform.position, endPoint);
    }

    private void DrawRay(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.enabled = true;
    }
}
