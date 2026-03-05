using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickDetector : MonoBehaviour
{
    public List<MonoBehaviour> receivers;

    private List<IClickTransmissor> clickTransmissors;

    private void Start()
    {
        foreach (var r in receivers)
        {
            if (r is IClickTransmissor receiver)
            {
                clickTransmissors.Add(receiver);
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (var r in clickTransmissors)
            {
                r.OnClickDOWN();
            }
        }
        if (Input.GetMouseButton(0))
        {
            foreach(var r in clickTransmissors)
            {
                r.OnClickHold(); 
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            foreach (var r in clickTransmissors)
            { 
                r.OnClickUP(); 
            }
        }
    }
}
