using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour {
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float maxJumpCharge = 1f;
    [SerializeField] private float maxJumpForce = 15f;

    [Header("Visual Feedback")]
    [SerializeField] private Color minChargeColor = Color.white;
    [SerializeField] private Color maxChargeColor = Color.red;

    [Header("Ground Check")]
    [SerializeField] private Collider2D groundCheckCollider;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded = false;
    private float jumpCharge = 0f;
    private bool isChargingJump = false;

    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private SpriteRenderer spriteRenderer;
    private bool movementEnabled = true;

    public void DisableMovement()
    {
        movementEnabled = false;
        rb.linearVelocity = Vector2.zero;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
    }

    void Update()
    {
        Move();
        Jump();
        UpdateColor();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (groundCheckCollider != null && other == groundCheckCollider) return;
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (groundCheckCollider != null && other == groundCheckCollider) return;
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;
        }
    }

    private void Move()
    {
        if (!movementEnabled) return;
        float moveInput = moveAction != null ? moveAction.ReadValue<Vector2>().x : 0f;
        if (isGrounded && !isChargingJump)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
        if (TimeController.Instance != null && !TimeController.Instance.timerStarted && Mathf.Abs(moveInput) > 0.01f)
        {
            TimeController.Instance.StartTimer();
        }
    }

    private void Jump()
    {
        if (!movementEnabled) return;
        float moveInput = moveAction != null ? moveAction.ReadValue<Vector2>().x : 0f;
        bool jumpPressed = jumpAction != null && jumpAction.ReadValue<float>() > 0;
        bool jumpReleased = jumpAction != null && jumpAction.WasReleasedThisFrame();
        if (TimeController.Instance != null && !TimeController.Instance.timerStarted && jumpPressed)
        {
            TimeController.Instance.StartTimer();
        }
        if (jumpPressed && isGrounded)
        {
            isChargingJump = true;
            jumpCharge += Time.deltaTime;
            jumpCharge = Mathf.Min(jumpCharge, maxJumpCharge);
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
        else if (isChargingJump && jumpReleased && isGrounded)
        {
            float chargePercent = jumpCharge / maxJumpCharge;
            float appliedJumpForce = Mathf.Lerp(jumpForce, maxJumpForce, chargePercent);
            Vector2 jumpDirection = new Vector2(moveInput, 1f).normalized;
            rb.AddForce(jumpDirection * appliedJumpForce, ForceMode2D.Impulse);
            jumpCharge = 0f;
            isChargingJump = false;
        }
        else if (!jumpPressed)
        {
            isChargingJump = false;
            jumpCharge = 0f;
        }
    }

    private void UpdateColor()
    {
        if (spriteRenderer != null)
        {
            float chargePercent = Mathf.Clamp01(jumpCharge / maxJumpCharge);
            spriteRenderer.color = Color.Lerp(minChargeColor, maxChargeColor, chargePercent);
            if (!isChargingJump)
            {
                spriteRenderer.color = minChargeColor;
            }
        }
    }
}
