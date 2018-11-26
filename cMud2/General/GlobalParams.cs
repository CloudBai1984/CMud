using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace cMud2
{
    public class GlobalParams
    {
        private static string connStr;
        public static string ConnnectionString
        {
            get
            {
                if (connStr == null)
                {
                    connStr = System.Configuration.ConfigurationManager.ConnectionStrings["Laptop"].ConnectionString;
                }
                return connStr;
            }
            private set{}
        }

        public static int Fadai = 300;

        private static Hashtable directionMapping;
        public static Hashtable DirectionMapping
        {
            get
            {
                if (directionMapping == null)
                {
                    directionMapping = new Hashtable();
                    directionMapping.Add("w", "e");
                    directionMapping.Add("e", "w");
                    directionMapping.Add("n", "s");
                    directionMapping.Add("s", "n");
                    directionMapping.Add("nw", "se");
                    directionMapping.Add("ne", "sw");
                    directionMapping.Add("sw", "ne");
                    directionMapping.Add("se", "nw");
                    directionMapping.Add("wu", "ed");
                    directionMapping.Add("eu", "wd");
                    directionMapping.Add("nu", "sd");
                    directionMapping.Add("su", "nd");
                    directionMapping.Add("wd", "eu");
                    directionMapping.Add("ed", "wu");
                    directionMapping.Add("nd", "su");
                    directionMapping.Add("sd", "nu");
                    directionMapping.Add("out", "enter");
                    directionMapping.Add("enter", "out");
                    directionMapping.Add("u", "d");
                    directionMapping.Add("d", "u");
                }
                return directionMapping;
            }
        }

        public static string WaitCommand = "dazuo 10";

        public static object Locker = new object();

        public static int Timer = 0;
    }
}
