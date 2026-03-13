using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Animar(string valor)
    {
        textMesh.text = "+" + valor;

        // 1. Mover hacia arriba (ej: 150 unidades) en 3 segundos
        RectTransform rt = GetComponent<RectTransform>();
        rt.DOAnchorPosY(150f, 3f).SetRelative(true).SetEase(Ease.OutQuad);

        // 2. Desvanecerse progresivamente y DESTRUIR el objeto al terminar
        canvasGroup.DOFade(0f, 1f).SetEase(Ease.InQuart).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    private void OnDestroy()
    {
        // Limpiamos los Tweens por si la escena cambia antes de los 3 segundos
        transform.DOKill();
        if (canvasGroup != null) canvasGroup.DOKill();
    }
}