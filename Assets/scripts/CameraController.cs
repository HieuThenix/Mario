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
        // 1. If target is missing (e.g., changing scenes), find the new one
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) 
            {
                target = playerObj.transform;
                
                // Snap instantly to the new player to avoid a long camera pan from the old level coordinates
                Vector3 snapPosition = target.position + offset;

                if (useBounds)
                {
                    snapPosition.x = Mathf.Clamp(snapPosition.x, minBounds.x, maxBounds.x);
                    snapPosition.y = Mathf.Clamp(snapPosition.y, minBounds.y, maxBounds.y);
                }

                transform.position = snapPosition;
            }
            return; // Wait until next frame to resume smooth following
        }

        // 2. Normal Camera Following Behavior (THIS WAS THE MISSING PART!)
        Vector3 targetPosition = target.position + offset;

        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        }

        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPosition, 
            ref velocity, 
            smoothTime
        );
    }
}