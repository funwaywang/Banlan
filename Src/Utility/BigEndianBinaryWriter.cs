using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Banlan
{
    public class BigEndianBinaryWriter : BinaryWriter
    {
        public BigEndianBinaryWriter(Stream stream)
            : base(stream)
        {
        }

        public override void Write(short value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(ushort value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(int value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(uint value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(long value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(ulong value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(float value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            base.Write(data);
        }

        public override void Write(double value)
        {
            var data = BitConverter.GetBytes(value);
            Array.Reverse(data);
            base.Write(data);
        }
    }
}
