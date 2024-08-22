using System.Collections.Generic;
using DataSystem;
using UnityEngine;

namespace DefaultNamespace
{
    public class EnterRoomManager
    {
        public static Dictionary<long, float> LastEnterRoomTimestamp = new Dictionary<long, float>();
        public static Dictionary<long, float> LastEnterRoomTimestamp2 = new Dictionary<long, float>();
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
        
        public static bool CheckCoolDown2(long uid, float coolDownTimeInSeconds=60)
        {
            if (!LastEnterRoomTimestamp2.ContainsKey(uid))
            {
                LastEnterRoomTimestamp2[uid] = Time.time;
                return true;
            }
            else
            {
                if (Time.time - LastEnterRoomTimestamp2[uid] > coolDownTimeInSeconds)
                {
                    LastEnterRoomTimestamp2[uid] = Time.time;
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

        public static int GetWeekAttendance(long roomId, long uid)
        {
            List<string> history = GameProgressData.GetWeekAttendanceHistory(roomId, uid);
            
            bool IsInSameWeek(string date1, string date2)
            {
                // if the two dates are in the same week, return true
                int date1DayOfWeek = System.DateTime.Parse(date1).DayOfWeek == 0 ? 6 : ((int)System.DateTime.Parse(date1).DayOfWeek - 1);
                int date2DayOfWeek = System.DateTime.Parse(date2).DayOfWeek == 0 ? 6 : ((int)System.DateTime.Parse(date2).DayOfWeek - 1);
                string date1Monday = System.DateTime.Parse(date1).AddDays(-1 * date1DayOfWeek).ToString("yyyy-MM-dd");
                string date2Monday = System.DateTime.Parse(date2).AddDays(-1 * date2DayOfWeek).ToString("yyyy-MM-dd");
                return date1Monday == date2Monday;
            }

            // for each date in history, if it is not in the same week, remove it from list
            for (int i = 0; i < history.Count; i++)
            {
                if (!IsInSameWeek(history[i], System.DateTime.Now.ToString("yyyy-MM-dd")))
                {
                    history.RemoveAt(i);
                    i -= 1;
                }
            }
            
            // add today's date to history, if it is not in the list
            if (!history.Contains(System.DateTime.Now.ToString("yyyy-MM-dd")))
            {
                history.Add(System.DateTime.Now.ToString("yyyy-MM-dd"));
            }
            
            GameProgressData.Save();
            
            return history.Count;
        }
    }
}