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
        List<SMSHash> m_hashid;
        List<byte[]> m_hash;
        ushort m_resendCounter;
        ushort m_numPos;
        public MORZEMessage(string msg)
        {
            m_Msg = msg;
            m_stat = MORZEMessageStatus.def;
            m_dateCreated = DateTime.Now;
            m_resendCounter = 1;
        }
        public void AddHashInfo(SMSHash hashid, byte[] hash)
        {
            
            if (m_hashid == null)
                m_hashid = new List<SMSHash>();
            if (m_hash == null)
                m_hash = new List<byte[]>();

            m_hashid.Add(hashid);
            m_hash.Add(hash);
        }
        public ushort GetOrderNumber
        {
            get
            {
                return m_numPos;
            }
        }
        public ushort GetResnedCount
        {
            get
            {
                return m_resendCounter;
            }
        }
        public MORZEMessage(string msg, SMSHash hashid, byte[]hash)
        {
            
            m_Msg = msg;
            m_stat = MORZEMessageStatus.sended;
            m_dateCreated = DateTime.Now;
            AddHashInfo(hashid, hash);
        }
        public MORZEMessage(string msg, SMSHash hashid, byte[] hash, ushort numpos)
        {

            m_Msg = msg;
            m_stat = MORZEMessageStatus.sended;
            m_dateCreated = DateTime.Now;

            AddHashInfo(hashid, hash);
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
        public bool isHash(SMSHash hashid, byte[] hash)
        {
            bool bres = false;
            if (m_hash!=null)
            {
                for(int i=0;i<m_hashid.Count && bres==false;i++)
                {
                    if (m_hashid[i]==hashid)
                    {
                        bres = true;
                        for(int j=0;j<m_hash[i].Length && bres==true;j++)
                        {
                            if (m_hash[i][j] != hash[j])
                                bres = false;
                        }
                    }
                }
            }
            return bres;
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
        public MORZEMessages(string contactAddress)
        {
            //m_account = account;
            //m_contact = contact;
            m_contactAddress = contactAddress;
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
                msghash = m_messages.Where(x => x.isHash(hashid, hash)==true).ToList();
                if (msghash!=null)
                {
                    foreach(MORZEMessage msg in msghash)
                        {
                            msg.Status = MORZEMessageStatus.sendedDelireved;
                            bres = true;
                        }
                    
                }
            }
            return bres;
        }
    }
}
