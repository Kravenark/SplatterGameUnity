using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;            // Movement speed of the player
    public float rotationSpeed = 720f;  // Speed at which the player rotates (degrees per second)

    private string playerName;

    private void Start()
    {
        // Store the name of the player for control assignment
        playerName = gameObject.name;
    }

    private void Update()
    {
        // Handle movement based on the player's name
        if (playerName == "Player1")
        {
            MoveWithWASD();
        }
        else if (playerName == "Player2")
        {
            MoveWithIJKL();
        }
    }

    private void MoveWithWASD()
    {
        // Movement for Player 1 using WASD
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D keys
        float vertical = Input.GetAxisRaw("Vertical");     // W/S keys

        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        // Rotate the player to face the movement direction
        RotatePlayer(new Vector3(horizontal, 0f, vertical));
    }

    private void MoveWithIJKL()
    {
        // Movement for Player 2 using IJKL
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.I)) vertical = 1f;   // Move forward
        if (Input.GetKey(KeyCode.K)) vertical = -1f;  // Move backward
        if (Input.GetKey(KeyCode.J)) horizontal = -1f; // Move left
        if (Input.GetKey(KeyCode.L)) horizontal = 1f;  // Move right

        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized * speed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        // Rotate the player to face the movement direction
        RotatePlayer(new Vector3(horizontal, 0f, vertical));
    }

    private void RotatePlayer(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            // Calculate the rotation towards the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

            // Smoothly rotate towards the target direction
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
