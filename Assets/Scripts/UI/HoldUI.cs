using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HoldUI : MonoBehaviour
{
    public Image holdImage;
    public RectTransform holdButton;

    public Image fingerImage;
    public RectTransform fingerRect;

    [Header("Configuración de Colores")]
    public Color colorInicial = Color.white;
    public Color colorCargado = Color.green;
    public Color colorBajando = Color.red;

    private Vector2 posOriginalButton;
    private Vector2 posOriginalFinger;
    private Tween heartbeatTween;
    private float lastProgress;
    private bool estabaBajando; // Para detectar el cambio de dirección

    void Awake()
    {
        posOriginalButton = holdButton.anchoredPosition;
        if (fingerRect != null)
            posOriginalFinger = fingerRect.anchoredPosition;
    }

    void Start()
    {
        IniciarLatido();
    }

    void IniciarLatido()
    {
        if (fingerImage == null) return;
        if (heartbeatTween != null && heartbeatTween.IsActive()) return;

        fingerImage.transform.localScale = Vector3.one;
        heartbeatTween = fingerImage.transform.DOScale(1.25f, 0.75f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    void Update()
    {
        float progress = ClickCounter.Instance.holdProgress;
        float required = ClickCounter.Instance.holdRequired;
        float t = progress / required;

        holdImage.fillAmount = t;

        // 1. ¿Está creciendo? (Presionando)
        if (progress > lastProgress && progress > 0)
        {
            // SI ESTABA BAJANDO Y AHORA SUBE -> ¡DESTELLO!
            if (estabaBajando)
            {
                EjecutarDestello();
                estabaBajando = false;
            }

            holdImage.color = Color.Lerp(colorInicial, colorCargado, t);
            float shakeForce = Mathf.Lerp(0, 25, t);

            if (heartbeatTween != null && heartbeatTween.IsActive())
                heartbeatTween.Kill();

            AplicarShake(holdButton, posOriginalButton, shakeForce);
            if (fingerRect != null)
                AplicarShake(fingerRect, posOriginalFinger, shakeForce/5);
        }
        // 2. ¿Está bajando o está en cero?
        else
        {
            if (progress < lastProgress) estabaBajando = true;

            if (progress > 0)
            {
                holdImage.color = colorBajando;
            }
            else
            {
                holdImage.color = colorInicial;
                estabaBajando = false; // Reset al llegar a cero
            }

            if (progress <= 0 && lastProgress > 0)
                ResetPosiciones();

            if (fingerImage != null)
                IniciarLatido();
        }

        lastProgress = progress;
    }

    void EjecutarDestello()
    {
        // 1. Flash de color blanco en la barra
        holdImage.DOKill();
        holdImage.color = Color.white;
        // Vuelve al color que le toca rápidamente
        holdImage.DOColor(Color.Lerp(colorInicial, colorCargado, holdImage.fillAmount), 0.2f);

        // 2. Pequeño salto de escala en el botón para sentir el "click"
        holdButton.DOKill(true);
        holdButton.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f, 10, 1);
    }

    void AplicarShake(RectTransform rt, Vector2 posOrig, float force)
    {
        rt.DOKill(true);
        rt.anchoredPosition = posOrig;
        rt.DOShakeAnchorPos(0.05f, force, 10, 90, false, true);
    }

    void ResetPosiciones()
    {
        holdButton.DOKill();
        holdButton.anchoredPosition = posOriginalButton;
        holdButton.localScale = Vector3.one;

        if (fingerRect != null)
        {
            fingerRect.DOKill();
            fingerRect.anchoredPosition = posOriginalFinger;
        }
    }

    private void OnDestroy()
    {
        if (heartbeatTween != null) heartbeatTween.Kill();
    }
}