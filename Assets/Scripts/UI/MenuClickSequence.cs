using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening; // Importante para los efectos

[Serializable]
public class ImagenClick
{
    public Image img;
    public RectTransform rect;
}

public class MenuClickSequence : MonoBehaviour
{
    public ImagenClick[] clickIndicators; // Tus 3 indicadores
    public float resetTime = 2f;

    private int clickCount = 0;
    private Sequence drainSequence; // Para controlar el vaciado ordenado

    void Start()
    {
        // Inicializamos todo en 0
        foreach (var item in clickIndicators)
        {
            item.img.fillAmount = 0;
        }
    }

    public void RegisterClick()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
            return;

        // Si ya hay un vaciado en curso, lo detenemos para procesar el nuevo click
        KillDrainSequence();

        if (clickCount < clickIndicators.Length)
        {
            ImagenClick actual = clickIndicators[clickCount];
            clickCount++;

            // --- EFECTO DOTween: Golpe de escala y color ---
            actual.rect.DOKill(true);
            actual.rect.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 0.2f);

            actual.img.DOKill();
            actual.img.fillAmount = 1; // Llenamos la imagen actual
            actual.img.color = Color.white;
            actual.img.DOColor(Color.yellow, 0.1f).SetLoops(2, LoopType.Yoyo);

            // Si llegamos al máximo (3)
            if (clickCount >= clickIndicators.Length)
            {
                // GameManager.Instance.OpenCloseCredits(); // Tu lógica
                Debug.Log("¡Créditos Abiertos!");
                GameManager.Instance.OpenCloseCredits();
                ResetClicks();
            }
            else
            {
                // Iniciamos el temporizador para empezar a vaciar desde la última llena
                StartDrainSequence();
            }
        }
    }

    void StartDrainSequence()
    {
        drainSequence = DOTween.Sequence();
        drainSequence.AppendInterval(resetTime);

        float duracionPorImagen = 1.0f / clickCount;
        duracionPorImagen = Mathf.Clamp(duracionPorImagen, 0.2f, 0.5f);

        // Recorremos en orden inverso (de la última llena a la primera)
        for (int i = clickCount - 1; i >= 0; i--)
        {
            // 1. Añadimos el vaciado de la imagen
            drainSequence.Append(clickIndicators[i].img.DOFillAmount(0, duracionPorImagen).SetEase(Ease.Linear));

            // 2. NUEVO: En cuanto termine de vaciarse ESTA imagen, bajamos el contador
            // Usamos una variable temporal para capturar el valor correcto de i
            int indexActual = i;
            drainSequence.AppendCallback(() => {
                clickCount = indexActual;
            });
        }

        // Ya no necesitamos el OnComplete porque los Callbacks se encargan de bajar el número
    }

    void KillDrainSequence()
    {
        if (drainSequence != null)
        {
            drainSequence.Kill();
            drainSequence = null;
        }
    }

    void ResetClicks()
    {
        KillDrainSequence();
        clickCount = 0;

        foreach (var item in clickIndicators)
        {
            item.img.DOKill(); // Detener cambios de color
            item.rect.DOKill(); // Detener punches de escala
            item.img.DOFillAmount(0, 0.3f).SetEase(Ease.OutQuad);
            item.rect.DOScale(Vector3.one, 0.2f);
        }
    }
}