using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LongPress : MonoBehaviour
{
    public float LongPressTime = 1.0f;
    
    public bool _longPressingTriggered = false;
    
    public UnityEvent OnLongPress;
    
    public void OnPressStart()
    {
        _longPressingTriggered = false;
        StartCoroutine(LongPressCoroutine());
    }
    
    public void OnPressEnd()
    {
        StopAllCoroutines();
        _longPressingTriggered = false;
    }
    
    private IEnumerator LongPressCoroutine()
    {
        yield return new WaitForSeconds(LongPressTime);
        if (!_longPressingTriggered)
        {
            OnLongPress.Invoke();
        }
    }
}
