
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace QQChat.Classes
{

    class PassHelper
    {
        //默认密钥向量 
        private static byte[] _iv = { 34, 51, 3, 61, 34, 5, 61, 13, 5, 6, 2, 5, 42, 245, 6, 4 };
        private static byte[] _key = { 52, 46, 72, 42, 62, 245, 6, 42, 46, 4, 24, 5, 62, 46, 44, 24 };

        /// <summary>
        /// AES加密算法
        /// </summary>
        /// <param name="plainText">明文字符串</param>
        /// <param name="strKey">密钥</param>
        /// <returns>返回加密后的密文</returns>
        public static string AESEncrypt(string plainText)
        {
            //分组加密算法
            Rijndael des = Rijndael.Create();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(plainText);//得到需要加密的字节数组
            //设置密钥及密钥向量
            des.Key = _key;
            des.IV = _iv;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            byte[] cipherBytes = ms.ToArray();//得到加密后的字节数组
            cs.Close();
            ms.Close();
            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="message">密文</param>
        /// <param name="strKey">密钥</param>
        /// <returns>返回解密后的字符串</returns>
        public static string AESDecrypt(string message)
        {
            var cipherText = Convert.FromBase64String(message);
            Rijndael des = Rijndael.Create();
            des.Key = _key;
            des.IV = _iv;
            byte[] decryptBytes = new byte[cipherText.Length];
            MemoryStream ms = new MemoryStream(cipherText);
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read);
            cs.Read(decryptBytes, 0, decryptBytes.Length);
            cs.Close();
            ms.Close();
            return Encoding.UTF8.GetString(decryptBytes);
        }
    }
}
