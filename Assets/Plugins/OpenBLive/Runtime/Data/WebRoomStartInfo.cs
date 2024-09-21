using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace OpenBLive.Runtime.Data
{
    public class WebRoomStartInfo
    {
        [JsonProperty("code")]
        public int Code;
        [JsonProperty("message")]
        public string Message;
        [JsonProperty("data")]
        public WebRoomStartInfoData Data;


        
        /// <summary>
        /// 获取长链地址
        /// </summary>
        /// <returns></returns>
        public IList<string> GetWssLink() => Data?.host_list?.Select(x => x.GetWssLink()).ToList();
        
        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        public string GetAuthBody() => Data?.token;

    }


    public class WebRoomStartInfoData
    {
        public string group = "live";
        public int business_id = 0;
        public float refresh_row_factor = 0.125f;
        public int refresh_rate = 100;
        public int max_delay = 5000;
        public string token;
        public List<AppStartHostInfo> host_list;
    }

    public class AppStartHostInfo
    {
        public string host;
        public int port;
        public int wss_port;
        public int ws_port;
        
        public string GetWssLink() => $"ws://{host}:{ws_port}/sub";
    }
}
