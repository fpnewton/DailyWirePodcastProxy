using System.Numerics;
using System.Text;

namespace PodcastProxy.Web.Extensions;

public static class ConvertExtensions
{
    private const string Digits = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
    
    public static string ToBase58String(this byte[] bytes)
    {
        BigInteger intData = 0;
        
        foreach (var @byte in bytes)
        {
            intData = intData * 256 + @byte;
        }

        var builder = new StringBuilder();
        
        while (intData > 0)
        {
            var remainder = (int)(intData % 58);
            intData /= 58;

            builder.Insert(0, Digits[remainder]);
        }

        // Append `1` for each leading 0 byte
        for (var i = 0; i < bytes.Length && bytes[i] == 0; i++)
        {
            builder.Insert(0, '1');
        }
        
        return builder.ToString();
    }
}