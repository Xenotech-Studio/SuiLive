using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterDelay : MonoBehaviour
{

    public float Delay;

    private float _timer;
    private bool _shown;

    private void OnEnable()
    {
        _timer = 0;
        _shown = true;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer > Delay)
        {
            _shown = false;
            gameObject.SetActive(false);
        }
    }
}
