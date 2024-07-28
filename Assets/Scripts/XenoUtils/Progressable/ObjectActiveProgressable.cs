using System;
using UnityEngine;

namespace VFX.Progressable
{
    [ExecuteInEditMode]
    public class ObjectActiveProgressable : Versee.GameFramework.Progressable
    {
        [Range(0, 1)]
        public float progress;
        public override float Progress { get => progress; set => progress = value; }
        
        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);
    
        public GameObject GameObject;

        private void Update()
        {
            if (GameObject == null) return;
            
            if (Progress < 0.5f) GameObject.SetActive(false);
            else GameObject.SetActive(true);
        }
    }
}