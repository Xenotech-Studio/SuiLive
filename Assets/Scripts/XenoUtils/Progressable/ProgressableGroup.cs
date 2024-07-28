using System;
using System.Collections.Generic;
using UnityEngine;

namespace Other.Progressable
{
    [ExecuteInEditMode]
    public class ProgressableGroup : Versee.GameFramework.Progressable
    {
        [Range(0, 1)]
        public float progress;
        public override float Progress { get => progress; set => progress = value; }
        
        public List<Versee.GameFramework.Progressable> Progressables = new List<Versee.GameFramework.Progressable>();

        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);

        public bool UseCurve
        {
            get => useCurve;
            set => useCurve = value;
        }
        public bool useCurve = true;

        private void Update()
        {
            foreach (var progressable in Progressables)
            {
                if (progressable != null)
                {
                    progressable.Progress = UseCurve ? Curve.Evaluate(Progress) : Progress;
                }
            }
        }
    }
}