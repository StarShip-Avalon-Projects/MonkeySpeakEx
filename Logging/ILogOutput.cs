namespace Shared.Core.Logging
{
    public interface ILogOutput
    {
        void Log(LogMessage logMsg);
    }
}