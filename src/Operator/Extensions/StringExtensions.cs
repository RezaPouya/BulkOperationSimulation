using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Operator.Extensions
{
    public static class StringExtensions
    {
        public static string GetRedisKey(this string filePath)
        {
            filePath = new Regex("[^a-zA-Z0-9 -]").Replace(filePath, "");
            var key = Convert.ToBase64String(Encoding.UTF8.GetBytes(filePath));
            return $"{key}_{key.GetHashCode()}";
        }
    }
}