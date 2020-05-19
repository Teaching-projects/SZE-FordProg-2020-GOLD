using System;
using System.IO;
using System.Text;
using GOLD.Engine;

namespace GOLD.Parser
{
    class Program
    {
        static Engine.Parser p = new Engine.Parser();
        static void Main(string[] args)
        {
            int l = args.Length;
            string path = string.Empty;
            ushort outputSize = 120;

            try
            {
                for (int i = 0; i < l; i++)
                {
                    switch (args[i].ToLower())
                    {
                        case "-f":
                            path = args[i + 1];
                            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                                Error("Error: no EGT file found!", true);
                            break;
                        case "-s":
                            bool parsed = ushort.TryParse(args[i + 1], out outputSize);
                            if (!parsed)
                                Error("Error: cannot set size parameter!", true);
                            break;

                    }
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                Error("Error: parameter missing.", true);
            }

            p.OutputMaxLength = outputSize;
            p.LoadTables(path);
            RecursiveParse();
        }

        static void Error(string msg, bool exitApp = false)
        {
            Console.WriteLine(msg);

            if (exitApp)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        static void RecursiveParse()
        {
            ParseMessage response;
            bool done;
            using (MemoryStream ms = new MemoryStream())
            {
                TextReader reader = new StreamReader(ms);
                p.Open(reader);
                p.TrimReductions = false;
                Console.Write("Type your syntax here: ");
                for (string text = Console.ReadLine(); !text.ToLower().Equals("exit"); text = Console.ReadLine())
                {
                    Console.WriteLine();
                    var bytes = Encoding.UTF8.GetBytes(text);
                    ms.Write(bytes, 0, bytes.Length);
                    ms.Position -= bytes.Length;

                    done = false;
                    while (!done)
                    {
                        response = p.Parse(true);
                        switch (response)
                        {
                            case ParseMessage.SyntaxError:
                            case ParseMessage.LexicalError:                               
                                ms.SetLength(0);
                                ms.Position = 0;
                                reader = new StreamReader(ms);
                                p.Open(reader);
                                done = true;
                                break;

                            case ParseMessage.Reduction:
                                break;

                            case ParseMessage.Accept:                        
                                p.Open(reader);
                                done = true;
                                break;

                            case ParseMessage.TokenRead:
                                break;

                            case ParseMessage.InternalError:
                                done = true;
                                break;

                            case ParseMessage.NotLoadedError:
                                done = true;
                                break;

                            case ParseMessage.GroupError:
                                done = true;
                                break;
                        }
                    }

                    Console.WriteLine();
                    Console.WriteLine();
                    Console.Write("Type your syntax here: ");
                }
                reader.Dispose();
            }
        }
    }
}
