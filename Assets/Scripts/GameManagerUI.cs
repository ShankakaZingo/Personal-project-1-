using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerUI : MonoBehaviour
{
    public GameObject winPanel;
    public Button winRestartButton;
    
    [Header("Start Game UI")]
    public GameObject startGamePanel;
    public Button startGameButton;

    void Start()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
        
        if (winRestartButton != null)
        {
            winRestartButton.onClick.AddListener(RestartGame);
        }
        
        if (startGamePanel != null)
        {
            startGamePanel.SetActive(true);
            Time.timeScale = 0f;
        }
        
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(StartGame);
        }
    }
    
    public void StartGame()
    {
        Debug.Log("Game Started!");
        if (startGamePanel != null)
        {
            startGamePanel.SetActive(false);
        }
        Time.timeScale = 1f;
    }
    
    public void ShowWinScreen()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    
    void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
