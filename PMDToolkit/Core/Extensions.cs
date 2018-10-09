/*The MIT License (MIT)

Copyright (c) 2014 PMU Staff

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;

namespace PMDToolkit.Core
{
    public static class Extensions
    {
        public static byte ToByte(this string str)
        {
            return !string.IsNullOrEmpty(str) && byte.TryParse(str, out byte result) ? result : (byte)0;
        }

        public static byte ToByte(this string str, byte defaultVal)
        {
            return str != null && byte.TryParse(str, out byte result) == true ? result : defaultVal;
        }

        public static int ToInt(this string str)
        {
            return !string.IsNullOrEmpty(str) && int.TryParse(str, out int result) ? result : 0;
        }

        public static int ToInt(this string str, int defaultVal)
        {
            return str != null && int.TryParse(str, out int result) == true ? result : defaultVal;
        }

        public static double ToDbl(this string str)
        {
            return str != null && double.TryParse(str, out double result) == true ? result : 0;
        }

        public static double ToDbl(this string str, double defaultVal)
        {
            return str != null && double.TryParse(str, out double result) == true ? result : defaultVal;
        }

        public static string ToIntString(this bool boolval)
        {
            if (boolval == true)
                return "1";
            else
                return "0";
        }

        public static int ToInt(this bool boolval)
        {
            if (boolval == true)
                return 1;
            else
                return 0;
        }

        public static bool IsNumeric(this string str)
        {
            return int.TryParse(str, out int result);
        }

        public static ulong ToUlng(this string str)
        {
            return ulong.TryParse(str, out ulong result) == true ? result : 0;
        }

        public static bool ToBool(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            switch (str.ToLower())
            {
                case "true":
                    return true;

                case "false":
                    return false;

                case "1":
                    return true;

                case "0":
                    return false;

                default:
                    return false;
            }
        }

        public static bool ToBool(this string str, bool defaultValue)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            switch (str.ToLower())
            {
                case "true":
                    return true;

                case "false":
                    return false;

                case "1":
                    return true;

                case "0":
                    return false;

                default:
                    return defaultValue;
            }
        }

        public static bool IsEnum<T>(this string enumString)
        {
            return Enum.IsDefined(typeof(T), enumString);
        }

        public static T ToEnum<T>(this string enumString)
        {
            return (T)Enum.Parse(typeof(T), enumString, true);
        }

        public static DateTime? ToDate(this string date)
        {
            return DateTime.TryParse(date, out DateTime tmpDate) ? (DateTime?)tmpDate : null;
        }

        public static string ToHex(this System.Drawing.Color color)
        {
            return string.Format("#{0:x2}{1:x2}{2:x2}{3:x2}", color.A, color.R, color.G, color.B);
        }

        public static System.Drawing.Color ToColor(this string hexString)
        {
            return HexStringToColor(hexString);
        }

        private static System.Drawing.Color HexStringToColor(string hex)
        {
            hex = hex.Replace("#", "");

            if (hex.Length != 8)
                throw new Exception(hex +
                    " is not a valid 8-place hexadecimal color code.");

            string a, r, g, b;
            a = hex.Substring(0, 2);
            r = hex.Substring(2, 2);
            g = hex.Substring(4, 2);
            b = hex.Substring(6, 2);

            return System.Drawing.Color.FromArgb(HexStringToBase10Int(a),
                                                 HexStringToBase10Int(r),
                                                 HexStringToBase10Int(g),
                                                 HexStringToBase10Int(b));
        }

        private static int HexStringToBase10Int(string hex)
        {
            int base10value = 0;

            try { base10value = Convert.ToInt32(hex, 16); } catch { base10value = 0; }

            return base10value;
        }
    }
}