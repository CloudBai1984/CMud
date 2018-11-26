using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace cMud2
{
    public class GotoTianzhufengRobot : Robot
    {



        private static GotoTianzhufengRobot Instance;

        public static GotoTianzhufengRobot GetInstance()
        {
            if (Instance == null)
            {
                Instance = new GotoTianzhufengRobot();
            }
            return Instance;
        }

        private GotoTianzhufengRobot()
            : base()
        {
            Name = "天柱峰";
        }

        Random _random = new Random();
        string _direction = string.Empty;


        private void DoStep1()
        {
            _comm.SendText("#goto 后门");
            _stepTicks = GlobalVariable.Ticks;
            _stepNumber = 1;
        }

        private void DoStep2()
        {

            if (Regex.IsMatch(_currentMessage, @"云自遥到达目标点后门", RegexOptions.Multiline))
            {
                _stepNumber = 2;
                _comm.SendText("#wa 200;emote 继续寻路");
            }
            else
            {
                if (GlobalVariable.Ticks - _stepTicks > 200)
                {
                    if (GlobalVariable.CurrentLocation.Equals("后门"))
                        _comm.SendText("emote 到达目标点后门");
                    _stepTicks = GlobalVariable.Ticks;
                }

            }
        }

        private void DoStep3()
        {
            if (Regex.IsMatch(_currentMessage, @"\s天柱峰下\s", RegexOptions.Multiline))
            {
                if (GlobalVariable.CurrentLocation.Equals("天柱峰下"))
                {
                    
                    _comm.SendText("emote 到达天柱峰下!");
                    Finish();
                }
                else
                {
                    _stepNumber = 3;
                    string tmp = GlobalVariable.CurrentDirections[_random.Next(GlobalVariable.CurrentDirections.Count)];
                    while (!string.IsNullOrEmpty(_direction) && tmp.Equals(_direction))
                    {
                        tmp = GlobalVariable.CurrentDirections[_random.Next(GlobalVariable.CurrentDirections.Count)];
                    }
                    _direction = tmp;
                    _comm.SendText("#wa 200;" + _direction);
                    _stepTicks = GlobalVariable.Ticks;
                }
            }
            if (Regex.IsMatch(_currentMessage, @"\[1A\[200D\[K天柱峰下\s-", RegexOptions.Multiline))
            {
                _comm.SendText("emote 到达天柱峰下!");
                Finish();
            }
            else if (GlobalVariable.CurrentLocation.Equals("后门"))
            {

                if (Regex.IsMatch(_currentMessage, @"云自遥继续寻路"))
                    _comm.SendText("#wa 200;n;emote 继续寻路");
                _stepTicks = GlobalVariable.Ticks;
            }
            else if (Regex.IsMatch(_currentMessage, @"云自遥继续寻路"))
            {
                if (GlobalVariable.CurrentDirections.Count > 0)
                {
                    _comm.SendText("#wa 200;" + GlobalVariable.CurrentDirections[_random.Next(GlobalVariable.CurrentDirections.Count)] + ";emote 继续寻路");
                }
                else
                {
                    _comm.SendText("emote 继续寻路");
                }
                _stepTicks = GlobalVariable.Ticks;
            }
            else if (Regex.IsMatch(_currentMessage, @"云自遥返回原路"))
            {

                string tmp = GlobalParams.DirectionMapping[_direction].ToString();
                _comm.SendText("#wa 200;" + tmp);
                _stepTicks = GlobalVariable.Ticks;
            }
            else
            {
                if (GlobalVariable.Ticks - _stepTicks > 200)
                {
                    _comm.SendText("emote 继续寻路");
                    _stepTicks = GlobalVariable.Ticks;
                }

            }
        }

        private void DoStep4()
        {
            if (Regex.IsMatch(_currentMessage, @"\[1A\[200D\[K天柱峰下\s-", RegexOptions.Multiline))
            {

                _comm.SendText("emote 到达天柱峰下!");
                Finish();
            }
            else
            {
                _stepNumber = 2;
                _comm.SendText("emote 返回原路");
                _stepTicks = GlobalVariable.Ticks;
            }
        }

        public override void Run()
        {
            base.Run();
            DoStep1();
        }


        protected override void GoNext()
        {
            base.GoNext();
            switch (_stepNumber)
            {
                case 1:
                    DoStep2();
                    break;
                case 2:
                    DoStep3();
                    break;
                case 3:
                    DoStep4();
                    break;
            }
        }


    }
}
