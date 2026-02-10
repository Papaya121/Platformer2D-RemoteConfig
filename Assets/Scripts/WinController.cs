using UnityEngine;

public class WinController : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;

    void Awake()
    {
        winScreen.SetActive(false);
    }

    void OnEnable()
    {
        GameController.OnWin += ShowWinScreen;
    }

    void OnDisable()
    {
        GameController.OnWin -= ShowWinScreen;
    }

    public void ShowWinScreen()
    {
        Time.timeScale = 0;
        AudioManager.PlaySound("Win");
        winScreen.SetActive(true);
    }
}