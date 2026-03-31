using UnityEngine;

// RequireComponent ensures we don't accidentally forget to add a SpriteRenderer
[RequireComponent(typeof(SpriteRenderer))]
public class Chest : MonoBehaviour
{
    // 1. We use an enum to explicitly define the chest's behavior in memory-efficient way.
    public enum ChestContentType
    {
        Coin,
        PowerUp
    }

    [Header("Chest Configuration")]
    [Tooltip("Determine what this specific chest instance will spawn.")]
    public ChestContentType contentType = ChestContentType.Coin;

    [Header("Visuals")]
    [Tooltip("The sprite to display after the block has been hit.")]
    public Sprite emptyBlockSprite;
    private SpriteRenderer spriteRenderer;

    [Header("Prefabs")]
    public GameObject coinPrefab;
    public GameObject mushroomPrefab;
    public GameObject flowerPrefab;
    public GameObject starPrefab;

    [Header("State")]
    private bool isEmpty = false; 

    void Awake()
    {
        // Grab the SpriteRenderer component when the game starts
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object hitting the block is the Player
        if (collision.gameObject.CompareTag("Player"))
        {
            if (isEmpty) return;

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
        // Update the state immediately
        isEmpty = true;

        // Change the visual to the empty brown block
        if (emptyBlockSprite != null)
        {
            spriteRenderer.sprite = emptyBlockSprite;
        }

        // 2. Deductive routing based on the designer's selected content type
        if (contentType == ChestContentType.Coin)
        {
            SpawnCoin();
        }
        else if (contentType == ChestContentType.PowerUp)
        {
            EvaluateAndSpawnItem();
        }
        
        // TODO: Coroutine for the "bounce" animation goes here
    }

    private void SpawnCoin()
    {
        if (coinPrefab != null)
        {
            // Spawn the item 1 unit above the chest. 
            // Using Vector3.up prevents allocating a new Vector3 struct in memory.
            Instantiate(coinPrefab, transform.position + Vector3.up, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("[Chest] Coin prefab is not assigned in the Inspector!");
        }
    }

    private void EvaluateAndSpawnItem()
    {
        GameObject itemToSpawn = null;

        // Deductive State Evaluation
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
            Instantiate(itemToSpawn, transform.position + Vector3.up, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("[Chest] Valid power-up prefab not assigned for current player state.");
        }
    }
}