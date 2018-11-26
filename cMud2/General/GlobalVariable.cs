using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cMud2
{
    public class GlobalVariable
    {
        #region 人物
        public static int HP;
        public static int MaxHP;
        public static int Neili;
        public static int MaxNeili;
        public static int Jingli;
        public static int MaxJinglie;
        public static int Exp;
        public static int QianNeng;
        public static int YinShui;
        public static int ShiWu;
        #endregion

        #region 地点
        public static List<string> CurrentDirections = new List<string>();
        public static string CurrentLocation = string.Empty;
        #endregion


        public static Int64 Ticks = 0;

    }
}
