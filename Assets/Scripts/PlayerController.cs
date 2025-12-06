using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // REQUIRED for the new system

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Horizontal move speed")] [SerializeField] private float speed = 6f;
    [Tooltip("Jump impulse")] [SerializeField] private float jumpForce = 7f;

    [Header("Ground Check")]
    [SerializeField] private Vector3 groundCheckOffset = new Vector3(0f, -0.6f, 0f);
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private bool isOpponentPlayer=true;

    private Rigidbody2D rb2d;

    private float horizontalInput;
    private bool jumpRequested;
    private bool isJumpingFallback;
    
    [Header("Pickup")]
    [Tooltip("Tags considered as pickups (GiftPile, item, etc.)")]
    [SerializeField] private string[] pickupTags = new string[] { "GiftPile"};

    private bool hasGift=false;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        
        // 1. Reset input
        horizontalInput = 0f;
        
        // 2. Check Keyboard if we are player
        if (!isOpponentPlayer){
            if (Keyboard.current != null)
            {
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                {
                    horizontalInput = -1f;
                }
                else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                {
                    horizontalInput = 1f;
                }

                if (Keyboard.current.spaceKey.wasPressedThisFrame && IsGrounded())
                {
                    jumpRequested = true;
                }
            }}
        else{
            //TODO for inputs coming from OpponentPlayer
        }
        
        // (Optional) Add Gamepad support here if needed using Gamepad.current
    }

    private void FixedUpdate()
    {
        // Horizontal movement
        if (rb2d != null)
        {
            rb2d.linearVelocity = new Vector2(horizontalInput * speed, rb2d.linearVelocity.y);

            if (jumpRequested)
            {
                rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpRequested = false;
            }
        }
        else
        {
            // Transform-based fallback movement
            var move = new Vector3(horizontalInput * speed * Time.fixedDeltaTime, 0f, 0f);
            transform.position += move;

            if (jumpRequested && !isJumpingFallback && IsGrounded())
            {
                StartCoroutine(FallbackJump());
                jumpRequested = false;
            }
        }
    }

    private IEnumerator FallbackJump()
    {
        isJumpingFallback = true;
        float peakTime = 0.25f;
        float elapsed = 0f;

        while (elapsed < peakTime)
        {
            transform.position += Vector3.up * (jumpForce * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        while (!IsGrounded())
        {
            transform.position += Vector3.down * (jumpForce * Time.deltaTime);
            yield return null;
        }

        isJumpingFallback = false;
    }

    private bool IsGrounded()
    {
        if (rb2d != null)
        {
            Vector2 checkPos = (Vector2)transform.position + (Vector2)groundCheckOffset;
            // Check for ANY collider below, not just specific layer
            Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPos, groundCheckRadius);
            // Exclude self collider
            foreach (Collider2D col in colliders)
            {
                if (col.gameObject != gameObject)
                {
                    return true;
                }
            }
            return false;
        }

        Vector2 originFallback = (Vector2)transform.position + (Vector2)groundCheckOffset;
        // Raycast downward with no layer mask - hits anything
        RaycastHit2D hit = Physics2D.Raycast(originFallback, Vector2.down, groundCheckRadius + 0.1f);
        return hit.collider != null && hit.collider.gameObject != gameObject;
    }

    private void OnDrawGizmosSelected() //Debuglarken yardım etsin diye
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + groundCheckOffset, groundCheckRadius);
    }

    //Decide if pick up according to its tag
    private bool IsPickup(GameObject other)
    {
        if (pickupTags == null || pickupTags.Length == 0) return false;
        foreach (string t in pickupTags)
        {
            if (string.IsNullOrEmpty(t)) continue;
            if (other.CompareTag(t)) return true;
        }
        return false;
    }

    //Handle all pick ups
    private void HandlePickup(GameObject pickup)
    {
        if (pickup.CompareTag("GiftPile")){
        this.hasGift=true;

        
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsPickup(other.gameObject)) HandlePickup(other.gameObject);
    }
}