using System;
using UnityEngine;
using UnityEngine.UI;

namespace VFX.Progressable
{
    /// <summary>
    /// 使用此轮子，可以便捷地动画驱动任何材质上的Color的透明度
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAlphaProgressable : Versee.GameFramework.Progressable
    {
        [Range(0, 1)]
        public float progress;
        public override float Progress { get => progress; set => progress = value; }
        
        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);
    
        private SpriteRenderer image;

        private void Update()
        {
            if (image == null && TryGetComponent<SpriteRenderer>(out var c)) image = c;
            if (image == null) return;
            
            image.color = new Color(image.color.r, image.color.g, image.color.b, Curve.Evaluate(progress));
        }
    }
}