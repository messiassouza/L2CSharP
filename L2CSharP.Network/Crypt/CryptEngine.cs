using L2CSharP.YoDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Network.Crypt
{

    public class CryptEngine
    {
        private bool updatedKey = false;
        private byte[] key = { 0x6b, 0x60, 0xcb, 0x5b, 0x82, 0xce, 0x90, 0xb1, 0xcc, 0x2b, 0x6c, 0x55, 0x6c, 0x6c, 0x6c, 0x6c };
        private BlowfishCipher cipher;

        public CryptEngine()
        {
            cipher = new BlowfishCipher(key);
        }

        public void updateKey(byte[] key)
        {
            this.key = key;
        }

        public bool Decrypt(byte[] data)
        {
            cipher.Decrypt(data);
            return VerifyChecksum(data);
        }

        public byte[] Encrypt(byte[] data)
        {
            Array.Resize(ref data, data.Length + 4);

            if (!updatedKey)
            {
                Array.Resize(ref data, data.Length + 4);
                Array.Resize(ref data, (data.Length + 8) - data.Length % 8);
                EncXORPass(data, (uint)L2Random.Next());
                cipher.Encrypt(data);
                cipher.Key = key;
                updatedKey = true;
            }
            else
            {
                Array.Resize(ref data, (data.Length + 8) - data.Length % 8);
                AppendChecksum(data);
                cipher.Encrypt(data);
            }
            return data;
        }

        private bool VerifyChecksum(byte[] Data)
        {
            long chksum = 0;
            for (int i = 0; i < (Data.Length - 4); i += 4)
                chksum ^= BitConverter.ToUInt32(Data, i);
            return 0 == chksum;
        }

        private void AppendChecksum(byte[] Data)
        {
            uint chksum = 0;
            int count = Data.Length - 8;
            int i;
            for (i = 0; i < count; i += 4)
                chksum ^= BitConverter.ToUInt32(Data, i);
            Array.Copy(BitConverter.GetBytes(chksum), 0, Data, i, 4);
        }

        private void EncXORPass(byte[] Data, uint Key)
        {
            int stop = Data.Length - 8;
            uint ecx = Key;
            uint edx;

            for (int i = 4; i < stop; i += 4)
            {
                edx = BitConverter.ToUInt32(Data, i);
                ecx += edx;
                edx ^= ecx;
                Array.Copy(BitConverter.GetBytes(edx), 0, Data, i, 4);
            }
            Array.Copy(BitConverter.GetBytes(ecx), 0, Data, stop, 4);
        }

        public static string MD5Encryption(string Data)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.ASCII.GetBytes(Data);
            data = x.ComputeHash(data);
            string ret = "";

            for (int i = 0; i < data.Length; i++) //XOR hash
                data[i] ^= 0x03;
            for (int i = 0; i < data.Length; i++)
                ret += data[i].ToString("x2").ToLower();
            return ret;
        }
    }

}
