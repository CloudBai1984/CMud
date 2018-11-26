using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace cMud2
{
    public class TriggererMananger
    {
        List<Triggerer> _trigerers = new List<Triggerer>();
        MudCommunication _comm;



        public List<string> LoadedTriggers = new List<string>();
        public bool Enable
        { get; set; }

        private static TriggererMananger Instance;
        public static TriggererMananger GetInstance()
        {
            if (Instance == null)
                Instance = new TriggererMananger();
            return Instance;
        }

        private TriggererMananger()
        {
            _comm = MudCommunication.GetInstance();
            LoadTriggerer();
        }

        private void LoadTriggerer()
        {
            lock (GlobalParams.Locker)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(@"Config\Triggerer\默认.txt", Encoding.Default))
                    {
                        string Triggerer = string.Empty;
                        do
                        {
                            Triggerer = sr.ReadLine();
                            if (string.IsNullOrEmpty(Triggerer)) break;
                            try
                            {
                                string[] arrAlias = Triggerer.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                                Triggerer triggerer = new Triggerer();
                                triggerer.Regex = arrAlias[0];
                                triggerer.Command = arrAlias[1];
                                triggerer.Group = arrAlias[2];
                                triggerer.IsActive = bool.Parse(arrAlias[3]);
                                _trigerers.Add(triggerer);
                            }
                            catch (Exception) { }
                        } while (true);
                    }
                    if (!LoadedTriggers.Contains("默认.txt"))
                        LoadedTriggers.Add("默认.txt");
                }
                catch (Exception e)
                {

                    System.Windows.MessageBox.Show(e.Message + "\r\n" + e.StackTrace);
                }
            }

        }

        public void ReloadTriggerer()
        {
            string[] tmp = new string[LoadedTriggers.Count];
            LoadedTriggers.CopyTo(tmp);
            ClearTriggerer();
            foreach (string name in tmp)
            {
                LoadTriggerer(name);
            }
        }

        public void ExecuteCommand(string text)
        {
            if (Enable)
                foreach (Triggerer triggerer in _trigerers)
                {
                    if (triggerer.IsActive)
                    {
                        Regex regex = new Regex(triggerer.Regex, RegexOptions.Multiline);
                        if (regex.IsMatch(text))
                        {
                            _comm.SendText(triggerer.Command);
                        }
                    }
                }
        }

        public void DisableGroupTriggerer(string group)
        {
            foreach (Triggerer trig in _trigerers)
            {
                if (trig.Group.Equals(group))
                {
                    trig.IsActive = false;
                }
            }
        }

        public void EnableGroupTriggerer(string group)
        {
            foreach (Triggerer trig in _trigerers)
            {
                if (trig.Group.Equals(group))
                {
                    trig.IsActive = true;
                }
            }
        }

        public void LoadTriggerer(string fileName)
        {
            lock (GlobalParams.Locker)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(@"Config\Triggerer\" + fileName, Encoding.Default))
                    {
                        string Triggerer = string.Empty;
                        do
                        {
                            Triggerer = sr.ReadLine();
                            if (string.IsNullOrEmpty(Triggerer)) break;
                            try
                            {
                                string[] arrAlias = Triggerer.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                                Triggerer triggerer = new Triggerer();
                                triggerer.Regex = arrAlias[0];
                                triggerer.Command = arrAlias[1];
                                triggerer.Group = arrAlias[2];
                                triggerer.IsActive = bool.Parse(arrAlias[3]);
                                _trigerers.Add(triggerer);
                            }
                            catch (Exception) { }
                        } while (true);
                    }
                    if (!LoadedTriggers.Contains(fileName))
                        LoadedTriggers.Add(fileName);
                }
                catch (Exception e)
                {

                    System.Windows.MessageBox.Show(e.Message + "\r\n" + e.StackTrace);
                }
            }
        }

        public void ClearTriggerer()
        {
            _trigerers.Clear();
            LoadedTriggers.Clear();
        }
    }
}
