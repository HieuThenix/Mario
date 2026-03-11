using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 15f;
    public float bounceForce = 5f;
    public Rigidbody2D rb;

    private float direction = 1f;

    public void SetDirection(float dir)
    {
        direction = dir;
        // Give the fireball an initial downward arc and forward speed
        rb.velocity = new Vector2(speed * direction, -bounceForce);
    }

    void Update()
    {
        // Force the fireball to always move left or right at a constant speed, 
        // regardless of physics friction slowing it down.
        rb.velocity = new Vector2(speed * direction, rb.velocity.y);
    }


    // This detects when the fireball enters ANY trigger collider (including enemies)
    // but Enemy uses Is Trigger = true, so we need OnTriggerEnter2D instead
    void OnTriggerEnter2D(Collider2D collision)
    {
        // If it hits an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Get and call the Enemy's Die method
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Die();
            }
            
            // Destroy the fireball
            Destroy(gameObject);
        }
        // Optional: Handle wall collisions if they are solid (not triggers)
        // Note: If walls are triggers, this will also trigger here
        else if (collision.gameObject.CompareTag("Wall")) 
        {
            // Destroy fireball if it hits a solid wall
            Destroy(gameObject);
        }
    }

    // Destroy the fireball if it goes off-screen
    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}