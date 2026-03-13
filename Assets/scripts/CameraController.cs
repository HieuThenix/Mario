using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // Drag your Player GameObject here
    public Vector3 offset = new Vector3(0f, 2f, -10f); // -10 is standard for 2D camera Z-axis

    [Header("Smoothing")]
    [Range(0f, 1f)]
    public float smoothTime = 0.25f; // Approximately the time it will take to reach the target

    private Vector3 velocity = Vector3.zero; // Used internally by SmoothDamp

    [Header("Level Bounds (Optional)")]
    public bool useBounds = false;
    public Vector2 minBounds; // E.g., Bottom-Left limit
    public Vector2 maxBounds; // E.g., Top-Right limit

    // LateUpdate is CRITICAL for cameras. 
    // It runs after all Update() and FixedUpdate() functions have finished.
    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // 1. Calculate the ideal target position based on the player's position + offset
        Vector3 targetPosition = target.position + offset;

        // 2. Clamp the target position if bounds are enabled
        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        }

        // 3. Smoothly interpolate the camera's current position towards the target position
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPosition, 
            ref velocity, 
            smoothTime
        );
    }
}