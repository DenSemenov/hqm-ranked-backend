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

        private byte SafeGetByte(int pos) => pos < buf.Length ? buf[pos] : (byte)0;

        public byte ReadByteAligned()
        {
            Align();
            return SafeGetByte(pos++);
        }

        public byte[] ReadBytesAligned(uint n)
        {
            Align();
            var res = new byte[n];
            for (int i = 0; i < n; i++)
            {
                res[i] = SafeGetByte(pos++);
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
            return b1 | (b2 << 8) | (b3 << 16) | (b4 << 24);
        }

        public float ReadF32Aligned()
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(ReadU32Aligned()), 0);
        }

        public uint ReadU16Aligned()
        {
            Align();
            uint b1 = SafeGetByte(pos);
            uint b2 = SafeGetByte(pos + 1);
            pos += 2;
            return b1 | (b2 << 8);
        }

        public uint ReadPos(byte b, uint? old_value)
        {
            int diff;
            int oldValue = (int)old_value.GetValueOrDefault();
            switch ((byte)ReadBits(2))
            {
                case 0:
                    diff = ReadBitsSigned(3);
                    break;
                case 1:
                    diff = ReadBitsSigned(6);
                    break;
                case 2:
                    diff = ReadBitsSigned(12);
                    break;
                case 3:
                    return ReadBits(b);
                default:
                    throw new InvalidOperationException();
            }
            return (uint)Math.Max(oldValue + diff, 0);
        }

        public int ReadBitsSigned(byte b)
        {
            int a = (int)ReadBits(b);
            return a >= 1 << (b - 1) ? (-1 << b) | a : a;
        }

        public uint ReadBits(byte b)
        {
            uint res = 0;
            int p = 0;
            while (b > 0)
            {
                byte bitsPossibleToWrite = (byte)(8 - bit_pos);
                byte bits = Math.Min(b, bitsPossibleToWrite);

                byte mask = bits == 8 ? byte.MaxValue : (byte)~(byte.MaxValue << bits);
                uint a = (uint)((SafeGetByte(pos) >> bit_pos) & mask);
                res |= (a << p);

                if (b >= bitsPossibleToWrite)
                {
                    b -= bitsPossibleToWrite;
                    bit_pos = 0;
                    pos++;
                }
                else
                {
                    bit_pos += b;
                    b = 0;
                }
                p += bits;
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