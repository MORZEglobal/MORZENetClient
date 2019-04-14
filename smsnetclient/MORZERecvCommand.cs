using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SMS
{
    public class MORZECommand
    {
        protected byte[] m_Data;
        
        public MORZECommand (byte [] data)
        {
            m_Data = data;
        }
    }
    public class MORZERecvCommand
    {
        Stream m_netStream;
        byte []m_header;
        ISMSAccount m_account;
        public MORZERecvCommand (ISMSAccount account, NetworkStream netstream)
        {
            m_account = account;
            m_header = new byte[4];
            m_netStream = netstream;
        }
        public MORZECommand recvCmd()
        {
            MORZECommand cmd = null;
            byte[] data;
            if (recv(m_header) == true)
            {
                data = recvdata();
                if (data != null)
                {
                    switch (m_header[0])
                    {
                        case 0x81:
                            cmd = new SMSRecvHello(data);
                            break;
                        case 0x82://приято сообщение
                            cmd = new MORZERecvMessages(m_account , data);
                            break;
                        case 0x85:
                            cmd = new MORZERecvSynKey(data);
                            break;

                    }
                }
            }
            return cmd;
        }
        private bool recv(byte[] buff)
        {
            bool isSuccess =false;
            int rv = 0;
            int rvi = 0;
            do
            {
                rvi = m_netStream.Read(buff, rv, buff.Length - rv);
                rv += rvi;
            } while (rv < buff.Length && rvi!=0);
            if (rvi != 0 && rv == buff.Length)
                isSuccess = true;
            return isSuccess;
        }
        private byte[] recvdata()
        {
            byte[] data = null;
            byte[] btlen = new byte[4];
            Array.Copy(m_header, 1, btlen, 0, 3);
            UInt32 len = BitConverter.ToUInt32(btlen, 0);
            if (len > m_header.Length)
            {
                int hlen = m_header.Length;
                for (int i = 0; i < hlen; i++)
                    len--;
                
                data = new byte[len];
                if (recv(data) == false)
                    data = null;
            }
            return data;
        }
    }
}
