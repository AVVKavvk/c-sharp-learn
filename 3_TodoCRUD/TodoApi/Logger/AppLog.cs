using System.Runtime.CompilerServices;
using Serilog;

namespace TodoApi.Logger;

public static class AppLog
{
    // ── Core method: builds enriched log with extras dict ──────────────────
    private static Serilog.ILogger Enrich(
        Dictionary<string, object>? extras,
        string filePath,
        string funcName,
        int line
    )
    {
        var enricher = new CallerInfoEnricher(filePath, funcName, line);

        var logger = Log.Logger.ForContext(enricher);

        if (extras is { Count: > 0 })
            logger = logger.ForContext("extras", extras, destructureObjects: true);

        return logger;
    }

    // ── Public API

    public static void Info(
        string message,
        Dictionary<string, object>? extras = null,
        [CallerFilePath] string file = "",
        [CallerMemberName] string func = "",
        [CallerLineNumber] int line = 0
    ) => Enrich(extras, file, func, line).Information(message);

    public static void Warn(
        string message,
        Dictionary<string, object>? extras = null,
        [CallerFilePath] string file = "",
        [CallerMemberName] string func = "",
        [CallerLineNumber] int line = 0
    ) => Enrich(extras, file, func, line).Warning(message);

    public static void Error(
        string message,
        Exception? ex = null,
        Dictionary<string, object>? extras = null,
        [CallerFilePath] string file = "",
        [CallerMemberName] string func = "",
        [CallerLineNumber] int line = 0
    ) => Enrich(extras, file, func, line).Error(ex, message);

    public static void Debug(
        string message,
        Dictionary<string, object>? extras = null,
        [CallerFilePath] string file = "",
        [CallerMemberName] string func = "",
        [CallerLineNumber] int line = 0
    ) => Enrich(extras, file, func, line).Debug(message);
}
