using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerHealthController playerHealthController;
    
    public static event Action OnGameOver;
    public static event Action OnWin;

    private void Awake()
    {
        if (playerHealthController != null)
        {
            playerHealthController.OnPlayerDied += HandlePlayerDeath;
        }

        Time.timeScale = 1;
    }

    private void OnDestroy()
    {
        if (playerHealthController != null)
        {
            playerHealthController.OnPlayerDied -= HandlePlayerDeath;
        }
    }

    public static void TriggerWin()
    {
        OnWin?.Invoke();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void HandlePlayerDeath()
    {
        OnGameOver?.Invoke();
    }
}