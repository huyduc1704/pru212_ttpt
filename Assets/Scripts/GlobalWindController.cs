using System.Collections;
using UnityEngine;

public class GlobalWindController : MonoBehaviour
{
    public float windForce = 10f;
    public float windDuration = 5f;
    public float windInterval = 15f;
    public float warningTime = 3f;

    public GameObject warningUI;
    public GameObject snowEffect;

    private bool windActive = false;

    void Start()
    {
        StartCoroutine(WindRoutine());
    }

    IEnumerator WindRoutine()
    {
        while (true)
        {
            // Chờ đến khi cần cảnh báo
            yield return new WaitForSeconds(windInterval - warningTime);

            // Bắt đầu nhấp nháy cảnh báo
            Debug.Log("⚠ Gió sắp thổi!");
            StartCoroutine(BlinkWarningUI(warningTime, 0.3f)); // nhấp nháy trong warningTime mỗi 0.3s

            yield return new WaitForSeconds(warningTime);

            // Gió bắt đầu
            windActive = true;
            snowEffect.SetActive(true);
            Debug.Log("💨 Gió đang thổi!");

            yield return new WaitForSeconds(windDuration);

            // Gió dừng
            windActive = false;

            yield return new WaitForSeconds(1f);
            snowEffect.SetActive(false);
            Debug.Log("✅ Gió đã ngừng.");
        }
    }

    IEnumerator BlinkWarningUI(float duration, float blinkInterval)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            warningUI.SetActive(!warningUI.activeSelf);
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }
        warningUI.SetActive(false); // Tắt UI sau khi nhấp nháy xong
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!windActive) return;
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.WakeUp();
                rb.linearVelocity = new Vector2(-0.1f, rb.linearVelocity.y);
                rb.AddForce(Vector2.left * windForce, ForceMode2D.Force);
            }
        }
    }
}
