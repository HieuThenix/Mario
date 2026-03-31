using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxBackground : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The camera to follow. Auto-resolved to Camera.main if left empty.")]
    public Camera cam;

    [Header("Parallax Settings")]
    [Range(0f, 1f)]
    [Tooltip(
        "How strongly this layer follows the camera.\n" +
        "0.0 = layer does not move (pinned to screen)\n" +
        "0.5 = layer moves at half camera speed (mid-ground)\n" +
        "1.0 = layer moves with the world (no parallax effect)"
    )]
    public float parallaxFactor = 0.5f;

    // ── Private state ────────────────────────────────────────────────────────
    private float textureUnitSizeX;
    private float camPrevX;
    private float relativeDrift;

void Start()
{
    if (cam == null)
        cam = Camera.main;

    if (cam == null)
    {
        Debug.LogError(
            "[ParallaxBackground] No camera found. Assign one in the Inspector " +
            "or ensure your camera is tagged 'MainCamera'.", this);
        enabled = false;
        return;
    }

    SpriteRenderer sr = GetComponent<SpriteRenderer>();

    if (sr.sprite == null)
    {
        Debug.LogError(
            "[ParallaxBackground] Sprite has zero width. " +
            "Make sure a sprite is assigned to this SpriteRenderer.", this);
        enabled = false;
        return;
    }

    // ── Sprite width in world units ──────────────────────────────────────────
    // sr.bounds.size.x reads the renderer's bounding box AFTER it has been
    // placed in the scene, which can accumulate floating-point rounding and
    // sprite atlas padding — causing hairline gaps between copies.
    //
    // sr.sprite.bounds.size.x reads the width straight from the sprite asset
    // (in local units), then we multiply by lossyScale.x to convert to world
    // space. This is exact and immune to scene-placement rounding.
    textureUnitSizeX = sr.sprite.bounds.size.x * transform.lossyScale.x;

    if (textureUnitSizeX <= 0f)
    {
        Debug.LogError("[ParallaxBackground] Computed sprite width is zero.", this);
        enabled = false;
        return;
    }

    SpawnCopy(-textureUnitSizeX*1.5f, sr); // left neighbour
    SpawnCopy( textureUnitSizeX*1.5f, sr); // right neighbour

    camPrevX      = cam.transform.position.x;
    relativeDrift = 0f;
}
    // Creates one additional SpriteRenderer copy, offset by localOffsetX,
    // parented to this transform so it travels with us automatically.
    private void SpawnCopy(float localOffsetX, SpriteRenderer source)
    {
        GameObject copy = new GameObject(gameObject.name + "_copy");

        // Parent under this GameObject.
        // false = keep the local offset we are about to set, not world space.
        copy.transform.SetParent(transform, false);
        copy.transform.localPosition = new Vector3(localOffsetX, 0f, 0f);

        SpriteRenderer copyRenderer = copy.AddComponent<SpriteRenderer>();

        // Mirror the source renderer's visual settings so the copies look
        // identical to the original: same sprite, layer, order, and color.
        copyRenderer.sprite         = source.sprite;
        copyRenderer.sortingLayerID = source.sortingLayerID;
        copyRenderer.sortingOrder   = source.sortingOrder;
        copyRenderer.color          = source.color;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // ── Step 1: Camera delta ─────────────────────────────────────────────
        float camDeltaX = cam.transform.position.x - camPrevX;

        // ── TELEPORT SAFETY CHECK ────────────────────────────────────────────
        // If the camera moves an impossible distance in a single frame, it is 
        // a teleport (like a scene reload or initial camera snap). 
        // We reset the baseline and skip moving the background this frame.
        if (Mathf.Abs(camDeltaX) > 10f) 
        {
            camPrevX = cam.transform.position.x;
            return; 
        }
        
        camPrevX = cam.transform.position.x;

        // ── Step 2: Move the layer ───────────────────────────────────────────
        // Moving this transform also moves the two child copies simultaneously.
        transform.position += new Vector3(camDeltaX * parallaxFactor, 0f, 0f);

        // ── Step 3: Accumulate relative drift ───────────────────────────────
        relativeDrift += camDeltaX * (1f - parallaxFactor);

        // ── Step 4: Seamless looping ─────────────────────────────────────────
        if (parallaxFactor > 0f && parallaxFactor < 1f)
        {
            if (relativeDrift >= textureUnitSizeX)
            {
                transform.position += new Vector3(textureUnitSizeX, 0f, 0f);
                relativeDrift       -= textureUnitSizeX;
            }
            if (relativeDrift <= -textureUnitSizeX)
            {
                transform.position -= new Vector3(textureUnitSizeX, 0f, 0f);
                relativeDrift       += textureUnitSizeX;
            }
        }
    }

    // ── Optional: public reset for camera teleports ──────────────────────────
    public void ResetToCamera()
    {
        if (cam == null) return;
        camPrevX      = cam.transform.position.x;
        relativeDrift = 0f;
    }
}