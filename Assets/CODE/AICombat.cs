using UnityEngine;

public class AICombat : MonoBehaviour
{
    public void HandlePlayerHit(GameObject target)
    {
        // Check if the target is a player
        PlayerCombat playerCombat = target.GetComponent<PlayerCombat>();
        if (playerCombat != null)
        {
            // Call the TakeDamage method on the player
           // playerCombat.TakeDamage();ss
            Debug.Log($"{target.name} was hit by AI and is now respawning.");
        }
        else
        {
            Debug.LogWarning($"AICombat: {target.name} does not have a PlayerCombat component.");
        }
    }

    public void HandleAIRespawn(GameObject aiPlayer, Vector3 initialPosition)
    {
        Debug.Log($"AICombat: Respawning {aiPlayer.name} at {initialPosition}");

        // Reset AI player to the initial position
        aiPlayer.transform.position = initialPosition;

        // Reactivate the AI player (if previously deactivated)
        aiPlayer.SetActive(true);
    }
}
