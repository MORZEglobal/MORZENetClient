using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS
{
    public class BufferBuilder
    {
        List<byte[]> m_arrays;
        List<int> m_sizes;
        int m_initSize;
        public BufferBuilder()
        {
            m_arrays = new List<byte[]>();
            m_sizes = new List<int>();
            m_initSize = 1024;
            
        }
        public void AddByte(byte val)
        {
            Write(new byte[]{val});
        }
        public void AddBytes(byte []val)
        {
            Write(val);
        }
        public void AddBytes(byte[] val, int len)
        {
            byte[] bt = new byte[len];
            Array.Copy(val, bt, len);
            Write(bt);
        }

        public void AddUshort(ushort val)
        {
            byte[] bt = BitConverter.GetBytes(val);
            Write(bt);
        }
        public void AddASCII(string val)
        {
            byte[] ascii = Encoding.ASCII.GetBytes(val);
            Write(ascii);
        }
        private void Write (byte [] data)
        {
            int pos;
            if (m_arrays.Count==0)
            {
                if (m_initSize < data.Length)
                    m_initSize = data.Length * 2;
                m_arrays.Add(new byte[m_initSize]);
                m_sizes.Add(0);
            }
            pos = m_arrays.Count - 1;
            if (m_arrays[pos].Length<data.Length+m_sizes[pos])
            {
                if (m_sizes[pos] != 0)
                {
                    m_arrays.Add(new byte[data.Length]);
                    m_sizes.Add(0);
                    pos++;
                }
                else
                    m_arrays[pos] = new byte[data.Length];
            }
            Array.Copy(data, 0, m_arrays[pos], m_sizes[pos], data.Length);
            m_sizes[pos] += data.Length;
        }
        public byte[] GetAllBytes()
        {
            byte[] bret = null;
            int size = 0;
            for (int i = 0; i < m_arrays.Count; i++)
                size += m_sizes[i];
            if(size!=0)
                bret = new byte[size];
            if (bret != null)
            {
                int di = 0;
                for (int i=0;i<m_arrays.Count;i++)
                {
                    Array.Copy(m_arrays[i], 0, bret, di, m_sizes[i]);
                    di += m_sizes[i];
                }
            }
            return bret;
        }

    }
}
