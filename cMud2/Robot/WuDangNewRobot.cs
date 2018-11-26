using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace cMud2
{
    public class WuDangNewRobot : Robot
    {
        private static WuDangNewRobot Instance;

        public static WuDangNewRobot GetInstance()
        {
            if (Instance == null)
            {
                Instance = new WuDangNewRobot();
            }
            return Instance;
        }

        private WuDangNewRobot()
            : base()
        {
            Name = "武当新手";
            _comm.FadaiHandler += new MudCommunication.FadaiEventHandler(DealFadai);
        }

        string _currentJob = string.Empty;

        private void DoStep1()
        {
            _comm.SendText("ask chongxu about quest");
            _stepNumber = 1;
            _stepTicks = GlobalVariable.Ticks;
        }

        private void DoStep2()
        {
            if (Regex.IsMatch(_currentMessage, "你向冲虚道长打听有关『quest』的消息.*冲虚道长.+说道：「今天全派弟子要在.*熟读典籍", RegexOptions.Singleline))
            {
                SongJingRobot.GetInstance().Run();
                _currentJob = "SongJing";
                _stepTicks = GlobalVariable.Ticks;
            }
            else if (Regex.IsMatch(_currentMessage, "你向冲虚道长打听有关『success』的消息.*冲虚道长.*奖励", RegexOptions.Singleline))
            {
                _comm.SendText("n;w;w;get baicai;get zhou;#3 eat baicai;#3 eat zhou;drop zhou;drop baicai;#wa 3000;e;e;n;n;e;e;drink;#wa 3000;#goto 武当广场;#wa 1000;ask chongxu about quest");
                _stepTicks = GlobalVariable.Ticks;
            }
            else if (Regex.IsMatch(_currentMessage, "你向冲虚道长打听有关『quest』的消息.*冲虚道长.+说道：「道家炼气最讲究", RegexOptions.Singleline))
            {
                CaiqiRobot.GetInstance().Run();
                _currentJob = "CaiQi";
                _stepTicks = GlobalVariable.Ticks;
            }
            else if (Regex.IsMatch(_currentMessage, "你向冲虚道长打听有关『quest』的消息.*冲虚道长.+说道：「武当派以真武七截阵闻名天下", RegexOptions.Singleline))
            {
                ZhenfaRobot.GetInstance().Run();
                _currentJob = "ZhenFa";
                _stepTicks = GlobalVariable.Ticks;
            }
            else if (Regex.IsMatch(_currentMessage, "你向冲虚道长打听有关『quest』的消息.*冲虚道长.+说道：「武当三侠最近迷上了炼丹", RegexOptions.Singleline))
            {
                LianDanRobot.GetInstance().Run();
                _currentJob = "LianDan";
                _stepTicks = GlobalVariable.Ticks;
            }
            else if (Regex.IsMatch(_currentMessage, "你向冲虚道长打听有关『quest』的消息.*冲虚道长.*现在还没有新的工作", RegexOptions.Singleline))
            {
                Robot rbt = WaitRobot.GetInstance();
                rbt.AfterCmd = "ask chongxu about quest";
                rbt.Run();
                _stepTicks = GlobalVariable.Ticks;
            }
            else if (Regex.IsMatch(_currentMessage, @"你向冲虚道长打听有关『quest』的消息.*冲虚道长.+说道：「.*还有任务在身", RegexOptions.Singleline))
            {
                _comm.SendText("ask chongxu about success;#wa 2000;ask chongxu about cancel;#wa 2000;ask chongxu about quest");
                _stepTicks = GlobalVariable.Ticks;
            }

            else
            {
                if (LianDanRobot.GetInstance().IsRuning)
                    _stepTicks = GlobalVariable.Ticks;
                if (CaiqiRobot.GetInstance().IsRuning)
                    _stepTicks = GlobalVariable.Ticks;
                if (SongJingRobot.GetInstance().IsRuning)
                    _stepTicks = GlobalVariable.Ticks;
                if (ZhenfaRobot.GetInstance().IsRuning)
                    _stepTicks = GlobalVariable.Ticks;
                if (FindPathRobot.GetInstance().IsRuning)
                    _stepTicks = GlobalVariable.Ticks;

                if (GlobalVariable.Ticks - _stepTicks > 500)
                {
                    if (GlobalVariable.CurrentLocation.Equals("武当广场"))
                    {
                        _comm.SendText("n;#wa 5000;s;ask chongxu about quest");
                        _stepTicks = GlobalVariable.Ticks;
                    }
                }
            }



        }


        public override void DealFadai()
        {
            if (IsRuning)
                _comm.SendText("#goto 武当广场;ask chongxu about quest");
        }

        protected override void GoNext()
        {
            switch (_stepNumber)
            {
                case 1:
                    DoStep2();
                    break;
            }
        }

        public override void Run()
        {
            base.Run();
            DoStep1();
        }
    }
}
