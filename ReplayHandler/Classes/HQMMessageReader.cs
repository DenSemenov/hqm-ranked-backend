using System.Collections;

namespace ReplayHandler.Classes
{
    public class HQMMessageReader
    {
        public HQMMessageReader(byte[] input_data)
        {
            buf = new BitArray(input_data);
            buf_length = buf.Length;
        }

        public HQMMessageReader()
        {
        }

        public int GetPos()
        {
            return pos;
        }

        public byte SafeGetByte(int in_pos)
        {
            if (in_pos < buf_length)
            {
                for (int i = 0; i < 8; i++)
                {
                    SafeGetByte_chunk[i] = buf[in_pos + i];
                }
                SafeGetByte_chunk.CopyTo(SafeGetByte_resultarray, 0);
                return SafeGetByte_resultarray[0];
            }
            return 0;
        }

        public byte ReadByteAligned()
        {
            Align();
            read_byte_aligned_res = SafeGetByte(pos);
            pos += 8;
            return read_byte_aligned_res;
        }

        public List<byte> ReadBytesAligned(int n)
        {
            Align();
            read_bytes_aligned_res.Clear();
            for (int i = pos; i < pos + n * 8; i += 8)
            {
                read_bytes_aligned_res.Add(SafeGetByte(i));
            }
            pos += n * 8;
            return read_bytes_aligned_res;
        }

        public ushort ReadU16Aligned()
        {
            Align();
            read_u16_aligned_b1 = (ushort)SafeGetByte(pos);
            read_u16_aligned_b2 = (ushort)SafeGetByte(pos + 8);
            pos += 16;
            return (ushort)((int)read_u16_aligned_b1 | (int)read_u16_aligned_b2 << 8);
        }

        public uint ReadU32Aligned()
        {
            Align();
            read_u32_aligned_b1 = SafeGetByte(pos);
            read_u32_aligned_b2 = SafeGetByte(pos + 8);
            read_u32_aligned_b3 = SafeGetByte(pos + 16);
            read_u32_aligned_b4 = SafeGetByte(pos + 24);
            pos += 32;
            read_u32_aligned_bytes[0] = read_u32_aligned_b1;
            read_u32_aligned_bytes[1] = read_u32_aligned_b2;
            read_u32_aligned_bytes[2] = read_u32_aligned_b3;
            read_u32_aligned_bytes[3] = read_u32_aligned_b4;
            return BitConverter.ToUInt32(read_u32_aligned_bytes, 0);
        }

        public float ReadF32Aligned()
        {
            read_f32_aligned_i = ReadU32Aligned();
            return BitConverter.ToSingle(BitConverter.GetBytes(read_f32_aligned_i), 0);
        }

        public uint ReadPos(byte b, uint? old_value)
        {
            read_pos_type = ReadBits(2);
            read_pos_signed_old_value = (int)old_value;
            read_pos_diff = 0;
            switch (read_pos_type)
            {
                case 0U:
                    read_pos_diff = ReadBitsSigned(3);
                    return (uint)Math.Max(0, read_pos_signed_old_value + read_pos_diff);
                case 1U:
                    read_pos_diff = ReadBitsSigned(6);
                    return (uint)Math.Max(0, read_pos_signed_old_value + read_pos_diff);
                case 2U:
                    read_pos_diff = ReadBitsSigned(12);
                    return (uint)Math.Max(0, read_pos_signed_old_value + read_pos_diff);
                case 3U:
                    return ReadBits(b);
                default:
                    return 0U;
            }
        }

        public int ReadBitsSigned(byte b)
        {
            read_bits_signed_a = (int)ReadBits(b);
            if (read_bits_signed_a >= 1 << (int)(b - 1))
            {
                return -1 << (int)b | read_bits_signed_a;
            }
            return read_bits_signed_a;
        }

        public uint ReadBits(byte b)
        {
            read_bits_bits_remaining = b;
            read_bits_res = 0U;
            read_bits_p = 0;
            while (read_bits_bits_remaining > 0)
            {
                read_bits_pos_w_bits = (byte)(8 - bit_pos);
                read_bits_bits = Math.Min(read_bits_bits_remaining, read_bits_pos_w_bits);
                read_bits_mask = ~(uint.MaxValue << (int)read_bits_bits);
                read_bits_a = ((uint)SafeGetByte(pos) >> (int)bit_pos & read_bits_mask);
                read_bits_res |= read_bits_a << (int)read_bits_p;
                if (read_bits_bits_remaining >= read_bits_pos_w_bits)
                {
                    read_bits_bits_remaining -= read_bits_pos_w_bits;
                    bit_pos = 0;
                    pos += 8;
                    read_bits_p += read_bits_bits;
                }
                else
                {
                    bit_pos += read_bits_bits_remaining;
                    read_bits_bits_remaining = 0;
                }
            }
            return read_bits_res;
        }

        public void Align()
        {
            if (bit_pos > 0)
            {
                bit_pos = 0;
                pos += 8;
            }
        }

        public void Next()
        {
            bit_pos = 0;
            pos += 8;
        }

        private int buf_length;

        private BitArray buf;

        public int pos;

        public byte bit_pos;

        public BitArray SafeGetByte_chunk = new BitArray(8);

        public byte[] SafeGetByte_resultarray = new byte[1];

        private byte read_byte_aligned_res;

        public List<byte> read_bytes_aligned_res = new List<byte>();

        public ushort read_u16_aligned_b1;

        public ushort read_u16_aligned_b2;

        public byte read_u32_aligned_b1;

        public byte read_u32_aligned_b2;

        public byte read_u32_aligned_b3;

        public byte read_u32_aligned_b4;

        public byte[] read_u32_aligned_bytes = new byte[4];

        public uint read_f32_aligned_i;

        public uint read_pos_type;

        public int read_pos_signed_old_value;

        public int read_pos_diff;

        public int read_bits_signed_a;

        public byte read_bits_bits_remaining;

        public uint read_bits_res;

        public byte read_bits_p;

        public byte read_bits_pos_w_bits;

        public byte read_bits_bits;

        public uint read_bits_mask;

        public uint read_bits_a;
    }
}
