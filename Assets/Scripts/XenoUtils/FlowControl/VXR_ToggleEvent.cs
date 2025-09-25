using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VXR_ToggleEvent : MonoBehaviour
{
    public UnityEvent TrueEvent;
    public UnityEvent FalseEvent;

    public bool Value = false;

    // Update is called once per frame
    public void Toggle()
    {
        Value = !Value;
        if(Value) TrueEvent?.Invoke();
        else FalseEvent?.Invoke();
    }
    
    public void Set(bool value)
    {
        Value = value;
        if(Value) TrueEvent?.Invoke();
        else FalseEvent?.Invoke();
    }
}
