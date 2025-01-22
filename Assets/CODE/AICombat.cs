using UnityEngine;
using UnityEngine.AI;

public class AICombat : MonoBehaviour
{
    public float shootingCooldown = 1f;
    public int hitChance = 50;
    public float shootingRange = 20f;
    public LayerMask raycastLayerMask;
    public float respawnDelay = 3f;
    public float chaseRange = 15f;

    private float lastShotTime = 0f;
    private bool isRespawning = false;
    private NavMeshAgent agent;
    public GameObject targetPlayer; // Make public for debugging
    private CityBlockManager cityBlockManager;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError($"AICombat: No NavMeshAgent found on {gameObject.name}.");
        }

        // Reference to the CityBlockManager
        cityBlockManager = FindObjectOfType<CityBlockManager>();
        if (cityBlockManager == null)
        {
            Debug.LogError("AICombat: CityBlockManager not found in the scene.");
        }
    }

    private void Update()
    {
        if (isRespawning) return;

        // Check if the current target is valid
        if (targetPlayer == null || !targetPlayer.activeSelf)
        {
            targetPlayer = null;
            FindAndMoveToClosestBase(); // Switch to base-searching behavior
            return;
        }

        // Check the distance to the current target
        float distanceToTarget = Vector3.Distance(transform.position, targetPlayer.transform.position);

        if (distanceToTarget > chaseRange)
        {
            // Target is out of range, reset to base-searching behavior
            targetPlayer = null;
            FindAndMoveToClosestBase();
            return;
        }

        if (distanceToTarget <= shootingRange)
        {
            agent.isStopped = true;
            if (Time.time >= lastShotTime + shootingCooldown)
            {
                Shoot(targetPlayer);
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(targetPlayer.transform.position);
        }
    }

    private void Shoot(GameObject target)
    {
        lastShotTime = Time.time;

        Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, directionToTarget, out RaycastHit hit, shootingRange, raycastLayerMask))
        {
            Debug.DrawRay(transform.position, directionToTarget * shootingRange, Color.red, 0.1f);

            if (hit.collider.gameObject == target)
            {
                int randomChance = Random.Range(1, 101);
                if (randomChance <= hitChance)
                {
                    HandleHit(target);
                }
            }
        }
    }

    private void HandleHit(GameObject target)
    {
        PlayerCombat playerCombat = target.GetComponent<PlayerCombat>();
        AICombat targetAI = target.GetComponent<AICombat>();

        if (playerCombat != null)
        {
            playerCombat.TakeDamage();
        }
        else if (targetAI != null)
        {
            targetAI.StartRespawn();
        }

        // Clear the target if it's dead or inactive
        if (target == targetPlayer)
        {
            targetPlayer = null;
            FindAndMoveToClosestBase(); // Reset to base-searching behavior
        }
    }

    public void StartRespawn()
    {
        isRespawning = true;

        // Disable the AI during respawn
        gameObject.SetActive(false);

        // Respawn after the delay
        Invoke(nameof(Respawn), respawnDelay);
    }

    private void Respawn()
    {
        if (cityBlockManager == null)
        {
            Debug.LogError("AICombat: CityBlockManager reference missing.");
            return;
        }

        // Find the spawn positions based on the AI's color tag
        GameObject[] spawnBlocks = cityBlockManager.GetCityBlocksByTag(gameObject.tag.Replace("Player", "CityBlock"));

        if (spawnBlocks.Length > 0)
        {
            // Choose a random spawn point
            GameObject randomSpawnBlock = spawnBlocks[Random.Range(0, spawnBlocks.Length)];
            Transform[] spawnPositions = randomSpawnBlock.GetComponentsInChildren<Transform>();
            foreach (Transform spawnPos in spawnPositions)
            {
                if (spawnPos.name == "CityBlock SpawnPos")
                {
                    transform.position = spawnPos.position;
                    break;
                }
            }
        }

        gameObject.SetActive(true); // Reactivate the AI
        isRespawning = false;

        // Reset behavior after respawning
        targetPlayer = null;
        FindAndMoveToClosestBase();
    }

    private void FindClosestPlayer()
    {
        GameObject[] redPlayers = GameObject.FindGameObjectsWithTag("PlayerRed");
        GameObject[] greenPlayers = GameObject.FindGameObjectsWithTag("PlayerGreen");
        GameObject[] bluePlayers = GameObject.FindGameObjectsWithTag("PlayerBlue");

        GameObject[] allPlayers = new GameObject[redPlayers.Length + greenPlayers.Length + bluePlayers.Length];
        redPlayers.CopyTo(allPlayers, 0);
        greenPlayers.CopyTo(allPlayers, redPlayers.Length);
        bluePlayers.CopyTo(allPlayers, redPlayers.Length + greenPlayers.Length);

        float closestDistance = Mathf.Infinity;
        GameObject closest = null;

        foreach (GameObject player in allPlayers)
        {
            if (player == gameObject || !player.activeSelf) continue; // Skip itself or inactive players

            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = player;
            }
        }

        targetPlayer = closest;
    }

    private void FindAndMoveToClosestBase()
    {
        // Search for the closest grey block or random non-matching block
        GameObject targetBlock = cityBlockManager.FindClosestGreyBlock(transform.position) ??
                                 cityBlockManager.FindRandomBlockExcludingTag(gameObject.tag.Replace("Player", "CityBlock"));

        if (targetBlock != null)
        {
            agent.isStopped = false;
            agent.SetDestination(targetBlock.transform.position);
        }
    }
}
