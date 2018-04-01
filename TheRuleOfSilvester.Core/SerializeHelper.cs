using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheRuleOfSilvester.Core
{
    public static class SerializeHelper
    {
        public static T Deserialize<T>(byte[] value) where T : IByteSerializable
        {
            var tmpObj = Activator.CreateInstance<T>();

            using (var memoryStream = new MemoryStream(value))
            {
                using (var binaryReader = new BinaryReader(memoryStream))
                {
                    tmpObj.Deserialize(binaryReader);
                }

                return tmpObj;
            }
        }

        public static byte[] ToByteArray<T>(T obj) where T : IByteSerializable
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(memoryStream))
                {
                    obj.Serialize(binaryWriter);
                }

                return memoryStream.ToArray();
            }
        }
    }
}
