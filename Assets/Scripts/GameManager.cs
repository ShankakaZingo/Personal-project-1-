using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Button StartButton;
    public Button restartButton;


    [Header("UI References")]
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI pickupsText;

    private bool isGameOver = false;
    private int pickupCount = 0;
    public static GameManager Instances;
   

    public void StartGame()
{
    Debug.Log("Start Game button pressed!");

    // Hide the Start Button (optional)
    if (StartButton != null)
        StartButton.gameObject.SetActive(false);

    // Start or load the main gameplay scene
    SceneManager.LoadScene("GameScene"); // ← Replace with your actual gameplay scene name
}


    

     public GameManagerUI uiManager;

    void Awake()
    {
        Instances = this;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        if (uiManager == null)
        {
            uiManager = GetComponent<GameManagerUI>();
        }
    }

    void Start()
    {
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
        if (restartButton != null)
        {

            restartButton.gameObject.SetActive(false);
            restartButton.onClick.AddListener(RestartGame);
        }
    

        UpdatePickupUI();
    }

    public void AddPickup()
    {
        pickupCount++;
        UpdatePickupUI();
        Debug.Log($"Pickups collected: {pickupCount}");
    }

    void UpdatePickupUI()
    {
        if (pickupsText != null)
        {
            pickupsText.text = $"Pickups: {pickupCount}";
        }
    }

    public void GameOver()

    {
        
         
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Game Over!");

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
        }
        restartButton.gameObject.SetActive(true); 
        Time.timeScale = 0f;

        

         
    }

    public void RestartGame()

    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void GameWon()
    {
        Debug.Log("You Won the Game!");
        if (uiManager !=null)
        {
            uiManager.ShowWinScreen();
        }
    }

    
}
