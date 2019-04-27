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
                throw new Exception("Неверные аргументы");
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
                throw new Exception("Неверные аргументы");
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
        /// подвержденные ключи
        /// </summary>
        [XmlArray("ExtKeys"), XmlArrayItem(typeof(ExtKey), ElementName ="ExtKey")]
        private List<ExtKey> m_Exts;
        /// <summary>
        /// временные ключи , ожидающие подверждения
        /// </summary>
        [XmlArray("TempKeys"), XmlArrayItem(typeof(ExtKey), ElementName ="ExtKey")]
        private List<ExtKey> m_TmpExts;
        [NonSerialized()]
        RSACryptoServiceProvider m_rsa = null;
        SMSAsyncAlgo m_alg;
        [NonSerialized()]
        RNGCryptoServiceProvider m_rngCsp;
        [NonSerialized()]
        const string m_pref = "MRZR";

        public MORZEContact(string Name, string address)
        {
            
            const string err = "Неверный формат адреса";
            m_publickKey = null;
            m_address = null;
            m_accAddress = Encoding.ASCII.GetBytes(address);
            if (address.IndexOf(m_pref)==0)
            {
                m_alg = SMSAsyncAlgo.RSA;
                InitRSA();
                m_address = address;

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
            if (m_Exts!=null)
            {
                foreach (ExtKey i in m_Exts)
                {
                    if (i != null && i.Ext != null && i.Ext.Length == ext.Length)
                    {
                        bool bf = true;
                        for(int j=0;j<i.Ext.Length && bf==true;j++)
                        {
                            if (i.Ext[j] != ext[j])
                                bf = false;
                        }
                        if (bf==true)
                            bret = bf;
                    }
                }
            }
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

    }
}
