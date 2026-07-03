/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Core\Theming\ThemePackSigner.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-06-08 00:00:00 +02:00
 *  Last Modified  : 2026-07-03 09:57:50 +02:00
 *  CRC32          : 7C0D85A7
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
// CRC32-BODY: 7C0D85A7

using System.Security.Cryptography;
using System.Text;

namespace CopperSkin.Core.Theming;

/// <summary>
/// Adds no-dependency binary-field ECC signatures to CopperSkin theme packs.
/// </summary>
public static class ThemePackSigner
{
    /// <summary>
    /// Metadata key that stores the B-233 ECDSA signature.
    /// </summary>
    public const string SignatureKey = "signature.eccgf2.signature";

    /// <summary>
    /// Metadata key that stores the uncompressed B-233 public key.
    /// </summary>
    public const string PublicKeyKey = "signature.eccgf2.publicKey";

    /// <summary>
    /// Metadata key that stores the active signing algorithm.
    /// </summary>
    public const string AlgorithmKey = "signature.algorithm";

    /// <summary>
    /// Metadata key that stores the active binary curve name.
    /// </summary>
    public const string CurveKey = "signature.curve";

    private const string AlgorithmValue = "ECDSA-GF2M-B233-SHA256";
    private const string CurveValue = "NIST-B-233";

    /// <summary>
    /// Creates a new B-233 private/public key pair encoded as hexadecimal strings.
    /// </summary>
    public static ThemePackSigningKeyPair CreateKeyPair() => BinaryFieldEcdsa.CreateKeyPair();

    /// <summary>
    /// Derives an uncompressed B-233 public key from a private key.
    /// </summary>
    public static string GetPublicKey(string privateKeyHex) => BinaryFieldEcdsa.GetPublicKey(privateKeyHex);

    /// <summary>
    /// Signs a pack in-place and returns the generated B-233 ECDSA signature.
    /// </summary>
    public static string Sign(ThemePack pack, string signer = "CopperSkin", string? privateKeyHex = null)
    {
        RemoveSignatureMetadata(pack);
        string? privateKey = privateKeyHex;
        if (string.IsNullOrWhiteSpace(privateKey))
        {
            ThemePackSigningKeyPair keyPair = CreateKeyPair();
            privateKey = keyPair.PrivateKeyHex;
            pack.Metadata[PublicKeyKey] = keyPair.PublicKeyHex;
        }
        else
        {
            pack.Metadata[PublicKeyKey] = GetPublicKey(privateKey!);
        }

        pack.Metadata[AlgorithmKey] = AlgorithmValue;
        pack.Metadata[CurveKey] = CurveValue;
        pack.Metadata["signature.signer"] = signer;
        pack.Metadata["signature.createdUtc"] = DateTime.UtcNow.ToString("O");
        string signature = BinaryFieldEcdsa.Sign(ComputeHashBytes(pack), privateKey);
        pack.Metadata[SignatureKey] = signature;
        return signature;
    }

    /// <summary>
    /// Verifies a pack against the B-233 ECDSA signature stored in metadata.
    /// </summary>
    public static bool Verify(ThemePack pack, string? trustedPublicKeyHex = null)
    {
        if (!pack.Metadata.TryGetValue(SignatureKey, out string? signature) ||
            !pack.Metadata.TryGetValue(PublicKeyKey, out string? embeddedPublicKey) ||
            !string.Equals(pack.Metadata.TryGetValue(AlgorithmKey, out string? algorithm) ? algorithm : string.Empty, AlgorithmValue, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        string publicKey = string.IsNullOrWhiteSpace(trustedPublicKeyHex) ? embeddedPublicKey : trustedPublicKeyHex!;
        return BinaryFieldEcdsa.Verify(ComputeHashBytes(pack), signature, publicKey);
    }

    /// <summary>
    /// Computes a deterministic SHA-256 payload hash for baselines, diffs, and signature input.
    /// </summary>
    public static string ComputeHash(ThemePack pack)
    {
        return ToHex(ComputeHashBytes(pack));
    }

    private static byte[] ComputeHashBytes(ThemePack pack)
    {
        using SHA256 sha = SHA256.Create();
        return sha.ComputeHash(Encoding.UTF8.GetBytes(Normalize(pack)));
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

    private static void RemoveSignatureMetadata(ThemePack pack)
    {
        pack.Metadata.Remove(SignatureKey);
        pack.Metadata.Remove(PublicKeyKey);
        pack.Metadata.Remove(AlgorithmKey);
        pack.Metadata.Remove(CurveKey);
        pack.Metadata.Remove("signature.sha256");
        pack.Metadata.Remove("signature.publicKey");
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
