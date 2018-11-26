using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cMud2
{
    public class RobotManager
    {

        public FindPathRobot FindPath
        { get; set; }
        public GotoTianzhufengRobot GotoTianzhufeng
        { get; set; }
        public LianDanRobot LianDan
        { get; set; }
        public SongJingRobot SongJing
        { get; set; }
        public ZhenfaRobot Zhenfa
        { get; set; }
        public AutoWanderRobot LuanZhou
        { get; set; }
        public CaiqiRobot Caiqi
        { get; set; }
        public WuDangNewRobot WuDangNew
        { get; set; }

        private static RobotManager Instance;

        public static RobotManager GetInstance()
        {
            if (Instance == null)
            {
                Instance = new RobotManager();
            }
            return Instance;
        }

        private RobotManager()
        {
            FindPath = FindPathRobot.GetInstance();
            GotoTianzhufeng = GotoTianzhufengRobot.GetInstance();
            LianDan = LianDanRobot.GetInstance();
            SongJing = SongJingRobot.GetInstance();
            Zhenfa = ZhenfaRobot.GetInstance();
            LuanZhou = AutoWanderRobot.GetInstance();
            Caiqi = CaiqiRobot.GetInstance();
            WuDangNew = WuDangNewRobot.GetInstance();
        }

        public void StopAll()
        {
            if (FindPath.IsRuning)
                FindPath.Stop();
            if (GotoTianzhufeng.IsRuning)
                GotoTianzhufeng.Stop();
            if (LianDan.IsRuning)
                LianDan.Stop();
            if (SongJing.IsRuning)
                SongJing.Stop();
            if (Zhenfa.IsRuning)
                Zhenfa.Stop();
            if (LuanZhou.IsRuning)
                LuanZhou.Stop();
            if (Caiqi.IsRuning)
                Caiqi.Stop();
            if (WuDangNew.IsRuning)
                WuDangNew.Stop();
        }

        public void ShowStatus()
        {
            MudCommunication comm = MudCommunication.GetInstance();
            if (FindPath.IsRuning)
                comm.SendText("emote " + FindPath.Name + "-" + FindPath.IsRuning + "-" + FindPath.StepNumber);
            if (GotoTianzhufeng.IsRuning)
                comm.SendText("emote " + GotoTianzhufeng.Name + "-" + GotoTianzhufeng.IsRuning + "-" + GotoTianzhufeng.StepNumber);
            if (LianDan.IsRuning)
                comm.SendText("emote " + LianDan.Name + "-" + LianDan.IsRuning + "-" + LianDan.StepNumber);
            if (SongJing.IsRuning)
                comm.SendText("emote " + SongJing.Name + "-" + SongJing.IsRuning + "-" + SongJing.StepNumber);
            if (Zhenfa.IsRuning)
                comm.SendText("emote " + Zhenfa.Name + "-" + Zhenfa.IsRuning + "-" + Zhenfa.StepNumber);
            if (LuanZhou.IsRuning)
                comm.SendText("emote " + LuanZhou.Name + "-" + LuanZhou.IsRuning + "-" + LuanZhou.StepNumber);
            if (Caiqi.IsRuning)
                comm.SendText("emote " + Caiqi.Name + "-" + Caiqi.IsRuning + "-" + Caiqi.StepNumber);
            if (WuDangNew.IsRuning)
                comm.SendText("emote " + WuDangNew.Name + "-" + WuDangNew.IsRuning + "-" + WuDangNew.StepNumber);
        }

        public void StartRobot(string name)
        {
            switch (name)
            {
                case "FindPath":
                    FindPath.Run();
                    break;
                case "GotoTianzhufeng":
                    GotoTianzhufeng.Run();
                    break;
                case "LianDan":
                    LianDan.Run();
                    break;
                case "SongJing":
                    SongJing.Run();
                    break;
                case "ZhenFa":
                    Zhenfa.Run();
                    break;
                case "LuanZhou":
                    LuanZhou.Run();
                    break;
                case "CaiQi":
                    Caiqi.Run();
                    break;
                case "WDNew":
                    WuDangNew.Run();
                    break;
            }
        }

        public void DealFadai()
        {

        }



    }
}
