using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class MetronomeColliderGame : MonoBehaviour
{
    [Header("Moving Object")]
    public Collider2D movingCollider;

    [Header("Attempts")]
    public int maxAttempts = 3;
    int currentAttempts;

    [Header("Success")]
    public int maxSuccess;
    public int currentSuccess;

    [Header("UI & Visuals")]
    public GameObject[] errorImages;
    public GameObject[] successImages;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip successSound;
    public AudioClip failSound;

    bool canClick = false;
    bool yaClicoEnEstePaso = false;
    // Cambia la inicialización de esta variable (o asegúrate de que esté en true al inicio)
    bool juegoTerminado = true;

    [Header("Events")]
    public UnityEvent OnFail;
    public UnityEvent OnSuccess;

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        // --- NUEVO: Delay inicial para evitar clics accidentales ---
        DOVirtual.DelayedCall(0.5f, () => {
            juegoTerminado = false;
        });
    }

    void OnDestroy()
    {
        if (ClickInputManager.Instance != null)
            ClickInputManager.Instance.OnClick -= RegisterClick;
    }

    public void RegisterClick()
    {
        // Si el juego ya terminó (esperando evento), no registramos más clics
        if (yaClicoEnEstePaso || juegoTerminado) return;

        if (canClick)
        {
            yaClicoEnEstePaso = true;
            CorrectClick();
        }
        else
        {
            WrongClick();
        }
    }

    void CorrectClick()
    {
        if (GameManager.Instance.isInCredits) return;
        if (currentSuccess >= maxSuccess) return;

        if (audioSource && successSound) audioSource.PlayOneShot(successSound);

        if (successImages != null && currentSuccess < successImages.Length)
        {
            GameObject img = successImages[currentSuccess];
            if (img != null) AnimarActivacion(img, Color.green);
        }

        currentSuccess++;

        if (currentSuccess >= maxSuccess)
        {
            juegoTerminado = true;
            // Espera 1 segundo antes de invocar el éxito
            // 
            if (OnSuccess.GetPersistentEventCount() > 0)
            {
                DOVirtual.DelayedCall(1f, () => OnSuccess.Invoke());
            }
            else
            {
                DOVirtual.DelayedCall(1f, () => OnSuccessNull());
            }
        }
    }

    void WrongClick()
    {
        if (GameManager.Instance.isInCredits) return;

        if (audioSource && failSound) audioSource.PlayOneShot(failSound);

        bool tieneSistemaDeVidas = errorImages != null && errorImages.Length > 0;

        if (tieneSistemaDeVidas)
        {
            if (currentAttempts >= maxAttempts) return;

            if (currentAttempts < errorImages.Length)
            {
                GameObject img = errorImages[currentAttempts];
                if (img != null) AnimarActivacion(img, Color.red);
            }

            currentAttempts++;

            if (currentAttempts >= maxAttempts)
            {
                juegoTerminado = true;
                // Espera 1 segundo antes de invocar la derrota
                
                DOVirtual.DelayedCall(1f, () => OnFail?.Invoke());
            }
        }
    }

    void AnimarActivacion(GameObject obj, Color colorFlash)
    {
        obj.SetActive(true);
        Transform t = obj.transform;
        t.DOKill(true);

        t.localScale = Vector3.zero;
        t.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
        t.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.2f, 10, 1);

        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        UnityEngine.UI.Image uiImg = obj.GetComponent<UnityEngine.UI.Image>();

        if (uiImg != null)
        {
            Color original = uiImg.color;
            uiImg.color = colorFlash;
            uiImg.DOColor(original, 0.4f);
        }
        else if (sr != null)
        {
            Color original = sr.color;
            sr.color = colorFlash;
            sr.DOColor(original, 0.4f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MovingBeat"))
        {
            canClick = true;
            yaClicoEnEstePaso = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("MovingBeat"))
        {
            canClick = false;
            yaClicoEnEstePaso = false;
        }
    }

    public void DestroyObject()
    {
        if (transform.parent != null)
            Destroy(transform.parent.gameObject, 0.4f);
        else
            Destroy(gameObject, 0.4f);
    }

    private void OnSuccessNull()
    {
        ModifierClick.Instance.IncreaseRandomModifier();

        DestroyObject();
    }
}