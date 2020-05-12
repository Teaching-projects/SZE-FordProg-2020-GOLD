using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GOLD.Engine;

namespace GOLD.Parser
{
    class Program
    {
        static Engine.Parser p = new Engine.Parser();
        static void Main(string[] args)
        {
            p.LoadTables(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "custom.egt"));
            RecursiveParse();
        }

        static void OneLineParser()
        {
            ParseMessage response;
            bool done;                      //Controls when we leave the loop
            bool accepted = false;          //Was the parse successful?

            string text = Console.ReadLine();
            p.Open(ref text);
            p.TrimReductions = false;  //Please read about this feature before enabling  

            done = false;
            while (!done)
            {
                response = p.Parse();

                switch (response)
                {
                    case ParseMessage.LexicalError:
                        //Cannot recognize token
                        done = true;
                        break;

                    case ParseMessage.SyntaxError:
                        //Expecting a different token
                        done = true;
                        break;

                    case ParseMessage.Reduction:
                        //Create a customized object to store the reduction
                        break;

                    case ParseMessage.Accept:
                        //Accepted!
                        //program = parser.CurrentReduction   //The root node!                 
                        done = true;
                        accepted = true;
                        break;

                    case ParseMessage.TokenRead:
                        //You don't have to do anything here.
                        break;

                    case ParseMessage.InternalError:
                        //INTERNAL ERROR! Something is horribly wrong.
                        done = true;
                        break;

                    case ParseMessage.NotLoadedError:
                        //This error occurs if the CGT was not loaded.                   
                        done = true;
                        break;

                    case ParseMessage.GroupError:
                        //GROUP ERROR! Unexpected end of file
                        done = true;
                        break;
                }
            }

            Console.WriteLine(accepted.ToString());
            Console.ReadKey();
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
                            case ParseMessage.LexicalError:                               
                                ms.SetLength(0);
                                ms.Position = 0;
                                reader = new StreamReader(ms);
                                p.Open(reader);
                                done = true;
                                break;

                            case ParseMessage.SyntaxError:
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
                }
                reader.Dispose();
            }
        }
    }
}
