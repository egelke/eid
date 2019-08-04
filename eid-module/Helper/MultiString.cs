/*
 *  This file is part of .Net eID Client.
 *  Copyright (C) 2014-2019 Egelke BVBA
 *
 *  .Net eID Client is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 2.1 of the License, or
 *  (at your option) any later version.
 *
 *  .Net eID Client is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with .Net eID Client.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Egelke.Eid.Client
{
    internal static class MultiString
    {

        public static char[] ToMultiString(String[] stringList)
        {
            int size = 1; //for the end char.
            foreach(String s in stringList)
            {
                size += s.Length + 1; //for the string + delimiter
            }

            int offset = 0;
            char[] retVal = new char[size];
            foreach (String s in stringList)
            {
                char[] sArray = s.ToCharArray();
                Array.Copy(sArray, 0, retVal, offset, sArray.Length);
                offset += sArray.Length;
                retVal[offset++] = '\0';
            }
            retVal[offset] = '\0';

            return retVal;
        }

        public static List<String> ToStringList(char[] multiStr)
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
                    str.Length = 0;
                }
                i++;
            }

            return list;
        }
    }
}
