using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private WaterPhysics2D waterPhysics;

    void Start()
    {
        waterPhysics = GetComponent<WaterPhysics2D>();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            if (waterPhysics != null && waterPhysics.IsInWater)
            {
                waterPhysics.SwimUp();
            }
            else if (IsGrounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            }
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f && (waterPhysics == null || !waterPhysics.IsInWater))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        Flip();
    }

    private void FixedUpdate()
    {
        float currentSpeed = (waterPhysics != null && waterPhysics.IsInWater) ? 5f : speed;
        rb.linearVelocity = new Vector2(horizontal * currentSpeed, rb.linearVelocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}