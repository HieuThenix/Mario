using UnityEngine;

public class Enemy : MonoBehaviour
{
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
            // --- NEW: Check for Star Power First ---
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
        Destroy(gameObject);
    }
}