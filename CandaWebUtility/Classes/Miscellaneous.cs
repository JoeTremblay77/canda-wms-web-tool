using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

namespace CandaWebUtility
{
    internal static class DeepCopy
    {
        public static T Copy<T>(T objectToCopy)
        {
            T template;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, objectToCopy);
                ms.Position = 0;
                template = (T)formatter.Deserialize(ms);
            }
            return template;
        }
    }
}