using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsPaused { get; private set; }

    [Header("UI Panels")]
    public GameObject gameplayPanel;
    public GameObject pausePanel;
    public GameObject creditsPanel;
    public GameObject mainMenuPanel;

    public bool isInCredits = false;
    //public bool isPaused = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Estado inicial
        SetGameplayUI();
    }

    public void TogglePause()
    {
        if (IsPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        IsPaused = true;
        //Time.timeScale = 0;

        SetPauseUI();

        Debug.Log("Juego en pausa");
    }

    public void ResumeGame()
    {
        IsPaused = false;
        //Time.timeScale = 1;

        SetGameplayUI();

        Debug.Log("Juego reanudado");
    }

    void SetPauseUI()
    {
        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    void SetGameplayUI()
    {
        if (gameplayPanel != null)
            gameplayPanel.SetActive(true);

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void OpenCloseCredits()
    {
        if (creditsPanel.activeSelf)
        {
            ClickCounter.Instance.ResetHold();
            creditsPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
            isInCredits = false;
        }
        else
        {
            creditsPanel.SetActive(true);
            mainMenuPanel.SetActive(false);
            isInCredits = true;
        }
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}