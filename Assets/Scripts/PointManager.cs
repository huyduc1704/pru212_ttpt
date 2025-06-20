using UnityEngine;

public class PointManager : MonoBehaviour
{
    public static PointManager Instance;

    public int currentScore = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void AddPoint(int amount)
    {
        currentScore += amount;
        Debug.Log("Điểm hiện tại: " + currentScore);
    }

    public void ResetScore()
    {
        currentScore = 0;
    }
}
