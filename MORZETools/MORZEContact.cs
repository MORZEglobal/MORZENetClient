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
        private List<ExtKey> m_Exts;
        /// <summary>
        /// временные ключи , ожидающие подверждения
        /// </summary>
        private List<ExtKey> m_TmpExts; 
        RSACryptoServiceProvider m_rsa = null;
        SMSAsyncAlgo m_alg;
        RNGCryptoServiceProvider m_rngCsp;
        public MORZEContact()
        {
        }
        public MORZEContact(string Name, string address)
        {
            const string pref="MRZR";
            const string err = "Неверный формат адреса";
            m_publickKey = null;
            m_address = null;
            m_accAddress = Encoding.ASCII.GetBytes(address);
            if (address.IndexOf(pref)==0)
            {
                m_alg = SMSAsyncAlgo.RSA;
                RSACryptoServiceProvider rsa = null;
                try
                {
                    byte[] pk;
                    string base64 = address.Substring(pref.Length);
                    pk = Convert.FromBase64String(base64);
                    rsa = new RSACryptoServiceProvider();
                    rsa.ImportCspBlob(pk);
                    m_rsa = new RSACryptoServiceProvider();
                    m_rsa.ImportCspBlob(pk);
                    m_address = address;
                    m_publickKey = pk;
                    m_rngCsp = new RNGCryptoServiceProvider();
                }
                finally
                {
                    if (rsa != null)
                        rsa.Dispose();
                }
                
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
       
        public byte[] EncryptPK(byte[] input)
        {
            byte[] res = null;
            try
            {
  
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

            return bret;
        }
        public bool PutReciveMessage(byte[] msg, byte[] hash, SMSHash hashid, byte[] ext)
        {
            bool bRes = false;

            return bRes;
        }
    }
}
