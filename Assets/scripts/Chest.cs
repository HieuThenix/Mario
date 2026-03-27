using UnityEngine;

// RequireComponent ensures we don't accidentally forget to add a SpriteRenderer
[RequireComponent(typeof(SpriteRenderer))]
public class Chest : MonoBehaviour
{
    [Header("Visuals")]
    [Tooltip("The sprite to display after the block has been hit.")]
    public Sprite emptyBlockSprite;
    private SpriteRenderer spriteRenderer;

    [Header("Power-Up Prefabs")]
    public GameObject mushroomPrefab;
    public GameObject flowerPrefab;
    public GameObject starPrefab;

    [Header("State")]
    // We keep this private because other scripts shouldn't be able to magically empty the block
    private bool isEmpty = false; 


    void Awake()
    {
        // Grab the SpriteRenderer component when the game starts
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Check if the object hitting the block is the Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // 2. If the block is already empty, ignore the hit
            if (isEmpty) return;

            // 3. Check the direction of the hit
            // collision.contacts[0].normal gives us the direction of the collision force
            Vector2 hitDirection = collision.GetContact(0).normal;

            // If the hit direction's Y value is positive, the hit came from below
            if (hitDirection.y > 0.5f)
            {
                HandleBottomHit();
            }
        }
    }

    void HandleBottomHit()
    {
        Debug.Log("Mario hit the block from below!");
        
        // Update the state
        isEmpty = true;

        // Change the visual to the empty brown block
        if (emptyBlockSprite != null)
        {
            spriteRenderer.sprite = emptyBlockSprite;
        }

        EvaluateAndSpawnItem();
        // TODO: We will add the Coroutine for the "bounce" animation here later
        // TODO: We will add the logic to spawn the coin/mushroom here later
    }

    private void EvaluateAndSpawnItem()
    {
        GameObject itemToSpawn = null;

        // Deductive State Evaluation
        // We cascade through the states to determine the optimal upgrade
        if (!GameManager.instance.savedIsBig)
        {
            itemToSpawn = mushroomPrefab;
        }
        else if (!GameManager.instance.savedIsFireShooting)
        {
            itemToSpawn = flowerPrefab;
        }
        else
        {
            // Player is big AND has fire shooting capability
            itemToSpawn = starPrefab;
        }

        if (itemToSpawn != null)
        {
            // Spawn the item 1 unit above the chest. 
            // Using Vector3.up prevents allocating a new Vector3 struct in memory.
            Instantiate(itemToSpawn, transform.position + Vector3.up, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning($"[Chest] Valid prefab not assigned for current player state.");
        }
    }
}