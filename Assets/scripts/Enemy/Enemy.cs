using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Enemy Data")]
    public EnemyData enemyData; // Reference to our ScriptableObject

    private Rigidbody2D rb;
    private float direction = -1f; // -1 means starting by moving left

    void Start()
    {
        // Grab the Rigidbody2D component when the enemy spawns
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (enemyData != null)
        {
            // Keep the enemy's current Y velocity (for gravity/falling) but set the X velocity based on speed and direction
            rb.velocity = new Vector2(enemyData.moveSpeed * direction, rb.velocity.y);
        }
    }

    // This handles hitting SOLID objects like walls or pipes
    void OnCollisionEnter2D(Collision2D collision)
    {   

        // Ignore the player so the enemy doesn't bounce off Mario!
        if (collision.gameObject.CompareTag("Player")) 
        {
            return; 
        }
        
        // Get the first contact point
        ContactPoint2D contact = collision.GetContact(0);

        // Check the X value of the normal to see if we hit a vertical wall
        if (Mathf.Abs(contact.normal.x) > 0.5f)
        {
            // Reverse direction
            direction *= -1f;

            // Flip the enemy's sprite visually
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
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