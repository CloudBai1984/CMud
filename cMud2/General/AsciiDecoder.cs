using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cMud2
{
    class AsciiDecoder
    {
        public static string AsciiToUnicode(byte[] bytes, int lengthToConvert)
        {
            return System.Text.Encoding.Default.GetString(bytes);
        }
    }
}
