using System;
using UnityEngine;

namespace VFX.Progressable
{
    /// <summary>
    /// 使用此轮子，可以便捷地动画驱动任何材质上的Color的透明度
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasAlphaProgressable : Versee.GameFramework.Progressable
    {
        [Range(0, 1)]
        public float progress;
        public override float Progress { get => progress; set => progress = value; }
        
        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);
    
        private CanvasGroup canvasGroup;

        private void Update()
        {
            if (canvasGroup == null && TryGetComponent<CanvasGroup>(out var c)) canvasGroup = c;
            if (canvasGroup == null) return;
            
            canvasGroup.alpha = Curve.Evaluate(progress);

            if (canvasGroup.alpha < 0.99f) canvasGroup.interactable = false;
            else canvasGroup.interactable = true;
        }
    }
}