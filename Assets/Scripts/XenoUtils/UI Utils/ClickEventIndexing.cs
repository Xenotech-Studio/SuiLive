using System;
using System.Collections.Generic;
using EditorUtils;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ClickEventIndexing : MonoBehaviour
{
    [HideInInspector]
    public Versee.Scripts.Utils.SerializableDictionary<string, RectTransform> ClickableRects = new Versee.Scripts.Utils.SerializableDictionary<string, RectTransform>();

    public Dictionary<string, Action> ClickEvents = new Dictionary<string, Action>();

    private void Awake()
    {
        // for each key in ClickableRects, try to create an Action in Actions
        foreach (var key in ClickableRects.Keys)
        {
            if (!ClickEvents.ContainsKey(key))
            {
                ClickEvents.Add(key, () => {});
            }
        }
        
        // for each ClickableRects, add a EventTrigger component to it and add a listener to it
        foreach (var clickableRect in ClickableRects)
        {
            var eventTrigger = clickableRect.Value.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            var entry = new UnityEngine.EventSystems.EventTrigger.Entry();
            entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { ClickEvents[clickableRect.Key](); });
            eventTrigger.triggers.Add(entry);
        }
    }
}

// editor for ClickableRects
#if UNITY_EDITOR
[CustomEditor(typeof(ClickEventIndexing))]
public class ClickEventIndexingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var clickEventIndexing = (ClickEventIndexing) target;
        DictionaryUtils.RenderStringToObjectDic("Clickable Rects", ref clickEventIndexing.ClickableRects);
    }
}
#endif
