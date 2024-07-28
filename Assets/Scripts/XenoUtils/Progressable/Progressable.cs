using System;
using System.Collections;
using UnityEngine;


namespace Versee.GameFramework
{
    
    [ExecuteInEditMode]
    public abstract class Progressable : MonoBehaviour, IProgressable
    { 
        public abstract float Progress { get; set; }

        private Coroutine CurrentCoroutine = null;

        public void LinearTransition(float time)
        {
            if (CurrentCoroutine != null) StopCoroutine(CurrentCoroutine);
            CurrentCoroutine = StartCoroutine(LinearTransitionCoroutine(time, 1, null));
        }

        public void LinearTransition(float time, float targetProgress, Action callback)
        {
            if (CurrentCoroutine != null) StopCoroutine(CurrentCoroutine);
            CurrentCoroutine = StartCoroutine(LinearTransitionCoroutine(time, targetProgress, callback));
        }
        
        private IEnumerator LinearTransitionCoroutine(float time, float targetProgress, Action callback)
        {
            float startProgress = Progress;
            float startTime = Time.time;
            float endTime = startTime + time;
            
            while (Time.time < endTime)
            {
                Progress = Mathf.Lerp(startProgress, targetProgress, (Time.time - startTime) / time);
                yield return null;
            }
            
            Progress = targetProgress;
            callback?.Invoke();
        }
        
        public void InverseLinearTransition(float time)
        {
            if (CurrentCoroutine != null) StopCoroutine(CurrentCoroutine);
            CurrentCoroutine = StartCoroutine(InverseLinearTransitionCoroutine(time, 0, null));
        }
        
        public void InverseLinearTransition(float time, float targetProgress, Action callback)
        {
            if (CurrentCoroutine != null) StopCoroutine(CurrentCoroutine);
            CurrentCoroutine = StartCoroutine(InverseLinearTransitionCoroutine(time, targetProgress, callback));
        }
        
        private IEnumerator InverseLinearTransitionCoroutine(float time, float targetProgress, Action callback)
        {
            float startProgress = Progress;
            float startTime = Time.time;
            float endTime = startTime + time;
            
            while (Time.time < endTime)
            {
                Progress = Mathf.Lerp(startProgress, targetProgress, (Time.time - startTime) / time);
                yield return null;
            }
            
            Progress = targetProgress;
            callback?.Invoke();
        }
        
    }
}