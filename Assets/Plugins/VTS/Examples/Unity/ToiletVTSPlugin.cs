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

	public partial class ToiletVTSPlugin : UnityVTSPlugin
	{

		public ToiletConfigManagerBase Config;
		
		
		
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

		public AudioClip CloseAudio;
		public AudioClip OpenAudio;
		public AudioClip FlushAudio;
		public AudioClip BubbleAudio;
		
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
		
		public Vector2 ToiletPosition => toiletPosition + new Vector2(Config.GetToiletPositionX(), Config.GetToiletPositionY());
		public float ToiletSize => toiletSize + Config.GetToiletSize();
		
		
		[Header("Live2D Model")]
		public Vector2 modelPosition = new Vector2(0, 0);
		public float modelSize = -40;
		public float modelRotation = 0;

		public Vector2 ModelPosition => modelPosition 
		                                + new Vector2(Config.GetToiletPositionX(), Config.GetToiletPositionY())
		                                + new Vector2(0, Config.GetModelPositionY());
		public float ModelSize => modelSize + Config.GetModelSize();
		
		

		private void OnEnable() {
			Connect();
		}

		private void OnDisable()
		{
			Disconnect();
		}


		private void Update()
		{
			UpdateGiftTimer();
			
			UpdateToilet();
			
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
		
		private bool closedLastFrame = false;

		private void FixedUpdate() {

			if(Closed && !closedLastFrame && CloseAudio!=null) AudioSource.PlayClipAtPoint(CloseAudio, Vector3.zero);
			if (!Closed && closedLastFrame && CloseAudio != null) AudioSource.PlayClipAtPoint(OpenAudio, Vector3.zero);
			closedLastFrame = Closed;


			if (randomFloatingTimer <= 0)
			{

				if (this.IsAuthenticated && this._modelMoving)
				{
					MoveModel(new VTSMoveModelData.Data()
					{
						positionX = ModelPosition.x,
						positionY = ModelPosition.y -
						            Progress * Progress * Progress * Progress * Progress * Progress * 1f,
						size = ModelSize - Progress * 20f,
						rotation = modelRotation + RotationAngle *
							(rotating ? (float)Math.Sin(Progress * Progress * RotationSpeed) : 0f)
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
								positionX = ToiletPosition.x,
								positionY = ToiletPosition.y,
								size = ToiletSize,
								rotation = toiletRotation,
								order = 5,
							}
						},
						new VTSItemMoveEntry()
						{
							itemInsanceID = ToiletBackInstanceId,
							options = new VTSItemMoveOptions()
							{
								positionX = ToiletPosition.x,
								positionY = ToiletPosition.y,
								size = ToiletSize,
								rotation = toiletRotation,
								order = ToiletBackOrder,
							}
						},
						new VTSItemMoveEntry()
						{
							itemInsanceID = ToiletLidInstanceId,
							options = new VTSItemMoveOptions()
							{
								positionX = ToiletPosition.x,
								positionY = ToiletPosition.y,
								size = ToiletSize * (Closed ? 1 : 0.1f),
								rotation = toiletRotation,
								order = Closed ? ToiletLidOrderClosed : ToiletLidOrderOpened,
							}
						},
					});
				}
			}
			else if (this.IsAuthenticated && this._modelMoving)
			{
				
				randomFloatingTimer -= Time.fixedDeltaTime;
				
				position += speed * Time.fixedDeltaTime;
				rotation += angularSpeed * Time.fixedDeltaTime;
				

				if (position.x < -1f || position.x > 1f)
				{
					speed.x = -speed.x;
					float newAngularSpeed = angularSpeed - Math.Sign(angularSpeed) * speed.magnitude * 360;
					newAngularSpeed = Math.Abs(newAngularSpeed) > Math.Abs(angularSpeed)/2 ? newAngularSpeed : -angularSpeed;
					angularSpeed = newAngularSpeed;
				}

				if (position.y < -1 || position.y > 1)
				{
					speed.y = -speed.y;
					float newAngularSpeed = angularSpeed - Math.Sign(angularSpeed) * speed.magnitude * 360;
					newAngularSpeed = Math.Abs(newAngularSpeed) > Math.Abs(angularSpeed)/2 ? newAngularSpeed : -angularSpeed;
					angularSpeed = newAngularSpeed;
				}
				
				
				
				MoveModel(new VTSMoveModelData.Data()
				{
					positionX = position.x,
					positionY = position.y,
					size = -93f,
					rotation = rotation % 360
				});
			}
		}
		
		[Header("Floating")]

		public float RandomFloatingDuration = 60;
		public float randomFloatingTimer = 0;
		
		Vector2 speed = new Vector2(0, 0);
		float angularSpeed = 0;
		
		Vector2 position = new Vector2(0, 0);
		float rotation = 0;
		
		public void StartRandomFloating()
		{
			randomFloatingTimer = RandomFloatingDuration;
			speed = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * 0.3f;
			angularSpeed = UnityEngine.Random.Range(-90f, 90f);
		}
	}
}

#if UNITY_EDITOR
namespace VTS.Unity.Examples {
	public partial class ToiletVTSPlugin : UnityVTSPlugin
	{
		[UnityEditor.CustomEditor(typeof(ToiletVTSPlugin))]
		public class ToiletVTSPluginEditor : UnityEditor.Editor
		{
			public override void OnInspectorGUI()
			{
				DrawDefaultInspector();
				ToiletVTSPlugin myScript = (ToiletVTSPlugin)target;
				if (GUILayout.Button("Start Random Floating"))
				{
					myScript.StartRandomFloating();
				}
				if (GUILayout.Button("Speed Up"))
				{
					myScript.ReceiveDanmaku(new Dm(){msg = "加速"});
				}
				if (GUILayout.Button("Speed Down"))
				{
					myScript.ReceiveDanmaku(new Dm(){msg = "停"});
				}
			}
		}
	}
}
#endif
