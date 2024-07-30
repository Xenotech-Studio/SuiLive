using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Versee.UI;

public class LoginMethodSave : MonoBehaviour
{
    public string LoginMethodKey;
    public string LoginMethod;
    
    public UnityEvent<string> OnLoginMethodGot;
    
    private void OnEnable()
    {
        StartCoroutine(OnEnableCoroutine());
    }

    IEnumerator OnEnableCoroutine()
    {
        yield return null;
        
        LoginMethod = PlayerPrefs.GetString(LoginMethodKey);
        if (LoginMethod == "") LoginMethod = "code_login";
        Debug.Log("Login Method:" + LoginMethod);
        
        OnLoginMethodGot.Invoke(LoginMethod);
    }
    
    public void SaveLoginMethod(string loginMethod)
    {
        LoginMethod = loginMethod;
        PlayerPrefs.SetString(LoginMethodKey, LoginMethod);
        Debug.Log("Login Method Saved: " + LoginMethod);
    }
}
