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
    private bool estabaBajando;

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

        if (progress > lastProgress && progress > 0)
        {
            holdImage.color = Color.Lerp(colorInicial, colorCargado, t);
            float shakeForce = Mathf.Lerp(0, 15, t);

            if (heartbeatTween != null && heartbeatTween.IsActive())
                heartbeatTween.Kill();

            holdButton.anchoredPosition = posOriginalButton + (UnityEngine.Random.insideUnitCircle * shakeForce);

            if (fingerRect != null)
                fingerRect.anchoredPosition = posOriginalFinger + (UnityEngine.Random.insideUnitCircle * (shakeForce / 5f));
        }
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
                estabaBajando = false;
            }

            if (progress <= 0 && lastProgress > 0)
                ResetPosiciones();

            if (fingerImage != null)
                IniciarLatido();
        }

        lastProgress = progress;
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