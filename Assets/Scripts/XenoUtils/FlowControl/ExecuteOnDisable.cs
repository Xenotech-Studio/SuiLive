using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExecuteOnDisable : MonoBehaviour
{
    public UnityEvent ExeOnDisable;
    
    // Start is called before the first frame update
    private void OnDisable()
    {
        ExeOnDisable?.Invoke();
    }
}
