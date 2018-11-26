using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace cMud2
{

    public class Robot
    {


        public event RobetCompleteHandler RobotComplete;
        public delegate void RobetCompleteHandler();

        protected List<string> _allMessage = new List<string>();
        protected string _currentMessage;
        protected MudCommunication _comm = MudCommunication.GetInstance();
        protected IncomingTextProcessor _inTextProcessor = IncomingTextProcessor.GetInstance();
        protected Regex _regex;
        protected int _stepNumber;
        protected Int64 _stepTicks;

        public bool IsRuning
        { get; set; }
        public string Name
        { get; set; }
        public int StepNumber
        {
            get
            {
                return _stepNumber;
            }

        }
        public string AfterCmd
        {
            get;
            set;
        }
        public Robot()
        {
            _inTextProcessor.incomingText += new IncomingTextProcessor.IncomingTextEventHandler(GetIncomingText);
        }

        public virtual void Run()
        {
            if (IsRuning) return;
            IsRuning = true;
        }

        public virtual void Finish()
        {
            IsRuning = false;
            _comm.SendText("tell darkbyf " + Name + "机器人已完成任务！");
            _comm.SendText(AfterCmd);
        }

        public virtual void Stop()
        {
            _stepNumber = 0;
            IsRuning = false;
            _comm.SendText("tell darkbyf " + Name + "机器人已停止！");
        }

        public virtual void DealFadai()
        {

        }

        public virtual void RobotCompleteHandler()
        {
            if (RobotComplete != null)
            {
                RobotComplete();
            }
        }

        protected virtual void GoNext()
        {
        }

        protected virtual void GetIncomingText(string text)
        {
            if (IsRuning)
            {
                _currentMessage = text;
                _allMessage.Add(text);
                GoNext();
            }
        }
    }
}
