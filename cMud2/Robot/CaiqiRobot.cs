using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace cMud2
{
    public class CaiqiRobot : Robot
    {
        private static CaiqiRobot Instance;

        public static CaiqiRobot GetInstance()
        {
            if (Instance == null)
            {
                Instance = new CaiqiRobot();
            }
            return Instance;
        }

        private CaiqiRobot()
            : base()
        {
            Name = "采气";
        }


        public void DoStep1()
        {
            _comm.SendText("#goto 天柱峰下;caiqi");
            _stepNumber = 1;
            _stepTicks = GlobalVariable.Ticks;
        }

        public void DoStep2()
        {
            if (Regex.IsMatch(_currentMessage, @"仅子午两个时辰能够进行采气"))
            {
                Robot rbt = WaitRobot.GetInstance();
                rbt.AfterCmd = "caiqi";
                rbt.Run();
            }
            else if (Regex.IsMatch(_currentMessage, @"小仙姑正在慢慢采集"))
            {
                _stepNumber = 2;
                _stepTicks = GlobalVariable.Ticks;
            }
            else
            {
                if (GlobalVariable.Ticks - _stepTicks > 2000)
                {
                    _comm.SendText("caiqi");
                    _stepTicks = GlobalVariable.Ticks;
                }
            }
        }

        public void DoStep3()
        {
            if (Regex.IsMatch(_currentMessage, @"小仙姑大脑一片空明，感觉一股热气"))
            {
                _comm.SendText("#wa 3000;#goto 武当广场;wc");
                Finish();
            }
            else
            {
                if (GlobalVariable.Ticks - _stepTicks > 2000)
                {
                    _comm.SendText("#wa 3000;#goto 武当广场;wc");
                    Finish();
                }
            }
        }

        public override void Run()
        {
            base.Run();
            DoStep1();
        }

        protected override void GoNext()
        {
            switch (_stepNumber)
            {
                case 1:
                    DoStep2();
                    break;
                case 2:
                    DoStep3();
                    break;
            }
        }
    }
}
