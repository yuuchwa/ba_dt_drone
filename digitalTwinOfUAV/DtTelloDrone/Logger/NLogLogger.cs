using Mars.Common.Core.Logging;
using NLog;

namespace DtTelloDrone.Logger;

public sealed class NLogLogger : ILogger
{
    private readonly NLog.Logger _logger;
    public NLogLogger(string name)
    {
        _logger = NLog.LogManager.GetLogger(name); // Liefert bei einem neuen Namen eine neue Logger Instanz.
    }

    public void Log(LogEntry entry)
    {
        var nlogLevel = GetNLogLevel(entry.Level);
        if (_logger.IsEnabled(nlogLevel))
        {
            var nlogEvent = LogEventInfo.Create(nlogLevel, _logger.Name, null, entry.Message);
            _logger.Log(typeof(NLogLogger), nlogEvent);
        }
    }
    
    private NLog.LogLevel GetNLogLevel(LoggingEventType level)
    {
        switch (level)
        {
            case LoggingEventType.Debug: return NLog.LogLevel.Debug;
            case LoggingEventType.Information: return NLog.LogLevel.Info;
            case LoggingEventType.Warning: return NLog.LogLevel.Warn;
            case LoggingEventType.Error: return NLog.LogLevel.Error;
            case LoggingEventType.Fatal: return NLog.LogLevel.Fatal;
            default: return NLog.LogLevel.Trace;
        }
    }
}