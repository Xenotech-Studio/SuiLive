using UnityEngine;
using Versee.GameFramework;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class MR_ShaderVar : Progressable
{
    public override float Progress { get => Bianyuan_UD; set => Bianyuan_UD = value; }
    
    private bool beforeTransition = true;
    
    bool Switch_Scene;
    float f_Switch_Scene;

    [Range(-1, 1)] public float Bianyuan_LR;
    [Range(-0.1f, 0.1f)] public float Bianyuan_lizi;
    [Range(0.1f, 0.9f)] public float Bianyuan_UD;

    public bool HoldProgress = false;

    float time1 = 0;
    float time2 = 0;

    private void Awake()
    {
        if(!HoldProgress) Bianyuan_UD = 0.1f;
    }


    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalFloat("Bianyuan_LR", Bianyuan_LR);
        Shader.SetGlobalFloat("Bianyuan_lizi", Bianyuan_lizi);
        Shader.SetGlobalFloat("Bianyuan_UD", Bianyuan_UD);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MR_ShaderVar))]
public class MR_ShaderVarEditor : Editor{
    public override void OnInspectorGUI()
    {
        MR_ShaderVar comp = (MR_ShaderVar)target;
        Shader.SetGlobalFloat("Bianyuan_LR", comp.Bianyuan_LR);
        Shader.SetGlobalFloat("Bianyuan_lizi", comp.Bianyuan_lizi);
        DrawDefaultInspector();
    }
}
#endif