using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{   
    public CharacterController2D controller;
    float horizontalMove = 0f;
    public float runSpeed = 40f;
    bool jump = false;
    bool cround = false;
    public Animator animator;
    public bool isBig = false;
    public bool isFireShooting = false;
    public bool isInvincible = false;
    
    // --- NEW: Star Power Settings ---
    public bool isStarPower = false;
    public float starDuration = 5f;

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 2f; 
    public float flashInterval = 0.1f;       

    [Header("Fireball Settings")]
    public GameObject fireballPrefab; 
    public Transform firePoint;       

    private Vector3 originalScale;
    private float originalJumpForce;
    private SpriteRenderer spriteRenderer;

    private float fireballCooldown = 0.3f;
    private float lastFireTime = 0f;

    void Start()
    {
        originalScale = transform.localScale;
        originalJumpForce = controller.m_JumpForce;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButtonDown("Jump")){
            jump = true;
            animator.SetBool("IsJumping", true);
        }

        if (Input.GetButtonDown("Cround")){
            cround = true;
        } else if (Input.GetButtonUp("Cround")){
            cround = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && isFireShooting)
        {
            Shoot();
        }
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, cround, jump);
        jump = false;
    }

    public void OnPoweredUpItemCollected(string itemName)
    {   
        itemName = itemName.ToLower();

        float currentDirection = Mathf.Sign(transform.localScale.x);

        if (itemName == "mushroom")
        {
            transform.localScale = new Vector3(1.6f * currentDirection, 1.6f, 1f);
            controller.m_JumpForce = originalJumpForce * 1.2f;
            isBig = true;
        }
        else if (itemName == "flower") 
        {
            transform.localScale = new Vector3(1.6f * currentDirection, 1.6f, 1f); 
            controller.m_JumpForce = originalJumpForce * 1.2f;
            isBig = true;
            isFireShooting = true;
        }
        // --- NEW: Star Power Logic ---
        else if (itemName == "star")
        {
            StartCoroutine(StarPowerCoroutine());
        }
    }

    // --- NEW: Star Power Coroutine ---
    private IEnumerator StarPowerCoroutine()
    {
        isStarPower = true;
        float elapsed = 0f;

        while (elapsed < starDuration)
        {
            // Create a rainbow flash effect using HSV colors
            float hue = Mathf.Repeat(Time.time * 5f, 1f); // 5f is the color cycle speed
            spriteRenderer.color = Color.HSVToRGB(hue, 1f, 1f);
            
            elapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Reset back to normal after 5 seconds
        spriteRenderer.color = Color.white; 
        isStarPower = false;
    }

    void Shoot()
    {   
        if (firePoint == null)
        {
            Debug.LogError("Fire point not assigned!");
            return;
        }

        if (Time.time - lastFireTime < fireballCooldown) return;

        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
        Fireball fireballScript = fireball.GetComponent<Fireball>();
        
        float facingDirection = Mathf.Sign(transform.localScale.x);
        fireballScript.SetDirection(facingDirection);
        lastFireTime = Time.time;
    }

    public void TakeDamage()
    {
        // --- UPDATED: Prevent damage if Mario has Star Power OR standard Invincibility ---
        if (isInvincible || isStarPower) return;

        if (isBig)
        {
            transform.localScale = originalScale;
            controller.m_JumpForce = originalJumpForce;
            isBig = false;
            isFireShooting = false; 
            StartCoroutine(InvincibilityCoroutine());
        }
        else
        {
            Die();
        }
    }

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        spriteRenderer.enabled = true;
        isInvincible = false;
    }

    public void Die()
    {   
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object we collided with has the tag "Pit"
        if (other.CompareTag("Pit"))
        {
            Die(); // This calls your existing Die() method which resets the scene
        }
    }
}