using System;
using System.Collections;
using System.Collections.Generic;
using OpenBLive.Runtime.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using VTS.Core;

namespace VTS.Unity.Examples {

	public class ToiletVTSPlugin : UnityVTSPlugin {
		
		[Header("Controls")]
		[Range(0, 1)]
		public float Progress = 0.0f;
		public bool Closed = false;
		private bool _modelMoving = true;
		
		[Header("Assets")]
		
		[SerializeField]
		private Image _connectionLight = null;
		[SerializeField]
		private TMP_Text _connectionText = null;

		
		public Texture2D ToiletFrontTexure;
		public Texture2D ToiletBackTexure;
		public Texture2D ToiletLidTexure;
		
		private int ToiletFrontOrder = 5;
		private int ToiletBackOrder = -5;
		private int ToiletLidOrderOpened = -6;
		private int ToiletLidOrderClosed = 6;

		private bool _toiletFrontReady = false;
		private bool _toiletBackReady = false;
		private bool _toiletLidReady = false;
		
		private string ToiletFrontInstanceId;
		private string ToiletBackInstanceId;
		private string ToiletLidInstanceId;
		
		
		[Header("Toilet")]
		public Vector2 toiletPosition = new Vector2(0, 0);
		public float toiletSize = 0;
		public float toiletRotation = 0;
		
		
		[Header("Live2D Model")]
		public Vector2 modelPosition = new Vector2(0, 0);
		public float modelSize = -40;
		public float modelRotation = 0;
		
		// 冲水
		private bool flushing = false;
		public float FlushingTime = 3;
		public float RescueTime = 5;

		// 冲水后回复前的冷却时间
		public float afterFlushTimer = 0;
		public float AfterFlushTime = 20;

		// 爬回来的过程
		public float recoverIntervalTimer = 0f;
		public float RecoverInterval = 5f;
		public float PerRecoverAmount = 0.05f;
		
		// 触发机制：计数器和计时器
		public float flushTriggerCounter = 0;
		public float flushTriggerTimeout = 5;
		private float flushTriggerTimer = 0;
		
		public float rescueTriggerCounter = 0;
		public float rescueTriggerTimeout = 5;
		private float rescueTriggerTimer = 0;
		

		private void Awake() {
			Connect();
		}

		public void ReceiveGift(SendGift gift)
		{
			Debug.Log("Gift ID "+gift.giftId);
			if (gift.giftId == 31036)
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

		private void Update()
		{
			UpdateGiftTimer();
			
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
			
			// Debug Test:
			if (Input.GetKeyDown(KeyCode.Space))
			{
				Flush();
			}

			if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space))
			{
				Rescue();
			}
		}
		
		
		// 冲水
		private Coroutine RunningFlushCoroutine = null;
		public void Flush()
		{
			if (RunningFlushCoroutine != null) return;
			if (RunningRescueCoroutine != null) StopCoroutine(RunningRescueCoroutine);
			RunningFlushCoroutine = StartCoroutine(FlushCoroutine());
		}
		public IEnumerator FlushCoroutine()
		{
			afterFlushTimer = AfterFlushTime;
			flushing = true;
			float timer = 0f;
			while (timer < FlushingTime)
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
			if (RunningRescueCoroutine != null) return;
			if (RunningFlushCoroutine != null)
			{
				StopCoroutine(RunningFlushCoroutine);
				flushing = false;
			}
			RunningRescueCoroutine = StartCoroutine(RescueCoroutine());
		}
		public IEnumerator RescueCoroutine(float amount=1, float time=0)
		{
			if (time <= 0.1f) time = RescueTime;
			float timer = 0f;
			while (timer < time)
			{
				yield return null;
				if(Closed) continue;
				timer += Time.deltaTime;
				Progress -= amount * Time.deltaTime / time;
				
			}
			if(amount>= 0.9f) Progress = 0;
			RunningRescueCoroutine = null;
		}
		
		
		

		public void Connect() {
			this._connectionLight.color = Color.yellow;
			this._connectionText.text = "Connecting...";
			Initialize(new WebSocketSharpImpl(this.Logger), new NewtonsoftJsonUtilityImpl(), new TokenStorageImpl(Application.persistentDataPath),
			() => {
				this.Logger.Log("Connected!");
				this._connectionLight.color = Color.green;
				this._connectionText.text = "Connected!";
				
				LoadToilet();
			},
			() => {
				this.Logger.LogWarning("Disconnected!");
				this._connectionLight.color = Color.gray;
				this._connectionText.text = "Disconnected.";
			},
			(error) => {
				this.Logger.LogError("Error! - " + error.data.message);
				this._connectionLight.color = Color.red;
				this._connectionText.text = "Error!";
			});
		}
		
		public void ToggleModelMoving()
		{
			this._modelMoving = !this._modelMoving;
		}

		public void LoadToilet()
		{
			RequestPermission(VTSPermission.LoadCustomImagesAsItems, onSuccess: (VTSPermissionResponseData r) =>
			{
				// convert textures to base64
				string frontBase64 = Convert.ToBase64String(ToiletFrontTexure.EncodeToPNG());
				string backBase64 = Convert.ToBase64String(ToiletBackTexure.EncodeToPNG());
				string lidBase64 = Convert.ToBase64String(ToiletLidTexure.EncodeToPNG());
			
				LoadCustomDataItem(
					fileName: "ToiletFront.png", 
					base64: frontBase64, 
					options: new VTSCustomDataItemLoadOptions()
					{
						order = ToiletFrontOrder,
						unloadWhenPluginDisconnects = true,
					},
					onSuccess: (VTSItemLoadResponseData d) =>
					{
						ToiletFrontInstanceId = d.data.instanceID;
						_toiletFrontReady = true;
					},
					onError: (VTSErrorData e) =>
					{
						Debug.LogError("Error: " + e.data.message);
					}
				);
			
				LoadCustomDataItem(
					fileName: "ToiletBack.png", 
					base64: backBase64, 
					options: new VTSCustomDataItemLoadOptions()
					{
						order = ToiletBackOrder,
						unloadWhenPluginDisconnects = true,
					},
					onSuccess: (VTSItemLoadResponseData d) =>
					{
						ToiletBackInstanceId = d.data.instanceID;
						_toiletBackReady = true;
					},
					onError: (VTSErrorData e) =>
					{
						Debug.LogError("Error: " + e.data.message);
					}
				);
			
				LoadCustomDataItem(
					fileName: "ToiletLid.png", 
					base64: lidBase64, 
					options: new VTSCustomDataItemLoadOptions()
					{
						order = Closed ? ToiletLidOrderClosed : ToiletLidOrderOpened,
						unloadWhenPluginDisconnects = true,
					},
					onSuccess: (VTSItemLoadResponseData d) =>
					{
						ToiletLidInstanceId = d.data.instanceID;
						_toiletLidReady = true;
					},
					onError: (VTSErrorData e) =>
					{
						Debug.LogError("Error: " + e.data.message);
					}
				);
			}, onError: (e=> Debug.LogError(e.data.message)));
		}

		private void FixedUpdate() {

			
			if (this.IsAuthenticated && this._modelMoving) {
				MoveModel(new VTSMoveModelData.Data()
				{
					positionX = modelPosition.x,
					positionY = modelPosition.y - Progress * 1f,
					size = modelSize - Progress * 10f,
					rotation = modelRotation
				});
			}

			if (this.IsAuthenticated && this._toiletFrontReady && this._toiletBackReady && this._toiletLidReady)
			{

				MoveItem(new VTSItemMoveEntry[]
				{
					new VTSItemMoveEntry()
					{
						itemInsanceID = ToiletFrontInstanceId,
						options = new VTSItemMoveOptions()
						{
							positionX = toiletPosition.x,
							positionY = toiletPosition.y,
							size = toiletSize,
							rotation = toiletRotation,
							order = 5,
						}
					},
					new VTSItemMoveEntry()
					{
						itemInsanceID = ToiletBackInstanceId,
						options = new VTSItemMoveOptions()
						{
							positionX = toiletPosition.x,
							positionY = toiletPosition.y,
							size = toiletSize,
							rotation = toiletRotation,
							order = ToiletBackOrder,
						}
					},
					new VTSItemMoveEntry()
					{
						itemInsanceID = ToiletLidInstanceId,
						options = new VTSItemMoveOptions()
						{
							positionX = toiletPosition.x,
							positionY = toiletPosition.y,
							size = toiletSize * (Closed? 1 : 0.1f),
							rotation = toiletRotation,
							order = Closed ? ToiletLidOrderClosed : ToiletLidOrderOpened,
						}
					},
				});
			}
		}
	}
}
