using System.Runtime.CompilerServices;
using Serilog.Core;
using Serilog.Events;

namespace TodoApi.Logger;

public class CallerInfoEnricher : ILogEventEnricher
{
    private readonly string _fileName;
    private readonly string _functionName;
    private readonly int _lineNumber;

    public CallerInfoEnricher(
        [CallerFilePath] string fileName = "",
        [CallerMemberName] string functionName = "",
        [CallerLineNumber] int lineNumber = 0
    )
    {
        _fileName = Path.GetFileName(fileName);
        _functionName = functionName;
        _lineNumber = lineNumber;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory factory)
    {
        logEvent.AddPropertyIfAbsent(factory.CreateProperty("fileName", _fileName));
        logEvent.AddPropertyIfAbsent(factory.CreateProperty("functionName", _functionName));
        logEvent.AddPropertyIfAbsent(factory.CreateProperty("lineNumber", _lineNumber));
    }
}
