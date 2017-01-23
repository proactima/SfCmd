using System;

namespace SfRestApi
{
    public interface ILogOutput
    {
        void Log(string message);
        void LogError(string message);
        void LogException(Exception ex);
    }
}