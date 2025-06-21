using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public Button retryButton;
    public Button menuButton;

    public GameObject bronzeMedal;
    public GameObject silverMedal;
    public GameObject goldMedal;

    public int bronzeThreshold = 25;
    public int silverThreshold = 50;
    public int goldThreshold = 100;

    void Start()
    {
        HideAllMedals();

        int score = PointManager.Instance.currentScore;
        scoreText.text = score.ToString();

        // highScore vẫn có thể dùng PlayerPrefs
        int highScore = PlayerPrefs.GetInt("highScore", 0);
        if (score > highScore)
        {
            PlayerPrefs.SetInt("highScore", score);
            highScoreText.text = score.ToString();
        }
        else
        {
            highScoreText.text = highScore.ToString();
        }
        DisplayMedal(score);

        retryButton.onClick.AddListener(() => {
            PointManager.Instance.ResetScore();
            SceneManager.LoadScene("Scene1");
        });

        // Nếu có nút Menu (trong WinScene)
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(() => {
                PointManager.Instance.ResetScore();
                SceneManager.LoadScene("MainMenu");
            });
        }
    }

    void HideAllMedals()
    {
        if (bronzeMedal != null) bronzeMedal.SetActive(false);
        if (silverMedal != null) silverMedal.SetActive(false);
        if (goldMedal != null) goldMedal.SetActive(false);
    }

    void DisplayMedal(int score)
    {
        if (score >= goldThreshold && goldMedal != null)
        {
            goldMedal.SetActive(true);
        }
        else if (score >= silverThreshold && silverMedal != null)
        {
            silverMedal.SetActive(true);
        }
        else if (score >= bronzeThreshold && bronzeMedal != null)
        {
            bronzeMedal.SetActive(true);
        }
    }
}
