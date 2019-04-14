using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SMS
{
    // cmd 0x5
    
    public class MORZESendAsync :  SMSSendCommand 
    {
        SMSAsyncAlgo m_asynAlg;
        RSACryptoServiceProvider m_rsa;
        byte[] m_publicKey;
        public MORZESendAsync(byte []signhello) : base(0x05)
        {
            m_asynAlg = SMSAsyncAlgo.RSA;
        }
        private void GenKey()
        {
            switch (m_asynAlg)
            {
                case SMSAsyncAlgo.RSA:
                    //  var csp = new CspParameters(1, "Microsoft Strong Cryptographic Provider");
                    m_rsa = new RSACryptoServiceProvider();// 2048, csp);
                    m_publicKey=m_rsa.ExportCspBlob(false);

                    break;
            }
        }
        public new void Send(Stream sm)
        {
            byte alg = (byte)m_asynAlg;
            GenKey();
            WriteByte(alg);
            WriteBytes(m_publicKey);
            base.Send(sm);
        }
        public byte[] Decrypt(byte[] input)
        {
            byte[] res = null;
            res=m_rsa.Decrypt(input, false);
            return res;
        }
    }
    class MORZERecvSynKey : MORZECommand
    {
        SMSSync m_Sync;
        public MORZERecvSynKey(byte[] data) : base(data)
        {

        }
        public SMSSync SetAsync (MORZESendAsync async, byte[]hellosign)
        {
            byte [] syncKey=async.Decrypt(m_Data);
            byte[] t;
            byte algo = syncKey[0];
            SMSSync sync = null;
            
            switch (algo)
            {
                case 1://DES
                    t = new byte[syncKey.Length-1];
                    Array.Copy(syncKey, 1, t, 0, t.Length);
                    sync = new SMSDES(t, hellosign);
                    m_Sync = sync;
                    break;
                default:
                    throw new Exception(string.Format("Неподдреживаемый алгоритм симметричного шифрования - {0}", algo.ToString()));
                    
            }
            return sync;
        }
        public SMSSync SyncCryptor
        {
            get
            {
                return m_Sync;
            }
        }
    }
}
