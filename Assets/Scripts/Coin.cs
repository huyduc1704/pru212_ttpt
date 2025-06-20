using UnityEngine;

public class Coin : MonoBehaviour
{
    public int pointValue = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (PointManager.Instance != null)
            {
                PointManager.Instance.AddPoint(pointValue);
            }

            Destroy(gameObject); // Xóa coin
        }
    }
}
