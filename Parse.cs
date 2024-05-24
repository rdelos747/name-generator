using System;
using Microsoft.VisualBasic.FileIO;

/*
Sometimes the only data I can find is in CSV format, so
I wrote this helper to grab the data I want.  It should
probably be in a different repo but whatever. 
*/
namespace Name_Generator
{
    class Parse
    {
        public static void parseRawCsv(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Wrong number of args");
                return;
            }

            string path = args[1];
            int min = Utils.ParseInt(args[2]);
            int max = Utils.ParseInt(args[3]);

            Console.WriteLine($"PARSING RAW");
            try
            {
                using (TextFieldParser parser = new TextFieldParser($"./raw-data/{path}"))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        string s = fields[min];

                        for (int i = min + 1; i <= max; i++)
                        {
                            s += $" {fields[i]}";
                        }
                        Console.WriteLine(s);
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("COULD NOT FIND OR PARSE FILE");
            }
        }
    }
}