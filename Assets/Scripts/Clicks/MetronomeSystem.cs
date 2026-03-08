using UnityEngine;
using System;

public class MetronomeSystem : MonoBehaviour
{
    public float bpm = 120f;
    public float goodWindow = 0.2f;

    float beatInterval;
    float timer;

    public event Action OnGoodClick;
    public event Action OnBadClick;

    void Start()
    {
        beatInterval = 60f / bpm;

        ClickInputManager.Instance.OnClick += RegisterClick;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= beatInterval)
        {
            timer -= beatInterval;
        }
    }

    void RegisterClick()
    {
        float distanceToBeat = Mathf.Abs(timer - beatInterval / 2);

        if (distanceToBeat <= goodWindow)
        {
            OnGoodClick?.Invoke();
        }
        else
        {
            OnBadClick?.Invoke();
        }
    }
}