using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool hasJumped; // Prevent double jumps

    [Header("Ground Check")]
    public LayerMask groundLayerMask;
    public Transform groundCheck;
    public float groundCheckRadius = 0.02f; // Reduced radius

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.3f, 0); // Adjusted position
            groundCheck = groundCheckObj.transform;
        }
    }
    
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);

        if (isGrounded)
        {
            hasJumped = false;
        }
        
        float moveX = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !hasJumped)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            hasJumped = true;
        }
        
        float scaleX = Mathf.Abs(transform.localScale.x);
        if (moveX > 0)
            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        else if (moveX < 0)
            transform.localScale = new Vector3(-scaleX, transform.localScale.y, transform.localScale.z);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trap") || other.CompareTag("DeathZone"))
        {
            Debug.Log("Trap or DeathZone triggered!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
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