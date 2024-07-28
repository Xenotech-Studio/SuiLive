using System;
using System.Collections;
using System.Collections.Generic;
using OpenBLive.Runtime.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatDisplay : MonoBehaviour
{
    public RectTransform Template;
    public RectTransform ListContentParent;
    public Scrollbar VerticalScrollBar;
    public int counter;

    private void Awake()
    {
        Template.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    public void ReceiveDamaku(Dm dm)
    {
        
        RectTransform newItem = Instantiate(Template, ListContentParent);
        newItem.gameObject.SetActive(true);
        newItem.name = $"DamakuItem ({counter}) - {dm.userName} - {dm.timestamp}"; 
        counter += 1;
        
        TextFieldIndexing textFieldIndexing = newItem.GetComponentInChildren<TextFieldIndexing>();
        textFieldIndexing.TextFields["msg"].text = dm.msg;
        textFieldIndexing.TextFields["userName"].text = dm.userName;
        textFieldIndexing.TextFields["fansMedalLevel"].text = dm.fansMedalLevel.ToString();
        textFieldIndexing.TextFields["fansMedalName"].text = dm.fansMedalName;
        
        
        StopAllCoroutines();
        StartCoroutine(ScrollToBottom());
    }
    
    IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        VerticalScrollBar.value = 0f;
        yield return new WaitForEndOfFrame();
        VerticalScrollBar.value = 0f;
    }
}
