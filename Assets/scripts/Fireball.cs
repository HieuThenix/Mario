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
        rb.velocity = new Vector2(speed * direction, -bounceForce);
    }

    void FixedUpdate()
    {
        // Keep the horizontal speed constant, but allow Unity's gravity to handle the Y velocity
        rb.velocity = new Vector2(speed * direction, rb.velocity.y);
    }

    // 1. Handles hitting TRIGGERS (like your Enemies)
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.Die();
            }
            Destroy(gameObject);
        }
    }

    // 2. Handles hitting SOLID objects (like Floors and Walls)
    void OnCollisionEnter2D(Collision2D collision)
    {
        // OPTIMIZATION: GetContact(0) gets the first contact point without allocating a new array in memory!
        ContactPoint2D contact = collision.GetContact(0);

        // Check the X value of the normal to see if we hit a vertical wall
        if (Mathf.Abs(contact.normal.x) > 0.5f)
        {
            // We hit a wall! Vaporize.
            Destroy(gameObject);
        }
        // Check the Y value to see if we hit a floor from above
        else if (contact.normal.y > 0.5f)
        {
            // We hit the floor! Apply the bounce force.
            rb.velocity = new Vector2(rb.velocity.x, bounceForce);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}