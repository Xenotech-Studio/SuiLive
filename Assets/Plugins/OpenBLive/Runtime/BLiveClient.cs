﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenBLive.Runtime.Data;
using OpenBLive.Runtime.Utilities;
using UnityEngine;

namespace OpenBLive.Runtime
{
    public delegate void ReceiveDanmakuEvent(Dm dm);

    public delegate void ReceiveGiftEvent(SendGift sendGift);

    public delegate void ReceiveGuardBuyEvent(Guard guard);

    public delegate void ReceiveSuperChatEvent(SuperChat e);

    public delegate void ReceiveSuperChatDelEvent(SuperChatDel e);

    public delegate void ReceiveLikeEvent(Like like);

    public delegate void ReceiveRawNotice(string raw, JObject jObject);
    
    public delegate void EnterRoomEvent(EnterRoom e);

    public delegate void FollowRoomEvent(InteractWord e);

    public abstract class BLiveClient : IDisposable
    {
        protected Timer m_Timer;
        protected string token;

        /// <summary>
        /// 弹幕数据
        /// </summary>
        public event ReceiveDanmakuEvent OnDanmaku;

        /// <summary>
        /// 赠送礼物
        /// </summary>
        public event ReceiveGiftEvent OnGift;

        public event ReceiveGuardBuyEvent OnGuardBuy;

        /// <summary>
        /// SC赠送
        /// </summary>
        public event ReceiveSuperChatEvent OnSuperChat;

        /// <summary>
        /// SC删除
        /// </summary>
        public event ReceiveSuperChatDelEvent OnSuperChatDel;

        /// <summary>
        /// 点赞信息
        /// </summary>
        public event ReceiveLikeEvent OnLike;

        /// <summary>
        /// 原始数据包
        /// </summary>
        public event ReceiveRawNotice ReceiveNotice;

        /// <summary>
        /// 更新人气
        /// </summary>
        public event EventHandler<int> UpdatePopularity;
        
        /// <summary>
        /// 进房
        /// </summary>
        public event EnterRoomEvent OnEnterRoom;
        
        /// <summary>
        /// 关注直播间
        /// </summary>
        public event FollowRoomEvent OnFollowRoom;

        public event EventHandler Open;
        public abstract void Connect();

        public abstract void Connect(TimeSpan timeSpan, int count);

        public abstract void Disconnect();
        public abstract void Dispose();
        public abstract void Send(byte[] packet);
        public abstract Task SendAsync(byte[] packet);
        public abstract void Send(Packet packet);
        protected abstract Task SendAsync(Packet packet);

        protected virtual void OnOpen()
        {
            SendAsync(Packet.Authority(token));

            m_Timer?.Dispose();
            m_Timer = new Timer((e) => (
                (BLiveClient)e)?.SendAsync(Packet.HeartBeat()), this, 0, 30 * 1000);
        }
        
        protected void ProcessPacket(ReadOnlySpan<byte> bytes) =>
            ProcessPacketAsync(new Packet(bytes));


        private async void ProcessPacketAsync(Packet packet)
        {
            var header = packet.Header;
            switch (header.ProtocolVersion)
            {
                case ProtocolVersion.UnCompressed:
                case ProtocolVersion.HeartBeat:
                    break;
                case ProtocolVersion.Zlib:
                    Debug.Log("WebRoom: Zlib Message");
                    //no Zlib compress in OpenBLive wss
                    //await foreach (var packet1 in ZlibDeCompressAsync(packet.PacketBody))
                    //ProcessPacketAsync(packet1);
                    return;
                case ProtocolVersion.Brotli:
                    //Debug.Log("WebRoom: Brotli Message " + header.Operation);
                    await foreach (var packet1 in BrotliDecompressAsync(packet.PacketBody))
                    {
                        ProcessPacketAsync(new Packet(packet1));
                    }
                    return;
                default:
                    throw new NotSupportedException(
                        "New bilibili danmaku protocol appears, please contact the author if you see this Exception.");
            }

            switch (header.Operation)
            {
                case Operation.AuthorityResponse:
                    Open?.Invoke(this, null);
                    break;
                case Operation.HeartBeatResponse:
                    Array.Reverse(packet.PacketBody);

                    var popularity = BitConverter.ToInt32(packet.PacketBody);

                    UpdatePopularity?.Invoke(this, popularity);
                    break;
                case Operation.ServerNotify:
                    ProcessNotice(Encoding.UTF8.GetString(packet.PacketBody));
                    break;
                // HeartBeat packet request, only send by client
                case Operation.HeartBeat:
                // This operation key only used for sending authority packet by client
                case Operation.Authority:
                default:
                    break;
            }
        }

        private void ProcessNotice(string rawMessage)
        {
            var json = JObject.Parse(rawMessage);
            
            ReceiveNotice?.Invoke(rawMessage, json);
            var data = json["data"]?.ToString();
            string cmd = json["cmd"]?.ToString();
            if (cmd.Contains("LIVE_OPEN"))
            {
                try
                {
                    //Debug.Log("**********" + json["cmd"]?.ToString());
                    switch (json["cmd"]?.ToString())
                    {
                        case "LIVE_OPEN_PLATFORM_DM":
                            var dm = JsonConvert.DeserializeObject<Dm>(data);
                            OnDanmaku?.Invoke(dm);
                            break;
                        case "LIVE_OPEN_PLATFORM_SUPER_CHAT":
                            var superChat = JsonConvert.DeserializeObject<SuperChat>(data);
                            OnSuperChat?.Invoke(superChat);
                            break;
                        case "LIVE_OPEN_PLATFORM_SUPER_CHAT_DEL":
                            var superChatDel = JsonConvert.DeserializeObject<SuperChatDel>(data);
                            OnSuperChatDel?.Invoke(superChatDel);
                            break;
                        case "LIVE_OPEN_PLATFORM_SEND_GIFT":
                            var gift = JsonConvert.DeserializeObject<SendGift>(data);
                            OnGift?.Invoke(gift);
                            break;
                        case "LIVE_OPEN_PLATFORM_GUARD":
                            var guard = JsonConvert.DeserializeObject<Guard>(data);
                            OnGuardBuy?.Invoke(guard);
                            break;
                        case "LIVE_OPEN_PLATFORM_LIKE":
                            var like = JsonConvert.DeserializeObject<Like>(data);
                            OnLike?.Invoke(like);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Utilities.Logger.LogError("json数据解析异常 rawMessage: " + rawMessage + e.Message);
                }
            }
            else
            {
                Debug.Log("WebRoom: " + json["cmd"]?.ToString() + ": " + json);
                try
                {
                    switch (cmd)
                    {
                        case "INTERACT_WORD":
                            var interactWord = JsonConvert.DeserializeObject<InteractWord>(data);
                            //Debug.Log("IS ENTER ROOM: "+interactWord.IsEnterRoom + interactWord.msg_type);
                            if (interactWord.IsEnterRoom) OnEnterRoom?.Invoke(interactWord.ToEnterRoom());
                            else OnFollowRoom?.Invoke(interactWord);
                            break;
                        case "ENTRY_EFFECT":
                            var entranceEffect = JsonConvert.DeserializeObject<EntranceEffect>(data);
                            OnEnterRoom?.Invoke(entranceEffect.ToEnterRoom());
                            break;
                    }
                }
                catch (Exception e)
                {
                    Utilities.Logger.LogError("json数据解析异常 rawMessage: " + rawMessage + e.Message);
                }
            }
        }
        
        async IAsyncEnumerable<byte[]> BrotliDecompressAsync(byte[] compressedData)
        {
            using var compressedStream = new MemoryStream(compressedData);
            using var decompressor = new BrotliStream(compressedStream, CompressionMode.Decompress);

            var buffer = new byte[4096];
            int read;
            while ((read = await decompressor.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                byte[] data = new byte[read];
                Array.Copy(buffer, data, read);
                yield return data;
            }
        }
    }
}
