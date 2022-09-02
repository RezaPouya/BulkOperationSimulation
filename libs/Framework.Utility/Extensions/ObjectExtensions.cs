using Newtonsoft.Json;
using System.Text;


namespace Operator.Extensions
{
    public static class ObjectExtensions
    {
        public static byte[] SerializeToBinary(this object obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }
    }
}
