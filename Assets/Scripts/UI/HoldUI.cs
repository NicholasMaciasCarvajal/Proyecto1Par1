using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HoldUI : MonoBehaviour
{
    public Image holdImage;
    public RectTransform holdButton;

    private Vector2 posOriginal;

    void Awake()
    {
        // Guardamos la posición inicial para que siempre regrese ahí
        posOriginal = holdButton.anchoredPosition;
    }

    void Update()
    {
        float progress = ClickCounter.Instance.holdProgress;
        float required = ClickCounter.Instance.holdRequired;
        float t = progress / required;

        holdImage.fillAmount = t;

        if (progress > 0)
        {
            // 1. Calculamos la fuerza
            float shakeForce = Mathf.Lerp(0, 25, t);

            // 2. IMPORTANTE: Matamos el tween anterior para que no se acumulen
            // y forzamos el regreso a la posición original antes del siguiente frame
            holdButton.DOKill(true);
            holdButton.anchoredPosition = posOriginal;

            // 3. Usamos DOShakeAnchorPos (específico para UI)
            // Usamos una duración muy corta (Time.deltaTime) para que se refresque con el Update
            holdButton.DOShakeAnchorPos(
                0.05f,
                shakeForce,
                10,
                90,
                false,
                true
            );
        }
        else
        {
            // Si no hay progreso, nos aseguramos de que esté en su sitio y sin animaciones
            if (holdButton.anchoredPosition != posOriginal)
            {
                holdButton.DOKill();
                holdButton.anchoredPosition = posOriginal;
            }
        }
    }
}