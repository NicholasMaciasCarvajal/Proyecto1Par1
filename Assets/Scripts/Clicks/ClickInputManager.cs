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
                    SceneManager.LoadScene("MainGameplay");
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            holding = false;

            if (!ClickCounter.Instance.HoldComplete() && !GameManager.Instance.IsPaused)
            {
                ClickCounter.Instance.AddClick(ModifierClick.Instance.GetClickValue());
                FindObjectOfType<MetronomeColliderGame>()?.RegisterClick();
                FindObjectOfType<MenuClickSequence>()?.RegisterClick();
                OnClick?.Invoke();
            }

            ClickCounter.Instance.StopHold();
        }
    }
}