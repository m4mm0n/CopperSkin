namespace CopperSkin.Core.Theming;

public sealed record ThemeDiagnostic(string Severity, string Code, string Message, string? Path = null)
{
    public override string ToString() => string.IsNullOrWhiteSpace(Path)
        ? $"{Severity} {Code}: {Message}"
        : $"{Severity} {Code} at {Path}: {Message}";
}
