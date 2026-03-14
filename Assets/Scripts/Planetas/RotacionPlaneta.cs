using UnityEngine;
using DG.Tweening;

public class RotacionPlaneta : MonoBehaviour
{
    [Header("Configuración de Rotación")]
    [Tooltip("Tiempo en segundos que tarda en dar una vuelta completa")]
    public float duracionVuelta = 10f;

    [Tooltip("Eje sobre el que rotará (X, Y o Z). Pon 1 en el eje que quieras mover.")]
    public Vector3 ejeDeRotacion = new Vector3(0, 0, 1);

    void Start()
    {
        // Multiplicamos el eje por 360 grados para dar una vuelta completa
        Vector3 rotacionFinal = ejeDeRotacion * 360f;

        // Iniciamos la rotación infinita
        transform.DORotate(rotacionFinal, duracionVuelta, RotateMode.FastBeyond360)
            .SetRelative(true) // Hace que la rotación se sume a la actual
            .SetEase(Ease.Linear) // Mantiene una velocidad constante sin acelerar/frenar
            .SetLoops(-1); // -1 significa que se repetirá infinitamente
    }
}