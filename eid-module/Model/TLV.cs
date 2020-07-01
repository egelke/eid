using System;
using System.Collections.Generic;
using System.Text;

namespace Egelke.Eid.Client.Model
{
    internal static class TLV
    {
        public static IDictionary<byte, byte[]> Parse(this byte[] file)
        {
            int i = 0;
            var retVal = new Dictionary<byte, byte[]>();
            while (i < file.Length - 1) {
                byte tag = file[i++];
                if (tag == 0) break;

                int len = 0;
                byte lenByte;
                do
                {
                    lenByte = file[i++];
                    len = (len << 7) + (lenByte & 0x7F);
                } while ((lenByte & 0x08) == 0x80);

                byte[] val = new byte[len];
                Array.Copy(file, i, val, 0, len);
                retVal.Add(tag, val);
                i += len;
            }

            return retVal;
        }

        public static String ToString(this byte[] value) => Encoding.UTF8.GetString(value);
    }
}
