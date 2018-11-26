using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.ApplicationBlocks.Data;
using System.Data;

namespace cMud2
{

    public class FindPathRobot : Robot
    {
        #region 字段
        string _currentArea = "武当";
        string _targetArea = "武当";
        string _currentCenter = "三清殿";
        string _currentAddress = string.Empty;
        string _targetAddress = string.Empty;
        int _targetAddrNumber;

        Random _random = new Random();
        int _failedtimes = 0;
        int _waitTimes = 0;

        #endregion

        private static FindPathRobot Instance;

        public static FindPathRobot GetInstance()
        {
            if (Instance == null)
            {
                Instance = new FindPathRobot();
            }
            return Instance;
        }

        private FindPathRobot()
            : base()
        {
            Name = "找路";
        }


        #region override
        public override void Run()
        {
            base.Run();
            if (_targetAddress.Equals("天柱峰下"))
            {
                Stop();
                Robot rbt = GotoTianzhufengRobot.GetInstance();
                rbt.AfterCmd = this.AfterCmd;
                rbt.Run();
            }
            DoStep1();

        }
        public override void DealFadai()
        {
            base.DealFadai();
            _failedtimes = 0;
            _waitTimes = 0;
            _stepNumber = 0;
            DoStep1();
        }
        public override void Stop()
        {
            base.Stop();
            _failedtimes = 0;
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
                case 4:
                    DoStep5();
                    break;
                case 5:
                    DoStep6();
                    break;
                case 6:
                    DoStep7();
                    break;
                case 7:
                    DoStep8();
                    break;
            }

        }
        #endregion

        public void SetTargetAddress(string name)
        {
            SetTargetAddress(name, 1);
        }

        public void SetTargetAddress(string name, int number)
        {
            _targetAddress = name;
            _targetAddrNumber = number;
        }

        #region step

        private void DoStep1()//检查当前地点
        {
            DataSet ds = GetLocationPath(_targetAddress, _targetAddrNumber);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {

                _comm.SendText("emote 未检索到目标地点，请手动行走！");
                Stop();
                return;
            }
            _comm.SendText("emote 开始检查当前地点;l");
            _stepNumber = 1;
        }

        private void DoStep2()//检查当前地点
        {

            if (Regex.IsMatch(_currentMessage, @"\[1A\[200D\[K(?<name>.*)\s-", RegexOptions.Multiline))
            {
                _currentAddress = Regex.Match(_currentMessage, @"\[1A\[200D\[K(?<name>.*)\s-", RegexOptions.Multiline).Groups["name"].Value;
                _comm.SendText("emote 当前地点在" + _currentAddress);
                _stepNumber = 2;
                _failedtimes = 0;
            }
            if (Regex.IsMatch(_currentMessage, @"风景要慢慢的看"))
            {
                _comm.SendText("#wa 2000;l");
            }
            else
            {
                _waitTimes++;
                if (_waitTimes >= 10)
                {
                    _comm.SendText("emote 检查当前地点失败。重新尝试~;l");
                    _failedtimes++;
                    if (_failedtimes >= 10) Stop();
                    _waitTimes = 0;
                }

            }
        }

        private void DoStep3()
        {
            _stepNumber = 3;
            DataSet ds = GetLocationPath(_currentAddress);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                _stepNumber = 1;
                _comm.SendText("emote 获取返回地图中心地点路径失败!尝试移动其他地点返回！");

                string direction = GlobalVariable.CurrentDirections[_random.Next(GlobalVariable.CurrentDirections.Count)];
                _comm.SendText("#wa 500;" + direction);
                DoStep1();
                _failedtimes++;
                if (_failedtimes >= 10) Stop();
                return;
            }
            if (ds.Tables[0].Rows.Count > 1)
            {
                _stepNumber = 1;
                _comm.SendText("emote 当前地点名称存在多个，尝试移到其他地点返回。");
                string direction = GlobalVariable.CurrentDirections[_random.Next(GlobalVariable.CurrentDirections.Count)];
                _comm.SendText("#wa 500;" + direction);
                DoStep1();
                _failedtimes++;
                if (_failedtimes >= 10) Stop();
                return;
            }

            _comm.SendText("emote 开始返回地图中心地点;" + ds.Tables[0].Rows[0][4].ToString() + ";#wa 500;emote 返回地图中心点成功;");
            _stepTicks = GlobalVariable.Ticks;
            _failedtimes = 0;
        }

        private void DoStep4()
        {
            if (Regex.IsMatch(_currentMessage, @"云自遥返回地图中心点成功", RegexOptions.Multiline))
            {
                _stepNumber = 4;
                _comm.SendText("l;emote 检查是否为地图中心点");
                _stepTicks = GlobalVariable.Ticks;
            }
            else
            {
                if (GlobalVariable.Ticks - _stepTicks > 200)
                {
                    _comm.SendText("emote 返回地图中心点成功");
                    _stepTicks = GlobalVariable.Ticks;
                }
            }
        }

        private void DoStep5()
        {
            if (Regex.IsMatch(_currentMessage, @"\[1A\[200D\[K(?<name>.*)\s-", RegexOptions.Multiline))
            {
                _currentAddress = Regex.Match(_currentMessage, @"\[1A\[200D\[K(?<name>.*)\s-", RegexOptions.Multiline).Groups["name"].Value;
                if (!_currentAddress.Equals(_currentCenter))
                {
                    _comm.SendText("emote 此地点不为地图中心点。重新寻路!");
                    DoStep1();
                    return;
                }
                _stepNumber = 5;
            }
            if (Regex.IsMatch(_currentMessage, @"风景要慢慢的看"))
            {
                _comm.SendText("#wa 2000;l");
            }
            else
            {
                _waitTimes++;
                if (_waitTimes >= 10)
                {
                    _comm.SendText("l;#wa 2000;emote 检查是否为地图中心点");
                    _failedtimes++;
                    if (_failedtimes >= 10) Stop();
                    _waitTimes = 0;
                }

            }
        }

        private void DoStep6()
        {
            _stepNumber = 6;
            DataSet ds = GetLocationPath(_targetAddress, _targetAddrNumber);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                _comm.SendText("emote 未检索到目标地点，请手动行走！");
                Stop();
                return;
            }
            string cmd = ds.Tables[0].Rows[0][3].ToString();
            if (cmd.Contains("#goto 天柱峰下"))
            {
                cmd = "#wa 500;" + cmd + ";emote 目标地点到达;" + AfterCmd;
            }
            else
            {
                cmd = "#wa 500;" + cmd + ";emote 目标地点到达";
            }
            _comm.SendText(cmd);
            _stepTicks = GlobalVariable.Ticks;
        }

        private void DoStep7()
        {
            if (Regex.IsMatch(_currentMessage, @"云自遥目标地点到达", RegexOptions.Multiline))
            {
                _comm.SendText("#wa 500;l");
                _stepNumber = 7;
            }
            else
            {
                if (GlobalVariable.Ticks - _stepTicks > 200)
                {
                    _comm.SendText("emote 目标地点到达");
                    _stepTicks = GlobalVariable.Ticks;
                }
            }
        }

        private void DoStep8()
        {
            if (Regex.IsMatch(_currentMessage, @"\[1A\[200D\[K(?<name>.*)\s-", RegexOptions.Multiline))
            {
                _currentAddress = Regex.Match(_currentMessage, @"\[1A\[200D\[K(?<name>.*)\s-", RegexOptions.Multiline).Groups["name"].Value;
                if (!_currentAddress.Equals(_targetAddress))
                {
                    _comm.SendText("emoet 此地点不为目标地点。重新寻路！;#wa 1000");
                    DoStep1();
                    return;
                }
                _comm.SendText("emote 到达目标点" + _targetAddress + ";#wa 500");
                Finish();
            }
            if (Regex.IsMatch(_currentMessage, @"风景要慢慢的看"))
            {
                _comm.SendText("#wa 2000;l");
            }
            else
            {
                _waitTimes++;
                if (_waitTimes >= 10)
                {
                    _comm.SendText("#wa 2000;l");
                    _failedtimes++;
                    if (_failedtimes >= 10) Stop();
                    _waitTimes = 0;
                }

            }
        }
        #endregion

        private DataSet GetLocationPath(string address)
        {
            if (string.IsNullOrEmpty(address)) return null;
            DataSet ds = null;
            try
            {
                string sql = string.Format("SELECT * FROM Path NOLOCK WHERE Name = '{0}' and area='{1}'", address, _currentArea);
                ds = SqlHelper.ExecuteDataset(GlobalParams.ConnnectionString, CommandType.Text, sql);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message + "\r\n" + e.StackTrace);
            }
            return ds;
        }
        private DataSet GetLocationPath(string address, int number)
        {
            if (string.IsNullOrEmpty(address)) return null;
            DataSet ds = null;
            try
            {
                string sql = string.Format("SELECT * FROM Path NOLOCK WHERE Name = '{0}' and Number = {1} and area='{2}'", address, number, _currentArea);
                ds = SqlHelper.ExecuteDataset(GlobalParams.ConnnectionString, CommandType.Text, sql);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message + "\r\n" + e.StackTrace);
            }
            return ds;
        }



    }
}
