using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class ClickInputManager : MonoBehaviour
{
    public static ClickInputManager Instance;

    public event Action OnClick;
    public event Action OnHoldComplete;

    bool holding;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            holding = true;
            ClickCounter.Instance.StartHold();
        }

        if (Input.GetMouseButton(0))
        {
            if (!GameManager.Instance.isInCredits) {
                ClickCounter.Instance.AddHoldTime(Time.unscaledDeltaTime);
            }

            if (ClickCounter.Instance.HoldComplete())
            {
                OnHoldComplete?.Invoke();
                ClickCounter.Instance.ResetHold();
                if (SceneManager.GetActiveScene().name == "MainMenu")
                {
                    GameManager.Instance.SceneChange();
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            holding = false;

            if (!ClickCounter.Instance.HoldComplete())
            {
                if (!GameManager.Instance.IsPaused)
                {
                    ClickCounter.Instance.AddClick(ModifierClick.Instance.GetClickValue());
                    FindFirstObjectByType<MetronomeColliderGame>()?.RegisterClick();
                }
                FindFirstObjectByType<MenuClickSequence>()?.RegisterClick();
                FindFirstObjectByType<PauseClickSequence>()?.RegisterClick();
                OnClick?.Invoke();
            }

            ClickCounter.Instance.StopHold();
        }
    }
}