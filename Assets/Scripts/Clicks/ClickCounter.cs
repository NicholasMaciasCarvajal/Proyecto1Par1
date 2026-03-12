using DG.Tweening;
using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class ClickCounter : MonoBehaviour
{
    public static ClickCounter Instance;

    [Header("UI Reference")]
    private Vector2 posOriginal; // Para guardar el anclaje inicial
    public TMPro.TextMeshProUGUI clickText;
    public TMPro.TextMeshProUGUI upgradeCost;
    public GameObject winPanel;

    [Header("Clicks")]
    [SerializeField]
    private string initialClicks = "0"; // editable en inspector

    public BigInteger totalClicks;

    [Header("Hold System")]
    public float holdProgress;
    public float holdRequired = 3f;
    public float holdDecaySpeed = 1f;

    bool holding;

    void Awake()
    {
        Instance = this;

        // Convertimos el string a BigInteger
        BigInteger.TryParse(initialClicks, out totalClicks);

        ActualizarClicks();
    }

    void Start()
    {
        // Guardamos la posición exacta donde pusiste el texto en el Editor
        posOriginal = clickText.rectTransform.anchoredPosition;
    }

    void Update()
    {
        if (!holding && holdProgress > 0)
        {
            holdProgress -= holdDecaySpeed * Time.deltaTime;
            holdProgress = Mathf.Max(0, holdProgress);
        }

        if(totalClicks >= 10000000)
        {

        }

    }

    public void AddClick(BigInteger amount)
    {
        totalClicks += amount;

        float factorLog = (float)Math.Log10((double)amount + 1);
        float intensidad = 20f;
        float fuerzaFinal = Mathf.Min(5f + (factorLog * intensidad), 100f);

        float escalaPunch = Mathf.Clamp(factorLog * 0.05f, 0.05f, 0.3f);

        float direccionAleatoria = UnityEngine.Random.Range(0.5f, 1.5f) * (UnityEngine.Random.value > 0.5f ? 1f : -1f);
        float rotacionBase = Mathf.Clamp(factorLog * 2f, 5f, 15f);
        float rotacionFinal = rotacionBase * direccionAleatoria;

        clickText.rectTransform.DOKill(true);
        clickText.rectTransform.anchoredPosition = posOriginal;
        clickText.rectTransform.localScale = Vector3.one;
        clickText.rectTransform.localRotation = Quaternion.identity;


        clickText.rectTransform.DOShakeAnchorPos(0.15f, fuerzaFinal, 30, 90, false, true);

        clickText.rectTransform.DOPunchScale(
            new Vector3(escalaPunch, escalaPunch, 0),
            0.1f, 10, 1
        ).OnComplete(() => clickText.rectTransform.localScale = Vector3.one);

        clickText.rectTransform.DOPunchRotation(
            new Vector3(0, 0, rotacionFinal),
            0.15f,
            15,
            1
        ).OnComplete(() => clickText.rectTransform.localRotation = Quaternion.identity);

        ActualizarClicks();
    }

    public bool SpendClicks(BigInteger amount)
    {
        if (totalClicks < amount)
            return false;

        totalClicks -= amount;
        ActualizarClicks();
        return true;
    }

    public void StartHold()
    {
        holding = true;
    }

    public void StopHold()
    {
        holding = false;
    }

    public void AddHoldTime(float time)
    {
        holdProgress += time;
        holdProgress = Mathf.Min(holdProgress, holdRequired);
    }

    public bool HoldComplete()
    {
        return holdProgress >= holdRequired;
    }

    public void ResetHold()
    {
        holdProgress = 0;
    }

    public void ActualizarClicks()
    {
        if (totalClicks < 1000000)
        {
            clickText.text = "Clicks: " + totalClicks.ToString("N0");
        }
        else
        {
            double valorDouble = (double)totalClicks;

            int exponente = (int)Math.Floor(Math.Log10(valorDouble));

            double mantisa = valorDouble / Math.Pow(10, exponente);

            clickText.text = $"Clicks: {mantisa:F2} × 10<sup>{exponente}</sup>";
        }
    }

    public void ActualizarCosto()
    {
        if (ModifierClick.Instance.upgradeCost < 1000000)
        {
            upgradeCost.text = "Costo: " + ModifierClick.Instance.upgradeCost.ToString("N0");
        }
        else
        {
            double valorDouble = (double)ModifierClick.Instance.upgradeCost;

            int exponente = (int)Math.Floor(Math.Log10(valorDouble));

            double mantisa = valorDouble / Math.Pow(10, exponente);

            upgradeCost.text = $"Costo: {mantisa:F2} × 10<sup>{exponente}</sup>";
        }
    }

    public void Win()
    {
        winPanel.SetActive(true);

        DOVirtual.DelayedCall(5f, () =>
        {
            SceneManager.LoadScene("MainMenu");
            SceneManager.UnloadSceneAsync("MainGameplay");
        });
    }
}