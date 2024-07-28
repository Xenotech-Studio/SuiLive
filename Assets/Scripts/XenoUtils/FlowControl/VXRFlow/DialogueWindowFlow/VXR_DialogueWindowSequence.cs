#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Versee.UI
{
    public class VXR_DialogueWindowSequence : VXR_Flow
    {
        
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(VXR_DialogueWindowSequence))]
    public class VXR_DialogueWindowSequenceEditor : VXR_FlowEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
    #endif
}