using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.ApplicationBlocks.Data;
using System.Data;

namespace cMud2
{

    public class AutoWanderRobot : Robot
    {
        #region 机器人变量
        string[] _allDirection = new string[] { "west", "east", "north", "south", "northwest", "northeast", "southwest",
            "southeast", "eastup", "westup", "northup", "southup", "eastdown", "westdown", "northdown", "southdown", "enter", "out", "down", "up", };
        Random _random = new Random();
        string _direction = string.Empty;
        string _preDirection = string.Empty;
        string _name = string.Empty;
        string _desc = string.Empty;
        string _prename = string.Empty;
        string _predesc = string.Empty;
        #endregion


        private static AutoWanderRobot Instance;

        public static AutoWanderRobot GetInstance()
        {
            if (Instance == null)
            {
                Instance = new AutoWanderRobot();
            }
            return Instance;
        }

        private AutoWanderRobot()
            : base()
        {
            Name = "乱走";
        }

        public override void Run()
        {
            base.Run();
            _comm.SendText("l;emote gonext");
            _stepNumber = 1;
        }

        protected override void GoNext()
        {
            base.GoNext();
            switch (_stepNumber)
            {
                case 1:
                    CaptureLocation();
                    try
                    {
                        Save();
                    }
                    catch (Exception e)
                    {

                        System.Windows.MessageBox.Show(e.Message + "\r\n" + e.StackTrace);
                    }

                    if (GlobalVariable.CurrentDirections.Count > 0)
                    {
                        if (Regex.IsMatch(_currentMessage, "->云自遥gonext"))
                        {
                            _direction = GlobalVariable.CurrentDirections[_random.Next(GlobalVariable.CurrentDirections.Count)];
                            _comm.SendText("#wa 500;" + _direction + ";emote gonext");
                        }
                    }
                    _prename = _name;
                    _predesc = _desc;
                    _preDirection = _direction;
                    break;
            }
        }

        public override void DealFadai()
        {
            if (!IsRuning) return;
            base.DealFadai();
            _stepNumber = 1;
            _comm.SendText("#wa 500;emote gonext");
        }



        private void CaptureLocation()
        {

            if (Regex.IsMatch(_currentMessage, @"\[1A\[200D\[K(?<name>.*)\s-", RegexOptions.Multiline))
            {
                _name = Regex.Match(_currentMessage, @"\[1A\[200D\[K(?<name>.*)\s-", RegexOptions.Multiline).Groups["name"].Value;
            }
            if (Regex.IsMatch(_currentMessage, @"\[1A\[200D\[K.*\s-\s+\r\n\s+(?<desc>.*?)\r\n\s+", RegexOptions.Singleline))
            {
                _desc = Regex.Match(_currentMessage, @"\[1A\[200D\[K.*\s-\s+\r\n\s+(?<desc>.*?)\r\n\s+", RegexOptions.Singleline).Groups["desc"].Value;
                _desc = _desc.Replace("\r\n","");
            }
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(_predesc) && string.IsNullOrEmpty(_prename) && string.IsNullOrEmpty(_preDirection)) return;
            string sql = string.Format("select 1 from Location nolock where Name='{0}' and Description='{1}'", _prename, _predesc);

            DataSet ds = SqlHelper.ExecuteDataset(GlobalParams.ConnnectionString, CommandType.Text, sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                sql = string.Format("Update Location set {0} = '{1}' where Name='{2}' and Description='{3}' ", _preDirection, _name, _prename, _predesc);
                try
                {
                    SqlHelper.ExecuteNonQuery(GlobalParams.ConnnectionString, CommandType.Text, sql);
                }
                catch (Exception)
                {


                }

            }
            else
            {
                sql = string.Format("insert into Location(name,Description,{0}) select '{1}','{2}','{3}'", _preDirection, _prename, _predesc, _name);
                try
                {
                    SqlHelper.ExecuteNonQuery(GlobalParams.ConnnectionString, CommandType.Text, sql);
                }
                catch (Exception)
                {


                }

            }
        }
    }
}
