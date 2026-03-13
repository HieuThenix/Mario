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

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 2f; 
    public float flashInterval = 0.1f;       

    [Header("Fireball Settings")]
    public GameObject fireballPrefab; // Drag your Fireball prefab here in the inspector
    public Transform firePoint;       // Create an empty GameObject child on Mario for the spawn point

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

        // --- NEW: Shooting Input ---
        // You can change "Fire1" to a specific KeyCode like Input.GetKeyDown(KeyCode.Z)
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
        // Apply Time.fixedDeltaTime here where physics are actually calculated!
        controller.Move(horizontalMove * Time.fixedDeltaTime, cround, jump);
        jump = false;
    }

    public void OnPoweredUpItemCollected(string itemName)
    {   
        itemName = itemName.ToLower();

        // Check which direction Mario is currently facing (1 for right, -1 for left)
        float currentDirection = Mathf.Sign(transform.localScale.x);

        if (itemName == "mushroom")
        {
            // Multiply the new scale by the current direction to preserve rotation!
            transform.localScale = new Vector3(1.6f * currentDirection, 1.6f, 1f);
            controller.m_JumpForce = originalJumpForce * 1.2f;
            isBig = true;
        }
        else if (itemName == "flower") 
        {
            // Multiply the new scale by the current direction to preserve rotation!
            transform.localScale = new Vector3(1.6f * currentDirection, 1.6f, 1f); 
            controller.m_JumpForce = originalJumpForce * 1.2f;
            isBig = true;
            isFireShooting = true;
        }
    }

    // --- NEW: Shoot Method ---
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
        
        // Determine the direction Mario is facing. 
        // Assuming your CharacterController2D flips the localScale.x when turning.
        float facingDirection = Mathf.Sign(transform.localScale.x);
        fireballScript.SetDirection(facingDirection);
        lastFireTime = Time.time;
    }

    public void TakeDamage()
    {
        if (isInvincible) return;

        if (isBig)
        {
            transform.localScale = originalScale;
            controller.m_JumpForce = originalJumpForce;
            isBig = false;
            isFireShooting = false; // Lose fire ability on damage
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
}