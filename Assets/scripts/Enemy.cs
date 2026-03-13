using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Fires ONCE when Mario first touches the enemy
    void OnTriggerEnter2D(Collider2D other)
    {
        HandleContact(other);
    }

    // Fires EVERY FRAME while Mario stays inside the enemy's collider.
    // This catches the case where invincibility expires while still touching.
    void OnTriggerStay2D(Collider2D other)
    {
        HandleContact(other);
    }

    void HandleContact(Collider2D other)
    {
        // OPTIMIZATION: TryGetComponent is faster and does the null check for us!
        if (other.CompareTag("Player") && other.TryGetComponent<PlayerController>(out PlayerController player))
        {
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
        // Called when enemy is killed (by stomp, fireball, or other means)
        Destroy(gameObject);
    }
}