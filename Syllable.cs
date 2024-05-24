using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Name_Generator.Utils;


namespace Name_Generator
{
    class Syllable
    {
        // private static bool IsVowel = Utils.IsVowel;

        public static List<string> SplitSyllable(string input)
        {
            List<string> syllables = new List<string>();
            List<char> currentSyllable = new List<char> { input[0] };

            for (int i = 1; i < input.Length; i++)
            {
                char cur = input[i];

                if (ShouldSplitAt(cur, currentSyllable))
                {
                    syllables.Add(new string(currentSyllable.ToArray()));
                    currentSyllable = new List<char>();
                }

                currentSyllable.Add(cur);
            }

            if (IsStartEndException(currentSyllable))
            {
                foreach (char c in currentSyllable)
                {
                    syllables[syllables.Count - 1] += c;
                }
            }
            else
            {
                syllables.Add(new string(currentSyllable.ToArray()));
            }

            return syllables;
        }

        private static bool IsStartEndException(List<char> syll)
        {
            return syll.Count == 1 || (syll.Count == 2 && syll[syll.Count - 1] == 'e');
        }

        private static bool ShouldSplitAt(char newChar, List<char> syll)
        {
            if (IsStartEndException(syll)) return false;

            char last = syll[syll.Count - 1];
            if (newChar == 'h' && (last == 't' || last == 's'))
            {
                return false;
            }

            if (IsVowel(newChar) && CheckPattern(syll, new Pattern[] { IsConso, IsVowel, IsConso })) return true;
            if (IsConso(newChar) && CheckPattern(syll, new Pattern[] { IsConso, IsVowel, IsConso })) return true;
            if (IsConso(newChar) && CheckPattern(syll, new Pattern[] { IsConso, IsConso, IsVowel })) return true;
            if (IsVowel(newChar) && CheckPattern(syll, new Pattern[] { IsVowel, IsConso })) return true;
            if (IsVowel(newChar) && CheckPattern(syll, new Pattern[] { IsVowel, IsVowel }, 1)) return true;

            return false;
        }

        delegate bool Pattern(char c);

        private static bool CheckPattern(List<char> input, Pattern[] patterns, int from = 0)
        {
            if (input.Count < patterns.Length)
            {
                return false;
            }

            for (int i = from; i < patterns.Length; i++)
            {
                if (!patterns[i](input[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}