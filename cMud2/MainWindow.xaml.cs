using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Diagnostics;
using cMud2.UI;
using Microsoft.Win32;

namespace cMud2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 字段
        MudCommunication _comm;
        List<string> _lstCmdRecorder = new List<string>();
        int _intCurrentCmdIndex = 0;
        IncomingTextProcessor _inTextPorc = IncomingTextProcessor.GetInstance();
        RobotManager _rbtManager = RobotManager.GetInstance();
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        #region event
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.tbxInput.Focus();
            try
            {
                _comm.Connect("106.14.184.170", 8080);
                TriggererMananger.GetInstance().Enable = muiTriggererEnable.IsChecked;
            }
            catch (Exception e1)
            {

            }

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AliasProcessor.GetInstance().SaveAlias();
            CustomVariable.SaveVariable();
        }
        private void tbxInput_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    string cmd = tbxInput.Text.Replace("\r\n", "");
                    _comm.SendText(cmd);
                    if (cmd.Length >= 2)
                    {
                        if (!_lstCmdRecorder.Contains(cmd))
                            _lstCmdRecorder.Add(cmd);
                        if (_lstCmdRecorder.Count >= 20)
                        {
                            _lstCmdRecorder.RemoveAt(0);
                        }
                    }
                    _intCurrentCmdIndex = _lstCmdRecorder.Count;
                    tbxInput.SelectAll();
                    break;
                case Key.Up:
                    if (_intCurrentCmdIndex > 0)
                    {
                        tbxInput.Text = _lstCmdRecorder[--_intCurrentCmdIndex];
                    }
                    tbxInput.SelectAll();
                    break;
                case Key.Down:
                    if (_intCurrentCmdIndex < _lstCmdRecorder.Count - 1)
                    {
                        tbxInput.Text = _lstCmdRecorder[++_intCurrentCmdIndex];
                    }
                    tbxInput.SelectAll();
                    break;
                default:
                    break;
            }
        }
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            _comm.Disconnect();
            _comm.Connect("mud.pkuxkx.net", 8080);
        }
        private void muiEditAlias_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad", Environment.CurrentDirectory + @"\Config\Alias.txt");
        }
        private void muiLoadAlias_Click(object sender, RoutedEventArgs e)
        {
            AliasProcessor.GetInstance().ReloadAlias();
        }
        private void muiOriginalText_Click(object sender, RoutedEventArgs e)
        {
            Window win = new Window();
            TextBox tbx = new TextBox();
            tbx.Text = _inTextPorc.AllText;
            tbx.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            win.Content = tbx;
            win.ShowDialog();
        }
        private void TestMsg_Click(object sender, RoutedEventArgs e)
        {
            TestMessageWindow win = new TestMessageWindow();
            win.ShowDialog();
            if (win.Message != null)
                _comm.TestMessage(win.Message);
        }
        private void muiEditTriggerer_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + @"Config\Triggerer";
            dlg.ShowDialog();
            string fileName = dlg.SafeFileName;
            if (string.IsNullOrEmpty(fileName)) return;
            Process.Start("notepad", Environment.CurrentDirectory + @"\Config\Triggerer\" + fileName);
        }
        private void muiReloadTriggerer_Click(object sender, RoutedEventArgs e)
        {
            TriggererMananger.GetInstance().ReloadTriggerer();
        }
        private void muiTriggererEnable_Click(object sender, RoutedEventArgs e)
        {
            muiTriggererEnable.IsChecked = muiTriggererEnable.IsChecked == true ? false : true;
            if (muiTriggererEnable.IsChecked == true)
            {
                TriggererMananger.GetInstance().Enable = true;
            }
            else
            {
                TriggererMananger.GetInstance().Enable = false;
            }
        }
        private void muiLoadTriggerer_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + @"Config\Triggerer";
            dlg.ShowDialog();
            string fileName = dlg.SafeFileName;
            if (string.IsNullOrEmpty(fileName)) return;
            TriggererMananger.GetInstance().LoadTriggerer(fileName);
        }
        private void muiClearTriggerer_Click(object sender, RoutedEventArgs e)
        {
            TriggererMananger.GetInstance().ClearTriggerer();
        }
        #region 机器人
        private void muiStopRobot_Click(object sender, RoutedEventArgs e)
        {
            _rbtManager.StopAll();
        }

        private void muiQuest1_Click(object sender, RoutedEventArgs e)
        {
            _rbtManager.StartRobot("LianDan");
        }
        private void muiQuest2_Click(object sender, RoutedEventArgs e)
        {
            _rbtManager.StartRobot("ZhenFa");
        }
        private void muiQuest3_Click(object sender, RoutedEventArgs e)
        {
            _rbtManager.StartRobot("SongJing");
        }
        private void muiQuest4_Click(object sender, RoutedEventArgs e)
        {
            _rbtManager.StartRobot("CaiQi");
        }
        private void muiQuest5_Click(object sender, RoutedEventArgs e)
        {
            _rbtManager.StartRobot("WDNew");
        }
        #endregion
        #endregion

        #region 通讯事件处理
        void serverConnection_telnetMessage(string message)
        {

        }

        //when the server disconnects, notify the user via the output box
        void serverConnection_disconnected()
        {
            this.appendText("Disconnected.");
        }

        //when a content message arrives, display it in the main output box
        void serverConnection_serverMessage(List<MUDTextRun> genericRuns)
        {

            //convert the generic "MUD Text Runs" to "WPF Runs" so that they can be displayed in the UI
            List<Run> wpfRuns = new List<Run>();
            {
                foreach (MUDTextRun genericRun in genericRuns)
                {
                    Run newRun = new Run(genericRun.Content);
                    newRun.Foreground = new SolidColorBrush(this.getColor(genericRun.ForegroundColor));
                    newRun.Background = new SolidColorBrush(this.getColor(genericRun.BackgroundColor));
                    if (string.IsNullOrEmpty(genericRun.Content)) continue;
                    wpfRuns.Add(newRun);
                }
            }

            //display them
            this.appendRuns(wpfRuns.ToArray());


        }

        void DealFadai()
        {
            _rbtManager.DealFadai();
        }
        #endregion

        #region 其他
        //displays plain text in the main output window (by turning it into a WPF run first)
        private void appendText(string message)
        {
            //add a line to the output box
            Run run = new Run(message);
            run.Foreground = new SolidColorBrush(Colors.White);
            run.Background = new SolidColorBrush(Colors.CornflowerBlue);
            this.appendRuns(run);
        }
        ScrollViewer descendantScrollViewer;
        //displays rich text in the main output window
        private void appendRuns(params Run[] runs)
        {
            //create a new "paragraph" element, vertically separating this bunch of runs from the previous bunch

            Paragraph newParagraph = new Paragraph();

            //fill it with the provided runs
            newParagraph.Inlines.AddRange(runs);

            //add it to the document in the output box
            this.fdcOutput.Document.Blocks.Add(newParagraph);
            fdcOutput.Document.LineHeight = 1;
            if (this.fdcOutput.Document.Blocks.Count > 2000)
            {
                if (cbxAutoScroll.IsChecked == true)
                {
                    for (int i = 0; i < 100; i++)
                        this.fdcOutput.Document.Blocks.Remove(fdcOutput.Document.Blocks.FirstBlock);
                }
            }

            //automatically scroll to the bottom
            if (descendantScrollViewer == null)
                descendantScrollViewer = findScrollViewerDescendant(this.fdcOutput);

            if (descendantScrollViewer != null && cbxAutoScroll.IsChecked == true)
                descendantScrollViewer.ScrollToEnd();
        }

        //helper for above, because FlowDocumentScrollViewer doesn't have a convenient ScrollToEnd() method
        //if this looks like black magic, that's because it is (this is not a fun area of WPF)
        private static ScrollViewer findScrollViewerDescendant(DependencyObject control)
        {
            if (control is ScrollViewer) return (control as ScrollViewer);

            int childCount = VisualTreeHelper.GetChildrenCount(control);
            for (int i = 0; i < childCount; i++)
            {
                ScrollViewer result = findScrollViewerDescendant(VisualTreeHelper.GetChild(control, i));
                if (result != null) return result;
            }

            return null;
        }
        //associates an actual color with each of the 15 color numbers used by servers
        //any modern client should make these user-customizable!
        //the color values used in this color theme come from the ANSI control sequence page on wikipedia. they're garish.
        private Color getColor(int colorNumber)
        {
            switch (colorNumber)
            {
                //colors 0 through 7 are basic colors
                case 0:
                    return Color.FromRgb(0, 0, 0);
                case 1:
                    return Color.FromRgb(128, 0, 0);
                case 2:
                    return Color.FromRgb(0, 128, 0);
                case 3:
                    return Color.FromRgb(128, 128, 0);
                case 4:
                    return Color.FromRgb(0, 0, 128);
                case 5:
                    return Color.FromRgb(128, 0, 128);
                case 6:
                    return Color.FromRgb(0, 128, 128);
                case 7:
                    return Color.FromRgb(192, 192, 192);

                //colors 8 through 15 are "intense" versions of the basic colors above
                //in this example, 7 is medium gray, and its corresponding "intense" version at 15 is bright white                
                case 8:
                    return Color.FromRgb(128, 128, 128);
                case 9:
                    return Color.FromRgb(255, 0, 0);
                case 10:
                    return Color.FromRgb(0, 255, 0);
                case 11:
                    return Color.FromRgb(255, 255, 0);
                case 12:
                    return Color.FromRgb(0, 0, 255);
                case 13:
                    return Color.FromRgb(255, 0, 255);
                case 14:
                    return Color.FromRgb(0, 255, 255);
                case 100:
                    return Color.FromRgb(255, 255, 255);
                default: //case 15
                    return Color.FromRgb(39, 128, 0);
            }
        }
        private void Init()
        {
            CustomVariable.InitVariable();
            _comm = MudCommunication.GetInstance();
            this._comm.serverMessage += new MudCommunication.serverMessageEventHandler(serverConnection_serverMessage);
            this._comm.disconnected += new MudCommunication.disconnectionEventHandler(serverConnection_disconnected);
            this._comm.telnetMessage += new MudCommunication.serverTelnetEventHandler(serverConnection_telnetMessage);
            this._comm.FadaiHandler += new MudCommunication.FadaiEventHandler(DealFadai);
        }
        #endregion
    }
}
