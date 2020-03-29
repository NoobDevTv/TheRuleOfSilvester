using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using TheRuleOfSilvester.Runtime.Cells;
using TheRuleOfSilvester.Runtime.Interfaces;
using TheRuleOfSilvester.Runtime.Items;
using TheRuleOfSilvester.Network;
using TheRuleOfSilvester.Core;
using System.Collections;

namespace TheRuleOfSilvester.Runtime
{
    public static class SerializeHelper
    {
        public static Map Map { get; set; }

        private static readonly Dictionary<string, Type> mapCells;
        static SerializeHelper()
        {
            mapCells = Assembly.GetExecutingAssembly()
                 .GetTypes()
                 .Where(c => (typeof(MapCell).IsAssignableFrom(c) || typeof(BaseItemCell).IsAssignableFrom(c))
                                && c.GetCustomAttribute<GuidAttribute>() != null)
                 .ToDictionary(c => c.GetCustomAttribute<GuidAttribute>().Value,
                               c => c);
        }

        public static T Deserialize<T>(byte[] value) where T : IByteSerializable, new()
        {
            var tmpObj = new T();

            using (var memoryStream = new MemoryStream(value))
            {
                using (var binaryReader = new BinaryReader(memoryStream))
                {
                    tmpObj.Deserialize(binaryReader);
                }

                return tmpObj;
            }
        }

        public static ICollection<T> DeserializeToList<T>(byte[] value) where T : IByteSerializable, new()
        {
            var tmpList = new List<T>();

            if (value.Length > 0)
            {
                using (var memoryStream = new MemoryStream(value))
                {
                    using (var binaryReader = new BinaryReader(memoryStream))
                    {
                        var count = binaryReader.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            var tmpObj = new T();
                            tmpObj.Deserialize(binaryReader);
                            tmpList.Add(tmpObj);
                        }
                    }

                }
            }

            return tmpList;
        }

        public static Cell DeserializeMapCell(BinaryReader binaryReader)
        {
            var key = new Guid(binaryReader.ReadBytes(16)).ToString().ToUpper();
            var type = mapCells[key];

            var cell = (Cell)Activator.CreateInstance(type, new object[] { Map, binaryReader.ReadBoolean() });
            cell.Position = new Position(binaryReader.ReadInt32(), binaryReader.ReadInt32());
            cell.Color = Color.FromArgb(binaryReader.ReadInt32());
            return cell;
        }

        public static byte[] Serialize<T>(T obj) where T : IByteSerializable
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

        public static byte[] SerializeList<T>(IEnumerable<T> list) where T : IByteSerializable
        {
            if (list == null)
                return new byte[0];

            using var memoryStream = new MemoryStream();
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(list.Count());

                foreach (var obj in list)
                    obj.Serialize(binaryWriter);
            }

            return memoryStream.ToArray();
        }

        public static byte[] SerializeList(IEnumerable list)
        {
            if (list == null)
                return new byte[0];

            var castedEnumerable = list.Cast<IByteSerializable>();
            return SerializeList(castedEnumerable);
        }

        public static byte[] SerializeEnum(Enum e)
        {
            var underlyingType = Enum.GetUnderlyingType(e.GetType());
            var value = Convert.ChangeType(e, underlyingType);
            return GetBytes(value);
        }

        public static T DeserializeEnum<T>(byte[] e) where T : Enum
        {
            var underlyingType = Enum.GetUnderlyingType(typeof(T));

            var method = typeof(SerializeHelper)
                            .GetMethod(nameof(ToValue), BindingFlags.Static | BindingFlags.Public)
                            .MakeGenericMethod(underlyingType);

            var value = method.Invoke(null, new object[] { e });
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static byte[] GetBytes<T>(this T val) => val switch
        {
            byte[] b => b,
            byte b => new byte[] { b },
            int p => BitConverter.GetBytes(p),
            uint p => BitConverter.GetBytes(p),
            short p => BitConverter.GetBytes(p),
            ushort p => BitConverter.GetBytes(p),
            long p => BitConverter.GetBytes(p),
            ulong p => BitConverter.GetBytes(p),
            float p => BitConverter.GetBytes(p),
            bool p => BitConverter.GetBytes(p),
            double p => BitConverter.GetBytes(p),
            char p => BitConverter.GetBytes(p),
            string s => Encoding.UTF8.GetBytes(s),
            Enum e => SerializeEnum(e),
            IByteSerializable s => Serialize(s),
            IEnumerable enu => SerializeList(enu),
            _ => throw new NotSupportedException(),
        };

        public static T ToValue<T>(this byte[] value)
        {
            return typeof(T).Name switch
            {
                "Byte[]" => (T)(object)value,
                nameof(Byte) => (T)(object)value[0],
                nameof(Int32) => (T)(object)BitConverter.ToInt32(value),
                nameof(UInt32) => (T)(object)BitConverter.ToUInt32(value),
                nameof(Int16) => (T)(object)BitConverter.ToInt16(value),
                nameof(UInt16) => (T)(object)BitConverter.ToUInt16(value),
                nameof(Int64) => (T)(object)BitConverter.ToInt64(value),
                nameof(UInt64) => (T)(object)BitConverter.ToUInt64(value),
                nameof(Single) => (T)(object)BitConverter.ToSingle(value),
                nameof(Boolean) => (T)(object)BitConverter.ToBoolean(value),
                nameof(Double) => (T)(object)BitConverter.ToDouble(value),
                nameof(Char) => (T)(object)BitConverter.ToChar(value),
                nameof(String) => (T)(object)Encoding.UTF8.GetString(value),
                _ => (T)DefaulDeserialisation()
            };

            object DefaulDeserialisation()
            {
                var type = typeof(T);
                if (typeof(Enum).IsAssignableFrom(type))
                    return InvokeGenericMethod(nameof(DeserializeEnum), type);

                if (typeof(IByteSerializable).IsAssignableFrom(type))
                    return InvokeGenericMethod(nameof(Deserialize), type);

                if (typeof(IEnumerable).IsAssignableFrom(type))
                    return InvokeGenericMethod(nameof(DeserializeToList), type.GenericTypeArguments);

                throw new NotSupportedException();

                object InvokeGenericMethod(string methodName, params Type[] targetType)
                {
                    var method = typeof(SerializeHelper)
                        .GetMethod(methodName, BindingFlags.Static)
                        .MakeGenericMethod(targetType);

                    return method.Invoke(null, new object[] { value });
                }
            }
        }

    }
}
