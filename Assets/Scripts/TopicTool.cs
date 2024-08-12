using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TopicTool : MonoBehaviour
{
    public TopicToolUI topicToolUI;

    public TMP_InputField DurationInputField;

    public float Duration
    {
        get => DurationInputField.text == "" ? 0 : float.Parse(DurationInputField.text);
        set => DurationInputField.text = value.ToString();
    }

    public TMP_InputField RandomRangeStartField;

    public int RandomRangeStart
    {
        get => RandomRangeStartField.text == "" ? 0 : int.Parse(RandomRangeStartField.text);
        set => RandomRangeStartField.text = value.ToString();
    }

    public TMP_InputField RandomRangeEndField;

    public int RandomRangeEnd
    {
        get => RandomRangeEndField.text == "" ? 0 : int.Parse(RandomRangeEndField.text);
        set => RandomRangeEndField.text = value.ToString();
    }

    public Toggle IsRandomToggle;

    public bool IsRandom
    {
        get => IsRandomToggle.isOn;
        set => IsRandomToggle.isOn = value;
    }

    public TMP_InputField TopicsInputField;

    public string TopicsStr
    {
        get => TopicsInputField.text;
        set => TopicsInputField.text = value;
    }
    
    public List<string> Topics
    {
        get
        {
            // remove empty string
            var r = new List<string>(TopicsStr.Split(';')).ConvertAll(topic => topic.TrimStart());
            r.RemoveAll(topic => topic == "");
            return r;
        }
    }

    public TMP_InputField CurrentTopicEditInputField;

    public string CurrentTopicEdit
    {
        get => CurrentTopicEditInputField.text; 
        set => CurrentTopicEditInputField.text = value;
    }


    public void Start()
    {
        // update timer when duration changed
        DurationInputField.onValueChanged.AddListener((str) =>
        {
            timer = Duration;
        });
    }


    public void SetTopic(string topicsStr)
    {
        CurrentTopicEdit = topicsStr;
        topicToolUI.SetTopic(topicsStr);
    }
    
    public void SetTopicFromInput()
    {
        SetTopic(CurrentTopicEdit);
        historyTopicIndexes.Add(currentTopicIndex);
        timer = Duration;
    }
    
    public void StartPlay()
    {
        StopAllCoroutines();
        StartCoroutine(PlayCoroutine());
    }
    
    public void NextNow()
    {
        timer = 0;
    }

    public void LastNow()
    {
        // read history
        if (historyTopicIndexes.Count > 0)
        {
            historyTopicIndexes.RemoveAt(historyTopicIndexes.Count - 1);
            currentTopicIndex = historyTopicIndexes[^1];
            SetTopic(Topics[currentTopicIndex]);
            timer = Duration;
        }
    }

    
    
    
    
    
    private int currentTopicIndex = 0;
    private List<int> historyTopicIndexes = new List<int>();
    private float timer = 0;
    
    IEnumerator PlayCoroutine()
    {
        Debug.Log("话题助手开始播放");
        while (true)
        {
            Debug.Log("准备切换新话题");
            if (IsRandom)
            {
                RandomRangeEnd = Math.Max(1, RandomRangeEnd);
                RandomRangeEnd = Math.Min(Topics.Count, RandomRangeEnd);
                RandomRangeStart = Math.Max(1, RandomRangeStart);
                RandomRangeStart = Math.Min(RandomRangeEnd, RandomRangeStart);
                
                // random choose and avoid repeating
                int randomIndex = Random.Range(RandomRangeStart-1, RandomRangeEnd);
                int times = 0;
                while (historyTopicIndexes.Contains(randomIndex) && times < 20)
                {
                    randomIndex = Random.Range(RandomRangeStart, RandomRangeEnd);
                    times++;
                }
                if (times >= 20)
                {
                    historyTopicIndexes.Clear();
                }
                else
                {
                    historyTopicIndexes.Add(randomIndex);
                    currentTopicIndex = randomIndex;
                }
                SetTopic(Topics[randomIndex]);
            }
            else
            {
                currentTopicIndex = currentTopicIndex % Topics.Count;
                SetTopic(Topics[currentTopicIndex]);
                currentTopicIndex++;
                
                // add to history
                historyTopicIndexes.Add(currentTopicIndex);
            }
            
            // timer
            timer = Duration;
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                yield return null;
            }
        }
    }

    

    
}
