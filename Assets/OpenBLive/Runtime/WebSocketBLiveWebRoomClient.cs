using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenBLive.Runtime.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Threading.Tasks;
using OpenBLive.Runtime.Data;
using NativeWebSocket;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Logger = OpenBLive.Runtime.Utilities.Logger;

namespace OpenBLive.Runtime
{
    public class WebSocketBLiveWebRoomClient : BLiveClient
    {

        /// <summary>
        ///  wss 长连地址
        /// </summary>
        public IList<string> WssLink;

        public WebSocket ws;

        public int roomId;

        public WebSocketBLiveWebRoomClient(IList<string> wssLink, string authBody, int roomId)
        {
            WssLink = wssLink;
            token = authBody;
            this.roomId = roomId;
        }

        protected override void OnOpen()
        {
            Debug.Log("WebRoom: OnOpen\n"+roomId+"\n"+token);

            SendAsync(Packet.PredefinedWebRoomAuthority(1768085830));
            //SendAsync(Packet.WebRoomAuthority(uid: 3546730259810411, roomid: roomId, token: token));

            m_Timer?.Dispose();
            m_Timer = new Timer((e) => (
                (WebSocketBLiveWebRoomClient)e)?.SendAsync(Packet.HeartBeat()), this, 0, 30 * 1000);
        }

        public override async void Connect()
        {
            var url = WssLink.FirstOrDefault();
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("wsslink is invalid");
            }
            
            //尝试释放已连接的ws
            if (ws != null && ws.State != WebSocketState.Closed)
            {
                await ws.Close();
            }

            ws = new WebSocket(url);
            ws.OnOpen += OnOpen;
            ws.OnMessage += data =>
            {
                Debug.Log("WebRoom: OnMessage");
                ProcessPacket(data);
            };
            ws.OnError += msg =>
            {
                Debug.Log("WebRoom: OnError");
                Logger.LogError("WebSocket Error Message: " + msg);
            };
            ws.OnClose += code =>
            {
                Debug.Log("WebRoom: OnClose");
                Logger.Log("WebSocket Close: " + code);
            };
            await ws.Connect();
        }



        public override async void Connect(TimeSpan timeSpan, int count)
        {
            //var url = WssLink.FirstOrDefault();
            var url = "wss://zj-cn-live-comet.chat.bilibili.com:2245/sub";
            if (string.IsNullOrEmpty(url))
            {
                throw new Exception("wsslink is invalid");
            }
            Debug.Log("WebRoom: Connecting "+url);
            
            //尝试释放已连接的ws
            if (ws != null && ws.State != WebSocketState.Closed)
            {
                await ws.Close();
            }

            ws = new WebSocket(url);
            ws.OnOpen += OnOpen;
            ws.OnMessage += data =>
            {
                //Debug.Log("WebRoom: OnMessage");
                ProcessPacket(data);
            };
            ws.OnError += msg =>
            {
                Debug.Log("WebRoom: OnError");
                Logger.LogError("WebSocket Error Message: " + msg);
            };
            ws.OnClose += code =>
            {
                Debug.Log("WebRoom: OnClose");
                Logger.Log("WebSocket Close: " + code);
            };
            await ws.Connect(timeSpan, count);
        }


        public override void Disconnect()
        {
            ws?.Close();
            ws = null;
        }

        public override void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(this);
        }

        public override void Send(byte[] packet)
        {
            if (ws.State == WebSocketState.Open)
            {
                ws.Send(packet);
            }
        }


        public override void Send(Packet packet) => Send(packet.ToBytes);
        public override Task SendAsync(byte[] packet) => Task.Run(() => Send(packet));
        protected override Task SendAsync(Packet packet) => SendAsync(packet.ToBytes);
    }
}