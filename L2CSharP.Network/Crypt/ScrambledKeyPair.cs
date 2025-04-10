 


using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Network.Crypt
{
    using System.Security.Cryptography;
    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Math;
    using Org.BouncyCastle.Security;

    public sealed class ScrambledKeyPair : IDisposable
    {
        private const int KeySize = 1024;
        private const int Certainty = 10;
        private const int PublicExponent = 65537;
        private const int ModulusLength = 0x80;

        private readonly RsaKeyParameters _publicKey;
        private readonly byte[] _scrambledModulus;
        private readonly RsaPrivateCrtKeyParameters _privateKey;
        private bool _disposed;

        public byte[] ScrambledModulus => (byte[])_scrambledModulus.Clone();
        public RsaKeyParameters PublicKey => _publicKey;

        private ScrambledKeyPair(AsymmetricCipherKeyPair keyPair)
        {
            _publicKey = (RsaKeyParameters)keyPair.Public;
            _privateKey = (RsaPrivateCrtKeyParameters)keyPair.Private;
            _scrambledModulus = ScrambleModulus(_publicKey.Modulus);
        }

        public static ScrambledKeyPair Generate()
        {
            var rnd = new SecureRandom();
            var parameters = new RsaKeyGenerationParameters(
                BigInteger.ValueOf(PublicExponent),
                rnd,
                KeySize,
                Certainty);

            var generator = new RsaKeyPairGenerator();
            generator.Init(parameters);

            return new ScrambledKeyPair(generator.GenerateKeyPair());
        }

        public byte[] GetEncryptedModulus()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ScrambledKeyPair));

            return (byte[])_scrambledModulus.Clone();
        }

        public RSAParameters GetPrivateKeyParameters()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ScrambledKeyPair));

            return new RSAParameters
            {
                Modulus = _privateKey.Modulus.ToByteArrayUnsigned(),
                Exponent = _privateKey.PublicExponent.ToByteArrayUnsigned(),
                D = _privateKey.Exponent.ToByteArrayUnsigned(),
                P = _privateKey.P.ToByteArrayUnsigned(),
                Q = _privateKey.Q.ToByteArrayUnsigned(),
                DP = _privateKey.DP.ToByteArrayUnsigned(),
                DQ = _privateKey.DQ.ToByteArrayUnsigned(),
                InverseQ = _privateKey.QInv.ToByteArrayUnsigned()
            };
        }

        private static byte[] ScrambleModulus(BigInteger modulus)
        {
            byte[] scrambled = modulus.ToByteArrayUnsigned();

            // Ensure correct length
            if (scrambled.Length != ModulusLength)
            {
                var temp = new byte[ModulusLength];
                Buffer.BlockCopy(scrambled, 0, temp, ModulusLength - scrambled.Length, scrambled.Length);
                scrambled = temp;
            }

            // Step 1: Swap bytes 0x00-0x04 with 0x4d-0x50
            SwapBytes(scrambled, 0, 0x4d, 4);

            // Step 2: XOR first 0x40 bytes with last 0x40 bytes
            XorBytes(scrambled, 0, 0x40, 0x40);

            // Step 3: XOR bytes 0x0d-0x10 with bytes 0x34-0x38
            XorBytes(scrambled, 0x0d, 0x34, 4);

            // Step 4: XOR last 0x40 bytes with first 0x40 bytes
            XorBytes(scrambled, 0x40, 0, 0x40);

            return scrambled;
        }

        private static void SwapBytes(byte[] data, int offset1, int offset2, int length)
        {
            for (int i = 0; i < length; i++)
            {
                (data[offset1 + i], data[offset2 + i]) = (data[offset2 + i], data[offset1 + i]);
            }
        }

        private static void XorBytes(byte[] data, int destOffset, int srcOffset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                data[destOffset + i] ^= data[srcOffset + i];
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            // Clear sensitive data
            Array.Clear(_scrambledModulus, 0, _scrambledModulus.Length);

            // Note: BouncyCastle RSA parameters are immutable so we can't clear them
            // In a real high-security scenario, consider using System.Security.Cryptography.RSA

            _disposed = true;
        }
    }
}
