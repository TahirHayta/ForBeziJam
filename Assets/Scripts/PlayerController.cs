using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // REQUIRED for the new system
using AI;

namespace PlayerController {

[System.Serializable]
public struct BotCommand
{
    public float move;
    public bool jump;
    public bool down;
}

public interface IBotBrain
{
    BotCommand GetNextCommand(PlayerController self);
}
public class PlayerController : MonoBehaviour
{
    private Animator anim;
    [Header("Movement")]
    [Tooltip("Horizontal move speed")] [SerializeField] private float speed = 6f;
    [Tooltip("Jump impulse")] [SerializeField] private float jumpForce = 9f;

    [Header("Ground Check")]
    [SerializeField] private Vector3 groundCheckOffset = new Vector3(0f, -0.6f, 0f);
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private bool isOpponentPlayer=true;

    [SerializeField] private MonoBehaviour botBrainComponent;
    LayerMask stairsMask;

    public teamEnum this_player_team;
    

    [Header("Player Settings")]
    [SerializeField] private int playerNumber=0; // 0 WASD 1 Arrow Keysw
    [SerializeField] private bool isBotPlayer=false;

    private Rigidbody2D rb2d;
    private Collider2D playerCollider;

    private float horizontalInput;
    private bool jumpRequested;
    private bool downRequested;
    private bool isJumpingFallback;
    
    [Header("Pickup")]
    [Tooltip("Tags considered as pickups (GiftPile, item, etc.)")]
    [SerializeField] private string[] pickupTags = new string[] { "GiftPile"};

    private bool hasGift=false;
    private GameObject gift=null;
    public bool HasGift => hasGift;

    private void Start()
    {
        playerCollider = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        if (isBotPlayer && botBrainComponent == null)
        {
            botBrainComponent = GetComponent<AI.OpponentBehaviour>();
        }
        this.stairsMask= LayerMask.GetMask("Stairs");
    }

    private void Update()
    {
        
        // 1. Reset input
        horizontalInput = 0f;
        
        // 2. Check Keyboard if we are player
        switch (playerNumber)
        {
            case 0: // WASD
                if (Keyboard.current != null)
                {
                    if (Keyboard.current.aKey.isPressed) horizontalInput -= 1f;
                    if (Keyboard.current.dKey.isPressed) horizontalInput += 1f;
                    if (Keyboard.current.wKey.wasPressedThisFrame && IsGrounded()) jumpRequested = true;
                    if (Keyboard.current.sKey.isPressed) downRequested = true;

                }
                break;
            case 1: // Arrow Keys
                if (Keyboard.current != null)
                {
                    if (Keyboard.current.leftArrowKey.isPressed) horizontalInput -= 1f;
                    if (Keyboard.current.rightArrowKey.isPressed) horizontalInput += 1f;
                    if (Keyboard.current.upArrowKey.wasPressedThisFrame && IsGrounded()) jumpRequested = true;
                    if (Keyboard.current.downArrowKey.isPressed) downRequested = true;
                }
                break;
            default:
                HandleBotInput();
                break;
        }
        if (anim != null)
            {
                anim.SetFloat("IsRunning", Mathf.Abs(horizontalInput));
                if (horizontalInput != 0) transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);

                if (IsGrounded() && (rb2d == null || rb2d.linearVelocity.y <= 0.1f)) anim.SetBool("IsJumping", false);
            
            }
        // (Optional) Add Gamepad support here if needed using Gamepad.current
    }

    private void FixedUpdate()
    {
        if (rb2d != null)
        {
            rb2d.linearVelocity = new Vector2(horizontalInput * speed, rb2d.linearVelocity.y);
            if (jumpRequested)
            {
                rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                if (anim != null) anim.SetBool("IsJumping", true);
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
        
        if (downRequested)
        {
            TryDropThrough();
            downRequested = false;   
        }

    }

    private void HandleBotInput()
    {
        if(botBrainComponent is IBotBrain botBrain)
        {
            BotCommand cmd = botBrain.GetNextCommand(this);
            horizontalInput = Mathf.Clamp(cmd.move, -1f, 1f);
            if (cmd.jump && IsGrounded())
            {
                jumpRequested = true;
            }
            if (cmd.down)
            {
                downRequested = true;
            }
        }
    }

    private IEnumerator FallbackJump()
    {
        isJumpingFallback = true;
        if (anim != null) anim.SetBool("IsJumping", true);
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

    public bool IsGrounded()
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
    
    void TryDropThrough()
    {
        // Only drop if standing on a platform
        if (!IsGrounded()) return;
        float checkDistance =0.1f;
        // Find everything right under the player
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            playerCollider.bounds.center + Vector3.down * (playerCollider.bounds.extents.y + checkDistance),
            new Vector2(playerCollider.bounds.size.x * 0.9f, checkDistance * 2f),
            0f,
            stairsMask
            
        );


        if (hits.Length == 0) {Debug.Log("No hits"); return;}

        // Ignore collisions with all detected platforms for a short time
        foreach (var platform in hits)
        {
            StartCoroutine(TemporarilyIgnorePlatform(platform));
        }
    }
    IEnumerator TemporarilyIgnorePlatform(Collider2D platform) // When falling downwards
    {
        float ignoreDuration = 0.3f;
        Physics2D.IgnoreCollision(playerCollider, platform, true);
        yield return new WaitForSeconds(ignoreDuration);
        if (platform) Physics2D.IgnoreCollision(playerCollider, platform, false);
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
        if (pickup.CompareTag("GiftPile")&&pickup.GetComponent<GiftPile>().team==this_player_team){
            this.hasGift=true;
            if (anim != null) anim.SetTrigger("PickGift");
            //Create a new gift gameobject
            this.gift = Instantiate(pickup.GetComponent<GiftPile>().giftPrefab);
            this.gift.SetActive(false);
        
        }
    }

    private void HandleNPCCollisionWithPlayer(GameObject otherNPC)
    {
        
        if (!this.hasGift) return;
        if (otherNPC.GetComponentInParent<NPC.NPCBehaviour>().team==this_player_team) return;
        if (anim != null) anim.SetTrigger("Attack");

        otherNPC.GetComponentInParent<NPCGift.NPCGift>().HandleGiftInteraction(this.gift,this_player_team);
        this.hasGift=false;
        this.gift=null;
    }




    private void OnTriggerEnter2D(Collider2D other)
    {
            if (isBotPlayer)
            {
                print("Bot collided with: " + other.gameObject.name);
            }
        if (IsPickup(other.gameObject)) HandlePickup(other.gameObject);
        else if (other.gameObject.CompareTag("NPC")) HandleNPCCollisionWithPlayer(other.gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Stairs")){
            foreach (ContactPoint2D contact in collision.contacts)
    {
                // If the collision normal points UP, player is ABOVE the stairs
                if (contact.normal.y < 0.5f)
                {
                    StartCoroutine(TemporarilyIgnorePlatform(collision.collider));
                    rb2d.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
                    break;
                }
    }        
    }
    }
}
}
public enum teamEnum
    {
        Red,
        Blue,
        Green,
        Yellow,

        Nix
    }