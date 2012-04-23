/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;
using VersionOne.ServiceHost.Core;

namespace VersionOne.ServiceHost {
    internal class ConsoleMode : ModeBase {
        internal void Run() {
            try {
                InternalRun();
            } catch(Exception ex) {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Press enter to close...");
            Console.Read();
        }

        private void InternalRun() {
            Starter.Startup();

            Console.TreatControlCAsInput = true;
            var quit = false;

            while(!quit) {
                var info = Console.ReadKey(true);

                switch(info.Key) {
                    case ConsoleKey.Q:
                        quit = true;
                        break;
                    case ConsoleKey.H:
                        Console.WriteLine("\t\tq\t\tQuits console");
                        Console.WriteLine("\t\th\t\tPrints this help");
                        break;
                    case ConsoleKey.C:
                        if((info.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control) {
                            quit = true;
                        }
                        break;
                }
            }

            Starter.Shutdown();
        }
    }
}