using System;
using System.Collections;
using System.Collections.Generic;
using OpenBLive.Runtime.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR;
using VTS.Core;

namespace VTS.Unity.Examples {

	public partial class ToiletVTSPlugin : UnityVTSPlugin {
		
		
		// 冲水
		private bool flushing = false;
		
		[Header("冲水动画相关参数")]
		public float FlushingTime = 3;
		public float RescueTime = 5;
		public float RotationSpeed = 1;
		public float RotationAngle = 2;

		// 冲水后回复前的冷却时间
		private float afterFlushTimer = 0;
		public float afterFlushTime => Config.GetAfterFlushTime();

		// 爬回来的过程
		private float recoverIntervalTimer = 0f;
		public float RecoverInterval = 5f;
		public float PerRecoverAmount = 0.05f;

		private bool rotating => flushing;
		
		
		public void UpdateToilet()
		{
			// Update Toilet:
			if(afterFlushTimer > 0)
			{
				afterFlushTimer -= Time.deltaTime;
			}
			else
			{
				if (!Closed && Progress>= 0.05f)
				{
					if (recoverIntervalTimer > 0)
					{
						recoverIntervalTimer -= Time.deltaTime;
					}
					else
					{
						StartCoroutine(RescueCoroutine(PerRecoverAmount, 0.5f));
						recoverIntervalTimer = RecoverInterval;
					}
				}
			}
		}
		
		
		// 冲水
		private Coroutine RunningFlushCoroutine = null;
		public void Flush()
		{
			if(!gameObject.activeSelf) return;
			
			if (RunningFlushCoroutine != null) return;
			if (RunningRescueCoroutine != null)
			{
				StopCoroutine(RunningRescueCoroutine);
				RunningRescueCoroutine = null;
			}
			if(FlushAudio) AudioSource.PlayClipAtPoint(FlushAudio, Vector3.zero);
			RunningFlushCoroutine = StartCoroutine(FlushCoroutine());
		}
		public IEnumerator FlushCoroutine()
		{
			afterFlushTimer = afterFlushTime;
			flushing = true;
			float timer = 0f;
			while (Progress<1f && timer < FlushingTime)
			{
				timer += Time.deltaTime;
				Progress += 1 * Time.deltaTime / FlushingTime;
				yield return null;
			}
			Progress = 1;
			RunningFlushCoroutine = null;
			flushing = false;
		}
		
		// 捞
		private Coroutine RunningRescueCoroutine = null;
		public void Rescue()
		{
			if(!gameObject.activeSelf) return;
			
			if (RunningRescueCoroutine != null) return;
			if (RunningFlushCoroutine != null)
			{
				StopCoroutine(RunningFlushCoroutine);
				RunningFlushCoroutine = null;
				flushing = false;
			}
			RunningRescueCoroutine = StartCoroutine(RescueCoroutine());
		}
		public IEnumerator RescueCoroutine(float amount=1, float time=0)
		{
			if (time <= 0.1f) time = RescueTime;
			float timer = 0f;
			while (Progress > 0f && timer < time)
			{
				yield return null;
				if(Closed) continue;
				timer += Time.deltaTime;
				Progress -= amount * Time.deltaTime / time;
				
			}
			if(amount>= 0.9f) Progress = 0;
			RunningRescueCoroutine = null;
		}
	}
}
