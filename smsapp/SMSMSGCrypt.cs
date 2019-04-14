using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace smsapp
{
    public class SMSMSGCrypt
    {
        /*
         формат выходного сообщения
         1 байт алгоритм HASH
         2 байта длина HASH (N1)
         N1 байт - HASH
         1 байт алгоритм симетричного шифрования
         2 байта длина зашифрованного открытым ключем симетричного ключа (N2)
         N2 байт зашифрованный симетричный ключ
         остальный байт  - зашированное симметричным ключем сообщение
        */ 
        public byte [] PrepareMSGForSend(string msg, byte []To)
        {
            byte[] smssend=null;
            byte[] publicParam;
            byte[] msgKey = null;
            byte[] encodecmsg = null;
            byte[] enmsgKey = null;
            
            //publicParam = new byte[To.Length - 1];
            //Array.Copy(To,1, publicParam,0, To.Length - 1);
            //encodecmsg= DESEncode(To, out msgKey);
            ////1 байт ИД алгоритма ключа
            //switch (To[0])
            //{
            //    case 1://RSA
            //        enmsgKey=RSAEncode(publicParam, msgKey);
            //        break;
            //}
            //if (encodecmsg!=null && enmsgKey!=null)
            //{
            //    smssend = CreateMSGForSend(To[0], encodecmsg, enmsgKey);
            //}
            return smssend;
        }
        private byte[]RSAEncode(byte[]Key, byte [] msgkey)
        {
            byte[] ret;
            RSACryptoServiceProvider rsa = null;
            try
            {
                rsa = new RSACryptoServiceProvider();
                rsa.ImportCspBlob(Key);
                ret=rsa.Encrypt(msgkey, false);
            }
            catch
            {
                ret = null;
                
            }
            finally
            {
                if (rsa != null)
                    rsa.Dispose();
            }
            return ret;
        }
        public byte [] DESEncode (byte []msg,  byte []key)
        {
            byte[] crymsg =  null;
            DESCryptoServiceProvider des = null;
            CryptoStream stream = null;
            MemoryStream ms = null;
            BinaryWriter wr = null;
            
            try
            {
                des = new DESCryptoServiceProvider();
                ms = new MemoryStream();
                Array.Copy(key, des.Key, des.Key.Length);
                byte[] a=new byte[des.Key.Length];
                for (int i = 0; i < des.Key.Length; i++)
                    a[i] = key[i];
                des.Key = a;
                for (int i = 0; i < des.IV.Length; i++)
                    des.IV[i] = key[des.Key.Length+i];
                for (int i = 0; i < des.IV.Length; i++)
                    a[i] = key[des.Key.Length + i];
                des.IV = a;
                stream = new CryptoStream(ms, des.CreateEncryptor(des.Key, des.IV), CryptoStreamMode.Write);
                wr = new BinaryWriter(stream);
                wr.Write(msg, 0, (int)msg.Length);
                wr.Close();
                stream.Close();
                crymsg = ms.ToArray();
                
                
            }
            catch
            {
                crymsg = null;
            }
            finally
            {
                if (wr != null)
                    wr.Dispose();
                if (ms != null)
                    ms.Dispose();
                if (stream != null)
                    stream.Dispose();
                if (des != null)
                    des.Dispose();
            }
            return crymsg;
        }
        public byte[] DESDecode(byte[] msg, byte[] key)
        {
            byte[] crymsg = null;
            DESCryptoServiceProvider des = null;
            CryptoStream stream = null;
            MemoryStream ms = null;
            BinaryWriter wr = null;
            byte[] k;
            byte[] v;
            try
            {
                if (key.Length % 2 == 0)
                {
                    des = new DESCryptoServiceProvider();
                    ms = new MemoryStream();
                    k = new byte[key.Length / 2];
                    v = new byte[key.Length / 2];
                    Array.Copy(key, k, k.Length);
                    Array.Copy(key, k.Length, v, 0, v.Length);
                    stream = new CryptoStream(ms, des.CreateDecryptor(k, v), CryptoStreamMode.Write);
                    wr = new BinaryWriter(stream);
                    wr.Write(msg, 0, (int)msg.Length);
                    wr.Close();
                    stream.Close();
                    crymsg = ms.ToArray();
                }
            }
            catch
            {
                crymsg = null;
            }
            finally
            {
                if (wr != null)
                    wr.Dispose();
                if (ms != null)
                    ms.Dispose();
                if (stream != null)
                    stream.Dispose();
                if (des != null)
                    des.Dispose();
            }
            return crymsg;
        }
        public byte[] CreateMSGForSend(byte cryptkeyalgo, byte []encMsg, byte []enKey)
        {
            byte[] ret  =null;
            byte[] msgpart = null;
            byte[] hash = null;
            /*
            1 байт алгоритм симетричного шифрования
            2 байта длина зашифрованного открытым ключем симетричного ключа(N2)
            N2 байт зашифрованный симетричный ключ
            остальный байт - зашированное симметричным ключем сообщение
            */
            msgpart = new byte
                [1 +  //  1 байт алгоритм симетричного шифрования
                  2 + //   2 байта длина зашифрованного открытым ключем симетричного ключа(N2)
                  enKey.Length + //  N2 байт зашифрованный симетричный ключ
                  encMsg.Length // остальный байт - зашированное симметричным ключем сообщение
                ];
            msgpart[0] = cryptkeyalgo;
            Array.Copy(BitConverter.GetBytes((ushort)enKey.Length), 0, msgpart, 1, 2);
            Array.Copy(enKey, 0, msgpart, 3, enKey.Length);
            Array.Copy(encMsg, 0, msgpart, 3 + enKey.Length, encMsg.Length);
            try
            {
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    hash = md5.ComputeHash(msgpart);
                    ret = new byte[1 + 2 + hash.Length + msgpart.Length];
                    ret[0] = 1; /*
                                    1   hash is md5
                                    */
                    Array.Copy(BitConverter.GetBytes((ushort)hash.Length), 0, ret, 1, 2);
                    Array.Copy(hash, 0, ret, 3, hash.Length);
                    Array.Copy(msgpart, 0, ret, 3 + hash.Length, msgpart.Length);
                }
            }
            catch
            {
                ret = null;
            }
            return ret;
        }
    }
}
