using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json;
using OpenBLive.Runtime;
using OpenBLive.Runtime.Data;
using OpenBLive.Runtime.Utilities;
using UnityEngine;
using UnityEngine.Events;

public partial class ConnectViaCode : MonoBehaviour
{
    
    private void TestUpdate()
    {
        // F1 测试收弹幕
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Dm dm = new Dm()
            {
                uid = 1, openId = "openId", userName = "userName", userFace = "userFace", timestamp = 1, roomId = 1,
                fansMedalLevel = 1, fansMedalName = "fansMedalName", fansMedalWearingStatus = true, guardLevel = 1, 
                msg = "测试弹幕测试弹幕", 
            };
            WebSocketBLiveClientOnDanmaku(dm);
        }
        
        // F2 测试收长弹幕
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Dm dm = new Dm()
            {
                uid = 1, openId = "openId", userName = "userName", userFace = "userFace", timestamp = 1, roomId = 1,
                fansMedalLevel = 1, fansMedalName = "fansMedalName", fansMedalWearingStatus = true, guardLevel = 1, 
                msg = "测试弹幕测试弹幕测试弹幕测试弹幕测试弹幕测试弹幕测试", 
            };
            WebSocketBLiveClientOnDanmaku(dm);
        }
        
        // F3 测试小花花
        if (Input.GetKeyDown(KeyCode.F3))
        {
            SendGift sendGift = new SendGift()
            {
                roomId = 1, openId = "openId", userName = "userName", userFace = "userFace", giftId = 31036, giftName = "小花花",
                giftNum = 1, price = 100, paid = true,
            };
            WebSocketBLiveClientOnGift(sendGift);
        }
        
        // F4 测试100连小花花
        if (Input.GetKeyDown(KeyCode.F4))
        {
            SendGift sendGift = new SendGift()
            {
                roomId = 1, openId = "openId", userName = "userName", userFace = "userFace", giftId = 31036, giftName = "小花花",
                giftNum = 100, price = 10000, paid = true,
            };
            WebSocketBLiveClientOnGift(sendGift);
        }
            
        
        // F5 测试大航海
    }
}
