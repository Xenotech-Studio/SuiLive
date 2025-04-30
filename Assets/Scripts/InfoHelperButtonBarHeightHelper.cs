using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class InfoHelperButtonBarHeightHelper : MonoBehaviour
{
    public bool hasBottomBar;
    
    public bool HasBottomBar
    {
        get => hasBottomBar;
        set => hasBottomBar = value;
    }

    public RectTransform ScrollView;

    public RectTransform ConnectionStatus;

    public int ExtraSpace = 50;
    
    // Update is called once per frame
    void Update()
    {
        if (hasBottomBar)
        {
            //gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -100);
            
            // ScrollView is a strech laybout, change its "bottom" value
            ScrollView.offsetMin = new Vector2(10, 10 + ExtraSpace);
        }
        else
        {
            ScrollView.offsetMin = new Vector2(10, 10);
            
        }

        if (hasBottomBar && gameObject.activeSelf)
        {
            ConnectionStatus.anchoredPosition = new Vector2(ConnectionStatus.anchoredPosition.x, 22 + ExtraSpace);
        }
        else
        {
            ConnectionStatus.anchoredPosition = new Vector2(ConnectionStatus.anchoredPosition.x, 22);
        }
    }

    private void OnDisable()
    {
        ConnectionStatus.anchoredPosition = new Vector2(ConnectionStatus.anchoredPosition.x, 22);
    }
}
