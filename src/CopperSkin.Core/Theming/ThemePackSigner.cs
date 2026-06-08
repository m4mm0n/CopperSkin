/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Core\Theming\ThemePackSigner.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-06-08 00:00:00 +02:00
 *  Last Modified  : 2026-06-08 19:36:14 +02:00
 *  CRC32          : F200F33C
 *
 *  Description    :
 *                   Computes deterministic CopperSkin theme-pack signatures.
 *
 *  License        :
 *                   MIT
 *                   https://opensource.org/licenses/MIT
 *
 *  Notes          :
 *                   WPF theme engine extracted from the amChipper custom skin.
 * ====================================================================================================
 */
// CRC32-BODY: F200F33C
using System.Security.Cryptography;
using System.Text;

namespace CopperSkin.Core.Theming;

/// <summary>
/// Adds deterministic hash signatures to CopperSkin theme packs.
/// </summary>
public static class ThemePackSigner
{
    /// <summary>
    /// Metadata key that stores the SHA-256 signature.
    /// </summary>
    public const string SignatureKey = "signature.sha256";

    /// <summary>
    /// Signs a pack in-place and returns the generated SHA-256 hash.
    /// </summary>
    public static string Sign(ThemePack pack, string signer = "CopperSkin")
    {
        pack.Metadata["signature.algorithm"] = "SHA-256";
        pack.Metadata["signature.signer"] = signer;
        pack.Metadata["signature.createdUtc"] = DateTime.UtcNow.ToString("O");
        string hash = ComputeHash(pack);
        pack.Metadata[SignatureKey] = hash;
        return hash;
    }

    /// <summary>
    /// Verifies a pack against the SHA-256 signature stored in metadata.
    /// </summary>
    public static bool Verify(ThemePack pack)
    {
        return pack.Metadata.TryGetValue(SignatureKey, out string? signature) &&
            string.Equals(signature, ComputeHash(pack), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Computes the deterministic SHA-256 hash used by CopperSkin signatures.
    /// </summary>
    public static string ComputeHash(ThemePack pack)
    {
        using SHA256 sha = SHA256.Create();
        byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(Normalize(pack)));
        return ToHex(hash);
    }

    private static string Normalize(ThemePack pack)
    {
        var builder = new StringBuilder();
        builder.Append("pack|").Append(pack.Id).Append('|').Append(pack.Name).Append('|').Append(pack.Version).AppendLine();
        foreach (KeyValuePair<string, string> pair in pack.Metadata
            .Where(static p => !string.Equals(p.Key, SignatureKey, StringComparison.OrdinalIgnoreCase))
            .OrderBy(static p => p.Key, StringComparer.OrdinalIgnoreCase))
        {
            builder.Append("meta|").Append(pair.Key).Append('|').Append(pair.Value).AppendLine();
        }

        foreach (ThemeDefinition theme in pack.Themes.OrderBy(static t => t.Id, StringComparer.OrdinalIgnoreCase))
        {
            builder.Append("theme|").Append(theme.Id).Append('|').Append(theme.Name).Append('|').Append(theme.BaseThemeId).AppendLine();
            foreach (KeyValuePair<string, string> pair in theme.Metadata.OrderBy(static p => p.Key, StringComparer.OrdinalIgnoreCase))
                builder.Append("theme-meta|").Append(pair.Key).Append('|').Append(pair.Value).AppendLine();
            foreach (KeyValuePair<string, string> pair in theme.Tokens.OrderBy(static p => p.Key, StringComparer.OrdinalIgnoreCase))
                builder.Append("token|").Append(pair.Key).Append('|').Append(pair.Value).AppendLine();
        }

        return builder.ToString();
    }

    private static string ToHex(byte[] bytes)
    {
        char[] chars = new char[bytes.Length * 2];
        const string alphabet = "0123456789ABCDEF";
        for (int i = 0; i < bytes.Length; i++)
        {
            chars[i * 2] = alphabet[bytes[i] >> 4];
            chars[(i * 2) + 1] = alphabet[bytes[i] & 0xF];
        }

        return new string(chars);
    }
}
