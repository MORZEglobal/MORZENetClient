using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SMS
{
    class MORZEMessage
    {
        public MORZEMessage(byte ttl,  byte []msg, byte []ext)
        {

        }

    }
    class MORZERecvMessages : MORZECommand
    {
        List<byte[]> m_msgAsyncData;
        List<byte[]> m_msgData;
        List<byte[]> m_msgHash;
        List<byte[]> m_msgExt;
        ISMSAccount m_acc;

        List<SMSSendCommand> m_responses;

        public MORZERecvMessages(ISMSAccount acc, byte[] data) : base(data)
        {
            int off=0;
            byte[] msg;
            
            byte[] hash;
            byte[] ext;

            m_responses = null;
            byte[] addascii = Encoding.ASCII.GetBytes(acc.GetMyAccount());

            m_acc = acc;
            bool err = false;
            SMSHash mrzhash= SMSHash.None;
            while (off<data.Length && err==false)
            {
                msg = null;
                hash = null;
                ext = null;
                
                off++;//ttl
                ushort msglen = 0;
                switch (data[off])//hash
                {
                    case 0x01://md5
                        hash = new byte[0x10];
                        mrzhash = SMSHash.MD5;
                        break;
                    case 0x81:
                        hash = new byte[0x10];
                        mrzhash = SMSHash.MD5;
                        ext = new byte[0x10];
                        break;
                    default:
                        err = true;
                        break;
                }
                if (err==false)
                {
                    off++;
                    Array.Copy(data, off, hash, 0, hash.Length);
                    off += hash.Length;
                    if (ext!=null)
                    {
                        Array.Copy(data, off, ext, 0, ext.Length);
                        off += ext.Length;
                    }
                    msglen = BitConverter.ToUInt16(data, off);
                    off += 2;
                    if (msglen + off <= data.Length)
                    {
                        msg = new byte[msglen];
                        Array.Copy(data, off, msg, 0, msg.Length);
                        off += msg.Length;
                    }
                    else
                        err = true;
                }
                if (err==false)
                {
                    bool add = false;
                    if (checkHash(mrzhash, hash, msg) == true)
                    {
                        if (ext == null)
                        {//данные зашифрованные открытым ключем
                            byte asynclen = 0;
                            byte[] smsg = null;
                            byte[] async = null;
                            asynclen = (byte)((int)addascii[0x10] ^ (int)msg[0]);
                            if (asynclen > 0)
                            {
                                async = new byte[asynclen];
                                Array.Copy(msg, 1, async, 0, async.Length);
                                if ((int)asynclen < msg.Length)
                                {
                                    smsg = new byte[msg.Length - (1 + asynclen)];
                                    Array.Copy(msg, 1 + asynclen, smsg, 0, smsg.Length);
                                }
                                else
                                {
                                    if (msg.Length < asynclen)
                                        err = true;
                                }
                                if (err == false)
                                {
                                    byte[] dec;
                                    byte[] tail = null;
                                    int taillength = msg.Length - asynclen - 1;
                                    if (taillength < 0)
                                        err = true;
                                    else
                                    {
                                        tail = new byte[taillength];
                                        Array.Copy(msg, async.Length + 1, tail, 0, taillength);
                                        if (acc.decodeAsync(async, out dec) == true)
                                        {
                                            err = !operateAsync(dec, tail);
                                        }
                                    }

                                }
                            }
                            else
                                err = true;
                        }
                        else
                            add = true;

                        if (add == true)
                        {
                            if (m_msgData == null)
                                m_msgData = new List<byte[]>();
                            if (m_msgHash == null)
                                m_msgHash = new List<byte[]>();
                            if (m_msgExt == null)
                                m_msgExt = new List<byte[]>();
                            m_msgData.Add(msg);
                            m_msgHash.Add(hash);
                            m_msgExt.Add(ext);

                            IMORZEContact mc = m_acc.GetAddressBook().GetContact(ext);

                            if (mc != null)
                                mc.PutReciveMessage(msg, hash, mrzhash, ext);


                        }
                    }//if (checkHash(mrzhash, hash, msg) == true)
                }
            }//while (off<data.Length && err==false)
            if (err == true || off!=data.Length)
            {
                
                    m_msgData = null;
                    m_msgHash = null;
                    m_msgExt = null;
                    
            }
            
        }
        bool operateAsync(byte[] data, byte[] tail)
        {
            bool isSuccess = false;
            if (checkCRC16(data) == true)
            {
                switch (data[0]) //поле Type
                {
                    case 1:  // передача симметричного ключа и метки сообщений
                        isSuccess = operateAsyncType1(data, tail);
                        break;
                    default:
                        isSuccess = false;
                        break;
                }
            }
            return isSuccess;
        }
        bool operateAsyncType1 (byte[] data, byte[] tail)
        {
            bool isSuccess = false;
            int off = 1;
            byte[] ext = null;
            byte[] sync = null;
            byte[] iv = null;
            SMSHash hashid = SMSHash.None;
            SMSSyncAlgo syncid = SMSSyncAlgo.None;

            int extlen = 0;
            int synlen = 0;
            switch(data[off])
            {
                case 1://MD5
                    extlen = 0x10;
                    hashid = SMSHash.MD5;
                    
                    break;
            }

            if (extlen>0)
            {
                off++;
                ext = new byte[extlen];
                Array.Copy(data, off, ext, 0, ext.Length);
                off += ext.Length;
            }

            if (hashid != SMSHash.None)
            {
                switch (data[off])
                {
                    case 1://DES
                        syncid = SMSSyncAlgo.DES;
                        synlen = 8;
                        break;
                }
            }
            if (synlen > 0)
            {
                off++;
                sync = new byte[synlen];
                iv = new byte[synlen];
                Array.Copy(data, off, sync, 0, sync.Length);
                off+= sync.Length;
                Array.Copy(data, off, iv, 0, iv.Length);
                off += iv.Length;
            }
            if (off +2 == data.Length) // +2  - CRC16
            {
                byte[] contactaddres;
                string cont = null;
                if (string.IsNullOrEmpty(SMSCrypt.SyncDecode(syncid, tail, sync, iv, out contactaddres)) == false)
                    contactaddres = null;
                if (contactaddres!=null)
                {
                    try
                    {
                        cont = Encoding.ASCII.GetString(contactaddres);
                        Convert.FromBase64String(cont.Substring(4));
                        isSuccess = m_acc.updateSynKey(hashid, ext, syncid, sync , iv, cont);
                    }
                    catch
                    {
                        isSuccess = false;
                    }
                }
                
            }
            if (isSuccess==true)
            {

                MORZESendMessage cmdMsgTyp2;

                //---------------
                cmdMsgTyp2 = new MORZESendMessage();

                BufferBuilder bb = new BufferBuilder();
                bb.AddByte(2); // Type 2 - уведомление о получении ключей
                bb.AddByte((byte)hashid);
                bb.AddBytes(SMSCrypt.CalcHash(hashid, sync));

                //byte[] msg = bb.GetAllBytes();
                //bb = new BufferBuilder();
                //bb.AddByte((byte)((int)0x80 ^ (int)hashid));
                //bb.AddBytes(SMSCrypt.CalcHash(hashid, msg));
                //bb.AddBytes(ext);
                //bb.AddBytes(msg);
                //cmdMsgTyp2.WriteBytes(bb.GetAllBytes()); 
                byte[] res;
                if (string.IsNullOrEmpty(SMSCrypt.SyncEncode(syncid, bb.GetAllBytes(), sync, iv, out res)) == true)
                {
                    cmdMsgTyp2.AddMessageBody(res, hashid, ext);
                    if (m_responses == null)
                        m_responses = new List<SMSSendCommand>();

                    m_responses.Add(cmdMsgTyp2);
                }
                else
                    isSuccess = false;
            }
            return isSuccess;
        }

        public List<SMSSendCommand> Responses
        {
            get
            {
                return m_responses;
            }
        }
        public bool isResponses
        {
            get
            {
                bool isres= false;
                if (m_responses != null && m_responses.Any() == true)
                    isres = true;
                return isres;
            }
        }
        private bool checkHash(SMSHash type, byte[] hash, byte[]msg)
        {
            bool bret = false;
            if (type== SMSHash.MD5)
            {
                byte[] sh = null; 
                using (MD5 md5 = MD5.Create())
                {
                    sh = md5.ComputeHash(msg);
                }
                if (sh != null)
                {
                    if (sh.Length==hash.Length)
                    {
                        bret = true;
                        for(int i=0;i<sh.Length && bret==true;i++)
                        {
                            if (sh[i] != hash[i])
                                bret = false;
                        }
                    }
                }
            }
            return bret;
        }
        private bool checkCRC16(byte[] data)
        {
            ushort crc16;
            bool bret = false;
            crc16 = BitConverter.ToUInt16(data, data.Length - 2);

            if (crc16==0)/// версия 0.1  только значение 0
            {
                bret = true;
            }
            return bret;
        }
    }
}
