using UnityEngine;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;

    void Awake()
    {
        gameOverScreen.SetActive(false);
    }

    void OnEnable()
    {
        GameController.OnGameOver += ShowGameOverScreen;
    }

    void OnDisable()
    {
        GameController.OnGameOver -= ShowGameOverScreen;
    }

    public void ShowGameOverScreen()
    {
        Time.timeScale = 0;
        AudioManager.PlaySound("GameOver");
        gameOverScreen.SetActive(true);
    }
}