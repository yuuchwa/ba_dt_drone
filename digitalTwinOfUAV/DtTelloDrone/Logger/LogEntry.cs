using System;

namespace DtTelloDrone.Logger;

public struct LogEntry
{
    public LoggingEventType Level { get; }
    public string Message { get; }
    public Exception Exception { get; }

    public LogEntry(LoggingEventType level, string msg, Exception ex = null)
    {
        if (msg is null) throw new ArgumentNullException("msg");
        if (msg == string.Empty) throw new ArgumentException("empty", "msg");

        Level = level;
        Message = msg;
        Exception = ex;
    }
}