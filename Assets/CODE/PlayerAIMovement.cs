using UnityEngine;
using UnityEngine.AI;

public class PlayerAIMovement : MonoBehaviour
{
    public float movementUpdateInterval = 1f; // Time interval to update the target
    private float lastMovementUpdateTime = 0f; // Tracks the last time the target was updated
    private NavMeshAgent navMeshAgent; // Reference to the NavMeshAgent
    private CityBlockManager cityBlockManager; // Reference to the CityBlockManager
    private GameObject currentTarget; // The current target block
    private bool isRespawning = false; // Flag to indicate if the NPC is respawning

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        cityBlockManager = FindObjectOfType<CityBlockManager>();

        if (navMeshAgent == null)
        {
            Debug.LogError("PlayerAIMovement: No NavMeshAgent found on this object.");
        }

        if (cityBlockManager == null)
        {
            Debug.LogError("PlayerAIMovement: No CityBlockManager found in the scene.");
        }

        // Start by finding an initial target
        FindNextTarget();
    }

    private void Update()
    {
        if (isRespawning || navMeshAgent == null || cityBlockManager == null) return;

        // Update the target periodically
        // Update the target periodically
        if (Time.time >= lastMovementUpdateTime + movementUpdateInterval)
        {
            FindNextTarget();
            lastMovementUpdateTime = Time.time;
        }

        // Move towards the target
        if (currentTarget != null && navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(currentTarget.transform.position);
        }
    }

    private void FindNextTarget()
    {
        if (cityBlockManager == null) return;

        // First, look for the nearest grey city block
        GameObject[] greyBlocks = cityBlockManager.GetCityBlocksByTag("CityBlockGrey");
        currentTarget = FindNearestBlock(greyBlocks);

        // If no grey blocks are available, look for a random block that doesn't match the NPC's color
        if (currentTarget == null)
        {
            string npcColorTag = gameObject.tag.Replace("Player", "CityBlock");
            GameObject[] otherBlocks = cityBlockManager.GetCityBlocksExcludingTag(npcColorTag);
            currentTarget = FindNearestBlock(otherBlocks);
        }

        if (currentTarget != null)
        {
            Debug.Log($"{gameObject.name} is moving towards {currentTarget.name}.");
        }
        else
        {
            Debug.Log($"{gameObject.name} could not find a valid target.");
        }
    }

    private GameObject FindNearestBlock(GameObject[] blocks)
    {
        GameObject nearestBlock = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject block in blocks)
        {
            float distance = Vector3.Distance(transform.position, block.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestBlock = block;
            }
        }

        return nearestBlock;
    }

    public void HandleRespawn()
    {
        isRespawning = true; // Set the respawning flag
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true; // Stop the NavMeshAgent during respawn
        }
    }

    public void ResumeMovement()
    {
        isRespawning = false; // Clear the respawning flag
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = false; // Resume the NavMeshAgent
        }

        // Reassign the target to ensure the NPC starts moving again
        FindNextTarget();
    }
}
