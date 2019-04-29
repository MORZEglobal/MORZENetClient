using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SMS
{
    public class SMSCrypt
    {
        static public string SyncEncode(SMSSyncAlgo alg, byte[] data, byte[] key, byte[] iv, out byte[] res)
        {
            string err=null;
            res = null;
            try
            {
                switch (alg)
                {
                    case SMSSyncAlgo.DES:
                        err = EncDES(data, key, iv, out res);
                        break;
                    default:
                        err = "Invalid Symmetric algorithm";
                        break;

                }
            }
            catch(Exception exp)
            {
                res = null;
                err = exp.Message;
            }
            return err;
        }
        static public string SyncDecode(SMSSyncAlgo alg, byte[] data, byte[] key, byte[] iv, out byte[] res)
        {
            string err = null;
            res = null;
            try
            {
                switch (alg)
                {
                    case SMSSyncAlgo.DES:
                        err = DecDES(data, key, iv, out res);
                        break;
                    default:
                        err = "Invalid Symmetric algorithm";
                        break;

                }
            }
            catch (Exception exp)
            {
                res = null;
                err = exp.Message;
            }
            return err;
        }
        static private string EncDES(byte[] data, byte[] key, byte[] iv, out byte[] res)
        {
            string err = null;
            MemoryStream ms = null;
            BinaryWriter wr = null;
            CryptoStream cs = null;
            res = null;
            try
            {
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    //des.Padding = PaddingMode.
                    cs = new CryptoStream(ms, des.CreateEncryptor(key, iv), CryptoStreamMode.Write);
                    wr = new BinaryWriter(cs);
                    wr.Write(data, 0, (int)data.Length);
                    wr.Close();
                    cs.Close();
                    res = ms.ToArray();
                }

            }
            catch (Exception exp)
            {
                res = null;
                err = exp.Message;
            }
            finally
            {
                if (cs != null)
                    cs.Dispose();
                if (ms != null)
                    ms.Dispose();
                if (wr != null)
                    wr.Dispose();
            }
            return err;
        }
        static private string DecDES(byte[] data, byte[] key, byte[] iv, out byte[] res)
        {
            string err = null;
            MemoryStream ms = null;
            BinaryWriter wr = null;
            CryptoStream cs = null;
            res = null;
            try
            {
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    //des.Padding = PaddingMode.
                    cs = new CryptoStream(ms, des.CreateDecryptor(key, iv), CryptoStreamMode.Write);
                    wr = new BinaryWriter(cs);
                    wr.Write(data, 0, (int)data.Length);
                    wr.Close();
                    cs.Close();
                    res = ms.ToArray();
                }

            }
            catch (Exception exp)
            {
                res = null;
                err = exp.Message;
            }
            finally
            {
                if (cs != null)
                    cs.Dispose();
                if (ms != null)
                    ms.Dispose();
                if (wr != null)
                    wr.Dispose();
            }
            return err;
        }

        static public byte [] CalcHash(SMSHash hashid, byte[]input)
        {
            byte[] res = null;
            try
            {
                switch (hashid)
                {
                    case SMSHash.MD5:
                        res = CalcMD5(input);
                        break;
                    default:
                        res = null;
                        break;
                }
            }
            catch
            {
                res = null;
            }
            return res;
        }

        static private byte [] CalcMD5(byte []input)
        {
            byte[] res = null;
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                res = md5.ComputeHash(input);
            }
            return res;
        }

        static public bool CheckHash(SMSHash hashid, byte[]input, byte[]hash)
        {
            bool isEq = false;
            byte[] chash;
            chash = CalcHash(hashid, input);
            if (chash != null && hash != null && chash.Length == hash.Length)
            {
                isEq = true;
                for (int i = 0; i < chash.Length && isEq == true; i++)
                    isEq = chash[i] == hash[i];
                
            }
            return isEq;
        }

        static public string SyncEncode(SMSSyncAlgo alg, string text, byte[] key, byte[] iv, out byte[] res)
        {
            string err = null;
            res = null;
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(text);
                err = SyncEncode(alg, data, key, iv, out res);
                if (string.IsNullOrEmpty(err) == false)
                    res = null;
            }
            catch (Exception exp)
            {
                res = null;
                err = exp.Message;
            }
            return err;
        }
        static public string SyncDecode(SMSSyncAlgo alg, byte[] data, byte[] key, byte[] iv, out string text)
        {
            string err = null;
            text = null;
            try
            {
                byte[] res=null;
                err = SyncDecode(alg, data, key, iv, out res);
                if (res != null && string.IsNullOrEmpty(err)==false)
                    text=Encoding.ASCII.GetString(res);
            }
            catch (Exception exp)
            {
                text = null;
                err = exp.Message;
            }
            return err;
        }
    }
}
