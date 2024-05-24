using System;
using System.Collections.Generic;
using System.Runtime;

namespace Name_Generator
{
    class Utils
    {
        public static bool IsVowel(char c)
        {
            return "aeiouyAEIOUY".IndexOf(c) >= 0;
        }

        public static bool IsConso(char c)
        {
            return !IsVowel(c);
        }

        public static int ParseInt(string s, string message = null)
        {
            int v = 0;
            try
            {
                v = int.Parse(s);
            }
            catch (Exception)
            {
                throw new ArgumentException($"COULD NOT PARSE INT FROM STRING {s}");
            }
            return v;
        }

        public static string Join2DRow(List<List<string>> src, int row, int width = 20)
        {
            // string val = src[0][row];
            string val = "";
            for (int i = 0; i < src.Count; i++)
            {
                val += src[i][row].PadRight(width);
            }
            return val;
        }
    }
}