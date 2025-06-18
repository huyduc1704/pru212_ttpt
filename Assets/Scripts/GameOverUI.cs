using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public Button retryButton;

    void Start()
    {
        // Lấy điểm số hiện tại và điểm cao nhất đã lưu
        int score = PlayerPrefs.GetInt("score", 0);
        int highScore = PlayerPrefs.GetInt("highScore", 0);

        // Cập nhật hiển thị điểm số
        scoreText.text = score.ToString();
        highScoreText.text = highScore.ToString();

        // Kiểm tra và cập nhật điểm cao nhất nếu điểm hiện tại lớn hơn
        if (score > highScore)
        {
            PlayerPrefs.SetInt("highScore", score);
            // Cập nhật lại highScoreText để hiển thị điểm cao mới ngay lập tức
            highScoreText.text = score.ToString();
        }

        // Thêm Listener cho nút Retry để tải lại Scene1 khi click
        retryButton.onClick.AddListener(() => {
            SceneManager.LoadScene("Scene1");
        });
    }
}