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
        senedReaded =0x05,
        recived = 0x06
    };
    [Serializable]
    public class MORZEMessage
    {
        MORZEMessageStatus m_stat;
        DateTime m_dateCreated;
        string m_Msg;
        SMSHash m_hashid;
        byte[] m_hash;
        int m_resendCounter;
        uint m_numPos;
        public MORZEMessage(string msg)
        {
            m_Msg = msg;
            m_stat = MORZEMessageStatus.def;
            m_dateCreated = DateTime.Now;
        }
        public MORZEMessage(string msg, SMSHash hashid, byte[]hash)
        {
            
            m_Msg = msg;
            m_stat = MORZEMessageStatus.sended;
            m_dateCreated = DateTime.Now;
            m_hashid = hashid;
            m_hash = hash;
        }
        public MORZEMessage(string msg, SMSHash hashid, byte[] hash, uint numpos)
        {

            m_Msg = msg;
            m_stat = MORZEMessageStatus.sended;
            m_dateCreated = DateTime.Now;
            m_hashid = hashid;
            m_hash = hash;
            m_numPos = numpos;
        }
        public DateTime Date
        {
            get
            {
                return m_dateCreated;
            }
        }
        public MORZEMessageStatus Status
        {
            get
            {
                return m_stat;
            }
            set
            {
                if (value == MORZEMessageStatus.sended)
                    m_resendCounter++;
                m_stat = value;
            }
        }
        public override string ToString()
        {
            return m_Msg;
        }
        public SMSHash   HashID
        {
            get
            {
                return m_hashid;
            }
            set
            {
                m_hashid = value;
            }
        }
        public byte []Hash
        {
            get
            {
                return m_hash;
            }
            set
            {
                m_hash = value;
            }
        }
        
    }
    [Serializable]
    public class MORZEMessages
    {
        //[field: NonSerialized]
        //IMORZEAccount m_account;
        //[field: NonSerialized]
        //IMORZEContact m_contact;
        string m_contactAddress;
        List<MORZEMessage> m_messages;
        public MORZEMessages( IMORZEContact contact)
        {
            //m_account = account;
            //m_contact = contact;
            m_contactAddress = contact.GetAddress();
        }

        public string ContactAddress
        {
            get
            {
                return m_contactAddress;
            }
        }
        public List<MORZEMessage> Messages
        {
            get
            {
                return m_messages;
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
        public void AddSendedNewMessages(MORZEMessage msg)
        {

            msg.Status = MORZEMessageStatus.sended;

            Monitor.Enter(this);
            if (m_messages == null)
                m_messages = new List<MORZEMessage>();
            m_messages.Add(msg);
            Monitor.Exit(this);
        }
        public void AddRecivedMessages(MORZEMessage msg)
        {

            msg.Status = MORZEMessageStatus.sended;

            Monitor.Enter(this);
            if (m_messages == null)
                m_messages = new List<MORZEMessage>();
            msg.Status = MORZEMessageStatus.recived;
            m_messages.Add(msg);
            Monitor.Exit(this);
        }
        public List<MORZEMessage> UnsendedNewMessages
        {
            get
            {
                List<MORZEMessage> ret=null;
                if (m_messages != null)
                {
                    ret = m_messages.Where(x => x.Status == MORZEMessageStatus.unsendedNew).ToList();
                }
                return ret;
            }
        }
        public bool SetDeliveredMessage(SMSHash hashid, byte[] hash)
        {
            bool bres=false;
            List<MORZEMessage> msghash;
            if (m_messages!=null)
            {
                msghash = m_messages.Where(x => x.HashID == hashid && x.Status== MORZEMessageStatus.sended).ToList();
                if (msghash!=null)
                {
                    for(int i=0;i<msghash.Count;i++)
                    {
                        bool bfind = true;
                        byte[] ihash = msghash[i].Hash;
                        for(int j=0;j<hash.Length&&bfind==true;j++)
                        {
                            if (ihash[j] != hash[j])
                                bfind = false;
                        }
                        if (bfind==true)
                        {
                            msghash[i].Status = MORZEMessageStatus.sendedDelireved;
                            bres = true;
                        }
                    }
                }
            }
            return bres;
        }
    }
}
