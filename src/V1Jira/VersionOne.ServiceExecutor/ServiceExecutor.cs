/*(c) Copyright 2011, VersionOne, Inc. All rights reserved. (c)*/
using System;

namespace VersionOne.ServiceExecutor {
    internal class ServiceExecutor {
        private static void Main(string[] args) {
            new BatchMode().Run();

            if(args.Length > 0) {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}