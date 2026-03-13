using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Numerics; // Aseg�rate de incluir esto para BigInteger

public class ClickInputManager : MonoBehaviour
{
    public static ClickInputManager Instance;

    public event Action OnClick;
    public event Action OnHoldComplete;

    [Header("Floating Text Setup")]
    public FloatingText floatingTextPrefab;
    public RectTransform floatingTextContainer; // Un panel vac�o dentro de tu Canvas

    bool holding;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            holding = true;
            ClickCounter.Instance.StartHold();
        }

        if (Input.GetMouseButton(0))
        {
            if (!GameManager.Instance.isInCredits)
            {
                ClickCounter.Instance.AddHoldTime(Time.unscaledDeltaTime);
            }

            if (ClickCounter.Instance.HoldComplete())
            {
                OnHoldComplete?.Invoke();
                ClickCounter.Instance.ResetHold();
                if (SceneManager.GetActiveScene().name == "MainMenu")
                {
                    GameManager.Instance.SceneChange();
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            holding = false;

            if (!ClickCounter.Instance.HoldComplete())
            {
                if (!GameManager.Instance.IsPaused)
                {
                    // 1. Obtenemos el valor del click
                    BigInteger clickValue = ModifierClick.Instance.GetClickValue();

                    // 2. Sumamos el click al contador
                    ClickCounter.Instance.AddClick(clickValue);

                    // 3. GENERAMOS EL TEXTO FLOTANTE
                    GenerarTextoFlotante(clickValue);

                    FindFirstObjectByType<MetronomeColliderGame>()?.RegisterClick();
                }
                FindFirstObjectByType<MenuClickSequence>()?.RegisterClick();
                FindFirstObjectByType<PauseClickSequence>()?.RegisterClick();
                OnClick?.Invoke();
            }

            ClickCounter.Instance.StopHold();
        }
    }

    private void GenerarTextoFlotante(BigInteger valor)
    {
        if (floatingTextPrefab == null || floatingTextContainer == null) return;

        // Instanciamos el prefab dentro del contenedor
        FloatingText nuevoTexto = Instantiate(floatingTextPrefab, floatingTextContainer);

        // Convertimos la posici�n del mouse de la pantalla al Canvas
        UnityEngine.Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            floatingTextContainer,
            Input.mousePosition,
            null, // Cambia esto a Camera.main si tu Canvas est� en "Screen Space - Camera"
            out localPoint);

        // Asignamos la posici�n y le damos algo de aleatoriedad para que no salgan todos amontonados
        float randomX = UnityEngine.Random.Range(-20f, 20f);
        float randomY = UnityEngine.Random.Range(-20f, 20f);
        nuevoTexto.GetComponent<RectTransform>().anchoredPosition = localPoint + new UnityEngine.Vector2(randomX, randomY);

        // Formateamos el n�mero (usa tu misma l�gica de mantisas si lo prefieres)
        string valorTexto = valor < 1000000 ? valor.ToString("N0") : FormatBigNumber(valor);

        // Iniciamos la animaci�n
        nuevoTexto.Animar(valorTexto);
    }

    // Un peque�o helper para que los n�meros del texto flotante coincidan con tu formato
    private string FormatBigNumber(BigInteger value)
    {
        double valorDouble = (double)value;
        int exponente = (int)Math.Floor(Math.Log10(valorDouble));
        double mantisa = valorDouble / Math.Pow(10, exponente);
        return $"{mantisa:F2}x10^{exponente}";
    }

    public void GenerarTextoFlotanteAuto(BigInteger valor)
    {
        if (floatingTextPrefab == null || floatingTextContainer == null) return;

        // Instanciamos el prefab dentro del contenedor
        FloatingText nuevoTexto = Instantiate(floatingTextPrefab, floatingTextContainer);

        // --- EL CAMBIO PRINCIPAL ---
        // Calculamos el centro exacto de la pantalla actual
        UnityEngine.Vector2 centroPantalla = new UnityEngine.Vector2(Screen.width / 2f, Screen.height / 2f);

        // Convertimos esa posición central de la pantalla al espacio del Canvas
        UnityEngine.Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            floatingTextContainer,
            centroPantalla, // Usamos el centro en lugar de Input.mousePosition
            null,           // Cambia esto a Camera.main si tu Canvas está en "Screen Space - Camera"
            out localPoint);

        // Le damos un poco MÁS de aleatoriedad (ej. de -40 a 40) para que el auto-clicker
        // parezca una "fuente" de números burbujeando en el centro
        float randomX = UnityEngine.Random.Range(-40f, 40f);
        float randomY = UnityEngine.Random.Range(-40f, 40f);
        nuevoTexto.GetComponent<RectTransform>().anchoredPosition = localPoint + new UnityEngine.Vector2(randomX, randomY);

        // Formateamos el número usando tu misma lógica
        string valorTexto = valor < 1000000 ? valor.ToString("N0") : FormatBigNumber(valor);

        // Iniciamos la animación
        nuevoTexto.Animar(valorTexto);
    }
}