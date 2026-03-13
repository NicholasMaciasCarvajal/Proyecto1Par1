using System.Numerics;
using UnityEngine;

public class ModifierClick : MonoBehaviour
{
    public static ModifierClick Instance;

    [Header("Base")]
    public double baseClickValue = 1;

    [Header("Levels")]
    public int sumLevel;
    public int multiplierLevel;
    public int percentLevel;

    [Header("Scaling Per Level")]
    public double sumPerLevel = 1;
    public double multiplierPerLevel = 2;
    public double percentPerLevel = 0.5; // 50%

    public BigInteger upgradeCost = 3;
    public string upgradeCostS;

    void Awake()
    {
        Instance = this;
    }

    public BigInteger GetClickValue()
    {
        double value = baseClickValue;

        value += sumLevel * sumPerLevel;

        double multiplier = Mathf.Pow((float)multiplierPerLevel, multiplierLevel);
        value *= multiplier;

        double percent = percentLevel * percentPerLevel;
        value *= (1 + percent);

        // --- LÓGICA DE REDONDEO PERSONALIZADO ---
        // 1. Obtenemos solo la parte decimal (ej. de 5.75, obtenemos 0.75)
        double fractionalPart = value - System.Math.Truncate(value);

        // 2. Evaluamos si es 0.7 o mayor
        if (fractionalPart >= 0.7)
        {
            value = System.Math.Ceiling(value); // Redondea al siguiente entero (ej. 5.7 -> 6)
        }
        else
        {
            value = System.Math.Floor(value);   // Trunca los decimales (ej. 5.6 -> 5)
        }
        // ----------------------------------------

        Debug.Log("ClickValue: " + value);

        return new BigInteger(value);
    }

    public void IncreaseRandomModifier()
    {
        if (ClickCounter.Instance.totalClicks < upgradeCost)
            return;

        int roll = Random.Range(0, 3);

        switch (roll)
        {
            case 0:
                sumLevel++;
                Debug.Log("Upgrade SUMA nivel: " + sumLevel);
                IncreaseUpgradeCost();
                break;

            case 1:
                multiplierLevel++;
                Debug.Log("Upgrade MULTIPLICADOR nivel: " + multiplierLevel);
                IncreaseUpgradeCost();
                break;

            case 2:
                percentLevel++;
                Debug.Log("Upgrade PORCENTAJE nivel: " + percentLevel);
                IncreaseUpgradeCost();
                break;
        }
    }

    public void IncreaseUpgradeCost()
    {
        ClickCounter.Instance.totalClicks = ClickCounter.Instance.totalClicks - upgradeCost;
        ClickCounter.Instance.ActualizarClicks();
        upgradeCost = new BigInteger((double)upgradeCost * 2.15);
        upgradeCostS = upgradeCost.ToString();
        ClickCounter.Instance.ActualizarCosto();

    }
}