using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace cMud2
{
    public class FullmeTextProcessor
    {
        public string Message
        { get; set; }
        public bool IsNeedGetNextMessage
        { get; set; }

        public void ProcessText(string text)
        {
            Regex regex = new Regex(@"http://mud.pkuxkx.net/antirobot/robot.php\?filename=\d+");
            Match m = regex.Match(text);
            if (m.Length > 1)
            {
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ShowRobotWindow(m.Value);
                    }));
            }
        }

        private void ShowRobotWindow(string uri)
        {
            Window win = new Window();
            win.Topmost = true;
            WebBrowser web = new WebBrowser();
            win.Content = web;
            web.Navigate(uri);
            win.Show();
        }
    }
}
