using UnityEngine;

public class PauseController : MonoBehaviour
{
    bool waitingForHold;

    void Start()
    {
        ClickInputManager.Instance.OnClick += FirstClick;
        ClickInputManager.Instance.OnHoldComplete += TogglePause;
    }

    void FirstClick()
    {
        if (GameManager.Instance.IsPaused)
            return;

        waitingForHold = true;
    }

    void TogglePause()
    {
        if (!waitingForHold && !GameManager.Instance.IsPaused)
            return;

        GameManager.Instance.TogglePause();

        waitingForHold = false;
    }
}