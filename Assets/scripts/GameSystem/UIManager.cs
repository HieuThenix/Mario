using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Text References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI worldText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI livesText;

    private void OnEnable()
    {
        // Start listening to the GameManager
        GameManager.OnStatsChanged += UpdateUI;
    }

    private void OnDisable()
    {
        // Stop listening to prevent errors
        GameManager.OnStatsChanged -= UpdateUI;
    }

    private void Start()
    {
        // Initial update to set the starting values
        UpdateUI();
    }

    // This method ONLY runs when GameManager says something changed
    private void UpdateUI()
    {
        if (GameManager.instance == null) return;

        scoreText.text = GameManager.instance.Score.ToString("D6");
        coinsText.text = "x" + GameManager.instance.Coins.ToString("D2");
        worldText.text = GameManager.instance.World;
        timeText.text = GameManager.instance.TimeLeft.ToString("D3");
        livesText.text = GameManager.instance.Lives.ToString();
    }
}