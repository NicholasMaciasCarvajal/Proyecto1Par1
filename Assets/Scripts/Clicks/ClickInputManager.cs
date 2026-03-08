using UnityEngine;
using System;

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
            ClickCounter.Instance.AddHoldTime(Time.unscaledDeltaTime);

            if (ClickCounter.Instance.HoldComplete())
            {
                OnHoldComplete?.Invoke();
                ClickCounter.Instance.ResetHold();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            holding = false;

            if (!ClickCounter.Instance.HoldComplete() && !GameManager.Instance.IsPaused)
            {
                ClickCounter.Instance.AddClick(ModifierClick.Instance.GetClickValue());

                OnClick?.Invoke();
            }

            ClickCounter.Instance.StopHold();
        }
    }
}