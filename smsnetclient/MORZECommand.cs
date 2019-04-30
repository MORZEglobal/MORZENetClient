using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMS
{
    public class SMSSendCommand : IDisposable
    {
        MemoryStream m_mem;
        byte m_cmd;
        const int m_packLenSize= 3;
        protected List<byte[]> m_bodies;
        public SMSSendCommand(byte cmd)
        {
            m_bodies = null;
            m_cmd = cmd;
            m_mem = new MemoryStream();
            m_mem.WriteByte(cmd);
        }
       
        public void Dispose()
        {
            if (m_mem != null)
                m_mem.Dispose();
        }
        public void WriteBytes(byte []data)
        {
            m_mem.Write(data, 0, data.Length);

        }
        public void WriteByte(byte data)
        {
            m_mem.WriteByte(data);

        }
        public void Write2Bytes(ushort data)
        {
            byte[] bt = BitConverter.GetBytes(data);
            if (bt.Length != 2)
                throw new Exception("error of converting two bytes to an array");
            else
                m_mem.Write(bt, 0, bt.Length);
        }
        public virtual void Send(Stream sm)
        {
            if (m_bodies == null)
            {
                if (checkData() == true)
                {
                    Monitor.Enter(sm);
                    //---------------------------------
                    long size = m_mem.Length;
                    byte[] buff = new byte[size];
                    m_mem.Seek(0, SeekOrigin.Begin);
                    m_mem.Read(buff, 0, 1);
                    sm.Write(buff, 0, 1);
                    byte[] packsize = BitConverter.GetBytes(size + m_packLenSize);
                    m_mem.Read(buff, 0, buff.Length - 1);
                    sm.Write(packsize, 0, 2);
                    sm.Write(packsize, 2, 1);
                    sm.Write(buff, 0, buff.Length - 1);
                    //------------------------------
                    Monitor.Exit(sm);
                }
            }
            else
            {
                BufferBuilder bb = new BufferBuilder();
                int bodiessize = 0;
                for (int i = 0; i < m_bodies.Count; i++)
                    bodiessize += m_bodies[i].Length;
                bodiessize += 4;
                if (bodiessize < 0xFFFFFF)
                {
                    byte[] sz = BitConverter.GetBytes(bodiessize);
                    bb.AddByte(m_cmd);
                    bb.AddBytes(sz, 3);
                    for (int i = 0; i < m_bodies.Count; i++)
                    {
                        bb.AddBytes(m_bodies[i]);
                    }
                    byte[] bts = bb.GetAllBytes();
                    Monitor.Enter(sm);
                    //
                    sm.Write(bts, 0, bts.Length);
                    Monitor.Exit(sm);
                }
                else
                {
                    throw new Exception("data length is invalid");
                }
            }
            
        }
        
        private bool checkData()
        {
            bool bret = false;
            long size = m_mem.Length;
            if (m_cmd==0x01)
            {
                bret = size== 10- m_packLenSize ? true:false;
            }
            if (m_cmd==0x02)
            {
                bret = true;
            }
            if (m_cmd == 0x05)
            {
                bret = true;
            }
            if (m_cmd == 0x06)
            {
                bret = true;
            }
            return bret;
        }
    }
}
