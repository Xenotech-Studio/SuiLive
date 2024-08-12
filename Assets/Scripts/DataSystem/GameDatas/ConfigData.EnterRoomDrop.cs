namespace DataSystem
{
    public class EnterRoomDropConfigData : GameData
    {
        public bool DropNormalUser = false; // 普通用户是否有进房掉落
        public bool DropGuard3 = true; // 舰长是否有进房掉落
        public bool DropGuard2 = true; // 提督是否有进房掉落
        public bool DropGuard1 = true; // 总督是否有进房掉落
    }
}