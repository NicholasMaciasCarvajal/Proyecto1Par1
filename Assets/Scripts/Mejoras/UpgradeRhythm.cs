using UnityEngine;

public class UpgradeRhythm : MonoBehaviour
{
    public int clicksRequired = 50;

    public int hitsNeeded = 5;
    public int maxFails = 3;

    int hits;
    int fails;
    bool challengeActive;

    public void TryStartChallenge()
    {
        if (!ClickCounter.Instance.SpendClicks(clicksRequired))
        {
            Debug.Log("No tienes suficientes clicks");
            return;
        }

        challengeActive = true;
        hits = 0;
        fails = 0;

        Debug.Log("Reto iniciado");
    }

    public void GoodHit()
    {
        if (!challengeActive) return;

        hits++;

        if (hits >= hitsNeeded)
        {
            Debug.Log("Upgrade obtenido");

            ModifierClick.Instance.IncreaseRandomModifier();

            challengeActive = false;
        }
    }

    public void BadHit()
    {
        if (!challengeActive) return;

        fails++;

        if (fails >= maxFails)
        {
            Debug.Log("Reto fallado");
            challengeActive = false;
        }
    }
}