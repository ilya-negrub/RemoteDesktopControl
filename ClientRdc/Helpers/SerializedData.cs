using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ClientRdc.Helpers
{
    internal static class SerializedData
    {
        public static byte[] SerializeData(this object self)
        {
            if (self == null)
                return new byte[0];
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, self);
            return ms.ToArray();

        }

        public static T DeserializeData<T>(this byte[] self)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(self, 0, self.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                T obj = (T)binForm.Deserialize(memStream);
                return obj;
            }
        }

    }
}
