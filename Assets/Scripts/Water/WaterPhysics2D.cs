using UnityEngine;

public class WaterPhysics2D : MonoBehaviour
{
    [Header("Water Physics")]
    [SerializeField] private float buoyancyForce = 15f;
    [SerializeField] private float waterGravityScale = 0.3f;
    [SerializeField] private float waterDamping = 4f;
    [SerializeField] private float swimUpForce = 8f;

    private Rigidbody2D rb;
    private float originalGravityScale;
    private float originalDamping;
    private bool isInWater = false;
    private int waterContactCount = 0;

    public bool IsInWater => isInWater;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravityScale = rb.gravityScale;
        originalDamping = rb.linearDamping;
    }

    void FixedUpdate()
    {
        if (isInWater)
        {
            ApplyBuoyancy();
        }
    }

    private void ApplyBuoyancy()
    {
        float adjustedBuoyancy = buoyancyForce * rb.mass;
        rb.AddForce(Vector2.up * adjustedBuoyancy, ForceMode2D.Force);

        if (rb.linearVelocity.y < -1f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -1f);

        if (rb.linearVelocity.y < -0.5f)
            rb.AddForce(Vector2.up * adjustedBuoyancy * 0.5f, ForceMode2D.Force);
    }

    public void SwimUp()
    {
        if (isInWater)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, swimUpForce);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            waterContactCount++;
            if (!isInWater)
            {
                isInWater = true;
                EnterWater();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            waterContactCount--;
            if (waterContactCount <= 0)
            {
                waterContactCount = 0;
                isInWater = false;
                ExitWater();
            }
        }
    }

    private void EnterWater()
    {
        rb.gravityScale = waterGravityScale;
        rb.linearDamping = waterDamping;
    }

    private void ExitWater()
    {
        rb.gravityScale = originalGravityScale;
        rb.linearDamping = originalDamping;
    }
}