using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace cMud2
{
    public class LianDanRobot : Robot
    {
        private static LianDanRobot Instance;

        public static LianDanRobot GetInstance()
        {
            if (Instance == null)
            {
                Instance = new LianDanRobot();
            }
            return Instance;
        }

        private LianDanRobot()
            : base()
        {
            Name = "炼丹";
        }



        string color = string.Empty;



        private void DoStep1()
        {

            _comm.SendText("#goto 俞岱岩住处;#wa 500;ask yu about 炼丹");
            _stepNumber = 1;
            _stepTicks = GlobalVariable.Ticks;

        }
        private void DoStep2()
        {
            if (Regex.IsMatch(_currentMessage, @"^>?\s?你向俞岱岩打听有关『炼丹』"))
            {
                _comm.SendText("south;#wa 500;zuo;#wa 500;kan");
                _stepNumber = 2;
                _stepTicks = GlobalVariable.Ticks;
            }
            else if (!FindPathRobot.GetInstance().IsRuning)
            {
                if (GlobalVariable.Ticks - _stepTicks > 200)
                {
                    _comm.SendText("ask yu about 炼丹");
                    _stepTicks = GlobalVariable.Ticks;
                }

            }

        }

        private void DoStep3()
        {
            if (Regex.IsMatch(_currentMessage, @"^\s+\*+\[1;(?<color>[0-9]+)m\^", RegexOptions.Multiline))
            {
                color = Regex.Match(_currentMessage, @"^\s+\*+\[1;(?<color>[0-9]+)m\^", RegexOptions.Multiline).Groups["color"].Value;
                _stepTicks = GlobalVariable.Ticks;
            }
            if (Regex.IsMatch(_currentMessage, @"炉火似乎正在慢慢减弱", RegexOptions.Multiline))
            {
                if ("35".Equals(color))
                {
                    _comm.SendText("#wa 1500;change M");

                }
                if ("31".Equals(color))
                {
                    _comm.SendText("#wa 1500;change L");

                }
                if ("33".Equals(color))
                {
                    _comm.SendText("#wa 1500;change H");
                }
                _stepTicks = GlobalVariable.Ticks;
            }
            else if (Regex.IsMatch(_currentMessage, @"丹炉里传出来一阵清香，看来已经成丹了", RegexOptions.Multiline))
            {
                _comm.SendText("#wa 1500;zhan;n;#wa 1000;ask yu about 炼丹");
                _stepTicks = GlobalVariable.Ticks;
            }
            else if (Regex.IsMatch(_currentMessage, @"俞岱岩.*说道：「不错不错", RegexOptions.Multiline))
            {
                _comm.SendText("#goto 武当广场;wc");
                _stepNumber = 3;
                _stepTicks = GlobalVariable.Ticks;
                Finish();
            }
            else if (Regex.IsMatch(_currentMessage, @"你没有看好丹炉，发现里面的丹药已经结块了", RegexOptions.Multiline))
            {
                _comm.SendText("zhan;#wa 2000;n;ld;#wa 2000;#goto 武当广场;qx");
                Stop();
                _stepTicks = GlobalVariable.Ticks;
                return;
            }
            else if (!FindPathRobot.GetInstance().IsRuning)
            {
                if (GlobalVariable.Ticks - _stepTicks > 200)
                {
                    _comm.SendText("zhan;#wa 2000;n;ld;#wa 2000;#goto 武当广场;wc");
                    _stepTicks = GlobalVariable.Ticks;
                    Finish();
                }

            }

        }

        public override void Stop()
        {
            base.Stop();
            _stepNumber = 0;
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
                default:
                    break;
            }
        }



    }
}
