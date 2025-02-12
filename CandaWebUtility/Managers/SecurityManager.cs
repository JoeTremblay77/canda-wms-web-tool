using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

internal static class SecurityManager
{
    private static int a = 21;

    private static string k = "{BCD0B6CC-FA15-4F34-9F34-B12F7767E7E7}";

    private static byte[] Generate256BitsOfRandomEntropy()
    {
        var randomBytes = new byte[32];
        using (var rngCsp = new RNGCryptoServiceProvider())
        {
            rngCsp.GetBytes(randomBytes);
        }
        return randomBytes;
    }

    private static byte[] hideToArrayFromArray(byte[] plainTextBytes, string k)
    {
        var saltStringBytes = Generate256BitsOfRandomEntropy();
        var ivStringBytes = Generate256BitsOfRandomEntropy();

        CryptoStream cryptoStream = null;
        try
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (var password = new Rfc2898DeriveBytes(k, saltStringBytes, a))
                {
                    var keyBytes = password.GetBytes(32);
                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = 256;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                        {
                            cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            var cipherTextBytes = saltStringBytes;
                            cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                            cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();

                            return cipherTextBytes;
                        }
                    }
                }
            }
        }
        finally
        {
        }
    }

    private static string randomLetters(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private static string randomSymbols(int length)
    {
        const string chars = "!@#$%^&*()_+{[}]<>,./?~";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private static bool regex(string p, string pattern)
    {
        Regex regex = new Regex(pattern);
        Match match = regex.Match(p);
        return match.Success;
    }

    private static string showFromArrayToString(byte[] cipherTextBytesWithSaltAndIv, string k)
    {
        int z = 256;
        var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(z / 8).ToArray();
        var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(z / 8).Take(z / 8).ToArray();
        var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((z / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((z / 8) * 2)).ToArray();
        CryptoStream cryptoStream = null;
        byte[] plainTextBytes = new byte[cipherTextBytes.Length];
        int decryptedByteCount = 0;
        using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
        {
            using (var password = new Rfc2898DeriveBytes(k, saltStringBytes, a))
            {
                var keyBytes = password.GetBytes(z / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes);
                    cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                }

                decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            }
        }
        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
    }

    internal static string GetUniqueKey(int maxSize)
    {
        char[] chars = new char[62];
        chars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        byte[] data = new byte[1];
        using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
        {
            crypto.GetNonZeroBytes(data);
            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
        }
        StringBuilder result = new StringBuilder(maxSize);
        foreach (byte b in data)
        {
            result.Append(chars[b % (chars.Length)]);
        }
        return result.ToString();
    }

    internal static string Hide(string s)
    {
        byte[] b = hideToArrayFromArray(Encoding.UTF8.GetBytes(s), k);
        string result = Convert.ToBase64String(b);
        return result;
    }

    internal static bool PasswordInvalid(string p)
    {
        // has no spaces
        if (p.Contains(' '))
        {
            return true;
        }

        if (p.Length < Defaults.MinPasswordLength)
        {
            return true;
        }

        // has digit
        if (!regex(p, @"[0-9]{1,}"))
        {
            return true;
        }

        // has lowercase
        if (!regex(p, @"[a-z]{1,}"))
        {
            return true;
        }

        // has uppercase
        if (!regex(p, @"[A-Z]{1,}"))
        {
            return true;
        }

        // has non alpha
        if (!regex(p, @"[^a-zA-Z0-9]{1,}"))
        {
            return true;
        }

        return false;
    }

    internal static string Show(string s)
    {
        byte[] fullBytes = Convert.FromBase64String(s);
        string result = showFromArrayToString(fullBytes, k);
        return result;
    }

    internal static string TempToken()
    {
        // Passwords must be at least 9 characters.
        // Passwords must have at least one non letter or digit character.
        // Passwords must have at least one digit ('0'-'9').
        // Passwords must have at least one lowercase ('a'-'z').
        // Passwords must have at least one uppercase ('A'-'Z')

        string result = string.Empty;

        Random r = new Random();
        string a = r.Next(102, 1999).ToString();
        string b = randomLetters(5).ToUpper();
        string c = randomLetters(3).ToLower();
        string d = randomSymbols(3);
        string e = GetUniqueKey(16);

        result = a + b + c + d + e;

        return result;
    }
}