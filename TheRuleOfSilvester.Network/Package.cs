using System;
using System.Collections.Generic;
using System.Text;

namespace TheRuleOfSilvester.Network
{
    public class Package
    {
        public static int NextId => nextId++;
        private static int nextId;

        /// <summary>
        /// Headersize is Id + Command
        /// </summary>
        public const byte HEADER_SIZE = sizeof(int) + sizeof(int) + sizeof(short);

        public int Id { get; set; }

        public short Command { get; set; }
        public byte[] Data { get; set; }
        public CommandName CommandName => (CommandName)Command;

        public BaseClient Client { get; set; }


        public Package(int id)
        {
            if (id >= 0)
                Id = id;
            else
                Id = NextId;
        }
        public Package() : this(-1)
        {
        }
        public Package(short command, byte[] data, int id = -1) : this(id)
        {
            Command = command;
            Data = data;
        }
        public Package(CommandName commandName, byte[] data, int id = -1)
            : this((short)commandName, data, id) { }

        public override string ToString() => $"{CommandName} [{Data.Length}]";

        public byte[] ToByteArray()
        {
            //Write Header
            var i = 0;
            var rawByte = new byte[Data.Length + HEADER_SIZE];
            rawByte[i++] = (byte)(Id >> 24);
            rawByte[i++] = (byte)(Id >> 16);
            rawByte[i++] = (byte)(Id >> 8);
            rawByte[i++] = (byte)Id;
            rawByte[i++] = (byte)(Command >> 8);
            rawByte[i++] = (byte)Command;
            rawByte[i++] = (byte)(Data.Length >> 24);
            rawByte[i++] = (byte)(Data.Length >> 16);
            rawByte[i++] = (byte)(Data.Length >> 8);
            rawByte[i] = (byte)Data.Length;

            if (i > HEADER_SIZE)
                throw new IndexOutOfRangeException("That was to mutch for the header. Header could only be " + HEADER_SIZE);

            Buffer.BlockCopy(Data, 0, rawByte, HEADER_SIZE, Data.Length);
            return rawByte;
        }

        public static Package FromByteArray(byte[] data)
        {
            //Read Header
            int id = data[0] << 24 | data[1] << 16 | data[2] << 8 | data[3];
            short command = (short)(data[4] << 8 | data[5]);
            int length = data[6] << 24 | data[7] << 16 | data[8] << 8 | data[9];

            var rawData = new byte[length];
            Buffer.BlockCopy(data, HEADER_SIZE, rawData, 0, length);
            return new Package(command, rawData, id);
        }
    }
}
