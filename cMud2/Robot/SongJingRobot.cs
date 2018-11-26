using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;

namespace cMud2
{
    public class SongJingRobot : Robot
    {
        private static SongJingRobot Instance;

        public static SongJingRobot GetInstance()
        {
            if (Instance == null)
            {
                Instance = new SongJingRobot();
            }
            return Instance;
        }

        private SongJingRobot()
            : base()
        {
            Name = "诵经";
        }

        string _book = string.Empty;
        string _location = string.Empty;
        string _chapter = string.Empty;
        int _page = 1;
        int _failtimes = 0;

        #region Step
        private void DoStep1()
        {
            _comm.SendText("#goto 复真观四层;get jing;#wa 1000;chanting 1 1;drop jing;emote 诵经第一步完成");
            _stepNumber = 1;

        }
        private void DoStep2(Regex regex)
        {

            if (Regex.IsMatch(_currentMessage, "云自遥诵经第一步完成"))
            {
                if (string.IsNullOrEmpty(_location))
                {
                    DoStep1();
                    _failtimes++;
                    if (_failtimes > 10)
                        Stop();
                    return;
                }
                _failtimes = 0;

                DataSet ds = Util.GetPathDataSet("武当", _location, 1);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    _comm.SendText("emote 没有找到诵经地址!;#goto 武当广场;ask chongxu about cancel;#wa 2000;ask chongxu about quest");
                    Stop();
                    return;
                }
                _comm.SendText("#goto 复真观二层;#wa 2000;jie " + _book + ";#wa 2000;emote 找知客借书");
                _stepNumber = 2;
                _stepTicks = GlobalVariable.Ticks;
            }

        }
        private void DoStep3()
        {

            if (Regex.IsMatch(_currentMessage, "云自遥找知客借书"))
            {
                _comm.SendText("emote 借书完成");
                _stepTicks = GlobalVariable.Ticks;
            }
            else if (Regex.IsMatch(_currentMessage, "云自遥借书完成"))
            {
                _stepTicks = GlobalVariable.Ticks;
                _comm.SendText("#goto " + _location + ";l;#wa 2000;emote 到达诵经地点");
                _stepNumber = 3;
            }
            else if (!FindPathRobot.GetInstance().IsRuning)
            {
                if (GlobalVariable.Ticks - _stepTicks > 200)
                {
                    _comm.SendText("emote 找知客借书");
                    _stepTicks = GlobalVariable.Ticks;
                }

            }
        }
        private void DoStep4()
        {

            if (Regex.IsMatch(_currentMessage, @"云自遥到达诵经地点", RegexOptions.Multiline))
            {
                if (!GlobalVariable.CurrentLocation.Equals(_location))
                {
                    _comm.SendText("#goto " + _location + ";l;emote 到达诵经地点!");
                    return;
                }
                _stepNumber = 4;
                _comm.SendText("emote 查找书页");
                _stepTicks = GlobalVariable.Ticks;
            }
            else if (!FindPathRobot.GetInstance().IsRuning)
            {
                if (GlobalVariable.Ticks - _stepTicks > 2000)
                {
                    _comm.SendText("l;emote 到达诵经地点");
                    _stepTicks = GlobalVariable.Ticks;
                }

            }
        }
        private void DoStep5()
        {
            string tmpChapter = Regex.Match(_chapter, @"(?<name>.*)\(").Groups["name"].Value;
            int tmpChapterNumber = 0;
            if (!int.TryParse(Regex.Match(_chapter, @".*\((?<num>\d+)\)").Groups["num"].Value, out tmpChapterNumber))
            {
                _comm.SendText("chanting 1 1;#wa 1000;page " + _page);
                return;
            }

            //int tmpChapterNumber = 
            if (Regex.IsMatch(_currentMessage, @"^.*云自遥查找书页"))
            {
                _comm.SendText("page " + _page);
                _stepTicks = GlobalVariable.Ticks;
            }
            else if(Regex.IsMatch(_currentMessage, @".*杀你", RegexOptions.Multiline))
            {
                _comm.SendText("halt;#goto 武当广场;ask chongxu about quest");
                Stop();
                return;
            }
            else if (Regex.IsMatch(_currentMessage, @".*===========.*==========.*第.*页/总.*页", RegexOptions.Singleline))
            {
                _stepTicks = GlobalVariable.Ticks;
                if (Regex.IsMatch(_currentMessage, tmpChapter))
                {
                    int currentNumer = 0;
                    if (!int.TryParse(Regex.Match(_currentMessage, @tmpChapter + @"\((?<num>\d+)\)").Groups["num"].Value, out currentNumer))
                    {
                        _page++;
                        _comm.SendText("chanting 1 1;#wa 1000;page " + _page);
                        return;
                    }
                    _stepNumber = 5;
                    _page = _page + (tmpChapterNumber - currentNumer);
                    _comm.SendText("page " + _page + ";emote 找到书页");
                    _stepTicks = GlobalVariable.Ticks;
                }
                else
                {
                    _page += 10;
                    _comm.SendText("page " + _page);
                }
            }
            else if (Regex.IsMatch(_currentMessage, @"^这本.*一共只有.*"))
            {
                _stepTicks = GlobalVariable.Ticks;
                _page = 5;
                _comm.SendText("page " + _page);
            }
            else
            {
                if (GlobalVariable.Ticks - _stepTicks > 200)
                {
                    _comm.SendText("emote 查找书页");
                    _stepTicks = GlobalVariable.Ticks;
                }

            }
        }

        private void DoStep6()
        {
            if (Regex.IsMatch(_currentMessage, @".*===========.*==========.*第.*页/总.*页", RegexOptions.Singleline))
            {
                Regex regex = new Regex(@"==\[2;37;0m\s+(?<content>.*?)\s+\[");
                MatchCollection matches = regex.Matches(_currentMessage);
                string content = string.Empty;
                foreach (Match match in matches)
                {
                    content += match.Groups["content"].Value;
                }
                _comm.SendText("chanting " + _page + " " + content + ";#wa 5000;chanting " + _page + " " + content);
                _stepNumber = 6;
            }
            else
            {
                if (GlobalVariable.Ticks - _stepTicks > 200)
                {
                    _comm.SendText("page " + _page);
                    _stepTicks = GlobalVariable.Ticks;
                }

            }
        }

        private void DoStep7()
        {
            if (Regex.IsMatch(_currentMessage, @"你做完了冲虚道长布置的功课", RegexOptions.Singleline))
            {
                _comm.SendText("#wa 2000;#goto 复真观二层;give zhike jing;#wa 3000;#goto 武当广场;wc");
                Finish();
            }
            else if (Regex.IsMatch(_currentMessage, @"你诵经的内容和.*内容不符。"))
            {
                _stepNumber = 5;
                _comm.SendText("page " + _page);
                _stepTicks = GlobalVariable.Ticks;
            }
            else if (Regex.IsMatch(_currentMessage, @"诵经的格式是"))
            {
                _stepNumber = 5;
                _comm.SendText("page " + _page);
                _stepTicks = GlobalVariable.Ticks;
            }
            else
            {
                if (GlobalVariable.Ticks - _stepTicks > 2500)
                {
                    _comm.SendText("#wa 2000;#goto 复真观二层;give zhike jing;#wa 3000;#goto 武当广场;wc");
                    Finish();
                    _stepTicks = GlobalVariable.Ticks;
                }
            }

        }
        #endregion
        public override void Finish()
        {
            Stop();
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
            _regex = new Regex(@"^>?\s?.*诵经任务.*要求你拿着\[.*m(?<book>.*)\[.*在\[.*m(?<location>.*)\[.*诵唱\[.*m(?<chapter>.*)$", RegexOptions.Multiline);
            if (_regex.IsMatch(_currentMessage))
            {
                GroupCollection groups = _regex.Match(_currentMessage).Groups;
                _book = groups["book"].Value;
                _location = groups["location"].Value;
                _chapter = groups["chapter"].Value;
            }
            switch (_stepNumber)
            {
                case 1:
                    DoStep2(_regex);
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
            }
        }



    }
}
