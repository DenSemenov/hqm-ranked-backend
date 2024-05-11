using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplayHandler.Classes
{
    public struct HQMMessageReader
    {
        public byte[] buf;
        public int pos;
        public byte bit_pos;

        public HQMMessageReader(byte[] buf)
        {
            this.buf = buf;
            pos = 0;
            bit_pos = 0;
        }

        private byte SafeGetByte(int pos)
        {
            if (pos < buf.Length)
                return buf[pos];
            else
                return 0;
        }

        public byte ReadByteAligned()
        {
            Align();
            byte res = SafeGetByte(pos);
            pos++;
            return res;
        }

        public byte[] ReadBytesAligned(uint n)
        {
            Align();
            var res = new byte[n];

            for (int i = 0; i < n; i++)
            {
                res[i] = SafeGetByte(pos);
                pos += 1;
            }

            return res;
        }

        public uint ReadU32Aligned()
        {
            Align();
            uint b1 = SafeGetByte(pos);
            uint b2 = SafeGetByte(pos + 1);
            uint b3 = SafeGetByte(pos + 2);
            uint b4 = SafeGetByte(pos + 3);
            pos += 4;
            return b1 | b2 << 8 | b3 << 16 | b4 << 24;
        }
        public float ReadF32Aligned()
        {
            var i = ReadU32Aligned();
            return Convert.ToSingle(i);
        }

        public uint ReadU16Aligned()
        {
            Align();
            var b1 = SafeGetByte(pos);
            var b2 = SafeGetByte(pos + 1);
            pos += 2;
            return (uint)(b1 | b2 << 8);
        }

        public uint ReadPos(byte b, uint? old_value)
        {
            byte pos_type = (byte)ReadBits(2);
            switch (pos_type)
            {
                case 0:
                    int diff = ReadBitsSigned(3);
                    int oldValue = (int)old_value;
                    return (uint)Math.Max((oldValue + diff), 0);
                case 1:
                    diff = ReadBitsSigned(6);
                    oldValue = (int)old_value;
                    return (uint)Math.Max((oldValue + diff), 0);
                case 2:
                    diff = ReadBitsSigned(12);
                    oldValue = (int)old_value;
                    return (uint)Math.Max((oldValue + diff), 0);
                case 3:
                    return ReadBits(b);
                default:
                    throw new Exception();
            }
        }

        public int ReadBitsSigned(byte b)
        {
            int a = (int)ReadBits(b);

            if (a >= 1 << (b - 1))
                return (-1 << b) | a;
            else
                return a;
        }

        public uint ReadBits(byte b)
        {
            byte bitsRemaining = b;
            uint res = 0;
            int p = 0;
            while (bitsRemaining > 0)
            {
                byte bitsPossibleToWrite = (byte)(8 - bit_pos);
                byte bits = Math.Min(bitsRemaining, bitsPossibleToWrite);

                byte mask;
                if (bits == 8)
                    mask = byte.MaxValue;
                else
                    mask = (byte)~(byte.MaxValue << bits);

                uint a = (uint)((SafeGetByte(pos) >> bit_pos) & mask);
                res |= (a << p);

                if (bitsRemaining >= bitsPossibleToWrite)
                {
                    bitsRemaining -= bitsPossibleToWrite;
                    bit_pos = 0;
                    pos++;
                    p += bits;
                }
                else
                {
                    bit_pos += bitsRemaining;
                    bitsRemaining = 0;
                }
            }
            return res;
        }

        public void Align()
        {
            if (bit_pos > 0)
            {
                bit_pos = 0;
                pos++;
            }
        }

        public void Next()
        {
            pos++;
            bit_pos = 0;
        }
    }
}
