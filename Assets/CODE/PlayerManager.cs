using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    private Dictionary<GameObject, string> playerColors = new Dictionary<GameObject, string>(); // Track player colors
    private List<string> availableColors = new List<string> { "PlayerRed", "PlayerGreen", "PlayerBlue" };

    void Start()
    {
        PlayerManager playerManager = FindObjectOfType<PlayerManager>();
        CityBlockManager cityBlockManager = FindObjectOfType<CityBlockManager>();

        // Example players
        GameObject[] players = {
            GameObject.Find("Player1"),
            GameObject.Find("Player2"),
            GameObject.Find("Player3")
        };

        // Assign colors and start the coroutine for teleporting players
        playerManager.AssignRandomColors(players);
        StartCoroutine(playerManager.TeleportAllPlayersToSpawnWithDelay(players, cityBlockManager));
    }

    // Assign a unique random color to each player
    public void AssignRandomColors(GameObject[] players)
    {
        availableColors = new List<string> { "PlayerRed", "PlayerGreen", "PlayerBlue" };

        foreach (GameObject player in players)
        {
            if (player == null)
            {
                Debug.LogWarning("PlayerManager: Player is null, cannot assign random color.");
                continue;
            }

            string chosenColor = GetRandomAvailableColor();
            if (chosenColor != null)
            {
                playerColors[player] = chosenColor;
                UpdatePlayerColor(player, chosenColor);
                Debug.Log($"PlayerManager: Assigned {chosenColor} to player {player.name}.");
            }
        }
    }

    private string GetRandomAvailableColor()
    {
        if (availableColors.Count == 0) return null;

        int randomIndex = Random.Range(0, availableColors.Count);
        string color = availableColors[randomIndex];
        availableColors.RemoveAt(randomIndex);
        return color;
    }

    private void UpdatePlayerColor(GameObject player, string colorTag)
    {
        if (player == null)
        {
            Debug.LogWarning("PlayerManager: Player is null, cannot update color.");
            return;
        }

        player.tag = colorTag;

        PlayerTextureChange textureChange = player.GetComponent<PlayerTextureChange>();
        if (textureChange != null)
        {
            textureChange.UpdatePlayerTexture();
        }
        else
        {
            Debug.LogWarning($"PlayerManager: Player {player.name} does not have a PlayerTextureChange component.");
        }
    }

    // Coroutine to teleport all players to their respective spawn positions after a delay
    public IEnumerator TeleportAllPlayersToSpawnWithDelay(GameObject[] players, CityBlockManager cityBlockManager)
    {
        yield return new WaitForSeconds(0.1f); // Wait for 1 second

        foreach (GameObject player in players)
        {
            TeleportPlayerToRandomSpawn(player, cityBlockManager);

            if (player.gameObject.tag == "PlayerRed")
            {
                player.transform.position = GameObject.FindGameObjectWithTag("CityBlockRed").transform.position;
            }
            if (player.gameObject.tag == "PlayerGreen")
            {
                player.transform.position = GameObject.FindGameObjectWithTag("CityBlockGreen").transform.position;
            }
            if (player.gameObject.tag == "PlayerBlue")
            {
                player.transform.position = GameObject.FindGameObjectWithTag("CityBlockBlue").transform.position;
            }
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

        // Get the player's color from the dictionary
        if (!playerColors.TryGetValue(player, out string playerTag))
        {
            Debug.LogWarning($"PlayerManager: Player {player.name} does not have an assigned color.");
            return;
        }

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
