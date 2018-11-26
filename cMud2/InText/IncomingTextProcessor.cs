using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace cMud2
{
    public class IncomingTextProcessor
    {
        public event IncomingTextEventHandler incomingText;
        public delegate void IncomingTextEventHandler(string text);

        private static IncomingTextProcessor InTextProcessor;

        public static IncomingTextProcessor GetInstance()
        {
            if (InTextProcessor == null)
                InTextProcessor = new IncomingTextProcessor();
            return InTextProcessor;
        }

        private IncomingTextProcessor()
        {
        }
        private string _allText = string.Empty;
        public string AllText
        {
            get
            {
                return _allText;
            }
            set
            {
                if (_allText.Length > 20000)
                {
                    _allText.Substring(0, 1000);
                }
                _allText = value;
            }
        }
        public string CurrentText
        { get; set; }

        public void ProcessText()
        {
            if (incomingText != null)
                incomingText(CurrentText);
            new FullmeTextProcessor().ProcessText(CurrentText);
            TriggererMananger.GetInstance().ExecuteCommand(CurrentText);

        }
    }
}
