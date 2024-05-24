using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Name_Generator
{
    class Program
    {
        // just do this so later we can manually make different custom lists for different use cases
        static string[] ALL_SOURCES = { "roman", "greek", "cool-english", "lakes", "science", "space" };

        private static Random random;

        private static Dictionary<string, List<string>> allData;

        static void Main(string[] args)
        {
            //Console.Clear();
            Console.WriteLine("\n========== NAME GENERATOR ==========");
            if (args.Length == 0 || args[0] == "help")
            {
                showHelp();
            }
            else if (args[0] == "parse-raw-csv")
            {
                Parse.parseRawCsv(args);
            }
            else if (args[0] == "generate")
            {
                initGenerate(args);
            }
            else
            {
                Console.WriteLine("UNRECOGNIZED COMMAND");
            }
        }

        private static void showHelp()
        {
            Console.WriteLine("ARGUMENTS");
            Console.WriteLine("help: show help");
            Console.WriteLine("\tex: dotnet run help");

            Console.WriteLine("parse-raw-csv <file name> <min col> <max col>: parse raw file from the raw-data folder");
            Console.WriteLine("\tex: 'dotnet run parse-raw-csv my-file.csv 2 4': output between columns 2 and 4");

            Console.WriteLine("generate <args> : generate names");
            Console.WriteLine("\tex: dotnet run generate: generate random name with random seed from all sources");
            Console.WriteLine("\t\t same as running 'generate -src=random'");
            Console.WriteLine("    ARGS: (all are optional)");
            Console.WriteLine("\t-seed: set the random seed");
            Console.WriteLine("\t\t-seed=12345");
            Console.WriteLine("\t-srcA: set the first source to use");
            Console.WriteLine("\t\t-srcA=random: use random source");
            Console.WriteLine("\t\t-srcA=all: use all sources");
            Console.WriteLine("\t\t-srcA=roman,greek: use roman and greek sources");
            Console.WriteLine("\t-srcB: set the second source to use");
            Console.WriteLine("\t\t-srcB=random: use random source");
            Console.WriteLine("\t\t-srcB=all: use all sources");
            Console.WriteLine("\t\t-srcB=roman,greek: use roman and greek sources");
        }

        private static void initGenerate(string[] args)
        {
            /*
            Parse arguments
            */
            string[] sourcesA = { "random" };
            string[] sourcesB = { "random" };
            string[] wordsA = null;
            string[] wordsB = null;
            bool didSeed = false;
            int seed = 0;
            int numSamples = 6;

            for (int i = 1; i < args.Length; i++)
            {
                string[] vals = args[i].Split("=");
                string cmd = vals[0];
                if (cmd == "-seed")
                {
                    seed = Utils.ParseInt(vals[1]);
                    didSeed = true;
                }
                else if (cmd == "-srcA")
                {
                    sourcesA = vals[1].Split(",");
                }
                else if (cmd == "-srcB")
                {
                    sourcesB = vals[1].Split(",");
                }
                else if (cmd == "-wordsA")
                {
                    wordsA = vals[1].Split(",");
                }
                else if (cmd == "-wordsB")
                {
                    wordsB = vals[1].Split(",");
                }
                else if (cmd == "-samp")
                {
                    numSamples = Utils.ParseInt(vals[1]);
                }
                else
                {
                    throw new ArgumentException($"ARG {vals[0]} NOT VALID");
                }
            }

            /*
            Init random
            */
            if (didSeed)
            {
                random = new Random(seed);
            }
            else
            {
                Random seedGenerator = new Random();
                seed = seedGenerator.Next(int.MinValue, int.MaxValue);
                random = new Random(seed);
            }

            Console.WriteLine($"USING SEED {seed}\n");

            /*
            Get sources
            */
            if (sourcesA.Length == 1)
            {
                if (sourcesA[0].ToLower() == "all")
                {
                    sourcesA = ALL_SOURCES;
                }
                else if (sourcesA[0].ToLower() == "random")
                {
                    sourcesA = new string[] { ALL_SOURCES[random.Next(ALL_SOURCES.Length)] };
                }
            }

            if (sourcesB.Length == 1)
            {
                if (sourcesB[0].ToLower() == "all")
                {
                    sourcesB = ALL_SOURCES;
                }
                else if (sourcesB[0].ToLower() == "random")
                {
                    sourcesB = new string[] { ALL_SOURCES[random.Next(ALL_SOURCES.Length)] };
                }
            }


            foreach (string s in sourcesA)
            {
                if (!ALL_SOURCES.Contains(s))
                {
                    throw new ArgumentException($"'{s}' IS NOT VALID FOR SOURCE A. DID YOU ADD IT TO ALL_SOURCES?");
                }
            }

            foreach (string s in sourcesB)
            {
                if (!ALL_SOURCES.Contains(s))
                {
                    throw new ArgumentException($"'{s}' IS NOT VALID FOR SOURCE B. DID YOU ADD IT TO ALL_SOURCES?");
                }
            }

            // generate(0, sourcesA, sourcesB);
            /*
            Init data sources
            */
            allData = new Dictionary<string, List<string>>();
            string[] both = sourcesA.Union(sourcesB).ToArray();
            foreach (string s in both)
            {
                allData.Add(s, File.ReadAllLines($"./source-data/{s}.txt").ToList());
            }

            List<List<string>> samplesA;
            List<List<string>> samplesB;

            if (wordsA != null)
            {
                samplesA = new List<List<string>>();
                foreach (string s in wordsA)
                {
                    samplesA.Add(new List<string> { s.ToLower().Trim() });
                }
            }
            else
            {
                samplesA = getInitialSamples(sourcesA, numSamples);
                Console.WriteLine($"GENERATING FROM SOURCES A: {string.Join(",", sourcesA)}");
            }

            if (wordsB != null)
            {
                samplesB = new List<List<string>>();
                foreach (string s in wordsB)
                {
                    samplesB.Add(new List<string> { s.ToLower().Trim() });
                }
            }
            else
            {
                samplesB = getInitialSamples(sourcesB, numSamples);
                Console.WriteLine($"GENERATING FROM SOURCES B: {string.Join(",", sourcesB)}");
            }

            generate(samplesA, samplesB);
        }

        private static void generate(List<List<string>> samplesA, List<List<string>> samplesB)
        {
            // List<List<string>> samplesA = new List<List<string>>();
            // Console.WriteLine($"GENERATING FROM SOURCES A: {string.Join(",", sourcesA)}");
            // Console.WriteLine($"GENERATING FROM SOURCES B: {string.Join(",", sourcesB)}");
            // List<List<string>> samplesA = getInitialSamples(sourcesA, 6);
            // List<List<string>> samplesB = getInitialSamples(sourcesB, 6);
            Console.WriteLine($"ORGNL A\t\t{Utils.Join2DRow(samplesA, 0, 20)}");
            Console.WriteLine($"ORGNL B\t\t{Utils.Join2DRow(samplesB, 0, 20)}\n");

            string syllAllOutA = "SYLL ALL A\t";
            List<List<string>> allSyllsA = new List<List<string>>();
            foreach (List<string> sample in samplesA)
            {
                List<string> sylls = Syllable.SplitSyllable(sample[0]);
                syllAllOutA += string.Join(",", sylls).PadRight(20);
                allSyllsA.Add(sylls);
            }
            Console.WriteLine(syllAllOutA);

            string syllAllOutB = "SYLL ALL B\t";
            List<List<string>> allSyllsB = new List<List<string>>();
            foreach (List<string> sample in samplesB)
            {
                List<string> sylls = Syllable.SplitSyllable(sample[0]);
                syllAllOutB += string.Join(",", sylls).PadRight(20);
                allSyllsB.Add(sylls);
            }
            Console.WriteLine(syllAllOutB + "\n");

            foreach (List<string> sample in samplesA)
            {
                int numSyllsA = random.Next(1, 3);
                int numSyllsB = random.Next(1, 3);
                string s = "";
                for (int i = 0; i < numSyllsA; i++)
                {
                    List<string> word = allSyllsA[random.Next(allSyllsA.Count)];
                    s += i < word.Count ? word[i] : word[word.Count - 1];
                }
                for (int i = numSyllsB; i > 0; i--)
                {
                    List<string> word = allSyllsB[random.Next(allSyllsB.Count)];
                    // s += i < word.Count ? word[i] : word[word.Count - 1];
                    s += word.Count - i > 0 ? word[word.Count - i] : word[0];
                }
                sample.Add(s);
            }
            Console.WriteLine($"SYLL AB COMB\t{Utils.Join2DRow(samplesA, samplesA[0].Count - 1)}");

            for (int i = 0; i < 4; i++)
            {
                foreach (List<string> sample in samplesA)
                {
                    string swapped = sample[sample.Count - 1];
                    // string swapped = SwapChar(sample[sample.Count - 1]);
                    while (sample.Contains(swapped))
                    {
                        swapped = SwapChar(swapped);
                    }
                    sample.Add(swapped);
                }
                Console.WriteLine($"SWAP A\t\t{Utils.Join2DRow(samplesA, samplesA[0].Count - 1)}");
            }
            Console.WriteLine(" ");

            foreach (List<string> sample in samplesB)
            {
                int numSyllsB = random.Next(1, 3);
                int numSyllsA = random.Next(1, 3);
                string s = "";
                for (int i = 0; i < numSyllsB; i++)
                {
                    List<string> word = allSyllsB[random.Next(allSyllsB.Count)];
                    s += i < word.Count ? word[i] : word[word.Count - 1];
                }
                for (int i = numSyllsA; i > 0; i--)
                {
                    List<string> word = allSyllsA[random.Next(allSyllsA.Count)];
                    // s += i < word.Count ? word[i] : word[word.Count - 1];
                    s += word.Count - i > 0 ? word[word.Count - i] : word[0];
                }
                sample.Add(s);
            }
            Console.WriteLine($"SYLL BA COMB\t{Utils.Join2DRow(samplesB, samplesB[0].Count - 1)}");

            for (int i = 0; i < 4; i++)
            {
                foreach (List<string> sample in samplesB)
                {
                    string swapped = sample[sample.Count - 1];
                    // string swapped = SwapChar(sample[sample.Count - 1]);
                    while (sample.Contains(swapped))
                    {
                        swapped = SwapChar(swapped);
                    }
                    sample.Add(swapped);
                }
                Console.WriteLine($"SWAP B\t\t{Utils.Join2DRow(samplesB, samplesB[0].Count - 1)}");
            }

            //     foreach (List<string> sample in samplesA)
            //     {
            //         int numSylls = random.Next(2, 3);
            //         string s = "";
            //         for (int i = 0; i < numSylls; i++)
            //         {
            //             List<string> word = allSyllsA[random.Next(allSyllsA.Count)];
            //             s += i < word.Count ? word[i] : word[word.Count - 1];
            //         }
            //         sample.Add(s);
            //     }
            //     Console.WriteLine($"SYLL AA COMB\t{Utils.Join2DRow(samplesA, samplesA[0].Count - 1)}");

            //     foreach (List<string> sample in samplesB)
            //     {
            //         int numSylls = random.Next(2, 3);
            //         string s = "";
            //         for (int i = 0; i < numSylls; i++)
            //         {
            //             List<string> word = allSyllsB[random.Next(allSyllsB.Count)];
            //             s += i < word.Count ? word[i] : word[word.Count - 1];
            //         }
            //         sample.Add(s);
            //     }
            //     Console.WriteLine($"SYLL BB COMB\t{Utils.Join2DRow(samplesB, samplesB[0].Count - 1)}");
            // }
        }

        private static List<List<string>> getInitialSamples(string[] sources, int numSamples)
        {
            List<List<string>> samples = new List<List<string>>();
            for (int i = 0; i < numSamples; i++)
            {
                string sourceKey = sources[random.Next(sources.Length)];
                List<string> sourceData = allData[sourceKey];
                string word = sourceData[random.Next(sourceData.Count)];
                samples.Add(new List<string> { word.ToLower().Trim() });
            }

            return samples;
        }

        static int numSamples = 6;
        private static void generate2(int runNum, string[] sources)
        {
            Console.WriteLine($"GENERATING RUN NUM: {runNum}, SOURCES: {string.Join(",", sources)}");

            List<List<string>> sourceData = new List<List<string>>();
            foreach (string source in sources)
            {
                sourceData.Add(File.ReadAllLines($"./source-data/{source}.txt").ToList());
            }

            List<List<string>> samples = new List<List<string>>();
            for (int i = 0; i < numSamples; i++)
            {
                samples.Add(new List<string>());
                int sourceIdx = random.Next(sourceData.Count);
                int wordIdx = random.Next(sourceData[sourceIdx].Count);
                samples[i].Add(sourceData[sourceIdx][wordIdx].ToLower().Trim());
            }
            Console.WriteLine($"ORGNL\t\t{Utils.Join2DRow(samples, 0)}");

            for (int i = 0; i < 2; i++)
            {
                foreach (List<string> sample in samples)
                {
                    string swapped = SwapChar(sample[sample.Count - 1]);
                    sample.Add(swapped);
                }
                Console.WriteLine($"SWAP\t\t{Utils.Join2DRow(samples, i + 1)}");
            }

            /*
            split syllables
            */
            string syllAllOut1 = "SYLL ALL\t";
            List<List<string>> allSylls = new List<List<string>>();
            foreach (List<string> sample in samples)
            {
                List<string> sylls = Syllable.SplitSyllable(sample[0]);
                syllAllOut1 += string.Join(",", sylls) + "\t\t";
                allSylls.Add(sylls);
            }
            Console.WriteLine(syllAllOut1);

            /*
            combine random syllables
            */
            foreach (List<string> sample in samples)
            {
                int numSylls = random.Next(2, 3);
                string s = "";
                for (int i = 0; i < numSylls; i++)
                {
                    List<string> word = allSylls[random.Next(allSylls.Count)];
                    s += i < word.Count ? word[i] : word[word.Count - 1];
                }
                sample.Add(s);
            }
            Console.WriteLine($"SYLL ALL COMB\t{Utils.Join2DRow(samples, samples[0].Count - 1)}");

            /*
            TODO:
            - consider randomly picking a list of operations to perform. Eg, the list could look like:
                - [CHAR SWAP, SYLLABLE SWAP, CHAR SWAP, etc].
                - use this list to determine what actions will be taken
            - actually implement syllable swap:
                - find a way to split strings by syllable, doesnt need to be perfect.
                - store syllables by place. EG: the words "theodosius", "ganymede", "cordelia", "common"  can be split as:
                    - [ga, ny, mede]
                    - [the, o, dos, ius]
                    - [cor, del, ia] (for convenience, we might want to make vowel chains, eg "ia", one syllable instead of two)
                    - [com, mon]
                    - for each syllable index, pick from one of the word's syllables at that index to construct a new word
                        - ga | mon | ia
                        - ga (ganymede first syllable), mon (common second syllable), ia (cordelia third syllable)
                        - do this a few times and consider messing with the order some times
                    - consider adding params for choosing which words are chosen for beginning/ ending syllables. eg, force words to have roman suffixes.
                        - just use the roman set, pick random words, grab the ending suffixes and place them at the ends
            */
        }

        private static string SwapChar(string input)
        {
            char[] chars = input.ToCharArray();
            int idx1 = random.Next(chars.Length);
            int idx2 = idx1;
            while (input[idx2] == input[idx1])
            {
                idx2 = random.Next(chars.Length);
            }
            chars[idx1] = input[idx2];
            chars[idx2] = input[idx1];
            return new string(chars);
        }

        // private static 
    }
}
