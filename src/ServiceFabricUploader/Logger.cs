using System;
using SfRestApi;

namespace ServiceFabricUploader
{
    public class Logger : ILogOutput
    {
        private readonly bool _verbose;

        public Logger(bool verbose)
        {
            _verbose = verbose;
        }

        public void Log(string message)
        {
            if (_verbose)
                return;

            Console.WriteLine(message);
        }

        public void LogException(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        public void LogError(string message)
        {
            Console.WriteLine(message);
        }
    }
}