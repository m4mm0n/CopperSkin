namespace CopperSkin.Core.Theming;

public readonly record struct ThemeTokenKey(string Value)
{
    public override string ToString() => Value;

    public static ThemeTokenKey From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Theme token key cannot be empty.", nameof(value));

        return new ThemeTokenKey(value.Trim());
    }
}
