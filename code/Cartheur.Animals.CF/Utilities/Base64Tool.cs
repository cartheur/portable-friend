using System;
using System.Text;

namespace Cartheur.Animals.CF.Utilities
{
    /// <summary>
    /// An encryption/decryption activity for aeon source files.
    /// </summary>
    public static class Base64Tool
    {
        /// <summary>
        /// Performs a Base64 encode.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns></returns>
        public static string Encode(string plainText)
        {
            if (plainText == "") return "Input string is null.";
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        /// <summary>
        /// Performs a Base64 decode.
        /// </summary>
        /// <param name="base64EncodedData">The Base64 encoded data.</param>
        /// <returns></returns>
        public static string Decode(string base64EncodedData)
        {
            if (base64EncodedData == "") return "Input string is null.";
            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes, 0, base64EncodedBytes.Length);
        }
    }
}
