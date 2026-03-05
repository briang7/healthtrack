using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using HealthTrack.Domain.Attributes;

namespace HealthTrack.Domain.Services;

/// <summary>
/// Masks PHI-tagged properties in serialized data for audit logs.
/// Properties marked with [Phi] are replaced with masked values
/// based on their sensitivity level.
/// </summary>
public static class PhiMaskingService
{
    private static readonly ConcurrentDictionary<Type, Dictionary<string, PhiSensitivity>> PhiPropertyCache = new();

    /// <summary>
    /// Returns a JSON string with all PHI-marked properties masked.
    /// </summary>
    public static string MaskJson(object request)
    {
        var json = JsonSerializer.Serialize(request);
        var phiProperties = GetPhiProperties(request.GetType());

        if (phiProperties.Count == 0)
            return json;

        var node = JsonNode.Parse(json);
        if (node is not JsonObject obj)
            return json;

        foreach (var (propertyName, sensitivity) in phiProperties)
        {
            // Match case-insensitively since JSON serialization may change casing
            var key = obj.FirstOrDefault(kvp =>
                kvp.Key.Equals(propertyName, StringComparison.OrdinalIgnoreCase)).Key;

            if (key is not null && obj[key] is not null)
            {
                obj[key] = GetMaskValue(sensitivity);
            }
        }

        return obj.ToJsonString();
    }

    /// <summary>
    /// Gets all properties with [Phi] attribute for a given type, cached for performance.
    /// </summary>
    public static IReadOnlyDictionary<string, PhiSensitivity> GetPhiProperties(Type type)
    {
        return PhiPropertyCache.GetOrAdd(type, t =>
        {
            var result = new Dictionary<string, PhiSensitivity>(StringComparer.OrdinalIgnoreCase);
            foreach (var prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var attr = prop.GetCustomAttribute<PhiAttribute>();
                if (attr is not null)
                    result[prop.Name] = attr.Sensitivity;
            }
            return result;
        });
    }

    private static string GetMaskValue(PhiSensitivity sensitivity) => sensitivity switch
    {
        PhiSensitivity.Standard => "***PHI***",
        PhiSensitivity.Sensitive => "***SENSITIVE-PHI***",
        PhiSensitivity.HighlySensitive => "***HIGHLY-SENSITIVE-PHI***",
        _ => "***REDACTED***"
    };
}
