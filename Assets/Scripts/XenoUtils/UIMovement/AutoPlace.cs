using System;
using Unity.VisualScripting;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Temp
{
    public class AutoPlace : FollowTransform
    {
        public float Delay;

        protected override void Awake()
        {
            Validated = false; // Stop Executing Update()
            Invoke(nameof(Follow), Delay);
        }

        private void OnEnable()
        {
            Invoke(nameof(Follow), Delay);
        }

        void Update()
        {
            return;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AutoPlace))]
    class AutoPlaceEditor : FollowTransformEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
    #endif
}