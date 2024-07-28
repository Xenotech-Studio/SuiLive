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
    
    
    public bool Reverse { get => _reverse; set => _reverse = value; }
    public bool _reverse;
    private bool _reverseLastFrame;

    private Coroutine _scrollToBottomCoroutine;

    private void Awake()
    {
        Template.gameObject.SetActive(false);
    }

    private void Update()
    {
        ListContentParent.GetComponent<VerticalLayoutGroup>().reverseArrangement = Reverse;
        if (_reverse != _reverseLastFrame)
        {
            _reverseLastFrame = _reverse;
            StartCoroutine(ScrollToBottom(0.5f));
        }
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
        
        if (String.IsNullOrEmpty(dm.fansMedalName)) textFieldIndexing.TextFields["fansMedalName"].gameObject.SetActive(false);

        newItem.GetChild(1).GetComponent<ContentSizeFitter>().enabled = true;
        newItem.GetComponent<ContentSizeFitter>().enabled = true;
        
        if(_scrollToBottomCoroutine!=null) StopCoroutine(_scrollToBottomCoroutine);
        _scrollToBottomCoroutine = StartCoroutine(ScrollToBottom(0.5f));
        StartCoroutine(FadeIn(newItem.GetComponent<CanvasGroup>(), 0.5f));
    }
    
    public void ReceiveGift(SendGift gift)
    {
        RectTransform newItem = Instantiate(Template, ListContentParent);
        newItem.gameObject.SetActive(true);
        newItem.name = $"DamakuItem ({counter}) - {gift.userName} - {gift.timestamp}"; 
        counter += 1;
        
        TextFieldIndexing textFieldIndexing = newItem.GetComponentInChildren<TextFieldIndexing>();
        textFieldIndexing.TextFields["msg"].text = "投喂了" + gift.giftNum + "个" + gift.giftName + "(" + gift.price/10 + "电池)";
        textFieldIndexing.TextFields["userName"].text = gift.userName;
        textFieldIndexing.TextFields["fansMedalLevel"].text = gift.fansMedalLevel.ToString();
        textFieldIndexing.TextFields["fansMedalName"].text = gift.fansMedalName;
        
        if (String.IsNullOrEmpty(gift.fansMedalName)) textFieldIndexing.TextFields["fansMedalName"].gameObject.SetActive(false);

        newItem.GetChild(1).GetComponent<ContentSizeFitter>().enabled = true;
        newItem.GetComponent<ContentSizeFitter>().enabled = true;
        
        if(_scrollToBottomCoroutine!=null) StopCoroutine(_scrollToBottomCoroutine);
        _scrollToBottomCoroutine = StartCoroutine(ScrollToBottom(0.5f));
        StartCoroutine(FadeIn(newItem.GetComponent<CanvasGroup>(), 0.5f));
    }
    
    IEnumerator ScrollToBottom(float time)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            VerticalScrollBar.value = Reverse ? 1f : 0f;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator FadeIn(CanvasGroup cg, float time)
    {
        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            cg.alpha = Mathf.Lerp(0, 1, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
