using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class GameManager : MonoBehaviour
{
    public int score = 0;
    public float gameTime = 120f; // adjustable in Inspector
    private float currentTime;



    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    private bool gameActive = true;

    void Start()
    {
        currentTime = gameTime;
        UpdateUI();
    }

    void Update()
    {
        if (gameActive)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0f)
            {
                currentTime = 0f;
                gameActive = false;
                EndGame();
            }
            UpdateUI();
        }
    }

    public void AddScore(int amount)
    {
        if (!gameActive) return;
        score += amount;
        scoreText.text = "Score: " + score;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText) scoreText.text = "Score: " + score;
        if (timerText) timerText.text = "Time: " + Mathf.CeilToInt(currentTime);
    }

    void EndGame()
    {
        Debug.Log("Game Over! Final Score: " + score);
        // You can add logic to disable player movement, show restart UI, etc.
    }
}