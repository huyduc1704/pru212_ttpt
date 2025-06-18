using UnityEngine;

public class MushroomBounce : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float bounceForce = 15f; // Lực bật lên
    public float squashAmount = 0.8f; // Độ nén nấm
    public float squashDuration = 0.1f; // Thời gian hiệu ứng nhún

    private Vector3 originalScale;
    private bool isSquashing = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra nếu nhân vật rơi xuống từ phía trên
        if (collision.relativeVelocity.y <= 0f)
        {
            Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Bật nhân vật lên
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);

                // Gọi hiệu ứng nhún
                if (!isSquashing)
                {
                    StartCoroutine(SquashEffect());
                }
            }
        }
    }

    System.Collections.IEnumerator SquashEffect()
    {
        isSquashing = true;
        // Scale xuống trục Y (nhún)
        transform.localScale = new Vector3(originalScale.x, originalScale.y * squashAmount, originalScale.z);
        yield return new WaitForSeconds(squashDuration);
        // Trở lại bình thường
        transform.localScale = originalScale;
        isSquashing = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}