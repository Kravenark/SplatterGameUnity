using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{

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

private Dictionary<GameObject, string> playerColors = new Dictionary<GameObject, string>();

public void AssignRandomColors(GameObject[] players)
{
    List<string> availableColors = new List<string> { "PlayerRed", "PlayerGreen", "PlayerBlue" };

    foreach (GameObject player in players)
    {
        if (player == null)
        {
            Debug.LogWarning("PlayerManager: Player is null, cannot assign color.");
            continue;
        }

        if (availableColors.Count == 0)
        {
            Debug.LogError("PlayerManager: Not enough colors for all players!");
            return;
        }

        // Assign a random color
        int randomIndex = Random.Range(0, availableColors.Count);
        string chosenColor = availableColors[randomIndex];
        availableColors.RemoveAt(randomIndex);

        // Assign the tag to the player
        player.tag = chosenColor;

        // Store the color in the dictionary
        if (!playerColors.ContainsKey(player))
        {
            playerColors.Add(player, chosenColor);
        }
        else
        {
            playerColors[player] = chosenColor; // Update if already exists
        }

        // Confirm the assignment
        Debug.Log($"PlayerManager: Assigned {chosenColor} to {player.name}");
    }

    // Debug to ensure the dictionary has the assigned colors
    foreach (var entry in playerColors)
    {
        Debug.Log($"DEBUG: {entry.Key.name} is assigned {entry.Value}");
    }
}


    // Coroutine to teleport all players to their respective spawn positions after a delay
// PlayerManager.cs
private IEnumerator TeleportAllPlayersToSpawnWithDelay(GameObject[] players, CityBlockManager cityBlockManager)
{
    yield return new WaitForSeconds(1f);  // Delay to allow city blocks to initialize

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

    // Check if the player has an assigned color
    if (!playerColors.TryGetValue(player, out string playerTag))
    {
        Debug.LogWarning($"PlayerManager: Player {player.name} does not have an assigned color in dictionary.");
        return;
    }

    Debug.Log($"Teleporting {player.name} with color {playerTag}");

    // Proceed with teleportation based on the assigned tag...
}

}
