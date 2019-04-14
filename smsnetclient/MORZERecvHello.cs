using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS
{
    
    class SMSRecvHello  : MORZECommand
    {
        private byte [] m_IV;
        private byte[] m_SyncKey;
        public SMSRecvHello(byte[] data) : base(data)
        {

        }

        public bool isValid (byte [] helloSign)
        {
            bool bret = false;
            byte[] recvHello;
            if (m_Data.Length >= helloSign.Length +
                    1 +// Success field
                    1 +// VersSupportLen field
                    2 // one VerSupport
                )
            {
                recvHello = new byte[helloSign.Length];
                Array.Copy(m_Data, recvHello, recvHello.Length);
                if (Enumerable.SequenceEqual(recvHello, helloSign)==true)
                {
                    if (m_Data[recvHello.Length]==0) //check Success field
                    {
                        bret = true;
                    }
                }
            }
            
            return bret;
        }
    }
   
}
