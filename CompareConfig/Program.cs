using System;
using System.Xml.Linq;

namespace CompareConfig
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine("<config file 1> <config file 2>");
                return;
            }

            if (!File.Exists(args[0]) || !File.Exists(args[1]))
            {
                Console.Error.WriteLine("File Not Found.");
                return;
            }

            char[] separators = [':', '='];

            var main = ToDict(File.ReadAllLines(args[0]), separators);
            var sub = ToDict(File.ReadAllLines(args[1]), separators);
            int tl=Math.Max(main.Count, sub.Count).ToString().Length;

            Console.ForegroundColor = ConsoleColor.White;
            foreach (var data in main)
            {
                var index = sub.FindIndex(d => d.key == data.key);
                if (index!=-1)
                {
                    if (sub[index].value == data.value)
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.WriteLine($"{data.rawLine.ToString().PadLeft(tl)} {(sub[index].rawLine).ToString().PadLeft(tl)} {data.raw}");
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"{data.rawLine.ToString().PadLeft(tl)} {(sub[index].rawLine).ToString().PadLeft(tl)} {data.raw}");
                    }

                    sub.RemoveAt(index);
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"{data.rawLine.ToString().PadLeft(tl)} {"-".PadLeft(tl)} {data.raw}");
                }

            }

            foreach (var data in sub)
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"{"+".PadLeft(tl)} {data.rawLine.ToString().PadLeft(tl)} {data.raw}");
            }
            Console.BackgroundColor = ConsoleColor.Black;
        }

        static List<(string key, string value, string raw,int rawLine)> ToDict(string[] lines, char[] separators)
        {
            var dict = new List<(string key, string value, string raw, int rawLine)>();

            var lastIndent = 0;
            var lastKey = 0;

            int line = 1;
            foreach (var text in lines)
            {
                var index = text.IndexOfAny(separators);
                if (index != -1)
                {
                    var key = text[..index];
                    var indent = key.TakeWhile(t => t == ' ').Count();
                    key = key[indent..];
                    if (indent > lastIndent)
                    {
                        key = $"{lastKey}.{key}";
                    }
                    lastIndent = indent;

                    var value = text[(index + 1)..].Trim();

                    dict.Add((key, value, text,line));
                }
                else
                {
                    dict.Add((text, "", text,line));

                }

                line++;
            }

            return dict;

        }
    }
}
