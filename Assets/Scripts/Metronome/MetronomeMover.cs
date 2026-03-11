using UnityEngine;

public class MetronomeMover : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    float t;

    void Update()
    {
        t += Time.deltaTime * speed;

        transform.position = Vector2.Lerp(pointA.position, pointB.position, Mathf.PingPong(t, 1));
    }
}