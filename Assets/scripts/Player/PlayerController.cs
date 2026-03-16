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

    // --- NEW: Layer Settings for Invincibility ---
    [Header("Layer Names")]
    public string playerLayerName = "Player";
    public string enemyLayerName = "Enemy";

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
        else if (itemName == "star")
        {
            StartCoroutine(StarPowerCoroutine());
        }
    }

    private IEnumerator StarPowerCoroutine()
    {
        isStarPower = true;
        float elapsed = 0f;

        while (elapsed < starDuration)
        {
            float hue = Mathf.Repeat(Time.time * 5f, 1f); 
            spriteRenderer.color = Color.HSVToRGB(hue, 1f, 1f);
            
            elapsed += Time.deltaTime;
            yield return null; 
        }

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

        // --- NEW: Ignore collisions between Mario and Enemies ---
        int playerLayer = LayerMask.NameToLayer(playerLayerName);
        int enemyLayer = LayerMask.NameToLayer(enemyLayerName);
        
        if (playerLayer != -1 && enemyLayer != -1)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        }
        else
        {
            Debug.LogWarning("Player or Enemy layer not found! Please check your layer names.");
        }

        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        spriteRenderer.enabled = true;
        isInvincible = false;

        // --- NEW: Restore collisions when invincibility ends ---
        if (playerLayer != -1 && enemyLayer != -1)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        }
    }

    public void Die()
    {   
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pit"))
        {
            Die(); 
        }
    }
}