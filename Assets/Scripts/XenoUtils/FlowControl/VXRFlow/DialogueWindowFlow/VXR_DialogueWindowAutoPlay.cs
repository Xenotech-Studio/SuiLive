#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Versee.UI
{
    public class VXR_DialogueWindowAutoPlay : VXR_FlowItem
    {
        public float Life = 3f;
        private float _remainingTime;
        private bool _shown;

        public void OnEnable()
        {
            _remainingTime = Life;
            _shown = true;
        }

        public void Update()
        {
            if (_shown)
            {
                _remainingTime -= Time.deltaTime;
                if (_remainingTime <= 0)
                {
                    _remainingTime = 0;
                    _shown = false;
                    GetComponentInParent<VXR_DialogueWindowSequence>().Next();
                }
            }
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(VXR_DialogueWindowAutoPlay))]
    public class VXR_DialogueWindowAutoPlayEditor : VXR_FlowItemEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
    #endif
}