using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Versee.UI
{

    public interface VXR_FlowItemInterface
    {
        GameObject gameObject { get; }
        public string ID { get; }
    };

    public class VXR_FlowItem : MonoBehaviour, VXR_FlowItemInterface
    {
        public string id;
        public string ID { get=>id; }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(VXR_FlowItem))]
    public class VXR_FlowItemEditor : Editor
    {
        [FormerlySerializedAs("DialogWindow")] public VXR_FlowItem FlowItem;

        private void Awake()
        {
            FlowItem = (VXR_FlowItem)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawControlButtons(FlowItem);
        }

        public static void DrawControlButtons(VXR_FlowItemInterface FlowItem)
        {
            if (GUILayout.Button("Jump to Here"))
            {
                Activate(FlowItem.gameObject);
            }
        }

        public static void Activate( GameObject obj )
        {
            if (!obj.activeInHierarchy)
            {
                Activate(obj.transform.parent.gameObject);
            }

            bool hasParent =  obj.transform.GetComponentInParent<VXR_Flow>().gameObject != obj && obj.transform.parent != null;
            
            if (hasParent && obj.transform.parent.childCount > 1)
            {
                for (int i = 0; i < obj.transform.parent.childCount; i++)
                {
                    VXR_FlowItemInterface item;
                    if (obj.transform.parent.GetChild(i).TryGetComponent<VXR_FlowItemInterface>(out item))
                    {
                        item.gameObject.SetActive(false);
                    }
                }
            }
            obj.SetActive(true);
        }
    }
    #endif
}
