using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egelke.Eid.Client
{
    internal static class ExtensionMethods
    {

        public static List<String> ToStringList(this char[] multiStr)
        {
            if (multiStr.Length == 1) return new List<String>(); //no name found

            int i = 0;
            var str = new StringBuilder();
            var list = new List<String>();
            while (true)
            {
                if (multiStr[i] != '\0')
                {
                    str.Append(multiStr[i]);
                }
                else
                {
                    list.Add(str.ToString());
                    if (multiStr[i + 1] == '\0') break;
                    str.Clear();
                }
                i++;
            }

            return list;
        }
    }
}
