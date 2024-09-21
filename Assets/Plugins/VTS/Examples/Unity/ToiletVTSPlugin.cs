using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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


		private void Awake() {
			Connect();
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
