using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Data")]
    public EnemyData enemyData; // Reference to our new ScriptableObject

    void OnTriggerEnter2D(Collider2D other)
    {
        HandleContact(other); //
    }

    void OnTriggerStay2D(Collider2D other)
    {
        HandleContact(other); //
    }

    void HandleContact(Collider2D other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent<PlayerController>(out PlayerController player)) //
        {
            // --- Check for Star Power First ---
            if (player.isStarPower) //
            {
                Die(); //
                return; // Stop further checks, the enemy is dead!
            }

            // Original stomp detection
            bool isStomping = other.bounds.min.y > transform.position.y; //

            if (isStomping) //
            {
                Die(); //
            }
            else
            {
                player.TakeDamage(); //
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

        Destroy(gameObject); //
    }
}