using System.Numerics;
using UnityEngine;
using System.Collections;

public class AutoClicker : MonoBehaviour
{
    public static AutoClicker Instance; // Corregido para que sea la instancia del AutoClicker

    [Header("AutoClicker Settings")]
    public bool isAutoClickerActive = false;
    public float clickInterval = 0.4f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Iniciamos el bucle del AutoClicker en cuanto arranca el juego
        StartCoroutine(AutoClickRoutine());
    }

    private IEnumerator AutoClickRoutine()
    {
        // Este bucle se ejecutará infinitamente mientras el GameObject esté activo
        while (true) 
        {
            // Pausamos la ejecución por 0.4 segundos
            yield return new WaitForSeconds(clickInterval);

            // Solo generamos ganancias si el AutoClicker ya está activado
            if (isAutoClickerActive && !GameManager.Instance.IsPaused)
            {
                GenerateAutoClick();
                
                // Mostrar el texto flotante del auto-clicker
                BigInteger valorMostrar = ModifierClick.Instance.GetClickValue() / 2;
                if (valorMostrar < 1) valorMostrar = 1; // Para que el texto coincida con la lógica de GenerateAutoClick
                
                ClickInputManager.Instance.GenerarTextoFlotanteAuto(valorMostrar); 
            }
        }
    }

    private void GenerateAutoClick()
    {
        BigInteger currentClickValue = ModifierClick.Instance.GetClickValue();
        BigInteger autoClickValue = currentClickValue / 2;

        if (autoClickValue < 1 && currentClickValue > 0) 
        {
            autoClickValue = 1;
        }

        //ClickCounter.Instance.totalClicks += autoClickValue;
        //ClickCounter.Instance.ActualizarClicks();
        ClickCounter.Instance.AddClick(autoClickValue);

    }
}