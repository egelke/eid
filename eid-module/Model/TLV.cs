using System;
using System.Collections.Generic;
using System.Globalization;
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

        public static String ToStr(this byte[] value) => Encoding.UTF8.GetString(value).TrimEnd();

        public static DateTime ToDate(this byte[] value) => DateTime.ParseExact(TLV.ToStr(value).Replace(" ", "").Replace(".", ""), "ddMMyyyy", CultureInfo.InvariantCulture);

        public static DateTime ToBirthDate(this byte[] value)
        {
            String stringValue = TLV.ToStr(value);

            String[] parts = stringValue.Split(new char[] { '.', ' '}, StringSplitOptions.RemoveEmptyEntries); //split on . and ' '
            if (parts.Length == 3)
            {
                return new DateTime(
                    Int32.Parse(parts[2]),
                    parts[1].ToMonth(),
                    Int32.Parse(parts[0]));
            }
            else
            {
                //only year, set to 1st of Jan.
                return new DateTime(Int32.Parse(parts[0]), 1, 1);
            }
        }

        private static int ToMonth(this string value)
        {
            switch(value)
            {
                case "JAN":
                    return 1;
                case "FEB":
                case "FEV":
                    return 2;
                case "MÄR":
                case "MARS":
                case "MAAR":
                    return 3;
                case "APR":
                case "AVR":
                    return 4;
                case "MAI":
                case "MEI":
                    return 5;
                case "JUIN":
                case "JUN":
                    return 6;
                case "JUIL":
                case "JUL":
                    return 7;
                case "AOUT":
                case "AUG":
                    return 8;
                case "SEPT":
                case "SEP":
                    return 9;
                case "OCT":
                case "OKT":
                    return 10;
                case "NOV":
                    return 11;
                case "DEC":
                case "DEZ":
                    return 12;
                default:
                    throw new InvalidOperationException("Unknown Birth Month: " + value);
            }
        }

        public static Gender ToGender(this byte[] value)
        {
            switch(TLV.ToStr(value))
            {
                case "M":
                    return Gender.Male;
                case "V":
                case "F":
                case "W":
                    return Gender.Female;
                default:
                    return Gender.Unknown;

            }
        }

        public static DocType ToDocType(this byte[] value)
        {
            switch (TLV.ToStr(value))
            {
                case "1":
                case "01":
                    return DocType.IdentityCard;
                case "6":
                case "06":
                    return DocType.KidsCard;
                case "7":
                case "07":
                    return DocType.BootstrapCard;
                case "8":
                case "08":
                    return DocType.HabilitationCard;
                case "11":
                    return DocType.ForeignerA;
                case "12":
                    return DocType.ForeignerB;
                case "13":
                    return DocType.ForeignerC;
                case "14":
                    return DocType.ForeignerD;
                case "15":
                    return DocType.ForeignerE;
                case "16":
                    return DocType.ForeignerEplus;
                case "17":
                    return DocType.ForeignerF;
                case "18":
                    return DocType.ForeignerFplus;
                case "19":
                    return DocType.EuBlueCard;
                default:
                    throw new InvalidOperationException("Unknown Document Type: " + value.ToStr());
            }
        }

        public static Spec ToSpec(this byte[] value)
        {
            switch(TLV.ToStr(value))
            {
                case "0":
                    return Spec.None;
                case "1":
                    return Spec.WhiteCane;
                case "2":
                    return Spec.ExtendedMinor;
                case "3":
                    return Spec.WhiteCane | Spec.ExtendedMinor;
                case "4":
                    return Spec.YellowCane;
                case "5":
                    return Spec.YellowCane | Spec.ExtendedMinor;
                default:
                    throw new InvalidOperationException("Unknown Spec: " + value.ToStr());
            }
        }
    }
}
