using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS
{
    public class SMSSendExt : SMSSendCommand
    {
        List<SMSHash> m_hsh;
        List<byte[]> m_exts;
        
        ISMSNet m_net;
        public SMSSendExt(ISMSNet net) :base (0x06)
        {
            m_hsh = new List<SMSHash>();
            m_exts = new List<byte[]>();
            
            m_net = net;
        }
        public void pushExt(SMSHash hash, byte [] extval)
        {
            m_hsh.Add(hash);
            m_exts.Add(extval);

        }
        public new void Send(Stream sm)
        {
            BufferBuilder bb;

            bb = new BufferBuilder();
            for (int i = 0; i < m_hsh.Count; i++)
            {
                bb.AddByte((byte)m_hsh[i]);
                bb.AddBytes(m_exts[i]);
            }
            byte[] buff;
            buff = bb.GetAllBytes();
            buff = m_net.Encrypt(buff);
            base.WriteBytes(buff);
            base.Send(sm);
        }
    }
}
