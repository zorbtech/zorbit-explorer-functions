using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zorbit.Explorer.Functions
{
    internal static class RowKeyHelper
    {
        internal static string Format => new string(Enumerable.Range(0, int.MaxValue.ToString().Length).Select(c => '0').ToArray());
        private static readonly char[] Digit = Enumerable.Range(0, 10).Select(c => c.ToString()[0]).ToArray();

        //Convert '012' to '987'
        internal static string HeightToString(int height)
        {
            var input = height.ToString(Format);
            return ToggleChars(input);
        }

        internal static string ToggleChars(string input)
        {
            var result = new char[input.Length];
            for (var i = 0; i < result.Length; i++)
            {
                var index = Array.IndexOf(Digit, input[i]);
                result[i] = Digit[Digit.Length - index - 1];
            }
            return new string(result);
        }

        //Convert '987' to '012'
        internal static int StringToHeight(string rowkey)
        {
            return int.Parse(ToggleChars(rowkey));
        }
    }
}
