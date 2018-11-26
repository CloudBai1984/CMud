using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cMud2
{
    public class Triggerer
    {
        public string Regex
        { get; set; }
        public string Command
        { get; set; }
        public string Group
        { get; set; }
        public bool IsActive
        { get; set; }
    }
}
