using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    private Dictionary<GameObject, string> playerColors = new Dictionary<GameObject, string>(); // Track player colors
    public Dictionary<GameObject, int> playerHealth = new Dictionary<GameObject, int>(); // Track player health

    public int maxHealth = 100; // Maximum health for each player
    private List<string> availableColors = new List<string> { "PlayerRed", "PlayerGreen", "PlayerBlue" };

    void Start()
    {
        CityBlockManager cityBlockManager = FindObjectOfType<CityBlockManager>();

        if (cityBlockManager == null)
        {
            Debug.LogError("PlayerManager: CityBlockManager not found!");
            return;
        }

        GameObject[] players = {
            GameObject.Find("Player1"),
            GameObject.Find("Player2"),
            GameObject.Find("Player3")
        };

        foreach (GameObject player in players)
        {
            if (player == null)
            {
                Debug.LogError("PlayerManager: A player object was not found!");
                return;
            }

            // Initialize player health
            playerHealth[player] = maxHealth;
        }

        // Assign colors and ensure they are stored before teleporting
        AssignRandomColors(players);

        // Small delay before teleportation to ensure colors are assigned
        StartCoroutine(DelayedTeleport(players, cityBlockManager));
    }

    private IEnumerator DelayedTeleport(GameObject[] players, CityBlockManager cityBlockManager)
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(TeleportAllPlayersToSpawnWithDelay(players, cityBlockManager));
    }

    // Assign unique colors to players and update the dictionary
    public void AssignRandomColors(GameObject[] players)
    {
        List<string> colors = new List<string>(availableColors);

        foreach (GameObject player in players)
        {
            if (player == null)
            {
                Debug.LogWarning("PlayerManager: Player is null, cannot assign random color.");
                continue;
            }

            if (colors.Count == 0)
            {
                Debug.LogError("PlayerManager: Not enough colors for all players!");
                return;
            }

            int randomIndex = Random.Range(0, colors.Count);
            string chosenColor = colors[randomIndex];
            colors.RemoveAt(randomIndex);

            player.tag = chosenColor;
            playerColors[player] = chosenColor; // Store in dictionary

            // Apply the correct material
            PlayerTextureChange textureChange = player.GetComponent<PlayerTextureChange>();
            if (textureChange != null)
            {
                textureChange.UpdatePlayerTexture();
            }
            else
            {
                Debug.LogWarning($"PlayerManager: Player {player.name} does not have a PlayerTextureChange component.");
            }

            Debug.Log($"PlayerManager: Assigned {chosenColor} to player {player.name}.");
        }
    }

    // Reduce health of a player (call this from combat or collision scripts)
    public void ReducePlayerHealth(GameObject player, int damage)
    {
        if (playerHealth.ContainsKey(player))
        {
            playerHealth[player] -= damage;
            playerHealth[player] = Mathf.Clamp(playerHealth[player], 0, maxHealth);

            Debug.Log($"PlayerManager: {player.name} took {damage} damage. Current health: {playerHealth[player]}");

            if (playerHealth[player] <= 0)
            {
                Debug.Log($"{player.name} is eliminated!");
                // Handle player elimination (disable, respawn, etc.)
                player.SetActive(false);  // Example: deactivate the player
            }
        }
    }

    // Get player health (used by UIPlayerCanvas)
    public int GetPlayerHealth(GameObject player)
    {
        if (playerHealth.TryGetValue(player, out int health))
        {
            return health;
        }

        Debug.LogWarning($"PlayerManager: No health record found for {player.name}");
        return 0;
    }

    // Coroutine to teleport all players to their respective spawn positions after a delay
    private IEnumerator TeleportAllPlayersToSpawnWithDelay(GameObject[] players, CityBlockManager cityBlockManager)
    {
        yield return new WaitForSeconds(0.25f); // Small delay before teleporting

        if (players == null || players.Length == 0)
        {
            Debug.LogError("PlayerManager: No players found in the array.");
            yield break;
        }

        foreach (GameObject player in players)
        {
            if (player == null)
            {
                Debug.LogWarning("Skipping null player during teleport.");
                continue;
            }

            TeleportPlayerToRandomSpawn(player, cityBlockManager);
        }
    }

    // Teleport a player to a random spawn position under their corresponding city block
    public void TeleportPlayerToRandomSpawn(GameObject player, CityBlockManager cityBlockManager)
    {
        if (player == null)
        {
            Debug.LogWarning("PlayerManager: Player is null, cannot teleport.");
            return;
        }

        // Ensure the dictionary contains the player's color
        if (!playerColors.TryGetValue(player, out string playerTag))
        {
            Debug.LogWarning($"PlayerManager: Player {player.name} does not have an assigned color in dictionary.");
            return;
        }

        Debug.Log($"Teleporting {player.name} with color {playerTag}");

        // Find city blocks with the corresponding color tag using CityBlockManager
        GameObject[] cityBlocks = cityBlockManager.GetCityBlocksByTag(playerTag.Replace("Player", "CityBlock"));

        if (cityBlocks.Length == 0)
        {
            Debug.LogWarning($"PlayerManager: No city blocks found with tag {playerTag.Replace("Player", "CityBlock")}.");
            return;
        }

        // Choose a random city block from the available blocks
        GameObject matchingCityBlock = cityBlocks[Random.Range(0, cityBlocks.Length)];

        // Find all "CityBlock SpawnPos" objects under the matching city block
        Transform[] spawnPositions = matchingCityBlock.GetComponentsInChildren<Transform>();
        List<Transform> validSpawnPositions = new List<Transform>();

        foreach (Transform spawn in spawnPositions)
        {
            if (spawn.name == "CityBlock SpawnPos")
            {
                validSpawnPositions.Add(spawn);
            }
        }

        if (validSpawnPositions.Count == 0)
        {
            Debug.LogWarning($"PlayerManager: No spawn positions found in city block {matchingCityBlock.name}.");
            return;
        }

        // Choose a random spawn position
        Transform randomSpawn = validSpawnPositions[Random.Range(0, validSpawnPositions.Count)];

        // Teleport the player to the random spawn position
        player.transform.position = randomSpawn.position;
        Debug.Log($"PlayerManager: Teleported {player.name} to {randomSpawn.name} in {matchingCityBlock.name}.");
    }
}
