using UnityEngine;

namespace VFX.Progressable
{
    /// <summary>
    /// 使用此轮子，可以便捷地动画驱动任何材质上的Color的透明度
    /// </summary>
    [ExecuteInEditMode]
    public class PositionProgressable : Versee.GameFramework.Progressable
    {
        [Range(0, 1)]
        public float progress;
        public override float Progress { get => progress; set => progress = value; }

        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);

        public Vector3 StartRelativePosition = Vector3.zero;
        public Vector3 EndRelativePosition = Vector3.zero;
        
        private void Update()
        {
            Vector3 relativePosition = Vector3.Lerp(StartRelativePosition, EndRelativePosition, Curve.Evaluate(progress));
            transform.localPosition = relativePosition;
        }
    }
}