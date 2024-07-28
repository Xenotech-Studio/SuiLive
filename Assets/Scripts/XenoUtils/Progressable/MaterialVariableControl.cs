using System;
using UnityEngine;

namespace Other.PortalBall
{
    /// <summary>
    /// 使用此轮子，可以便捷地动画驱动任何材质上的Color的透明度
    /// </summary>
    [ExecuteInEditMode]
    public class MaterialVariableControl : Versee.GameFramework.Progressable
    {
        [Range(0, 1)]
        public float progress;
        public override float Progress { get => progress; set => progress = value; }
        
        public Material Material;

        public string PropertyName = "_";
        
        public float StartValue = 0;
        public float EndValue = 1;
        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);

        private void Update()
        {
            if (Material == null) return;
            float newValue = StartValue + (EndValue - StartValue) * Curve.Evaluate(Progress);
            Material.SetFloat(PropertyName, newValue);
        }
    }
}