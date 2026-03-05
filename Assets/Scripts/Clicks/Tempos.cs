using UnityEngine;

[System.Serializable]
public class TemposD : MonoBehaviour
{
    public Collider2D zonaBuena;
    public Collider2D indicador;
    public int contador;
    public float velocidad;
}


public class Tempos : MonoBehaviour
{

    public TemposD[] tempos;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
    }
}
