using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TempTest
{

    public class SaveLoadManager
    {
        private string _data;
        private bool _encrypt;
        private string _FileLocation;
        private string _FileName;

        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public SaveLoadManager(string fileLocation, string fileName, bool encrypt = true)
        {
            this._FileLocation = fileLocation;
            this._FileName = fileName;
            this._encrypt = encrypt;
        }

        public void CreateXML()
        {
            StreamWriter writer;
            if (!Directory.Exists(this._FileLocation) && !string.IsNullOrEmpty(this._FileLocation))
            {
                Directory.CreateDirectory(this._FileLocation);
            }
            FileInfo info = new FileInfo(this._FileLocation + @"\" + this._FileName);
            if (!info.Exists)
            {
                writer = info.CreateText();
            }
            else
            {
                info.Delete();
                writer = info.CreateText();
            }
            if (this._encrypt)
            {
                writer.Write(this.Encrypt(this._data));
            }
            else
            {
                writer.Write(this._data);
            }
            writer.Close();
        }

        public string Decrypt(string cipherString)
        {
            byte[] inputBuffer = Convert.FromBase64String(cipherString);
            string s = "SoulGame";
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] buffer = provider.ComputeHash(Encoding.UTF8.GetBytes(s));
            provider.Clear();
            TripleDESCryptoServiceProvider provider2 = new TripleDESCryptoServiceProvider
            {
                Key = buffer,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            byte[] bytes = provider2.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
            provider2.Clear();
            return Encoding.UTF8.GetString(bytes);
        }

        public object DeserializeObject(System.Type userType)
        {
            XmlSerializer serializer = new XmlSerializer(userType);
            MemoryStream w = new MemoryStream(this.StringToUTF8ByteArray(this._data));
            new XmlTextWriter(w, Encoding.UTF8);
            return serializer.Deserialize(w);
        }

        public string Encrypt(string toEncrypt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(toEncrypt);
            string s = "SoulGame";
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] buffer = provider.ComputeHash(Encoding.UTF8.GetBytes(s));
            provider.Clear();
            TripleDESCryptoServiceProvider provider2 = new TripleDESCryptoServiceProvider
            {
                Key = buffer,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            byte[] inArray = provider2.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
            provider2.Clear();
            return Convert.ToBase64String(inArray, 0, inArray.Length);
        }

        public bool isXMLExit()
        {
            FileInfo info = new FileInfo(this._FileLocation + @"\" + this._FileName);
            return info.Exists;
        }

        public void LoadXML()
        {
            StreamReader reader = File.OpenText(this._FileLocation + @"\" + this._FileName);
            string cipherString = reader.ReadToEnd();
            reader.Close();
            if (this._encrypt)
            {
                this._data = this.Decrypt(cipherString);
            }
            else
            {
                this._data = cipherString;
            }
        }

        public void SerializeObject(object pObject, System.Type userType)
        {
            string str = null;
            MemoryStream w = new MemoryStream();
            XmlSerializer serializer = new XmlSerializer(userType);
            XmlTextWriter writer = new XmlTextWriter(w, Encoding.UTF8);
            serializer.Serialize((XmlWriter)writer, pObject);
            str = this.UTF8ByteArrayToString(((MemoryStream)writer.BaseStream).ToArray());
            this._data = str;
        }

        private byte[] StringToUTF8ByteArray(string pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(pXmlString);
        }

        private string UTF8ByteArrayToString(byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetString(characters);
        }

    }

}

