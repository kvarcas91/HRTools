using Domain.Interfaces;
using System;

namespace Domain.Logs
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
