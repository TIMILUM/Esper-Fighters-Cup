using System;
using System.Security.Cryptography;
using System.Text;

public static class CryptoUtil
{
    /// <summary>
    /// 문자열을 SHA256으로 암호화된 해시로 변환하여 반환합니다.
    /// </summary>
    /// <param name="input">변환할 문자열</param>
    /// <returns></returns>
    public static string ToSHA256Hash(this string input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input), "변환할 문자열이 null입니다.");
        }

        var sha256 = new SHA256Managed();
        var hashcode = sha256.ComputeHash(Encoding.ASCII.GetBytes(input));
        var hashString = new StringBuilder();
        foreach (var bytecode in hashcode)
        {
            hashString.Append($"{bytecode:x2}");
        }

        return hashString.ToString();
    }
}
