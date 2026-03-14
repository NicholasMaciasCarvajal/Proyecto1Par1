using System;
using System.Numerics;
using UnityEngine;

// Usamos tu idea de la clase serializable, pero mejor estructurada
[Serializable]
public class EvolutionStage
{
    public Mesh mesh;
    public Material material; // Es mejor cambiar el Material del Renderer
}

public class MeshEvolutionManager : MonoBehaviour
{
    [Header("Configuración de Evolución")]
    [Tooltip("Cantidad de clicks para avanzar a la siguiente fase (Ej: 100,000,000)")]
    public string clicksPerStageString = "100000000";
    private BigInteger clicksPerStage;

    [Tooltip("Añade aquí los Meshes y Materiales en orden")]
    public EvolutionStage[] stages;

    [Header("Referencias del Objeto")]
    public MeshFilter targetMeshFilter;
    public MeshRenderer targetMeshRenderer;

    // Variables de control interno para máxima eficiencia
    private int currentIndex = -1;
    private BigInteger lastProcessedClicks = -1;

    void Awake()
    {
        // Parseamos el string a BigInteger igual que lo haces en tu ClickCounter
        if (!BigInteger.TryParse(clicksPerStageString, out clicksPerStage) || clicksPerStage <= 0)
        {
            Debug.LogWarning("El valor de clicksPerStageString no es válido. Se usará 100,000,000.");
            clicksPerStage = 100000000;
        }
    }

    void Start()
    {
        // Forzamos la primera revisión al iniciar el juego
        CheckEvolution();
    }

    void Update()
    {
        if (ClickCounter.Instance == null) return;

        // EFICIENCIA: Solo calculamos si los clicks realmente cambiaron desde el último frame
        if (ClickCounter.Instance.totalClicks != lastProcessedClicks)
        {
            lastProcessedClicks = ClickCounter.Instance.totalClicks;
            CheckEvolution();
        }
    }

    private void CheckEvolution()
    {
        if (stages == null || stages.Length == 0 || clicksPerStage == 0) return;

        // MATEMÁTICA EFICIENTE: Una simple división nos da el índice exacto sin usar bucles
        BigInteger stageBigInt = ClickCounter.Instance.totalClicks / clicksPerStage;

        // Convertimos a int (seguro, porque el número de elementos en el array nunca superará el límite de un int)
        int calculatedIndex = (int)stageBigInt;

        // Si sobrepasamos el número de fases, nos quedamos con la última (Clamping)
        int finalIndex = Mathf.Clamp(calculatedIndex, 0, stages.Length - 1);

        // SOLO aplicamos los cambios si el índice calculado es diferente al que ya tenemos puesto
        if (finalIndex != currentIndex)
        {
            currentIndex = finalIndex;
            ApplyStage(stages[currentIndex]);
        }
    }

    private void ApplyStage(EvolutionStage stage)
    {
        if (stage.mesh != null && targetMeshFilter != null)
        {
            targetMeshFilter.mesh = stage.mesh;
        }

        if (stage.material != null && targetMeshRenderer != null)
        {
            targetMeshRenderer.material = stage.material;
        }

        // Tip: Aquí es un lugar perfecto para instanciar un sistema de partículas 
        // o reproducir un sonido de "Evolución de nivel".
    }
}