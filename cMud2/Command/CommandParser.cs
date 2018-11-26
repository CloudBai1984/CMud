using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace cMud2
{
    public partial class MudCommunication
    {
        AliasProcessor _alias = AliasProcessor.GetInstance();
        private int ContinuingCmdCount = 0;
        public void ExecuteCommandLine(string text)
        {
            char[] cmdChars = text.ToCharArray();
            StringBuilder sb = new StringBuilder();
            Stack<char> bracks = new Stack<char>();
            for (int i = 0; i < cmdChars.Length; i++)
            {
                char c = cmdChars[i];
                if (c == '{')
                {
                    bracks.Push(cmdChars[i]);
                    sb.Append(cmdChars[i]);
                }
                else if (c == '}')
                {
                    bracks.Pop();
                    sb.Append(cmdChars[i]);
                }
                else if (c == ';' && bracks.Count == 0)
                {
                    string cmd = sb.ToString();
                    ParserAndExecute(cmd);
                    sb.Clear();
                }
                else
                {
                    sb.Append(cmdChars[i]);
                }
            }
            if (sb.Length > 0)
            {
                string cmd = sb.ToString();
                ParserAndExecute(cmd);
                sb.Clear();
            }
        }

        private void ParserAndExecute(string cmd)
        {
            if (cmd.StartsWith("#set"))
            {
                AddCustomVariable(cmd);
            }
            else if (cmd.StartsWith("#calc"))
            {

            }
            else
            {
                cmd = ProcessCustomVariable(cmd);
                if (cmd.StartsWith("#wa"))
                {
                    ProcessWaitCmd(cmd);
                }
                else if (Regex.IsMatch(cmd, @"^#\d+\s", RegexOptions.Compiled))
                {
                    PorcessMultipleCmd(cmd);
                }
                else if (cmd.StartsWith("#test"))
                {
                    TestMessage((char)27 + "[1;32m" + Regex.Match(cmd, @"#test\s(?<msg>.+)", RegexOptions.Compiled | RegexOptions.Singleline).Groups["msg"].Value + (char)27 + "[0;66m");
                }
                else if (cmd.StartsWith("#alias"))
                {
                    AddAlias(cmd);
                }
                else if (cmd.StartsWith("#mess"))
                {
                    ShowMessage(cmd);
                }
                else if (cmd.StartsWith("#entrg"))
                {
                    EnableTriggerer(cmd);
                }
                else if (cmd.StartsWith("#distrg"))
                {
                    DisableTriggerer(cmd);
                }
                else if (cmd.Equals("#showtrig"))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("#test ");
                    sb.Append("=================已启用的Triggerer=================\r\n");
                    foreach (string name in TriggererMananger.GetInstance().LoadedTriggers)
                    {
                        sb.Append(name);
                        sb.Append("\r\n");
                    }
                    sb.Append("===================================================\r\n");
                    SendText(sb.ToString());
                }
                else if (cmd.Equals("#showvar"))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("#test ");
                    sb.Append("=================自定义变量一览=================\r\n");
                    foreach (string name in CustomVariable.Variables.Keys)
                    {
                        sb.Append(name.Remove(0, 1));
                        sb.Append("=");
                        sb.Append(CustomVariable.Variables[name].ToString());
                        sb.Append("\r\n");
                    }
                    sb.Append("================================================\r\n");
                    SendText(sb.ToString());
                }
                else if (cmd.StartsWith("#stimer"))
                {
                    StartTimer(cmd);
                }

                else if (cmd.StartsWith("#etimer"))
                {
                    GlobalParams.Timer = 0;
                }
                else if (cmd.StartsWith("#if"))
                {
                    ProcessIfStatment(cmd);
                }
                else
                {
                    string tmpCmd = _alias.GetFullText(cmd);
                    if (!tmpCmd.Equals(cmd))
                    {
                        ExecuteCommandLine(tmpCmd);
                    }
                    else
                    {
                        ContinuingCmdCount++;
                        if (ContinuingCmdCount > 5)
                        {
                            Util.Wait(500);
                            ContinuingCmdCount = 0;
                        }
                        Send(cmd);
                    }
                }
            }
        }

        private string ProcessAlias(string cmdLine)
        {
            string[] cmds = cmdLine.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            foreach (string cmd in cmds)
            {
                string tmpCmd = _alias.GetFullText(cmd);
                if (tmpCmd.Contains(";"))
                {
                    tmpCmd = ProcessAlias(tmpCmd);
                }
                sb.Append(tmpCmd);
                sb.Append(";");
            }
            return sb.ToString();
        }
        private void PorcessMultipleCmd(string cmd)//例如：#3 eat baicai
        {
            Regex regex = new Regex(@"^#(?<times>\d+)\s(?<cmd>.+)", RegexOptions.Compiled);
            Match match = regex.Match(cmd);
            int times = int.Parse(match.Groups["times"].Value);
            string tmpCmd = match.Groups["cmd"].Value;
            for (int i = 0; i < times; i++)
            {
                string tmpCmd2 = _alias.GetFullText(tmpCmd);
                if (!tmpCmd.Equals(cmd))
                {
                    ExecuteCommandLine(tmpCmd);
                }
                else
                {
                    ContinuingCmdCount++;
                    if (ContinuingCmdCount > 5)
                    {
                        Util.Wait(500);
                        ContinuingCmdCount = 0;
                    }
                    Send(cmd);
                }
            }

        }
        private void ProcessWaitCmd(string cmd)//例如：#wa 1000
        {
            try
            {
                string[] arrTmp = cmd.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                Util.Wait(Int32.Parse(arrTmp[1]));
            }
            catch (Exception)
            {

            }

        }
        private void AddAlias(string cmd)
        {
            Regex regex = new Regex(@"^#alias\s(?<alias>\w+)(?:\s(?<cmd>.+)|)", RegexOptions.Compiled);
            Match match = regex.Match(cmd);
            string alias = match.Groups["alias"].Value;
            string tmpCmd = match.Groups["cmd"].Value;
            if (tmpCmd.StartsWith("{"))
            {
                tmpCmd = tmpCmd.Remove(0, 1);
                tmpCmd = tmpCmd.Remove(tmpCmd.Length - 1);
            }

            _alias.AddAlias(alias, tmpCmd);
        }
        private void ShowMessage(string cmd)
        {
            System.Windows.MessageBox.Show(Regex.Match(cmd, @"^#mess\s(?<msg>.+)", RegexOptions.Compiled).Groups["msg"].Value);
        }
        private void EnableTriggerer(string cmd)
        {
            TriggererMananger.GetInstance().EnableGroupTriggerer(Regex.Match(cmd, @"#entrg\s(?<group>.+)", RegexOptions.Compiled).Groups["group"].Value);
        }
        private void DisableTriggerer(string cmd)
        {
            TriggererMananger.GetInstance().DisableGroupTriggerer(Regex.Match(cmd, @"#distrg\s(?<group>.+)", RegexOptions.Compiled).Groups["group"].Value);
        }
        private void ProcessTriggerer(string cmd)
        {

        }
        private void AddCustomVariable(string cmd)
        {
            GroupCollection groups = Regex.Match(cmd, @"#set\s(?<name>@[a-z,A-z,0-9]+)=?(?<value>.+)?", RegexOptions.Compiled).Groups;
            string value = groups["value"].Value;
            if (value.StartsWith("@"))
            {
                value = CustomVariable.GetVariable(value);
            }
            CustomVariable.AddVariable(groups["name"].Value, value);
        }
        private string ProcessCustomVariable(string cmd)
        {
            if (cmd.Contains('@'))
            {
                MatchCollection matches = Regex.Matches(cmd, @"@[a-z,A-z,0-9]+", RegexOptions.Compiled);
                foreach (Match match in matches)
                {
                    string name = match.Value;
                    cmd = cmd.Replace(name, CustomVariable.GetVariable(name));
                }
            }
            return cmd;
        }
        private void StartTimer(string cmd)
        {

            try
            {
                Match match = Regex.Match(cmd, @"#stimer\s(?<time>\d+)\s{?(?<cmd>[^}]+)}?", RegexOptions.Compiled);
                string time = match.Groups["time"].Value;
                GlobalParams.Timer = int.Parse(time);
                AliasProcessor.GetInstance().AddAlias("timer", match.Groups["cmd"].Value);
            }
            catch (Exception)
            {

            }

        }
        private void ProcessIfStatment(string cmd)
        {
            MatchCollection matches = Regex.Matches(cmd, @"\((?<condition>.+?)\){(?<cmd>.+?)}", RegexOptions.Compiled);
            foreach (Match match in matches)
            {
                string condition = match.Groups["condition"].Value;
                try
                {
                    if (Util.ParseBooleanExpression(condition))
                    {
                        string cmdline = match.Groups["cmd"].Value;
                        SendText(cmdline);
                        break;
                    }
                }
                catch (Exception e)
                {
                    SendText("#test Error:" + e.Message);
                }


            }
        }
        private void ProcessCalcuate(string cmd)
        {
            try
            {
                GroupCollection groups = Regex.Match(cmd, @"#calc\s(?<name>@[a-z,A-z,0-9]+)=(?<value>.+)?", RegexOptions.Compiled).Groups;
                string value = groups["value"].Value;
                value = Regex.Replace(value, @"@[a-z,A-z,0-9]+", new MatchEvaluator(new Func<Match, string>(m =>
                {
                    return CustomVariable.GetVariable(m.Value);
                })), RegexOptions.Compiled);
                double result = Util.ParseArithmeticalExpression(value);
                CustomVariable.AddVariable(groups["name"].Value, result.ToString());
            }
            catch (Exception e)
            {
                SendText("#test Error:" + e.Message);
            }

        }
    }
}
