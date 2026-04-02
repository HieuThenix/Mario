using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{   
    public CharacterController2D controller;
    float horizontalMove = 0f;
    public float runSpeed = 40f;
    bool cround = false;

    [Header("Jump Buffer")]
    public float jumpBufferWindow = 0.15f;  
    private float lastJumpPressTime = -1f;

    [Header("Power-up Settings")]
    public float bigScale = 1.6f;
    public float bigJumpMultiplier = 1.2f;
        
    public Animator animator;
    public bool isBig = false;
    public bool isFireShooting = false;
    public bool isInvincible = false;
    
    public bool isStarPower = false;
    public float starDuration = 5f;

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 2f; 
    public float flashInterval = 0.1f;       

    [Header("Fireball Settings")]
    public GameObject fireballPrefab; 
    public Transform firePoint;       


    [Header("Color Settings")] // NEW: Added color management
    //[SerializeField] private Color fireColor = new Color(1f, 0.45f, 0f); // orange-fire default
    public Color fireColor = Color.yellow;
    private Color originalColor;

    [Header("Layer Names")]
    public string playerLayerName = "Player";
    public string enemyLayerName = "Enemy";

    [Header("Mobile Controls")]
    public MobileTouchButton leftButton;
    public MobileTouchButton rightButton;
    public MobileTouchButton jumpButton;
    public MobileTouchButton crouchButton;
    public MobileTouchButton fireButton;

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

        originalColor = spriteRenderer.color;

        // NEW: Automatically grab UI buttons from the Persistent Canvas
        if (MobileInputHub.Instance != null)
        {
            leftButton = MobileInputHub.Instance.leftBtn;
            rightButton = MobileInputHub.Instance.rightBtn;
            jumpButton = MobileInputHub.Instance.jumpBtn;
            crouchButton = MobileInputHub.Instance.crouchBtn;
            fireButton = MobileInputHub.Instance.fireBtn;

            // Subscribe to events now that we have the references
            if (jumpButton != null) jumpButton.OnButtonDown += HandleJump;
            if (crouchButton != null)
            {
                crouchButton.OnButtonDown += HandleCrouchDown;
                crouchButton.OnButtonUp += HandleCrouchUp;
            }
            if (fireButton != null) fireButton.OnButtonDown += HandleShoot;
        }

        if (GameManager.instance != null)
        {
            isBig = GameManager.instance.savedIsBig;
            isFireShooting = GameManager.instance.savedIsFireShooting;

            // Apply physical changes if Mario was saved as Big
            if (isBig)
            {
                float currentDirection = Mathf.Sign(transform.localScale.x);
                transform.localScale = new Vector3(bigScale * currentDirection, bigScale, 1f);
                controller.m_JumpForce = originalJumpForce * bigJumpMultiplier;
            }

            if (isFireShooting)
            {
                spriteRenderer.color = fireColor;
            }
        }
    }

    private void OnDestroy()
    {
        // Prevent memory leaks when the player is destroyed
        if (jumpButton != null) jumpButton.OnButtonDown -= HandleJump;
        if (crouchButton != null)
        {
            crouchButton.OnButtonDown -= HandleCrouchDown;
            crouchButton.OnButtonUp -= HandleCrouchUp;
        }
        if (fireButton != null) fireButton.OnButtonDown -= HandleShoot;
    }

    void Update()
    {
        // 1. Calculate Horizontal Movement (Combine PC and Mobile)
        float moveInput = Input.GetAxisRaw("Horizontal"); // Default PC input

        // Track mobile button states
        bool leftPressed = leftButton != null && leftButton.IsPressed;
        bool rightPressed = rightButton != null && rightButton.IsPressed;

        // Override with mobile button states if either is being pressed
        if (leftPressed || rightPressed)
        {
            moveInput = 0f; // Reset PC input to prioritize mobile
            
            if (leftPressed) moveInput -= 1f;
            if (rightPressed) moveInput += 1f;
            
            // If BOTH are pressed, moveInput becomes (-1 + 1) = 0, so Mario stops!
        }

        horizontalMove = moveInput * runSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        // 2. PC Input Fallbacks (So you can still test on your keyboard)
        if (Input.GetButtonDown("Jump")) HandleJump();
        
        if (Input.GetButtonDown("Cround")) HandleCrouchDown();
        else if (Input.GetButtonUp("Cround")) HandleCrouchUp();

        if (Input.GetKeyDown(KeyCode.LeftShift)) HandleShoot();
    }

    // Extracted Action Methods ---
    private void HandleJump()
    {
        lastJumpPressTime = Time.time;
        animator.SetBool("IsJumping", true);
    }

    private void HandleCrouchDown()
    {
        cround = true;
    }

    private void HandleCrouchUp()
    {
        cround = false;
    }

    private void HandleShoot()
    {
        if (isFireShooting)
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
        bool jump = (Time.time - lastJumpPressTime) <= jumpBufferWindow;
        controller.Move(horizontalMove * Time.fixedDeltaTime, cround, jump);

        if (jump) lastJumpPressTime = -1f;
    }

    public void OnPoweredUpItemCollected(string itemName)
    {   
        float currentDirection = Mathf.Sign(transform.localScale.x);

        if (string.Equals(itemName, "mushroom", System.StringComparison.OrdinalIgnoreCase))
        {
            transform.localScale = new Vector3(bigScale * currentDirection, bigScale, 1f);
            controller.m_JumpForce = originalJumpForce * bigJumpMultiplier;
            isBig = true;
            GameManager.instance.savedIsBig = isBig;
        }
        else if (string.Equals(itemName, "flower", System.StringComparison.OrdinalIgnoreCase)) 
        {
            isFireShooting = true;
            spriteRenderer.color = fireColor;

            GameManager.instance.savedIsFireShooting = isFireShooting;
        }
        else if (string.Equals(itemName, "star", System.StringComparison.OrdinalIgnoreCase))
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

        spriteRenderer.color = isFireShooting ? fireColor : originalColor; 
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
        if (fireballScript == null) { Destroy(fireball); return; };

        float facingDirection = Mathf.Sign(transform.localScale.x);
        fireballScript.SetDirection(facingDirection);
        lastFireTime = Time.time;
    }

    public void TakeDamage()
    {
        if (isInvincible || isStarPower) return;

        if (isBig)
        {
            float currentDirection = Mathf.Sign(transform.localScale.x);
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x) * currentDirection, originalScale.y, originalScale.z);
            
            controller.m_JumpForce = originalJumpForce;
            isBig = false;
            isFireShooting = false; 
            spriteRenderer.color = originalColor;
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

        int playerLayer = LayerMask.NameToLayer(playerLayerName);
        int enemyLayer = LayerMask.NameToLayer(enemyLayerName);
        
        if (playerLayer != -1 && enemyLayer != -1)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        }

        WaitForSeconds flashWait = new WaitForSeconds(flashInterval);

        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return flashWait;
            elapsed += flashInterval;
        }

        spriteRenderer.enabled = true;
        isInvincible = false;

        if (playerLayer != -1 && enemyLayer != -1)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        }
    }

    public void Die()
    {   
        // Reset power-ups on death so we don't respawn big!
        if (GameManager.instance != null)
        {
            GameManager.instance.savedIsBig = false;
            GameManager.instance.savedIsFireShooting = false;
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pit")) Die();

    }
}