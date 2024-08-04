using System.Collections;
using System.Collections.Generic;
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
    
    public void SetTopic(string topic)
    {
        Debug.Log("话题文字更新："+topic);
        CurrentTopic.text = topic;
    }
}
