using System;
using System.IO;

public enum LoggingProviders
{
    Consol,
    File
}

interface ILogger
{
    void Info(string message);
    void Debug();
    void Warning();
    void Error();
}

public class ConsoleLogger : ILogger
{
    public void Info(string message)
    {
        Console.WriteLine("Console logger: {0}", message);
    }

    public void Debug() { }
    public void Warning() { }
    public void Error() { }
}

public class FileLogger : ILogger
{
    public void Message() { }

    public void Info(string message)
    {
        using (StreamWriter streamWriter = new StreamWriter("Log.txt", true))
        {
            streamWriter.WriteLine("File logger:" + message);
        }
    }

    public void Debug() { }
    public void Warning() { }
    public void Error() { }
}

class LoggerProviderFactory
{
    private LoggerProviderFactory() { }
    private static LoggerProviderFactory _loggerProviderFactoryInstance = null;
    public static LoggerProviderFactory GetInstance()
    {
        if (_loggerProviderFactoryInstance == null)
        {
            _loggerProviderFactoryInstance = new LoggerProviderFactory();
        }
        return _loggerProviderFactoryInstance;
    }

    private static int loggerCount;

    public ILogger GetLoggingProvider(LoggingProviders logProviders)
    {
        if (loggerCount > 0)
            throw new Exception("You already have one logger");
        loggerCount++;
        switch (logProviders)
        {
            case LoggingProviders.Consol:
                return new ConsoleLogger();
            case LoggingProviders.File:
                return new FileLogger();
            default:
                return new ConsoleLogger();
        }
    }
}

