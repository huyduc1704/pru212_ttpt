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

            // Cảnh báo
            warningUI.SetActive(true);
            Debug.Log("⚠️ Gió sắp thổi!");
            yield return new WaitForSeconds(warningTime);
            warningUI.SetActive(false);

            // Gió bắt đầu
            windActive = true;
            snowEffect.SetActive(true);
            Debug.Log("💨 Gió đang thổi!");

            yield return new WaitForSeconds(windDuration);

            // Gió dừng
            windActive = false;
            snowEffect.SetActive(false);
            Debug.Log("✅ Gió đã ngừng.");
        }
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
