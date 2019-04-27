using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SMS
{
    public class SMSInitialMessage : MORZESendMessage
    {
        IMORZEAccount m_account;

        ExtKey m_newExt;
        public SMSInitialMessage(IMORZEAccount account, IMORZEContact contact)
        {
            BufferBuilder bb =new BufferBuilder();
            
            m_account = account;
            bb.AddByte(1);//- инициирующее сообщение
            m_newExt = contact.getInitalData();
            bb.AddByte((byte)m_newExt.HashID);
            bb.AddBytes(m_newExt.Ext);
     
            bb.AddByte((byte)m_newExt.SyncID);
            bb.AddBytes(m_newExt.SyncKey);
            bb.AddBytes(m_newExt.SyncIV);
            bb.AddUshort(0);//или CRC16
            byte [] data = bb.GetAllBytes();
            data = contact.EncryptPK(data);
            byte[] addressdata = m_newExt.SyncEncrypt(account.GetMyAccount());
            bb = new BufferBuilder();

            bb.AddByte(contact.XOR((byte)data.Length ,0x10));
            bb.AddBytes(data);

            bb.AddBytes(addressdata);
            AddMessageBody(bb.GetAllBytes(), m_newExt.HashID, null);

        }
        public void InitExtParam(SMSSendExt ext)
        {
            ext.pushExt(m_newExt.HashID, m_newExt.Ext);
        }
    }
}
