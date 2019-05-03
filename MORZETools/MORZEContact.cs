using SMS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace SMS
{
    [Serializable]
    public class ExtKey
    {
        private byte[] m_pk =null;
        private byte[] m_pkIV = null;
        private DateTime ?m_date =null;
        private byte[] m_inExt=null;

        private SMSSyncAlgo m_alg= SMSSyncAlgo.None;
        private SMSHash m_halg= SMSHash.None;
        
        public ExtKey(RNGCryptoServiceProvider rng, SMSSyncAlgo alg, SMSHash hash)
        {
            m_date = DateTime.Now;
            
            if (alg== SMSSyncAlgo.DES && hash== SMSHash.MD5)
            {
                m_alg = alg;
                m_halg = hash;
                m_pk = new byte[8];
                rng.GetBytes(m_pk);
                m_pkIV = new byte[8];
                rng.GetBytes(m_pkIV);
                m_inExt = new byte[16];
                rng.GetBytes(m_inExt);
            }
            else
            {
                throw new Exception("Invalid arguments");
            }
        }
        public ExtKey(SMSHash hash, byte[] ext, SMSSyncAlgo sync, byte[] key, byte[] iv)
        {
            m_date = DateTime.Now;

            if (sync == SMSSyncAlgo.DES && hash == SMSHash.MD5)
            {
                m_alg = sync;
                m_halg = hash;
                m_pk = key;
                m_pkIV = iv;
                m_inExt = ext;
                
            }
            else
            {
                throw new Exception("Invalid arguments");
            }
        }
        public SMSHash HashID
        {
            get
            {
                return m_halg ;
            }
        }
        public byte[] Ext
        {
            get
            {
                return m_inExt;
            }
        }
        public SMSSyncAlgo SyncID
        {
            get
            {
                return m_alg;
            }
        }
        public byte[] SyncKey
        {
            get
            {
                return m_pk;
            }
        }
        public byte[] SyncIV
        {
            get
            {
                return m_pkIV;
            }

        }
        public byte[] SyncEncrypt(string input)
        {
            return SyncEncrypt(Encoding.UTF8.GetBytes(input));
        }
        public byte[] SyncEncrypt(byte[] input)
        {
            byte[] result = null;
            if (string.IsNullOrEmpty(SMSCrypt.SyncEncode(m_alg, input, SyncKey, SyncIV, out result)) == false)
                result = null;
            
            return result;
          }
    

    };

    [Serializable]
    public class MORZEContact : IDisposable ,
        IMORZEContact
    {
        private byte[] m_accAddress;
        private string m_address;
        private string m_DisplayName;
        private byte[] m_publickKey;
        /// <summary>
        /// confirmed keys
        /// </summary>
        private List<ExtKey> m_Exts;
        /// <summary>
        /// unconfirmed keys
        /// </summary>
        private List<ExtKey> m_TmpExts;
        [NonSerialized()]
        RSACryptoServiceProvider m_rsa = null;
        SMSAsyncAlgo m_alg;
        [NonSerialized()]
        RNGCryptoServiceProvider m_rngCsp;
        [NonSerialized()]
        const string m_pref = "MRZR";

        [field: NonSerialized]
        public event RecvNotifyAcceptecExtKey OnRecvNotifyAcceptecExtKey;
        [field: NonSerialized]
        public event RecvMessage OnRecvMessage;
        [field: NonSerialized]
        private List<byte[]> m_responseMsg;
        [field: NonSerialized]
        public event RecvDeliveredMsgNotify OnRecvDeliveredMsgNotify;
        public MORZEContact(string Name, string address)
        {
            
            const string err = "Неверный формат адреса";
            m_publickKey = null;
            m_address = null;
            m_accAddress = Encoding.ASCII.GetBytes(address);
            if (address.IndexOf(m_pref)==0)
            {
                m_alg = SMSAsyncAlgo.RSA;
                m_address = address;
                InitRSA();
                

                m_rngCsp = new RNGCryptoServiceProvider();

                


            }
            else
            {
                throw new Exception(err);
            }
            if (string.IsNullOrEmpty(Name) == true)
                m_DisplayName = "Unknown";
            else
                m_DisplayName = Name;
        }
        private void InitRSA()
        {

            byte[] pk;
            string base64 = m_address.Substring(m_pref.Length);
            pk = Convert.FromBase64String(base64);
            m_publickKey = pk;
            m_rsa = new RSACryptoServiceProvider();
            m_rsa.ImportCspBlob(pk);
        }
        public byte[] EncryptPK(byte[] input)
        {
            byte[] res = null;
            try
            {
                if (m_rsa == null && m_alg == SMSAsyncAlgo.RSA)
                    InitRSA();

                res = m_rsa.Encrypt(input, false);
            }
            catch(Exception exp)
            {
                res = null;
                throw exp;
            }
            return res;
        }
        public void Dispose()
        {
            if (m_rsa != null)
            {
                m_rsa.Dispose();
                m_rsa = null;
            }
            if (m_rngCsp != null)
            {
                m_rngCsp.Dispose();
                m_rngCsp = null;
            }

        }

        public bool isHasConfirmEtx()
        {
            bool bres = false;
            if (m_Exts != null)
                bres = m_Exts.Any();
            return bres;
        }
        public void clearAllExt()
        {
            if (m_Exts != null)
            {
                m_Exts.Clear();
                m_Exts = null;
            }
        }
        public ExtKey getInitalData()
        {
            
            if (m_rngCsp==null)
                m_rngCsp= new RNGCryptoServiceProvider();

            ExtKey exkey = new SMS.ExtKey(m_rngCsp, SMSSyncAlgo.DES, SMSHash.MD5);
            
            Monitor.Enter(this);
            if (m_TmpExts == null)
                m_TmpExts = new List<ExtKey>();
            m_TmpExts.Add(exkey);
            
            Monitor.Exit(this);
            return exkey;
        }

        public byte XOR(byte vl, int pos)
        {
            return (byte)((int)vl ^ (int)m_accAddress[pos]);
        }

        public string GetAddress()
        {
            return m_address;
        }

        public bool updateSynKey(SMSHash hash, byte[] ext, SMSSyncAlgo sync, byte[] key, byte[] iv)
        {
            bool bret = false;
            try
            {
                ExtKey extKey = new ExtKey(hash, ext, sync, key, iv);
                if (m_Exts == null)
                    m_Exts = new List<ExtKey>();
                m_Exts.Add(extKey);

                bret = true;
            }
            catch
            {
                bret = false;
            }
            return bret;
        }
        public bool isHasExt(byte[] ext)
        {
            bool bret = false;
            Monitor.Enter(this);
            if (getExt(ext) !=null)
                bret = true;
            if (bret==false)
            {
                if (m_TmpExts != null)
                {
                    for(int i=0;i< m_TmpExts.Count && bret==false;i++)
                    {
                        if (m_TmpExts[i] != null && m_TmpExts[i].Ext != null && m_TmpExts[i].Ext.Length == ext.Length)
                        {
                            bool bf = true;
                            for (int j = 0; j < m_TmpExts[i].Ext.Length && bf == true; j++)
                            {
                                if (m_TmpExts[i].Ext[j] != ext[j])
                                    bf = false;
                            }
                            if (bf == true)
                            {
                                bret = bf;
                                if (m_Exts==null)
                                {
                                    m_Exts = new List<ExtKey>();
                                    m_Exts.Add(m_TmpExts[i]);
                                    m_TmpExts.RemoveAt(i);
                                }
                            }
                        }
                    }
                }
            }
            Monitor.Exit(this);
            return bret;
        }
        public bool PutReciveMessage(byte[] msg, byte[] hash, SMSHash hashid, byte[] ext)
        {
            bool bRes = false;
            ExtKey key = getExt(ext);
            if (SMSCrypt.CheckHash(hashid,msg, hash)==true)
            {
                byte []res;
                string err;
                err = SMSCrypt.SyncDecode(key.SyncID, msg, key.SyncKey, key.SyncIV, out res);
                if (string.IsNullOrEmpty(err) == true)
                {
                    if (res.Length!=0)
                    {
                        switch (res[0])
                        {
                            case 2://Type 2 - уведомление о получении ключей
                                bRes=recvNotifyRecvExtKeys(key, res);
                                
                                break;
                            case 3://Type 3 - new messages
                                bRes=recvNewMessage(key, res, hashid, hash);
                                break;
                            case 4://Type 4 уведомление о принятых сообщениях .
                                bRes = recvDeliveryNotify(key, res);
                                break;
                        }
                    }
                }
            }
            return bRes;
        }
        public override string ToString()
        {
            return m_DisplayName;
        }
        public string DisplayName
        {
            set
            {
                m_DisplayName=value;
            }
        }


        private ExtKey getExt(byte[] ext)
        {
            ExtKey key = null;
            if (m_Exts != null)
            {
                foreach (ExtKey i in m_Exts)
                {
                    if (i != null && i.Ext != null && i.Ext.Length == ext.Length)
                    {
                        bool bf = true;
                        for (int j = 0; j < i.Ext.Length && bf == true; j++)
                        {
                            if (i.Ext[j] != ext[j])
                                bf = false;
                        }
                        if (bf == true)
                            key = i;
                    }
                }
            }
            return key;
        }
        private bool recvNotifyRecvExtKeys(ExtKey key, byte[] msg)
        {
            bool bres = false;
            try
            {
                byte[] rcvhash=null;
                SMSHash hashid = (SMSHash)msg[1];
                switch (hashid)
                {
                    case SMSHash.MD5:
                        rcvhash = new byte[0x10];
                        break;
                }
                if (rcvhash != null)
                {
                    Array.Copy(msg, 2, rcvhash, 0, rcvhash.Length);
                    bres = SMSCrypt.CheckHash(hashid, key.SyncKey, rcvhash);
                }
                if (bres == true && OnRecvNotifyAcceptecExtKey != null)
                    OnRecvNotifyAcceptecExtKey(this);
            }
            catch
            {
                bres = false;
            }

            return bres;
        }
        private bool recvNewMessage(ExtKey key,byte[] msg, SMSHash hashid, byte[]hash)
        {
            ushort nummsg = BitConverter.ToUInt16(msg, 1);

            ushort retry = BitConverter.ToUInt16(msg, 3);

            string text = Encoding.UTF8.GetString(msg, 5, msg.Length - 5);
            MORZEMessage mmsg = new MORZEMessage(text, hashid, hash, nummsg);
            if (OnRecvMessage != null)
                OnRecvMessage(this, mmsg);


            

            BufferBuilder bb = new BufferBuilder();
            bb.AddByte(4); // Type 4 - notify recived message
            bb.AddByte((byte)hashid);
            bb.AddBytes(hash);
            

            byte []netmsg;
            bool bres = false;
            string err = SMSCrypt.SyncEncode(key.SyncID, bb.GetAllBytes(), key.SyncKey, key.SyncIV, out netmsg);
            if (string.IsNullOrEmpty(err) == true)
            {
                if (m_responseMsg == null)
                    m_responseMsg = new List<byte[]>();
                m_responseMsg.Add(netmsg);
                bres = true;
            }
            
            return bres;


        }
        private bool recvDeliveryNotify(ExtKey key, byte[] msg)
        {
            bool bres = false;
            byte[] hash;
            uint off = 2;
            SMSHash hashid = (SMSHash)msg[off-1];
            hash = new byte[msg.Length - off];

            Array.Copy(msg, off, hash, 0, hash.Length);

            if (OnRecvDeliveredMsgNotify != null)
                OnRecvDeliveredMsgNotify(this, hashid, hash);
            bres = true;
            return bres;
        }
        public List<byte[]> Responses
        {
            get
            {
                //List<byte[]> rsp;
                //rsp = m_responseMsg;
                //m_responseMsg.Clear();
                return m_responseMsg;
            }
        }
        public byte[] getMORZENetMessage(MORZEMessage msg, out ExtKey ext)
        {
            byte[] enetmsg = null;
            ext = null;
            if (m_Exts!=null&& m_Exts.Any()==true)
            {
                byte[] bmsg=null;
                byte[] netmsg = null;
                string err;
                ExtKey key = m_Exts[m_Exts.Count - 1];
                bmsg=Encoding.UTF8.GetBytes(msg.ToString());
                

                netmsg = new byte[bmsg.Length + 5];
                netmsg[0] = 3; //type of message  - text message

                byte[] num;

                num=BitConverter.GetBytes(msg.GetOrderNumber); //must 2butes
                Array.Copy(num, 0, netmsg, 1, num.Length);

                num = BitConverter.GetBytes(msg.GetResnedCount); //must 2butes
                Array.Copy(num, 0, netmsg, 3, num.Length);

                Array.Copy(bmsg, 0, netmsg, 5, bmsg.Length);


                err = SMSCrypt.SyncEncode(key.SyncID, netmsg, key.SyncKey, key.SyncIV, out enetmsg);
                if (string.IsNullOrEmpty(err) == false)
                    enetmsg = null;
                else
                    ext = key;
            }

            return enetmsg;
        }
        public List<ExtKey> ExtKeys
        {
            get
            {
                return m_Exts;
            }
        }
        public int UnconfirmedKeysCount
        {
            get
            {
                int ret = 0;
                if (m_TmpExts != null)
                    ret = m_TmpExts.Count;
                return ret;
            }
        }
        public int ConfirmedKeysCount
        {
            get
            {
                int ret = 0;
                if (m_Exts != null)
                    ret = m_Exts.Count;
                return ret;
            }
        }
    }
}
