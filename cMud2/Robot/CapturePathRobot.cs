using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace cMud2
{
    public class CapturePathRobot : Robot
    {
        #region 字段
        string _currentArea = "武当";
        string _name;
        Hashtable _htPath = new Hashtable();
        Hashtable _htDirMapping = new Hashtable();//方向
       
        int _curretnStep;
        #endregion

        private static CapturePathRobot Instance;

        public static CapturePathRobot GetInstance()
        {
            if (Instance == null)
            {
                Instance = new CapturePathRobot();
            }
            return Instance;
        }

        private CapturePathRobot()
            : base()
        {
            Name = "抓取路径";
            _htDirMapping.Add("west","east");
            _htDirMapping.Add("east", "west");
            _htDirMapping.Add("north", "south");
            _htDirMapping.Add("south", "north");
            _htDirMapping.Add("northwest", "southeast");
            _htDirMapping.Add("northeast", "southwest");
            _htDirMapping.Add("southwest", "northeast");
            _htDirMapping.Add("southeast", "northwest");
            _htDirMapping.Add("westup", "eastdown");
            _htDirMapping.Add("eastup", "westdown");
            _htDirMapping.Add("northup", "southdown");
            _htDirMapping.Add("southup", "northdown");
            _htDirMapping.Add("westdown", "eastup");
            _htDirMapping.Add("eastdown", "westup");
            _htDirMapping.Add("northdown", "southup");
            _htDirMapping.Add("southdown", "northup");
            _htDirMapping.Add("out", "enter");
            _htDirMapping.Add("enter", "out");
            _htDirMapping.Add("up", "down");
            _htDirMapping.Add("down", "up");
        }

        public override void Run()
        {
            base.Run();
        }

        protected override void GoNext()
        {
            base.GoNext();
            _curretnStep = 1;
            FindPath(_curretnStep);
        }

        private void FindPath(int steps)
        {

        }

    
    }
}
