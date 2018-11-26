using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace cMud2
{
    public class WaitRobot : Robot
    {
          private static WaitRobot Instance;

        public static WaitRobot GetInstance()
        {
            if (Instance == null)
            {
                Instance = new WaitRobot();
            }
            return Instance;
        }

        private WaitRobot()
            : base()
        {
            Name = "等待";
        }

        private void DoStep1()
        {
            _comm.SendText(GlobalParams.WaitCommand);
            _stepNumber = 1;
            _stepTicks = GlobalVariable.Ticks;
        }
        private void DoStep2()
        {
            if (Regex.IsMatch(_currentMessage, @"^>?\s?你运功完毕", RegexOptions.Multiline))
            {
                Finish();
            }
            else
            {
                if (GlobalVariable.Ticks - _stepTicks > 200)
                {
                    _comm.SendText(GlobalParams.WaitCommand);
                    _stepTicks = GlobalVariable.Ticks;
                }
            }
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

        public override void Finish()
        {
            Stop();
            _comm.SendText(AfterCmd);
        }

        public override void Run()
        {
            base.Run();
            DoStep1();
        }
    }
}
