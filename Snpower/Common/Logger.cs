using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snpower.Common
{
    public class Logger
    {
        public static void Step(string message)
        {
            Console.WriteLine("STEP: " + message);
        }

        public static void Info(string message)
        {
            Console.WriteLine("INFO: " + message);
        }

        public static void Verify(string message)
        {
            Console.WriteLine("VERIFY: " + message);
        }
    }
}
