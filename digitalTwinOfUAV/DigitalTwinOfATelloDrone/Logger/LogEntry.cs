using System;

namespace DigitalTwinOfATelloDrone.Logger;

public struct LogEntry
{
    public LoggingEventType Level { get; }
    public string Message { get; }
    public Exception Exception { get; }

    public LogEntry(LoggingEventType Level, string msg, Exception ex = null)
    {
        if (msg is null) throw new ArgumentNullException("msg");
        if (msg == string.Empty) throw new ArgumentException("empty", "msg");

        this.Level = Level;
        this.Message = msg;
        this.Exception = ex;
    }
}