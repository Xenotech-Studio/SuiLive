using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

#if UNITY_2021_2_OR_NEWER || NET5_0_OR_GREATER
using System.Buffers;
#else
using System.Net;
#endif


namespace OpenBLive.Runtime.Data
{
    public partial struct Packet
    {
        public static Packet WebRoomAuthority(long uid=0, int roomid=0, string token="",
            ProtocolVersion protocolVersion = ProtocolVersion.Brotli)
        {
            var authInfo = new
            {
                uid = uid,
                roomid = roomid,
                protover = (int)protocolVersion,
                buvid = "",
                platform = "web",
                type = 2,
                key = token
            };
            
            Debug.Log("WebRoom: WebRoomAuthority\n"+JsonConvert.SerializeObject(authInfo));

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