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
            string grammarPath = string.Empty;
            string filePath = string.Empty;
            ushort outputSize = 120;

            try
            {
                for (int i = 0; i < l; i++)
                {
                    switch (args[i].ToLower())
                    {
                        case "-g":
                            grammarPath = args[i + 1];
                            if (string.IsNullOrWhiteSpace(grammarPath) || !File.Exists(grammarPath))
                                WriteMessage("Error: no EGT file found!", true);
                            break;
                        case "-s":
                            bool parsed = ushort.TryParse(args[i + 1], out outputSize);
                            if (!parsed)
                                WriteMessage("Error: cannot set size parameter!", true);
                            break;
                        case "-f":
                            filePath = args[i + 1];
                            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                                WriteMessage("Error: file not found!", true);
                            break;
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                WriteMessage("Error: parameter missing.", true);
            }

            p.OutputMaxLength = outputSize;
            p.LoadTables(grammarPath);
            if (string.IsNullOrEmpty(filePath))
                InteractiveParse();
            else
                ParseFile(filePath);
        }

        static void WriteMessage(string msg, bool exitApp = false)
        {
            Console.WriteLine();
            Console.WriteLine(msg);

            if (exitApp)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        static void ParseFile(string file)
        {
            ParseMessage response;
            TextReader reader = new StreamReader(file);
            p.Open(reader);
            p.TrimReductions = false;

            bool done = false;
            while (!done)
            {
                response = p.Parse(true);
                switch (response)
                {
                    case ParseMessage.Accept:
                    case ParseMessage.SyntaxError:
                    case ParseMessage.LexicalError:
                    case ParseMessage.InternalError:
                    case ParseMessage.NotLoadedError:
                    case ParseMessage.GroupError:
                        done = true;
                        break;

                    case ParseMessage.Reduction:
                    case ParseMessage.TokenRead:
                        break;
                }
            }
            reader.Dispose();
            WriteMessage(string.Empty, true);        
        }

        static void InteractiveParse()
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

                            case ParseMessage.Accept:                        
                                p.Open(reader);
                                done = true;
                                break;

                            case ParseMessage.InternalError:
                            case ParseMessage.NotLoadedError:
                            case ParseMessage.GroupError:
                                done = true;
                                break;

                            case ParseMessage.Reduction:
                            case ParseMessage.TokenRead:
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
