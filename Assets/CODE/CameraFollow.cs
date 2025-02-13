using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public bool isPlayerDead = false;
    public Transform target;       // The player's transform (drag the player here in the Inspector)
    public Vector3 offset = new Vector3(0, 5, -10); // Default camera offset
    public float followSpeed = 5f; // Speed at which the camera follows
    public float rotationSpeed = 5f; // Speed at which the camera rotates
    public Vector3 deadCameraPosition = new Vector3(0, 20, 10); // Position to move the camera when the player is dead

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No target assigned. Please assign the player transform.");
            return;
        }

        // If the player is dead, move the camera to the dead position
        if (isPlayerDead)
        {
            MoveCameraToDeadPosition();
        }
        else
        {
            // Desired position for the camera when the player is alive
            Vector3 desiredPosition = target.position + offset;

            // Smoothly move the camera towards the desired position
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

            // Smoothly rotate the camera to look at the target
            Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void MoveCameraToDeadPosition()
    {
        // Lerp to the dead camera position
        transform.position = Vector3.Lerp(transform.position, deadCameraPosition, followSpeed * Time.deltaTime);
    }

    public void OnPlayerDeath()
    {
        if (target != null)
        {
            // Disable the MeshRenderer to make the player invisible
            MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false; // Turn off the mesh renderer
            }

            isPlayerDead = true; // Set the player dead status
            MoveCameraToDeadPosition(); // Move the camera to the dead position
        }
    }

    public void OnPlayerRespawn(Transform newTarget)
    {
        target = newTarget;
        target.gameObject.SetActive(true); // Enable the player

        // Enable the MeshRenderer to make the player visible again
        MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = true; // Turn on the mesh renderer
        }

        isPlayerDead = false; // Reset player dead status
        transform.position = target.position + offset; // Reset the camera position
    }
}