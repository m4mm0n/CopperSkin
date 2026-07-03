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
    private const int FieldBits = 233;
    private const int FieldBytes = 30;
    private const int ReductionTerm = 74;
    private static readonly BigInteger FieldPolynomial = (BigInteger.One << FieldBits) | (BigInteger.One << ReductionTerm) | BigInteger.One;
    private static readonly BigInteger A = BigInteger.One;
    private static readonly BigInteger B = FromHex("066647EDE6C332C7F8C0923BB58213B333B20E9CE4281FE115F7D8F90AD");
    private static readonly BigInteger Gx = FromHex("0FAC9DFCBAC8313BB2139F1BB755FEF65BC391F8B36F8F8EB7371FD558B");
    private static readonly BigInteger Gy = FromHex("1006A08A41903350678E58528BEBF8A0BEFF867A7CA36716F7E01F81052");
    private static readonly BigInteger N = FromHex("01000000000000000000000000000013E974E72F8A6922031D2603CFE0D7");
    private static readonly Point BasePoint = new(Gx, Gy);

    public static ThemePackSigningKeyPair CreateKeyPair()
    {
        BigInteger privateKey = RandomScalar();
        Point publicKey = Multiply(BasePoint, privateKey);
        return new ThemePackSigningKeyPair(ToFixedHex(privateKey), EncodePublicKey(publicKey));
    }

    public static string GetPublicKey(string privateKeyHex)
    {
        BigInteger privateKey = ParsePrivateKey(privateKeyHex);
        return EncodePublicKey(Multiply(BasePoint, privateKey));
    }

    public static string Sign(byte[] digest, string? privateKeyHex)
    {
        BigInteger privateKey = string.IsNullOrWhiteSpace(privateKeyHex) ? RandomScalar() : ParsePrivateKey(privateKeyHex!);
        BigInteger z = FromBigEndian(digest) % N;
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        while (true)
        {
            BigInteger k = RandomScalar(rng);
            Point point = Multiply(BasePoint, k);
            BigInteger r = Mod(point.X, N);
            if (r.IsZero)
                continue;

            BigInteger s = Mod(ModInverse(k, N) * (z + (privateKey * r)), N);
            if (s.IsZero)
                continue;

            return $"{ToFixedHex(r)}.{ToFixedHex(s)}";
        }
    }

    public static bool Verify(byte[] digest, string signature, string publicKeyHex)
    {
        if (!TryParseSignature(signature, out BigInteger r, out BigInteger s))
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

        BigInteger z = FromBigEndian(digest) % N;
        BigInteger w = ModInverse(s, N);
        BigInteger u1 = Mod(z * w, N);
        BigInteger u2 = Mod(r * w, N);
        Point candidate = Add(Multiply(BasePoint, u1), Multiply(publicKey, u2));
        if (candidate.IsInfinity)
            return false;

        return Mod(candidate.X, N) == r;
    }

    public static string PublicKeyFromSignatureOwner(ThemePack pack)
    {
        pack.Metadata.TryGetValue(ThemePackSigner.PublicKeyKey, out string? publicKey);
        return publicKey ?? string.Empty;
    }

    private static BigInteger ParsePrivateKey(string privateKeyHex)
    {
        BigInteger privateKey = FromHex(privateKeyHex);
        if (privateKey <= BigInteger.Zero || privateKey >= N)
            throw new ArgumentOutOfRangeException(nameof(privateKeyHex), "Private key must be in the B-233 scalar range.");
        return privateKey;
    }

    private static bool TryParseSignature(string signature, out BigInteger r, out BigInteger s)
    {
        r = BigInteger.Zero;
        s = BigInteger.Zero;
        string[] parts = signature.Split('.');
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

    private static string EncodePublicKey(Point point) => "04" + ToFixedHex(point.X) + ToFixedHex(point.Y);

    private static Point DecodePublicKey(string publicKeyHex)
    {
        string hex = CleanHex(publicKeyHex);
        if (!hex.StartsWith("04", StringComparison.OrdinalIgnoreCase) || hex.Length != 2 + (FieldBytes * 4))
            throw new FormatException("Public key must be an uncompressed B-233 point.");

        BigInteger x = FromHex(hex.Substring(2, FieldBytes * 2));
        BigInteger y = FromHex(hex.Substring(2 + (FieldBytes * 2), FieldBytes * 2));
        return new Point(x, y);
    }

    private static bool IsOnCurve(Point point)
    {
        if (point.IsInfinity)
            return false;

        BigInteger left = Add(Square(point.Y), Multiply(point.X, point.Y));
        BigInteger x2 = Square(point.X);
        BigInteger right = Add(Add(Multiply(point.X, x2), Multiply(A, x2)), B);
        return left == right;
    }

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

        BigInteger lambda = Divide(Add(first.Y, second.Y), Add(first.X, second.X));
        BigInteger x = Add(Add(Add(Square(lambda), lambda), first.X), Add(second.X, A));
        BigInteger y = Add(Add(Multiply(lambda, Add(first.X, x)), x), first.Y);
        return new Point(x, y);
    }

    private static Point Double(Point point)
    {
        if (point.IsInfinity || point.X.IsZero)
            return Point.Infinity;

        BigInteger lambda = Add(point.X, Divide(point.Y, point.X));
        BigInteger x = Add(Add(Square(lambda), lambda), A);
        BigInteger y = Add(Square(point.X), Multiply(Add(lambda, BigInteger.One), x));
        return new Point(x, y);
    }

    private static Point Multiply(Point point, BigInteger scalar)
    {
        Point result = Point.Infinity;
        Point addend = point;
        BigInteger k = scalar;
        while (k > BigInteger.Zero)
        {
            if (!k.IsEven)
                result = Add(result, addend);
            addend = Double(addend);
            k >>= 1;
        }

        return result;
    }

    private static BigInteger Add(BigInteger first, BigInteger second) => first ^ second;

    private static BigInteger Square(BigInteger value) => Multiply(value, value);

    private static BigInteger Divide(BigInteger numerator, BigInteger denominator) => Multiply(numerator, Invert(denominator));

    private static BigInteger Multiply(BigInteger first, BigInteger second)
    {
        BigInteger result = BigInteger.Zero;
        BigInteger a = first;
        BigInteger b = second;
        while (b > BigInteger.Zero)
        {
            if (!b.IsEven)
                result ^= a;
            b >>= 1;
            a <<= 1;
        }

        return Reduce(result);
    }

    private static BigInteger Invert(BigInteger value)
    {
        if (value.IsZero)
            throw new DivideByZeroException();

        BigInteger u = value;
        BigInteger v = FieldPolynomial;
        BigInteger g1 = BigInteger.One;
        BigInteger g2 = BigInteger.Zero;
        while (u != BigInteger.One)
        {
            int shift = Degree(u) - Degree(v);
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

    private static BigInteger Reduce(BigInteger value)
    {
        BigInteger result = value;
        for (int bit = Degree(result); bit >= FieldBits; bit = Degree(result))
        {
            int shift = bit - FieldBits;
            result ^= BigInteger.One << bit;
            result ^= BigInteger.One << (shift + ReductionTerm);
            result ^= BigInteger.One << shift;
        }

        return result;
    }

    private static BigInteger ModInverse(BigInteger value, BigInteger modulus)
    {
        BigInteger t = BigInteger.Zero;
        BigInteger newT = BigInteger.One;
        BigInteger r = modulus;
        BigInteger newR = Mod(value, modulus);
        while (newR != BigInteger.Zero)
        {
            BigInteger quotient = r / newR;
            (t, newT) = (newT, t - quotient * newT);
            (r, newR) = (newR, r - quotient * newR);
        }

        if (r > BigInteger.One)
            throw new InvalidOperationException("Scalar is not invertible.");
        return Mod(t, modulus);
    }

    private static BigInteger Mod(BigInteger value, BigInteger modulus)
    {
        BigInteger result = value % modulus;
        return result.Sign < 0 ? result + modulus : result;
    }

    private static BigInteger RandomScalar() 
    {
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        return RandomScalar(rng);
    }

    private static BigInteger RandomScalar(RandomNumberGenerator rng)
    {
        byte[] bytes = new byte[FieldBytes];
        while (true)
        {
            rng.GetBytes(bytes);
            bytes[0] &= 0x01;
            BigInteger scalar = FromBigEndian(bytes);
            if (scalar > BigInteger.Zero && scalar < N)
                return scalar;
        }
    }

    private static int Degree(BigInteger value)
    {
        int degree = -1;
        BigInteger current = value;
        while (current > BigInteger.Zero)
        {
            current >>= 1;
            degree++;
        }

        return degree;
    }

    private static string ToFixedHex(BigInteger value) => BitConverter.ToString(ToBigEndian(value, FieldBytes)).Replace("-", string.Empty);

    private static BigInteger FromHex(string hex) => FromBigEndian(HexToBytes(CleanHex(hex)));

    private static string CleanHex(string hex)
    {
        string cleaned = hex.Trim();
        if (cleaned.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            cleaned = cleaned.Substring(2);
        return cleaned.Replace(" ", string.Empty).Replace("_", string.Empty);
    }

    private static byte[] HexToBytes(string hex)
    {
        if (hex.Length % 2 != 0)
            hex = "0" + hex;

        byte[] bytes = new byte[hex.Length / 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            string pair = hex.Substring(i * 2, 2);
            if (!byte.TryParse(pair, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out bytes[i]))
                throw new FormatException("Invalid hexadecimal value.");
        }

        return bytes;
    }

    private static BigInteger FromBigEndian(byte[] bytes)
    {
        byte[] littleEndian = new byte[bytes.Length + 1];
        for (int i = 0; i < bytes.Length; i++)
            littleEndian[i] = bytes[bytes.Length - 1 - i];
        return new BigInteger(littleEndian);
    }

    private static byte[] ToBigEndian(BigInteger value, int length)
    {
        if (value.Sign < 0)
            throw new ArgumentOutOfRangeException(nameof(value));

        byte[] littleEndian = value.ToByteArray();
        byte[] bytes = new byte[length];
        for (int i = 0; i < littleEndian.Length && i < length; i++)
            bytes[length - 1 - i] = littleEndian[i];
        return bytes;
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
}
