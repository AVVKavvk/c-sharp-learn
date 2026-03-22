using System.Text.Json;
using Serilog.Events;
using Serilog.Formatting;

namespace TodoApi.Logger;

public class JsonFormatter : ITextFormatter
{
    public void Format(LogEvent logEvent, TextWriter output)
    {
        // Console.WriteLine("=== ALL LOG PROPERTIES ===");
        // foreach (var (key, val) in logEvent.Properties)
        //     Console.WriteLine($"  key='{key}' type={val.GetType().Name} val={val}");
        // Console.WriteLine("==========================");

        string GetProp(string key) =>
            logEvent.Properties.TryGetValue(key, out var v) ? v.ToString().Trim('"') : "";

        int GetIntProp(string key) =>
            logEvent.Properties.TryGetValue(key, out var v) && int.TryParse(v.ToString(), out var n)
                ? n
                : 0;

        var reservedKeys = new HashSet<string>
        {
            "fileName",
            "functionName",
            "lineNumber",
            "extras",
            "MachineName",
            "EnvironmentName",
            "SourceContext",
        };

        var extras = new Dictionary<string, string>();

        // ── Parse extras ───────────────────────────────────────────────────
        if (logEvent.Properties.TryGetValue("extras", out var extrasVal))
        {
            switch (extrasVal)
            {
                // ── THIS is what Serilog actually uses for Dictionary<string, object> ──
                case DictionaryValue dv:
                    foreach (var kv in dv.Elements)
                        extras[Stringify(kv.Key)] = Stringify(kv.Value);
                    break;

                case StructureValue sv:
                    foreach (var prop in sv.Properties)
                        extras[prop.Name] = Stringify(prop.Value);
                    break;

                case SequenceValue seq:
                    foreach (var element in seq.Elements)
                    {
                        if (element is StructureValue kv)
                        {
                            var key = kv.Properties.FirstOrDefault(p => p.Name == "Key")?.Value;
                            var val = kv.Properties.FirstOrDefault(p => p.Name == "Value")?.Value;
                            if (key is not null && val is not null)
                                extras[Stringify(key)] = Stringify(val);
                        }
                    }
                    break;
            }
        }

        // ── Ad-hoc ForContext properties ───────────────────────────────────
        foreach (var (key, val) in logEvent.Properties)
        {
            if (!reservedKeys.Contains(key))
                extras[key] = val.ToString().Trim('"');
        }

        var payload = new
        {
            timestamp = logEvent.Timestamp.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            level = logEvent.Level.ToString(),
            fileName = GetProp("fileName"),
            functionName = GetProp("functionName"),
            lineNumber = GetIntProp("lineNumber"),
            message = logEvent.RenderMessage(),
            extras = extras.Count > 0 ? extras : null,
            exception = logEvent.Exception?.ToString(),
        };

        output.WriteLine(
            JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = false })
        );
    }

    private static string Stringify(LogEventPropertyValue value) =>
        value switch
        {
            ScalarValue { Value: null } => "null",
            ScalarValue { Value: string s } => s,
            ScalarValue { Value: bool b } => b.ToString().ToLower(),
            ScalarValue { Value: int i } => i.ToString(),
            ScalarValue { Value: long l } => l.ToString(),
            ScalarValue { Value: double d } => d.ToString(),
            ScalarValue { Value: float f } => f.ToString(),
            _ => value.ToString().Trim('"'),
        };
}
