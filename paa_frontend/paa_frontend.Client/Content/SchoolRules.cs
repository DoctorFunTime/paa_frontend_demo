namespace paa_frontend.Client.Content;

// The demo has no school-specific rules. The page remains so its navigation
// matches the main frontend, but it must not publish another school's policy.
public static class SchoolRules
{
    public static IReadOnlyList<string> Finance { get; } = Array.Empty<string>();
    public static IReadOnlyList<string> General { get; } = Array.Empty<string>();
}
