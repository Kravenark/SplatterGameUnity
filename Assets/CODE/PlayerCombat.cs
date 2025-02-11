using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    [Header("Shooting Settings")]
    public float shootingCooldown = 1f;
    public float shootingRange = 50f;
    public LayerMask raycastLayerMask;
    public float lineRendererLength = 10f;

    [Header("Health Settings")]
    public float health = 100f;
    public float damagePerSecond = 20f;
    public float respawnDelay = 3f;

    private float lastShotTime = 0f;
    private bool isRespawning = false;
    private LineRenderer lineRenderer;
    private Coroutine damageCoroutine = null;
    private PlayerCombat currentTarget = null; // Tracks the player being hit

    private static Dictionary<PlayerCombat, int> activeAttackers = new Dictionary<PlayerCombat, int>();

    [Header("Player Settings")]
    public int playerNumber = 1; // 1 for Player 1, 2 for Player 2

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("PlayerCombat: No Line Renderer component found.");
            return;
        }

        // Configure Line Renderer
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

        if (ShouldShoot() && Time.time >= lastShotTime + shootingCooldown)
        {
            Shoot();
        }
        else if (ShouldStopShooting())
        {
            StopShooting();
        }
    }

    private bool ShouldShoot()
    {
        return (playerNumber == 1 && Input.GetKey(KeyCode.E)) ||
               (playerNumber == 2 && Input.GetKey(KeyCode.O));
    }

    private bool ShouldStopShooting()
    {
        return (playerNumber == 1 && Input.GetKeyUp(KeyCode.E)) ||
               (playerNumber == 2 && Input.GetKeyUp(KeyCode.O));
    }

    private void Shoot()
    {
        lastShotTime = Time.time;
        Vector3 shootingDirection = transform.forward;
        Vector3 startPoint = transform.position;
        Vector3 endPoint = startPoint + (shootingDirection * lineRendererLength);

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
        lineRenderer.enabled = true;

        if (Physics.Raycast(startPoint, shootingDirection, out RaycastHit hit, lineRendererLength, raycastLayerMask))
        {
            Debug.Log($"Shoot: Ray hit {hit.collider.gameObject.name}");
            endPoint = hit.point;
            GameObject target = hit.collider.gameObject;

            // **Handle player hit**
            if (target.CompareTag("PlayerRed") || target.CompareTag("PlayerGreen") || target.CompareTag("PlayerBlue"))
            {
                Debug.Log($"[Combat] {gameObject.name} hit {target.name} ({target.tag})");

                if (target.tag != this.gameObject.tag) // Ensure target is an enemy (different color)
                {
                    PlayerCombat targetCombat = target.GetComponent<PlayerCombat>();

                    if (targetCombat != null)
                    {
                        // **Start damage only if it's a new target**
                        if (currentTarget != targetCombat)
                        {
                            StopDamage(); // Stop damage on previous target
                            currentTarget = targetCombat;
                            targetCombat.StartTakingDamage(this);
                        }
                    }
                }
            }
            // **Handle city building spray**
            else if (target.CompareTag("CityBuildingRed") || target.CompareTag("CityBuildingGreen") ||
                     target.CompareTag("CityBuildingBlue") || target.CompareTag("CityBuildingGrey"))
            {
                CityBuildingTextureChange building = target.GetComponent<CityBuildingTextureChange>();
                if (building != null)
                {
                    building.StartSpraying(gameObject.tag);
                }
            }
            else
            {
                StopDamage(); // **Only stops damage if the raycast does not hit a player or building**
            }
        }
        else
        {
            StopDamage(); // **Stop damage if raycast misses entirely**
        }

        DrawRay(startPoint, endPoint);
    }

    private void StopShooting()
    {
        lineRenderer.enabled = false;
        StopSprayingBuildings();
        StopDamage();
    }

    private void StopSprayingBuildings()
    {
        CityBuildingTextureChange[] allBuildings = FindObjectsOfType<CityBuildingTextureChange>();
        foreach (CityBuildingTextureChange building in allBuildings)
        {
            building.StopSpraying();
        }
    }

    private void StopDamage()
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }

        if (currentTarget != null)
        {
            currentTarget.StopTakingDamage(this);
            currentTarget = null; // Reset current target
        }
    }

    public void StartTakingDamage(PlayerCombat attacker)
    {
        if (!activeAttackers.ContainsKey(this))
        {
            activeAttackers[this] = 0;
        }

        activeAttackers[this]++; // Track number of attackers

        if (damageCoroutine == null)
        {
            damageCoroutine = StartCoroutine(DamageOverTime());
        }
    }

    public void StopTakingDamage(PlayerCombat attacker)
    {
        if (activeAttackers.ContainsKey(this))
        {
            activeAttackers[this]--;

            if (activeAttackers[this] <= 0)
            {
                activeAttackers.Remove(this);
                if (damageCoroutine != null)
                {
                    StopCoroutine(damageCoroutine);
                    damageCoroutine = null;
                }
            }
        }
    }

    private IEnumerator DamageOverTime()
    {
        while (health > 0 && activeAttackers.ContainsKey(this) && activeAttackers[this] > 0)
        {
            health -= damagePerSecond * Time.deltaTime;
            Debug.Log($"[Damage] {gameObject.name} is taking damage. Health: {health}");

            if (health <= 0)
            {
                Debug.Log($"[Eliminated] {gameObject.name} has been defeated!");
                HandleDeath();
                yield break;
            }

            yield return null; // Wait for the next frame
        }

        damageCoroutine = null;
    }

    private void HandleDeath()
    {
        Debug.Log($"{gameObject.name} has died. Respawning in {respawnDelay} seconds...");

        // Disable shooting & movement
        this.enabled = false;

        // Reset health
        health = 100;

        // Respawn after delay
        StartCoroutine(RespawnAfterDelay(respawnDelay));
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log($"{gameObject.name} has respawned.");

        // Enable shooting & movement
        this.enabled = true;
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
}
