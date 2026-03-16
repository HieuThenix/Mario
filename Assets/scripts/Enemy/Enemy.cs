using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Enemy Data")]
    public EnemyData enemyData; // Reference to our ScriptableObject

    private Rigidbody2D rb;
    private float direction = -1f; // -1 means starting by moving left
    private float lastFlipTime = 0f;
    private const float flipCooldown = 0.2f; // seconds before allowing another flip
    
    void Start()
    {
        // Grab the Rigidbody2D component when the enemy spawns
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (enemyData != null)
        {
            Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y);
            Vector2 rayDirection = new Vector2(direction, 0f);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, enemyData.wallDetectionDistance, enemyData.groundLayer);
            Debug.DrawRay(rayOrigin, rayDirection * enemyData.wallDetectionDistance, Color.red);

            // Only flip if enough time has passed since the last flip
            if (hit.collider != null && Time.time - lastFlipTime > flipCooldown)
            {
                Flip();
                lastFlipTime = Time.time;
            }

            rb.velocity = new Vector2(enemyData.moveSpeed * direction, rb.velocity.y);
        }
    }

    // Extracted your flipping logic into its own reusable method
    private void Flip()
    {
        direction *= -1f; // Reverse direction

        // Flip the sprite visually
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // This handles hitting SOLID objects (Removed the old wall-bounce logic since Raycasts handle it now)
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Ignore the player so the enemy doesn't bounce off Mario!
        if (collision.gameObject.CompareTag("Player")) 
        {
            return; 
        }
        
        // You can leave other specific collision logic here if needed in the future
    }

    // --- Existing logic for hitting the player ---
    void OnTriggerEnter2D(Collider2D other)
    {
        HandleContact(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        HandleContact(other);
    }

    void HandleContact(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            // --- Check for Star Power First ---
            if (player.isStarPower)
            {
                Die();
                return; // Stop further checks, the enemy is dead!
            }

            // Original stomp detection
            bool isStomping = other.bounds.min.y > transform.position.y;

            if (isStomping)
            {
                Die();
            }
            else
            {
                player.TakeDamage();
            }
        }
    }

    public void Die()
    {
        // Reward the player with the score defined in the ScriptableObject!
        if (enemyData != null && GameManager.instance != null)
        {
            GameManager.instance.AddScore(enemyData.scoreValue);
        }
        else if (enemyData == null)
        {
            Debug.LogWarning($"EnemyData is missing on {gameObject.name}!");
        }

        Destroy(gameObject);
    }
}