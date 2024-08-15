namespace DataSystem
{
    public class EnterRoomDropConfigData : GameData
    {
        public bool DropNormalUser = false; // 普通用户是否有进房掉落
        public bool DropGuard3 = true; // 舰长是否有进房掉落
        public bool DropGuard2 = true; // 提督是否有进房掉落
        public bool DropGuard1 = true; // 总督是否有进房掉落
        public int DropCountNormalUser = 1; // 进房掉落数量
        public int DropCountGuard3 = 1; // 进房掉落数量
        public int DropCountGuard2 = 1; // 进房掉落数量
        public int DropCountGuard1 = 1; // 进房掉落数量
        public float DropCoolDown = 1f; // 进房掉落冷却时间(分钟)
    }
}