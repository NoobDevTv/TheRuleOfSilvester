using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using TheRuleOfSilvester.Core.Cells;
using TheRuleOfSilvester.Core.Interfaces;
using TheRuleOfSilvester.Core.Items;

namespace TheRuleOfSilvester.Core
{
    public static class SerializeHelper
    {
        public static Map Map { get; set; }

        private static Dictionary<string, Type> mapCells;
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

        public static ICollection<T> DeserializeToList<T>(byte[] value) where T : IByteSerializable, new()
        {
            var tmpList = Activator.CreateInstance<List<T>>();

            if (value.Length > 0)
            {
                using (var memoryStream = new MemoryStream(value))
                {
                    using (var binaryReader = new BinaryReader(memoryStream))
                    {
                        var count = binaryReader.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            var tmpObj = Activator.CreateInstance<T>();
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
            cell.Position = new Point(binaryReader.ReadInt32(), binaryReader.ReadInt32());
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

        public static byte[] SerializeList<T>(ICollection<T> list) where T : IByteSerializable
        {
            if (list == null)
                return new byte[0];

            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(memoryStream))
                {
                    binaryWriter.Write(list.Count);

                    foreach (var obj in list.ToList())
                        obj.Serialize(binaryWriter);
                }

                return memoryStream.ToArray();
            }
        }
    }
}
