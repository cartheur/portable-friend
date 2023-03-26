using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Cartheur.Animals.CF.Utilities
{
    /// <summary>
    /// Class to encrypt and decrypt files as required.
    /// </summary>
    public static class Cryptography
    {
        public static string Key { get; set; }
        /// <summary>
        /// Encrypts the file.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="outputFile">The output file.</param>
        /// <param name="password">The password.</param>
        public static void EncryptFile(string inputFile, string outputFile, string password)
        {
            try
            {
                //string password = @"myKey123"; // Your Key Here
                UnicodeEncoding encoding = new UnicodeEncoding();
                byte[] key = encoding.GetBytes(password);
                string cryptFile = outputFile;
                FileStream fileStreamCrypt = new FileStream(cryptFile, FileMode.Create);
                RijndaelManaged rijnManaged = new RijndaelManaged();
                CryptoStream cryptoStream = new CryptoStream(fileStreamCrypt, rijnManaged.CreateEncryptor(key, key), CryptoStreamMode.Write);
                FileStream fileStreamInput = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fileStreamInput.ReadByte()) != -1)
                    cryptoStream.WriteByte((byte)data);

                fileStreamInput.Close();
                cryptoStream.Close();
                fileStreamCrypt.Close();
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.Cryptography);
            }
        }
        /// <summary>
        /// Decrypts the file.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="outputFile">The output file.</param>
        /// <param name="password">The password.</param>
        public static void DecryptFile(string inputFile, string outputFile, string password)
        {
            try
            {
                //string password = @"myKey123"; // Your Key Here
                UnicodeEncoding encoding = new UnicodeEncoding();
                byte[] key = encoding.GetBytes(password);
                FileStream fileStreamCrypt = new FileStream(inputFile, FileMode.Open);
                RijndaelManaged rijnManaged = new RijndaelManaged();
                CryptoStream cryptoStream = new CryptoStream(fileStreamCrypt, rijnManaged.CreateDecryptor(key, key), CryptoStreamMode.Read);
                FileStream fileStreamOutput = new FileStream(outputFile, FileMode.Create);

                int data;
                while ((data = cryptoStream.ReadByte()) != -1)
                    fileStreamOutput.WriteByte((byte) data);

                fileStreamOutput.Close();
                cryptoStream.Close();
                fileStreamCrypt.Close();
            }
            catch (Exception ex)
            {
                Logging.WriteLog(ex.Message, Logging.LogType.Error, Logging.LogCaller.Cryptography);
            }
            
        }
    }
}
