/*
 * ====================================================================================================
 *  Project        : CopperSkin
 *  File           : src\CopperSkin.Core\Theming\BinaryFieldEcdsa.cs
 *  Author         : Geir Gustavsen, ZeroLinez Softworx 2024 - 2026
 *  Created        : 2026-07-03 00:00:00 +02:00
 *  Last Modified  : 2026-07-03 09:57:50 +02:00
 *  CRC32          : B3ACE0F2
 *
 *  Description    :
 *                   Implements no-dependency binary-field ECDSA helpers for CopperSkin theme pack signing.
 *
 *  License        :
 *                   MIT
 *                   https://opensource.org/licenses/MIT
 *
 *  Notes          :
 *                   WPF theme engine extracted from the amChipper custom skin.
 * ====================================================================================================
 */
// CRC32-BODY: B3ACE0F2

using System.Numerics;
using System.Security.Cryptography;

namespace CopperSkin.Core.Theming;

/// <summary>
/// Carries a B-233 theme-pack signing key pair encoded as hexadecimal strings.
/// </summary>
public sealed record ThemePackSigningKeyPair(string PrivateKeyHex, string PublicKeyHex);

internal static class BinaryFieldEcdsa
{
    private static readonly BigInteger A = BigInteger.One;

    private static Point Add(Point first, Point second)
    {
        if (first.IsInfinity)
            return second;
        if (second.IsInfinity)
            return first;

        if (first.X == second.X)
        {
            if (first.Y == second.Y)
                return Double(first);
            return Point.Infinity;
        }

        var lambda = Divide(Add(first.Y, second.Y), Add(first.X, second.X));
        var x = Add(Add(Add(Square(lambda), lambda), first.X), Add(second.X, A));
        var y = Add(Add(Multiply(lambda, Add(first.X, x)), x), first.Y);
        return new Point(x, y);
    }

    private static BigInteger Add(BigInteger first, BigInteger second) => first ^ second;
    private static readonly BigInteger B = FromHex("066647EDE6C332C7F8C0923BB58213B333B20E9CE4281FE115F7D8F90AD");

    private static string CleanHex(string hex)
    {
        var cleaned = hex.Trim();
        if (cleaned.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            cleaned = cleaned.Substring(2);
        return cleaned.Replace(" ", string.Empty).Replace("_", string.Empty);
    }

    public static ThemePackSigningKeyPair CreateKeyPair()
    {
        var privateKey = RandomScalar();
        var publicKey = Multiply(BasePoint, privateKey);
        return new ThemePackSigningKeyPair(ToFixedHex(privateKey), EncodePublicKey(publicKey));
    }

    private static Point DecodePublicKey(string publicKeyHex)
    {
        var hex = CleanHex(publicKeyHex);
        if (!hex.StartsWith("04", StringComparison.OrdinalIgnoreCase) || hex.Length != 2 + (FieldBytes * 4))
            throw new FormatException("Public key must be an uncompressed B-233 point.");

        var x = FromHex(hex.Substring(2, FieldBytes * 2));
        var y = FromHex(hex.Substring(2 + (FieldBytes * 2), FieldBytes * 2));
        return new Point(x, y);
    }

    private static int Degree(BigInteger value)
    {
        var degree = -1;
        var current = value;
        while (current > BigInteger.Zero)
        {
            current >>= 1;
            degree++;
        }

        return degree;
    }

    private static BigInteger Divide(BigInteger numerator, BigInteger denominator) => Multiply(numerator, Invert(denominator));

    private static Point Double(Point point)
    {
        if (point.IsInfinity || point.X.IsZero)
            return Point.Infinity;

        var lambda = Add(point.X, Divide(point.Y, point.X));
        var x = Add(Add(Square(lambda), lambda), A);
        var y = Add(Square(point.X), Multiply(Add(lambda, BigInteger.One), x));
        return new Point(x, y);
    }

    private static string EncodePublicKey(Point point) => "04" + ToFixedHex(point.X) + ToFixedHex(point.Y);
    private const int FieldBits = 233;
    private const int FieldBytes = 30;
    private static readonly BigInteger FieldPolynomial = (BigInteger.One << FieldBits) | (BigInteger.One << ReductionTerm) | BigInteger.One;

    private static BigInteger FromBigEndian(byte[] bytes)
    {
        var littleEndian = new byte[bytes.Length + 1];
        for (var i = 0; i < bytes.Length; i++)
            littleEndian[i] = bytes[bytes.Length - 1 - i];
        return new BigInteger(littleEndian);
    }

    private static BigInteger FromHex(string hex) => FromBigEndian(HexToBytes(CleanHex(hex)));

    public static string GetPublicKey(string privateKeyHex)
    {
        var privateKey = ParsePrivateKey(privateKeyHex);
        return EncodePublicKey(Multiply(BasePoint, privateKey));
    }
    private static readonly BigInteger Gx = FromHex("0FAC9DFCBAC8313BB2139F1BB755FEF65BC391F8B36F8F8EB7371FD558B");
    private static readonly BigInteger Gy = FromHex("1006A08A41903350678E58528BEBF8A0BEFF867A7CA36716F7E01F81052");
    private static readonly Point BasePoint = new(Gx, Gy);

    private static byte[] HexToBytes(string hex)
    {
        if (hex.Length % 2 != 0)
            hex = "0" + hex;

        var bytes = new byte[hex.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            var pair = hex.Substring(i * 2, 2);
            if (!byte.TryParse(pair, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out bytes[i]))
                throw new FormatException("Invalid hexadecimal value.");
        }

        return bytes;
    }

    private static BigInteger Invert(BigInteger value)
    {
        if (value.IsZero)
            throw new DivideByZeroException();

        var u = value;
        var v = FieldPolynomial;
        var g1 = BigInteger.One;
        var g2 = BigInteger.Zero;
        while (u != BigInteger.One)
        {
            var shift = Degree(u) - Degree(v);
            if (shift < 0)
            {
                (u, v) = (v, u);
                (g1, g2) = (g2, g1);
                shift = -shift;
            }

            u ^= v << shift;
            g1 ^= g2 << shift;
        }

        return Reduce(g1);
    }

    private static bool IsOnCurve(Point point)
    {
        if (point.IsInfinity)
            return false;

        var left = Add(Square(point.Y), Multiply(point.X, point.Y));
        var x2 = Square(point.X);
        var right = Add(Add(Multiply(point.X, x2), Multiply(A, x2)), B);
        return left == right;
    }

    private static BigInteger Mod(BigInteger value, BigInteger modulus)
    {
        var result = value % modulus;
        return result.Sign < 0 ? result + modulus : result;
    }

    private static BigInteger ModInverse(BigInteger value, BigInteger modulus)
    {
        var t = BigInteger.Zero;
        var newT = BigInteger.One;
        var r = modulus;
        var newR = Mod(value, modulus);
        while (newR != BigInteger.Zero)
        {
            var quotient = r / newR;
            (t, newT) = (newT, t - quotient * newT);
            (r, newR) = (newR, r - quotient * newR);
        }

        if (r > BigInteger.One)
            throw new InvalidOperationException("Scalar is not invertible.");
        return Mod(t, modulus);
    }

    private static Point Multiply(Point point, BigInteger scalar)
    {
        var result = Point.Infinity;
        var addend = point;
        var k = scalar;
        while (k > BigInteger.Zero)
        {
            if (!k.IsEven)
                result = Add(result, addend);
            addend = Double(addend);
            k >>= 1;
        }

        return result;
    }

    private static BigInteger Multiply(BigInteger first, BigInteger second)
    {
        var result = BigInteger.Zero;
        var a = first;
        var b = second;
        while (b > BigInteger.Zero)
        {
            if (!b.IsEven)
                result ^= a;
            b >>= 1;
            a <<= 1;
        }

        return Reduce(result);
    }
    private static readonly BigInteger N = FromHex("01000000000000000000000000000013E974E72F8A6922031D2603CFE0D7");

    private static BigInteger ParsePrivateKey(string privateKeyHex)
    {
        var privateKey = FromHex(privateKeyHex);
        if (privateKey <= BigInteger.Zero || privateKey >= N)
            throw new ArgumentOutOfRangeException(nameof(privateKeyHex), "Private key must be in the B-233 scalar range.");
        return privateKey;
    }

    private readonly record struct Point(BigInteger X, BigInteger Y)
    {
        public static Point Infinity { get; } = new(BigInteger.Zero, BigInteger.Zero, true);

        public bool IsInfinity { get; }

        private Point(BigInteger x, BigInteger y, bool isInfinity)
            : this(x, y)
        {
            IsInfinity = isInfinity;
        }
    }

    public static string PublicKeyFromSignatureOwner(ThemePack pack)
    {
        pack.Metadata.TryGetValue(ThemePackSigner.PublicKeyKey, out var publicKey);
        return publicKey ?? string.Empty;
    }

    private static BigInteger RandomScalar() 
    {
        using var rng = RandomNumberGenerator.Create();
        return RandomScalar(rng);
    }

    private static BigInteger RandomScalar(RandomNumberGenerator rng)
    {
        var bytes = new byte[FieldBytes];
        while (true)
        {
            rng.GetBytes(bytes);
            bytes[0] &= 0x01;
            var scalar = FromBigEndian(bytes);
            if (scalar > BigInteger.Zero && scalar < N)
                return scalar;
        }
    }

    private static BigInteger Reduce(BigInteger value)
    {
        var result = value;
        for (var bit = Degree(result); bit >= FieldBits; bit = Degree(result))
        {
            var shift = bit - FieldBits;
            result ^= BigInteger.One << bit;
            result ^= BigInteger.One << (shift + ReductionTerm);
            result ^= BigInteger.One << shift;
        }

        return result;
    }
    private const int ReductionTerm = 74;

    public static string Sign(byte[] digest, string? privateKeyHex)
    {
        var privateKey = string.IsNullOrWhiteSpace(privateKeyHex) ? RandomScalar() : ParsePrivateKey(privateKeyHex!);
        var z = FromBigEndian(digest) % N;
        using var rng = RandomNumberGenerator.Create();
        while (true)
        {
            var k = RandomScalar(rng);
            var point = Multiply(BasePoint, k);
            var r = Mod(point.X, N);
            if (r.IsZero)
                continue;

            var s = Mod(ModInverse(k, N) * (z + (privateKey * r)), N);
            if (s.IsZero)
                continue;

            return $"{ToFixedHex(r)}.{ToFixedHex(s)}";
        }
    }

    private static BigInteger Square(BigInteger value) => Multiply(value, value);

    private static byte[] ToBigEndian(BigInteger value, int length)
    {
        if (value.Sign < 0)
            throw new ArgumentOutOfRangeException(nameof(value));

        var littleEndian = value.ToByteArray();
        var bytes = new byte[length];
        for (var i = 0; i < littleEndian.Length && i < length; i++)
            bytes[length - 1 - i] = littleEndian[i];
        return bytes;
    }

    private static string ToFixedHex(BigInteger value) => BitConverter.ToString(ToBigEndian(value, FieldBytes)).Replace("-", string.Empty);

    private static bool TryParseSignature(string signature, out BigInteger r, out BigInteger s)
    {
        r = BigInteger.Zero;
        s = BigInteger.Zero;
        var parts = signature.Split('.');
        if (parts.Length != 2)
            return false;

        try
        {
            r = FromHex(parts[0]);
            s = FromHex(parts[1]);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public static bool Verify(byte[] digest, string signature, string publicKeyHex)
    {
        if (!TryParseSignature(signature, out var r, out var s))
            return false;
        if (r <= BigInteger.Zero || r >= N || s <= BigInteger.Zero || s >= N)
            return false;

        Point publicKey;
        try
        {
            publicKey = DecodePublicKey(publicKeyHex);
        }
        catch (FormatException)
        {
            return false;
        }

        if (!IsOnCurve(publicKey))
            return false;

        var z = FromBigEndian(digest) % N;
        var w = ModInverse(s, N);
        var u1 = Mod(z * w, N);
        var u2 = Mod(r * w, N);
        var candidate = Add(Multiply(BasePoint, u1), Multiply(publicKey, u2));
        if (candidate.IsInfinity)
            return false;

        return Mod(candidate.X, N) == r;
    }
}
