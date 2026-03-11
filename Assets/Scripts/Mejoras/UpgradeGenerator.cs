using UnityEngine;
using System.Collections;

public class UpgradeGenerator : MonoBehaviour
{
    public GameObject prefab;

    [Header("Rango de aparición")]
    [Range(1f, 25f)] public float minTime = 3f;
    [Range(1f, 25f)] public float maxTime = 10f;

    GameObject currentInstance;

    void Start()
    {
        StartCoroutine(GenerarInfinitamente());
    }

    IEnumerator GenerarInfinitamente()
    {
        while (true)
        {
            // Solo genera si no hay uno existente
            if (currentInstance == null)
            {
                float tiempo = Random.Range(minTime, maxTime);
                yield return new WaitForSeconds(tiempo);

                float x = Random.Range(-6f, 6f);
                float y = Random.Range(-3f, 3f);

                Vector3 posicion = new Vector3(x, y, 0f);

                currentInstance = Instantiate(prefab, posicion, Quaternion.identity);
            }

            yield return null;
        }
    }
}