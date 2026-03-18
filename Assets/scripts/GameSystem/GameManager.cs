using UnityEngine;
using System; // Required for Action

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Actions that UI components can listen to
    public static event Action OnStatsChanged;

    [Header("Player Stats")]
    [SerializeField] private int _score = 0;
    [SerializeField] private int _coins = 0;
    [SerializeField] private string _world = "1-1";
    [SerializeField] private int _time = 500;
    [SerializeField] private int _lives = 8;

    // Public properties to access data
    public int Score => _score;
    public int Coins => _coins;
    public string World => _world;
    public int TimeLeft => _time;
    public int Lives => _lives;

    private float timeElapsed = 0f;

    void Awake()
    {
        if (instance == null) {
            instance = this;

        } else {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (_time > 0)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= 1f)
            {
                _time--;
                timeElapsed = 0f;
                // Shout that the time has changed
                OnStatsChanged?.Invoke();
            }
        }
    }

    public void AddScore(int amount) 
    {
        _score += amount;
        OnStatsChanged?.Invoke(); // Shout that the score changed
    }

    public void AddCoin() 
    {
        _coins++;
        if (_coins >= 100) {
            _lives++;
            _coins = 0;
        }
        OnStatsChanged?.Invoke(); // Shout that coins/lives changed
    }
}