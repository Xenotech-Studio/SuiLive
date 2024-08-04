using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if UNITY_2021_2_OR_NEWER || NET5_0_OR_GREATER
using System.Buffers;
#else
using System.Net;
#endif


namespace OpenBLive.Runtime.Data
{
    public partial struct Packet
    {
        /*
        /// <summary>
        /// 生成验证用数据包
        /// </summary>
        /// <param name="token">http请求获取的token</param>
        /// <param name="protocolVersion">协议版本</param>
        /// <returns>验证请求数据包</returns>
        public static Packet Authority(string token,
           ProtocolVersion protocolVersion = ProtocolVersion.Brotli)
        {
           var obj = Encoding.UTF8.GetBytes(token);

           return new Packet
           {
               Header = new PacketHeader
               {
                   Operation = Operation.Authority,
                   ProtocolVersion = ProtocolVersion.HeartBeat,
                   SequenceId = 1,
                   HeaderLength = PacketHeader.KPacketHeaderLength,
                   PacketLength = PacketHeader.KPacketHeaderLength + obj.Length
               },
               PacketBody = obj
           };
        }
        */

        
        public static Packet WebRoomAuthority(int uid, int roomid, string token="",
            ProtocolVersion protocolVersion = ProtocolVersion.Brotli)
        {
            var authInfo = new
            {
                uid = uid,
                roomid = roomid,
                protover = (int)protocolVersion,
                platform = "web",
                type = 2,
                key = token
            };

            string json = JsonConvert.SerializeObject(authInfo);
            var obj = Encoding.UTF8.GetBytes(json);

            return new Packet
            {
                Header = new PacketHeader
                {
                    Operation = Operation.Authority,
                    ProtocolVersion = ProtocolVersion.HeartBeat,
                    SequenceId = 1,
                    HeaderLength = PacketHeader.KPacketHeaderLength,
                    PacketLength = PacketHeader.KPacketHeaderLength + obj.Length
                },
                PacketBody = obj
            };
        }
    }
}