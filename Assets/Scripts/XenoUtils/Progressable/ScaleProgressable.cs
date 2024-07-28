using UnityEngine;

namespace VFX.Progressable
{
    /// <summary>
    /// 使用此轮子，可以便捷地动画驱动任何材质上的Color的透明度
    /// </summary>
    [ExecuteInEditMode]
    public class ScaleProgressable : Versee.GameFramework.Progressable
    {
        [Range(0, 1)]
        public float progress;
        public override float Progress { get => progress; set => progress = value; }

        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);

        public Vector3 StartRelativeScale = Vector3.one;
        public Vector3 EndRelativeScale = Vector3.one;
        private void Update()
        {
            Vector3 relativeScale = Vector3.Lerp(StartRelativeScale, EndRelativeScale, Curve.Evaluate(progress));
            transform.localScale = relativeScale;
        }
    }
}