using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace cMud2
{
    public partial class MudCommunication
    {
        private static MudCommunication Instance;
        public static MudCommunication GetInstance()
        {
            if (Instance == null)
                 Instance = new MudCommunication();
            return Instance;
        }
        #region 字段
        TcpClient _tcpClient = new TcpClient();
        NetworkStream _stream;
        byte[] _rcvBuffer = new byte[1024 * 64];
        IncomingTextProcessor _textProcessor = IncomingTextProcessor.GetInstance();

        TelnetParser _telnetParser;
        ANSIColorParser _ansiColorParser = new ANSIColorParser();
        Queue<string> _queueCmds = new Queue<string>();//命令队列
        int fadai;//发呆时间
        #endregion

        #region 事件定义
        //disconnection callback and handler definition
        public event disconnectionEventHandler disconnected;
        public delegate void disconnectionEventHandler();

        //incoming message callback and handler definition
        public event serverMessageEventHandler serverMessage;
        public delegate void serverMessageEventHandler(List<MUDTextRun> runs);

        //incoming telnet control sequence callback and handler definition
        public event serverTelnetEventHandler telnetMessage;
        public delegate void serverTelnetEventHandler(string message);

        public event FadaiEventHandler FadaiHandler;
        public delegate void FadaiEventHandler();
        #endregion

        #region  构造函数
        private MudCommunication()
        {
            System.Threading.Thread td = new System.Threading.Thread(new System.Threading.ThreadStart(new Action(() =>
                 {
                     while (true)
                     {
                         System.Threading.Thread.Sleep(100);
                         CheckFadai();
                         GlobalVariable.Ticks++;
                     }

                 })));
            td.IsBackground = true;
            td.Start();
        }
        #endregion
        //called when receiving any message
        void handleServerMessage(IAsyncResult result)
        {
            try
            {
                NetworkStream ns;
                //get length of data in buffer
                int receivedCount;
                try
                {
                    ns = (NetworkStream)result.AsyncState;
                    receivedCount = ns.EndRead(result);
                }
                catch
                {
                    //if there was any issue reading the server text, ignore the message (what else can we do?)
                    return;
                }

                //0 bytes received means the server disconnected
                if (receivedCount == 0)
                {
                    this.Disconnect();
                    return;
                }

                //list of bytes which aren't telnet sequences
                //ultimately, this will be the original buffer minus any telnet messages from the server
                List<string> telnetMessages;
                List<byte> contentBytes = this._telnetParser.HandleAndRemoveTelnetBytes(this._rcvBuffer, receivedCount, out telnetMessages);

                //report any telnet sequences seen to the caller
                App.Current.Dispatcher.BeginInvoke(new Action(delegate
                {
                    foreach (string telnetMessage in telnetMessages)
                    {
                        //fire the "received a server message" event
                        this.telnetMessage(telnetMessage);
                    }
                }));

                //now we've filtered-out and responded accordingly to any telnet data.
                //next, convert the actual MUD content of the message from ASCII to Unicode
                string message = AsciiDecoder.AsciiToUnicode(contentBytes.ToArray(), contentBytes.Count);
                _textProcessor.AllText += message;
                _textProcessor.CurrentText = message;
                _textProcessor.ProcessText();
                //run the following on the main thread so that calling code doesn't have to think about threading

                if (this.serverMessage != null)
                {
                    App.Current.Dispatcher.BeginInvoke(new Action(delegate
                    {

                        //pass the message to the mudTranslator to parse any ANSI control sequences (colors!)
                        List<MUDTextRun> runs = this._ansiColorParser.Translate(message);

                        //fire the "received a server message" event with the runs to be displayed
                        this.serverMessage(runs);
                    }));
                }

                //now that we're done with this message, listen for the next message
                ns.BeginRead(_rcvBuffer, 0, _rcvBuffer.Length, new AsyncCallback(handleServerMessage), _stream);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message + "\r\n" + e.StackTrace);
            }
        }

        public void TestMessage(string text)
        {
            string message = text;
            _textProcessor.AllText += message;
            _textProcessor.CurrentText = message;
            _textProcessor.ProcessText();
            //run the following on the main thread so that calling code doesn't have to think about threading

            if (this.serverMessage != null)
            {
                App.Current.Dispatcher.BeginInvoke(new Action(delegate
                {

                    //pass the message to the mudTranslator to parse any ANSI control sequences (colors!)
                    List<MUDTextRun> runs = this._ansiColorParser.Translate(message);

                    //fire the "received a server message" event with the runs to be displayed
                    this.serverMessage(runs);
                }));
            }
        }

        public void CheckFadai()
        {
            fadai++;
            if (fadai >= GlobalParams.Fadai)
            {
                SendText("#test 发呆了~，开始启动发呆处理程序~");
                if (FadaiHandler != null)
                {
                    SendText("fd");
                    FadaiHandler();
                }
                fadai = 0;
            }
            if (GlobalVariable.Ticks % 200 == 0 && GlobalVariable.Ticks>500)
            {
                _alias.SaveAlias();
                CustomVariable.SaveVariable();
                TestMessage(".......................................");
            }
            if (GlobalParams.Timer != 0)
            {
                if (GlobalVariable.Ticks % GlobalParams.Timer == 0)
                {
                    SendText("timer");
                }
            }
        }

        private void Send(string cmd)
        {
            cmd = cmd + "\r\n";
            byte[] sndBuffer = Encoding.Default.GetBytes(cmd);
            string outText = AsciiDecoder.AsciiToUnicode(sndBuffer, sndBuffer.Length);
            _textProcessor.AllText += outText;
            List<MUDTextRun> runs = this._ansiColorParser.Translate((char)27 + "[1;33m" + cmd.Replace("\r\n", "") + (char)27 + "[0;66m");
            //fire the "received a server message" event with the runs to be displayed
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (serverMessage != null)
                    this.serverMessage(runs);
            }));

            try
            {
                //send to server
                _stream.Write(sndBuffer, 0, sndBuffer.Length);
            }
            catch (Exception e)
            {
                Disconnect();
                Connect("mud.pkuxkx.net", 8080);
            }
        }

        public void SendText(string text)
        {
            //  if (!this._tcpClient.Connected) return;
            try
            {
                fadai = 0;

                System.Threading.Thread td = new System.Threading.Thread(new System.Threading.ThreadStart(new Action(() =>
                {
                    ExecuteCommandLine(text);
                })));
                td.IsBackground = true;
                td.Start();

            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message + "\r\n" + e.StackTrace);
            }
        }

        internal void Connect(string address, int port)
        {
            try
            {
                _tcpClient.Connect(address, port);
            }
            catch (Exception e)
            {
                this.Disconnect();
                this.Connect("mud.pkuxkx.net", 8080);

            }
            if (this._tcpClient.Connected)
            {
                //initialize the telnet parser
                this._telnetParser = new TelnetParser(this._tcpClient);
                _stream = _tcpClient.GetStream();
                _stream.BeginRead(_rcvBuffer, 0, _rcvBuffer.Length, new AsyncCallback(handleServerMessage), _stream);
                this._telnetParser.sendTelnetBytes((byte)Telnet.WILL, (byte)Telnet.NAWS);
            }
        }

        internal void Disconnect()
        {
            //if not connected, do nothing

            //close the connection
            this._tcpClient.Close();

            //initialize a new object
            this._tcpClient = new TcpClient();

            //fire disconnection notification event on main UI thread
            if (this.disconnected != null)
            {
                App.Current.Dispatcher.BeginInvoke(new Action(delegate
                {
                    this.disconnected.Invoke();
                }));
            }
        }

    }
}
