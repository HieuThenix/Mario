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
    public float invincibilityDuration = 2f; // Seconds of invincibility after taking a hit
    public float flashInterval = 0.1f;       // How fast Mario flashes

    private Vector3 originalScale;
    private float originalJumpForce;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        originalScale = transform.localScale;
        originalJumpForce = controller.m_JumpForce;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed * Time.fixedDeltaTime;
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
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }

    void FixedUpdate()
    {
        controller.Move(horizontalMove, cround, jump);
        jump = false;
    }

    public void OnPoweredUpItemCollected(string itemName)
    {   
        itemName = itemName.ToLower();

        if (itemName == "mushroom")
        {
            transform.localScale = new Vector3(1.6f, 1.6f, 1f);
            controller.m_JumpForce = originalJumpForce * 1.2f;
            isBig = true;
        }

        else if (itemName == "fireball")
        {
            
        }
    }

    public void TakeDamage()
    {
        // Ignore damage entirely during invincibility window
        if (isInvincible) return;

        if (isBig)
        {
            // Shrink back to small Mario and trigger invincibility window
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

    // Grants invincibility and makes Mario flash for the duration
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        float elapsed = 0f;
        while (elapsed < invincibilityDuration)
        {
            // Toggle visibility to create a flashing effect
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        // Ensure Mario is visible and invincibility is off when done
        spriteRenderer.enabled = true;
        isInvincible = false;
    }

    public void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}