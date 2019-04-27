using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMS
{

    public enum MORZEMessageStatus
    {
        def = 0x00,
        unsendedNew = 0x01,
        unsended = 0x02,
        sended =0x03,
        sendedDelireved = 0x04,
        senedReaded =0x05
    };
    public class MORZEMessage
    {
        MORZEMessageStatus m_stat;
        DateTime m_created;
        string m_Msg;
        public MORZEMessage(string msg)
        {
            m_Msg = msg;
            m_stat = MORZEMessageStatus.def;
            m_created = DateTime.Now;
        }
        public MORZEMessageStatus Status
        {
            get
            {
                return m_stat;
            }
            set
            {
                m_stat = value;
            }
        }
    }
    public class MORZEMessages
    {
        IMORZEAccount m_account;
        IMORZEContact m_contact;
        List<MORZEMessage> m_messages;
        public MORZEMessages(IMORZEAccount account, IMORZEContact contact)
        {
            m_account = account;
            m_contact = contact;
        }

        public IMORZEContact Contact
        {
            get
            {
                return m_contact;
            }
        }


        public void AddUnsendedNewMessages(string text)
        {
            MORZEMessage msg;
            msg = new MORZEMessage(text);
            msg.Status = MORZEMessageStatus.unsendedNew;

            Monitor.Enter(this);
            if (m_messages == null)
                m_messages = new List<MORZEMessage>();
            m_messages.Add(msg);
            Monitor.Exit(this);
        }
    }
}
