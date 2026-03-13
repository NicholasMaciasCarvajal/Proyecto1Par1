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
    private Vector2 posOriginalUpgrade; // NUEVO: Para guardar el anclaje inicial de upgradeCost
    private Tween upgradeHeartbeatTween; // NUEVO: Para controlar la animación de latido

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
        // Guardamos las posiciones exactas donde pusiste el texto en el Editor
        posOriginal = clickText.rectTransform.anchoredPosition;

        if (upgradeCost != null)
        {
            posOriginalUpgrade = upgradeCost.rectTransform.anchoredPosition;
            IniciarLatidoCosto(); // Arrancamos el latido al iniciar la escena
        }
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
            Win();
        }

    }

    public void IniciarLatidoCosto()
    {
        if (upgradeCost == null) return;

        upgradeCost.rectTransform.DOKill();

        upgradeCost.rectTransform.anchoredPosition = posOriginalUpgrade;
        upgradeCost.rectTransform.localScale = Vector3.one;

        upgradeHeartbeatTween = upgradeCost.rectTransform.DOScale(1.15f, 0.6f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
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

    [ContextMenu("Win")]
    public void Win()
    {
        // 1. Obtenemos el CanvasGroup de tu panel
        CanvasGroup canvasGroup = winPanel.GetComponent<CanvasGroup>();

        // (Opcional) Si olvidaste ponerlo en el editor, esto se lo agrega automáticamente
        if (canvasGroup == null)
        {
            canvasGroup = winPanel.AddComponent<CanvasGroup>();
        }

        // 2. Antes de activar el panel, lo hacemos totalmente transparente (Alpha = 0)
        canvasGroup.alpha = 0f;

        // 3. Activamos el panel (el jugador no lo verá de golpe porque es transparente)
        winPanel.SetActive(true);

        // 4. Hacemos el "Fade In": Llevamos el Alpha a 1 (totalmente opaco) en 1 segundo
        canvasGroup.DOFade(1f, 1.5f).SetEase(Ease.InOutQuad);

        DOVirtual.DelayedCall(5f, () =>
        {
            SceneManager.LoadScene("MainMenu");
            SceneManager.UnloadSceneAsync("MainGameplay");
        });
    }

    private void OnDestroy()
    {
        // Matamos la animación de latido si la escena se descarga o el objeto se destruye
        if (upgradeHeartbeatTween != null && upgradeHeartbeatTween.IsActive())
        {
            upgradeHeartbeatTween.Kill();
        }

        // Por seguridad, limpiamos también el clickText
        if (clickText != null) clickText.rectTransform.DOKill();
    }
}