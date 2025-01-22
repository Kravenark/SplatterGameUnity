using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // The player's transform (drag the player here in the Inspector)
    public Vector3 offset = new Vector3(0, 5, -10); // Default camera offset
    public float followSpeed = 5f; // Speed at which the camera follows
    public float rotationSpeed = 5f; // Speed at which the camera rotates

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No target assigned. Please assign the player transform.");
            return;
        }

        // Desired position for the camera
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera towards the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Smoothly rotate the camera to look at the target
        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
    }
}
