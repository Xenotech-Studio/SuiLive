using System;
using System.Collections;
using System.Collections.Generic;
using SuiLive;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class TopicToolUI : MonoBehaviour
{
    [Range(-1, 1)]
    public float ScrollProgress;
    
    public TMP_Text LastTopic;
    public TMP_Text CurrentTopic;
    public TMP_Text NextTopic;

    public CanvasGroup canvasGroup;
    
    public void SetTopic(string topic)
    {
        Debug.Log("话题文字更新："+topic);
        CurrentTopic.text = topic;
    }

    private void Update()
    {
        canvasGroup.alpha = ConfigManager.Config.TopicHelper.Enabled?1:0;
    }
}
