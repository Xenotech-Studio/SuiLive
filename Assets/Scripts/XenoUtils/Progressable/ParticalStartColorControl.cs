using System;
using UnityEngine;

namespace Other.PortalBall
{
    /// <summary>
    /// 使用此轮子，可以便捷地动画驱动任何材质上的Color的透明度
    /// </summary>
    [ExecuteInEditMode]
    public class ParticleStartColorControl : Versee.GameFramework.Progressable
    {
        [Range(0, 1)]
        public float progress;
        public override float Progress { get => progress; set => progress = value; }
        
        public ParticleSystem _particleSystem;
        public ParticleSystem ParticleSystem
        {
            get {
                if (_particleSystem == null) _particleSystem = GetComponent<ParticleSystem>();
                return _particleSystem;
            }
        }
        
        public Color StartColor = Color.white;
        public Color EndColor = Color.white;
        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);

        private void Update()
        {
            if (ParticleSystem == null) return;

            var mainModule = ParticleSystem.main;
            Color newColor = Color.Lerp(StartColor, EndColor, progress);
            mainModule.startColor = newColor;
        }
    }
}