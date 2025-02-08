using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTasks.VowelCounting
{
    public class StringProcessor
    {
        public (char symbol, int count)[] GetCharCount(string veryLongString, char[] countedChars)
        {
            var charCounts = new Dictionary<char, int>();

            foreach (char c in veryLongString)
            {
                if (charCounts.ContainsKey(c))
                {
                    charCounts[c]++;
                }
                else
                {
                    charCounts[c] = 1;
                }
            }
            return countedChars.Select(c => (c, charCounts.GetValueOrDefault(c, 0))).ToArray();
        }
    }
}
