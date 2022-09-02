using System;
using System.Collections.Generic;
using System.Text;

namespace Operator.Extensions
{
    public static class ObjectExtensions
    {
        public static byte[] SerializeToBinary(this object obj)
        {
            return Encoding.UTF8.GetBytes(obj.ToString());
        }
    }
}
