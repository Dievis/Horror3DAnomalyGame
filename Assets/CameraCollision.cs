using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float smoothSpeed = 10f;
    public float distance = 5f;
    public float collisionOffset = 0.3f; // To prevent the camera from clipping through walls

    private Vector3 velocity = Vector3.zero;

    private void Update()
    {
        // Raycast from the player to the camera
        Vector3 targetPosition = player.position - transform.forward * distance;

        RaycastHit hit;
        if (Physics.Raycast(player.position, -transform.forward, out hit, distance))
        {
            targetPosition = hit.point + transform.forward * collisionOffset; // Move the camera closer to the player
        }

        // Smoothly move the camera to the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed * Time.deltaTime);
    }
}
