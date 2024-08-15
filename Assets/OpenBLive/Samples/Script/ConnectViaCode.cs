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
    public static ConnectViaCode Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ConnectViaCode>();
            }
            return instance;
        }
    }
    private static ConnectViaCode instance;
    // Start is called before the first frame update
    private WebSocketBLiveClient m_WebSocketBLiveClient;
    private WebSocketBLiveWebRoomClient m_WebSocketBLiveWebRoomClient;
    private InteractivePlayHeartBeat m_PlayHeartBeat;
    private string gameId;
    public string accessKeySecret;
    public string accessKeyId;
    public string appId;

    public Action ConnectSuccess;
    public Action ConnectFailure;
    
    public UnityEvent OnConnectSuccess;

    public UnityEvent<Dm> OnDanmaku;
    public UnityEvent<SendGift> OnGift;
    public UnityEvent<EnterRoom> OnEnterRoom;
    
    public int RoomId = 24910329;

    public async void WebRoomLinkStart()
    {
        
        var ret = await BApi.GetWebRoomInfo( roomId: RoomId );
        Debug.Log("WebRoom: 获取房间信息: " + ret);
        var resObj = JsonConvert.DeserializeObject<WebRoomStartInfo>(ret);
        if (resObj.Code != 0)
        {
            Debug.LogError(resObj.Message);
            ConnectFailure?.Invoke();
            return;
        }
        else
        {
            Debug.Log("WebRoom: 获取房间信息成功: " + "\ntoken: " + resObj.GetAuthBody());
        }

        m_WebSocketBLiveWebRoomClient = new WebSocketBLiveWebRoomClient(resObj.GetWssLink(), resObj.GetAuthBody(), RoomId);
        m_WebSocketBLiveWebRoomClient.OnEnterRoom += WebSocketBLiveWebRoomClientOnEnterRoom;
        
        try
        {
            m_WebSocketBLiveWebRoomClient.Connect(TimeSpan.FromSeconds(1), 5);
            //ConnectSuccess?.Invoke();
            Debug.Log("WebRoom: 连接成功");
            OnConnectSuccess?.Invoke();
        }
        catch (Exception ex)
        {
            //ConnectFailure?.Invoke();
            Debug.Log("WebRoom: 连接失败");
            throw;
        }
    }

    public async void LinkStart(string code)
    {
        //测试的密钥
        SignUtility.accessKeySecret = accessKeySecret;
        //测试的ID
        SignUtility.accessKeyId = accessKeyId;
        var ret = await BApi.StartInteractivePlay(code, appId);
        //打印到控制台日志
        var gameIdResObj = JsonConvert.DeserializeObject<AppStartInfo>(ret);
        if (gameIdResObj.Code != 0)
        {
            Debug.LogError(gameIdResObj.Message);
            ConnectFailure?.Invoke();
            return;
        }

        m_WebSocketBLiveClient = new WebSocketBLiveClient(gameIdResObj.GetWssLink(), gameIdResObj.GetAuthBody());
        m_WebSocketBLiveClient.OnDanmaku += WebSocketBLiveClientOnDanmaku;
        m_WebSocketBLiveClient.OnGift += WebSocketBLiveClientOnGift;
        m_WebSocketBLiveClient.OnGuardBuy += WebSocketBLiveClientOnGuardBuy;
        m_WebSocketBLiveClient.OnSuperChat += WebSocketBLiveClientOnSuperChat;

        try
        {
            m_WebSocketBLiveClient.Connect(TimeSpan.FromSeconds(1), 1000000);
            ConnectSuccess?.Invoke();
            Debug.Log("连接成功");
            OnConnectSuccess?.Invoke();
        }
        catch (Exception ex)
        {
            ConnectFailure?.Invoke();
            Debug.Log("连接失败");
            throw;
        }

        gameId = gameIdResObj.GetGameId();
        m_PlayHeartBeat = new InteractivePlayHeartBeat(gameId);
        m_PlayHeartBeat.HeartBeatError += M_PlayHeartBeat_HeartBeatError;
        m_PlayHeartBeat.HeartBeatSucceed += M_PlayHeartBeat_HeartBeatSucceed;
        m_PlayHeartBeat.Start();

        
    }


    public async Task LinkEnd()
    {
        m_WebSocketBLiveClient.Dispose();
        m_WebSocketBLiveWebRoomClient.Dispose();
        m_PlayHeartBeat.Dispose();
        await BApi.EndInteractivePlay(appId, gameId);
        Debug.Log("游戏关闭");
    }

    private void WebSocketBLiveClientOnSuperChat(SuperChat superChat)
    {
        StringBuilder sb = new StringBuilder("收到SC!");
        sb.AppendLine();
        sb.Append("来自用户：");
        sb.AppendLine(superChat.userName);
        sb.Append("留言内容：");
        sb.AppendLine(superChat.message);
        sb.Append("金额：");
        sb.Append(superChat.rmb);
        sb.Append("元");
        Debug.Log(sb);
    }

    private void WebSocketBLiveClientOnGuardBuy(Guard guard)
    {
        StringBuilder sb = new StringBuilder("收到大航海!");
        sb.AppendLine();
        sb.Append("来自用户：");
        sb.AppendLine(guard.userInfo.userName);
        sb.Append("赠送了");
        sb.Append(guard.guardUnit);
        Debug.Log(sb);
    }

    private void WebSocketBLiveClientOnGift(SendGift sendGift)
    {
        OnGift?.Invoke(sendGift);
        StringBuilder sb = new StringBuilder("收到礼物!");
        sb.AppendLine();
        sb.Append("来自用户：");
        sb.AppendLine(sendGift.userName);
        sb.Append("赠送了");
        sb.Append(sendGift.giftNum);
        sb.Append("个");
        sb.Append(sendGift.giftName);
        sb.Append("ID");
        sb.Append(sendGift.giftId);
        sb.Append("价值");
        sb.Append(sendGift.price);
        Debug.Log(sb);
    }

    private void WebSocketBLiveClientOnDanmaku(Dm dm)
    {
        OnDanmaku?.Invoke(dm);
        StringBuilder sb = new StringBuilder("收到弹幕!");
        sb.AppendLine();
        sb.Append("用户：");
        sb.AppendLine(dm.userName);
        sb.Append("弹幕内容：");
        sb.Append(dm.msg);
        Debug.Log(sb);
    }
    
    private void WebSocketBLiveWebRoomClientOnEnterRoom(EnterRoom enterRoom)
    {
        OnEnterRoom?.Invoke(enterRoom);
        StringBuilder sb = new StringBuilder("进入房间!");
        sb.AppendLine();
        sb.Append("用户: ");
        sb.Append(enterRoom.userName);
        sb.Append("; ");
        if (enterRoom.fansMedalName != null)
        {
            sb.Append("粉丝勋章: ");
            sb.Append(enterRoom.fansMedalName);
            sb.Append(" ");
            sb.Append(enterRoom.fansMedalLevel);
            sb.Append("级; ");
        }
        else
        {
            sb.Append("没有粉丝勋章; ");
        }
        if (enterRoom.guardLevel > 0)
        {
            sb.Append("大航海: ");
            sb.Append(enterRoom.guardLevel==3?"舰长, ":enterRoom.guardLevel==2?"提督, ":"总督, ");
            sb.Append("剩余天数: ");
            sb.Append(enterRoom.daysBeforeGuardExpired);
        }
        else
        {
            sb.Append("没有大航海; ");
        }
        sb.Append("财富等级: ");
        sb.Append(enterRoom.wealthLevel);
        Debug.Log(sb);
    }


    private static void M_PlayHeartBeat_HeartBeatSucceed()
    {
        Debug.Log("心跳成功");
    }

    private static void M_PlayHeartBeat_HeartBeatError(string json)
    {
        Debug.Log("心跳失败" + json);
    }


    private void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
        if (m_WebSocketBLiveClient is { ws: { State: WebSocketState.Open } })
        {
            m_WebSocketBLiveClient.ws.DispatchMessageQueue();
        }
        if (m_WebSocketBLiveWebRoomClient is { ws: { State: WebSocketState.Open } })
        {
            m_WebSocketBLiveWebRoomClient.ws.DispatchMessageQueue();
        }
        #endif
        
        TestUpdate();
    }

    private void OnDestroy()
    {
        if (m_WebSocketBLiveClient != null)
        {
            m_PlayHeartBeat.Dispose();
            m_WebSocketBLiveClient.Dispose();
        }

        if (m_WebSocketBLiveWebRoomClient != null)
        {
            m_WebSocketBLiveWebRoomClient.Dispose();
        }
    }
}
