using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SMS
{
    public abstract class SMSSync
    {
        private byte[] m_check;
        private byte[] m_hello;
        public SMSSync(byte[]checkpack, byte[]hello)
        {
            m_check = new byte[checkpack.Length/2];
            Array.Copy(checkpack, checkpack.Length /2, m_check, 0, m_check.Length);
            m_hello = hello;
        }
        public abstract byte[] Encrypt(byte[] src);
        public abstract byte[] Decrypt(byte[] src);

        protected byte[] initIV( int len)
        {
            int copied = 0;
            byte[] IV;
            IV = new byte[len];
            do
            {
                int copylen = 0;
                if ( IV.Length - copied < m_hello.Length)
                    copylen = IV.Length - copied;
                else
                    copylen = m_hello.Length;
                Array.Copy(m_hello, 0, IV, copied, copylen);
                copied += copylen;
            } while (copied < IV.Length);
            return IV;
        }
        protected bool check()
        {
            bool isSuccess = false;
            byte[] res;
            res=Decrypt(m_check);
            if (res!=null && res.Length==m_hello.Length)
            {
                isSuccess = true;
                for (int i = 0; i < res.Length && isSuccess == true; i++)
                {
                    if (res[i] != m_hello[i])
                        isSuccess = false;
                }
            }
            return isSuccess;
        }
    }


    public class SMSDES : SMSSync, IDisposable
    {
        DESCryptoServiceProvider m_des = null;
        public SMSDES(byte[] SyncData, byte[] hello) : base(SyncData, hello)
        {
            m_des = new DESCryptoServiceProvider();
            if (m_des.Key.Length == SyncData.Length/2)
            {
                byte[] key;
                key = new byte[m_des.Key.Length];
                
                Array.Copy(SyncData, key, key.Length);
                m_des.Key = key;
#if DEBUG
                for (int i = 0; i < key.Length; i++)
                    key[i] = 0;
#endif

                m_des.IV=initIV(m_des.IV.Length);
                if (check() == false)
                    throw new Exception("Ошибка обмена ключами шифрования");
            }
            else
            {
                throw new Exception("Неверная длина принятого ключа симметричного шифрования DES");
            }
            
        }
        
        public override byte[] Decrypt(byte[] src)
        {
            
            CryptoStream stream = null;
            MemoryStream ms = null;
            BinaryWriter wr = null;
            byte[] res=null;
            try
            {

                ms = new MemoryStream();

                stream = new CryptoStream(ms, m_des.CreateDecryptor(), CryptoStreamMode.Write);
                wr = new BinaryWriter(stream);
                wr.Write(src, 0, (int)src.Length);
                wr.Close();
                stream.Close();
                res = ms.ToArray();

            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (wr != null)
                    wr.Dispose();
                if (ms != null)
                    ms.Dispose();
                if (stream != null)
                    stream.Dispose();
            }
            return res;
        }

        public void Dispose()
        {
            try
            {
                if (m_des != null)
                {
                    m_des.Dispose();
                }
            }
            finally
            {
                m_des = null;
            }

        }

        public override byte[] Encrypt(byte[] src)
        {
            CryptoStream stream = null;
            MemoryStream ms = null;
            BinaryWriter wr = null;
            byte[] res = null;
            try
            {

                ms = new MemoryStream();


                stream = new CryptoStream(ms, m_des.CreateEncryptor(), CryptoStreamMode.Write);
                wr = new BinaryWriter(stream);
                wr.Write(src, 0, (int)src.Length);
                wr.Close();
                stream.Close();
                res = ms.ToArray();

            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                if (wr != null)
                    wr.Dispose();
                if (ms != null)
                    ms.Dispose();
                if (stream != null)
                    stream.Dispose();
            }
            return res;
        }
    }
}
