using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private float direction = 0f;

    [Header("Ground Check")]
    public LayerMask groundLayerMask;
    public Transform groundCheck;
    public float groundCheckRadius = 0.02f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool hasJumped;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
        direction = Input.GetAxis("Horizontal");

        if (direction > 0f)
        {
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        }
        else if (direction < 0f)
        {
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        float moveX = Input.GetAxis("Horizontal");
        if (moveX == 0) return;

        float scaleX = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(
            moveX > 0 ? scaleX : -scaleX,
            transform.localScale.y,
            transform.localScale.z
        );
    }
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}