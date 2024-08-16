using System.Collections.Generic;
using DataSystem;
using UnityEngine;

namespace DefaultNamespace
{
    public class EnterRoomManager
    {
        public static Dictionary<long, float> LastEnterRoomTimestamp = new Dictionary<long, float>();
        
        public static bool CheckCoolDown(long uid, float coolDownTimeInSeconds=60)
        {
            if (!LastEnterRoomTimestamp.ContainsKey(uid))
            {
                LastEnterRoomTimestamp[uid] = Time.time;
                return true;
            }
            else
            {
                if (Time.time - LastEnterRoomTimestamp[uid] > coolDownTimeInSeconds)
                {
                    LastEnterRoomTimestamp[uid] = Time.time;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        
        public static int GetContinueAttendance(long roomId, long uid)
        {
            if (!GameProgressData.GetLatestAttendanceDate(roomId).ContainsKey(uid))
            {
                GameProgressData.GetLatestAttendanceDate(roomId)[uid] = System.DateTime.Now.ToString("yyyy-MM-dd");
                GameProgressData.GetContinueAttendance(roomId)[uid] = 1;
                GameProgressData.Save();
                return 1;
            }
            else
            {
                // if last attendance date is yestorday, then continue attendance + 1
                // if last attendance date is today, then return the continue attendance.
                // if last attendance date is before yestorday, then reset the continue attendance to 1.
                if (GameProgressData.GetLatestAttendanceDate(roomId)[uid] == System.DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"))
                {
                    GameProgressData.GetLatestAttendanceDate(roomId)[uid] = System.DateTime.Now.ToString("yyyy-MM-dd");
                    GameProgressData.GetContinueAttendance(roomId)[uid] += 1;
                    GameProgressData.Save();
                    return GameProgressData.GetContinueAttendance(roomId)[uid];
                }
                else if (GameProgressData.GetLatestAttendanceDate(roomId)[uid] == System.DateTime.Now.ToString("yyyy-MM-dd"))
                {
                    return GameProgressData.GetContinueAttendance(roomId)[uid];
                }
                else
                {
                    GameProgressData.GetLatestAttendanceDate(roomId)[uid] = System.DateTime.Now.ToString("yyyy-MM-dd");
                    GameProgressData.GetContinueAttendance(roomId)[uid] = 1;
                    GameProgressData.Save();
                    return 1;
                }
            }
        }
    }
}