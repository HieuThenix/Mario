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
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                bool isStomping = other.bounds.min.y > transform.position.y;

                if (isStomping)
                {
                    Die(); // Player stomped the enemy — kill it
                }
                else
                {
                    player.TakeDamage(); // Shrinks if big, dies if small (ignored if invincible)
                }
            }
        }
    }

    public void Die()
    {
        // Called when enemy is killed (by stomp, fireball, or other means)
        Destroy(gameObject);
    }
}