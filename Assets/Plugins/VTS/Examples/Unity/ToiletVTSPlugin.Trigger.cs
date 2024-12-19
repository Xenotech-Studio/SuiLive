using System;
using System.Collections;
using System.Collections.Generic;
using OpenBLive.Runtime.Data;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using VTS.Core;

namespace VTS.Unity.Examples {

	public partial class ToiletVTSPlugin : UnityVTSPlugin {
		
		
		// 触发机制：计数器和计时器
		private float flushTriggerCounter = 0;
		[Header("触发参数")] public float flushTriggerTimeout = 5;
		private float flushTriggerTimer = 0;
		
		private float rescueTriggerCounter = 0;
		public float rescueTriggerTimeout = 5;
		private float rescueTriggerTimer = 0;
		
		
		public void ReceiveGift(SendGift gift)
		{
			Debug.Log("Gift ID "+gift.giftId);
			//if (gift.giftId == 31036)
			if (gift.giftId == 33988)
			{
				flushTriggerCounter += gift.giftNum;
				flushTriggerTimer = flushTriggerTimeout;
				if ( (flushTriggerCounter) >= 50 )
				{
					if (!flushing)
					{
						Debug.Log("Flush!");
						flushTriggerCounter = 0;
						Flush();
					}
					// else: ignore for now, but preserve the counter
				}
			}

			if (gift.giftId == 32760)
			{
				rescueTriggerCounter += gift.giftNum;
				flushTriggerTimer = rescueTriggerTimeout;
				if ( (rescueTriggerCounter) >= 50 )
				{
					if (!Closed)
					{
						Debug.Log("Rescue!");
						rescueTriggerCounter = 0;
						Rescue();
					}
					// else: ignore for now, but preserve the counter
				}
			}

			if (gift.giftId == 31088)
			{
				StartRandomFloating();
			}
		}
		
		public void ReceiveDanmaku(Dm danmaku)
		{
			if (!gameObject.activeSelf) return;
			
			Debug.Log(danmaku.msg);
			
			if (danmaku.msg.Contains("加速")||danmaku.msg.Contains("冲"))
			{
				if (speed.magnitude < 5)
				{
					speed *= 2;
				}
				else
				{
					angularSpeed *= 2;
				}
			}

			if (danmaku.msg.Contains("停")||danmaku.msg=="定")
			{
				if (speed.magnitude > 0.3)
				{
					speed = speed.normalized * 0.3f;
				}

				angularSpeed = Math.Sign(angularSpeed) * 20;
			}
		}

		private void UpdateGiftTimer()
		{
			if(flushTriggerTimer>0 && flushTriggerCounter>0)
			{
				flushTriggerTimer -= Time.deltaTime;
			}
			else
			{
				flushTriggerCounter = 0;
			}
			
			if (rescueTriggerTimer > 0 && rescueTriggerCounter > 0)
			{
				rescueTriggerTimer -= Time.deltaTime;
			}
			else
			{
				rescueTriggerCounter = 0;
			}
		}
	}
}
