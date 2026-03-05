namespace HealthTrack.Domain.Attributes;

/// <summary>
/// Marks a property as containing Protected Health Information (PHI)
/// under HIPAA regulations. PHI fields are automatically masked in
/// audit logs and require elevated access to view.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class PhiAttribute : Attribute
{
    /// <summary>
    /// The sensitivity level of this PHI field.
    /// </summary>
    public PhiSensitivity Sensitivity { get; }

    /// <summary>
    /// Description of what category of PHI this represents.
    /// </summary>
    public string Category { get; set; }

    public PhiAttribute(PhiSensitivity sensitivity = PhiSensitivity.Standard)
    {
        Sensitivity = sensitivity;
        Category = "General";
    }
}

public enum PhiSensitivity
{
    /// <summary>Standard PHI - name, address, dates, etc.</summary>
    Standard,
    /// <summary>Sensitive PHI - diagnoses, mental health, substance abuse</summary>
    Sensitive,
    /// <summary>Highly sensitive - HIV status, genetic data, psychotherapy notes</summary>
    HighlySensitive
}
