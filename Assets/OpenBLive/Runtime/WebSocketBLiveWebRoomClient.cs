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
            
            var data = new byte[] {
                0x00, 0x00, 0x01, 0x48, 0x00, 0x10, 0x00, 0x01,
                0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x01,
                // 接下来的部分是JSON字符串的十六进制表示
                0x7b, 0x22, 0x75, 0x69, 0x64, 0x22, 0x3a, 0x30, 0x2c, 0x22, 0x72, 0x6f, 0x6f, 0x6d, 0x69, 0x64, 0x22, 0x3a,
                0x32, 0x34, 0x39, 0x31, 0x30, 0x33, 0x32, 0x39, 0x2c, 0x22, 0x70, 0x72, 0x6f, 0x74, 0x6f, 0x76, 0x65, 0x72,
                0x22, 0x3a, 0x33, 0x2c, 0x22, 0x62, 0x75, 0x76, 0x69, 0x64, 0x22, 0x3a, 0x22, 0x31, 0x33, 0x37, 0x36, 0x46,
                0x31, 0x30, 0x35, 0x2d, 0x39, 0x39, 0x36, 0x45, 0x2d, 0x43, 0x38, 0x35, 0x36, 0x2d, 0x32, 0x36, 0x38, 0x36,
                0x2d, 0x46, 0x45, 0x36, 0x35, 0x30, 0x30, 0x43, 0x36, 0x44, 0x45, 0x39, 0x45, 0x35, 0x33, 0x38, 0x34, 0x36,
                0x69, 0x6e, 0x66, 0x6f, 0x63, 0x22, 0x2c, 0x22, 0x70, 0x6c, 0x61, 0x74, 0x66, 0x6f, 0x72, 0x6d, 0x22, 0x3a,
                0x22, 0x77, 0x65, 0x62, 0x22, 0x2c, 0x22, 0x74, 0x79, 0x70, 0x65, 0x22, 0x3a, 0x32, 0x2c, 0x22, 0x6b, 0x65,
                0x79, 0x22, 0x3a, 0x22, 0x55, 0x50, 0x4d, 0x37, 0x4c, 0x77, 0x49, 0x61, 0x39, 0x35, 0x54, 0x6e, 0x65, 0x42,
                0x56, 0x75, 0x39, 0x35, 0x31, 0x49, 0x66, 0x78, 0x42, 0x33, 0x72, 0x4a, 0x7a, 0x32, 0x75, 0x59, 0x79, 0x69,
                0x6c, 0x34, 0x64, 0x46, 0x58, 0x78, 0x52, 0x33, 0x65, 0x49, 0x56, 0x5f, 0x49, 0x6a, 0x67, 0x77, 0x77, 0x36,
                0x6a, 0x69, 0x45, 0x77, 0x72, 0x6f, 0x5a, 0x4f, 0x47, 0x30, 0x32, 0x77, 0x4b, 0x6f, 0x69, 0x6a, 0x53, 0x76,
                0x36, 0x47, 0x46, 0x4f, 0x38, 0x49, 0x41, 0x68, 0x5a, 0x61, 0x79, 0x4d, 0x61, 0x56, 0x49, 0x79, 0x6b, 0x48,
                0x59, 0x6c, 0x6d, 0x33, 0x30, 0x50, 0x38, 0x32, 0x66, 0x36, 0x4c, 0x31, 0x6a, 0x5a, 0x36, 0x79, 0x62, 0x6c,
                0x6d, 0x38, 0x64, 0x46, 0x4f, 0x69, 0x5f, 0x4b, 0x54, 0x32, 0x5a, 0x51, 0x7a, 0x6a, 0x6a, 0x78, 0x68, 0x57,
                0x68, 0x79, 0x36, 0x52, 0x54, 0x78, 0x30, 0x48, 0x5a, 0x4c, 0x49, 0x71, 0x62, 0x68, 0x74, 0x31, 0x48, 0x47,
                0x66, 0x35, 0x4f, 0x6e, 0x6a, 0x65, 0x6c, 0x38, 0x42, 0x4e, 0x36, 0x7a, 0x55, 0x41, 0x6c, 0x39, 0x68, 0x66,
                0x4f, 0x6a, 0x62, 0x64, 0x62, 0x6a, 0x43, 0x45, 0x79, 0x31, 0x47, 0x79, 0x4a, 0x50, 0x45, 0x69, 0x35, 0x72,
                0x37, 0x50, 0x73, 0x3d, 0x22, 0x7d
            };

            SendAsync(data);
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