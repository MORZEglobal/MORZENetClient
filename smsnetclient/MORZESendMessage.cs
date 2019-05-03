using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SMS
{
    public class MORZESendMessage : SMSSendCommand
    {

        private byte[] m_hash;
        private SMSHash m_hashID;

        public MORZESendMessage() : base(0x02)
        {
            
        }
    

        public void AddMessageBody(byte []msgBody, SMSHash hash, byte []ext)
        {
            BufferBuilder bb = new BufferBuilder();
            bb.AddByte(0x32); //TTL
            byte idhash = (byte)hash;
            if (idhash!=0)
            {
                if (msgBody.Length < 0xFFFF)
                {
                    byte[] bthash=null;
                    switch(hash)
                    {
                        case SMSHash.MD5:
                            bthash = calcMD5(msgBody);
                            break;
                        default:
                            throw new Exception("Unsupported hash algorithm");
                            
                    }
                    if (ext != null)
                    {
                        byte e = 0x80;
                        idhash |= e;
                        if (ext.Length!=bthash.Length)
                            throw new Exception("Invalid ext");
                    }
                    bb.AddByte(idhash);

                    bb.AddBytes(bthash);
                    if (ext != null)
                        bb.AddBytes(ext);
                    bb.AddUshort((ushort)msgBody.Length);
                    bb.AddBytes(msgBody);
                    if (m_bodies == null)
                        m_bodies = new List<byte[]>();
                    m_bodies.Add(bb.GetAllBytes());
                }
                else
                {
                    throw new Exception("Unsupported length message");
                }
            }
            else
            {
                throw new Exception("did't select hash algorithm");
            }
        }
        private byte [] calcMD5(byte[] input)
        {
            byte[] md5=null;
            using (MD5 md5Hash = MD5.Create())
            {
                md5=md5Hash.ComputeHash(input);
            }
            return md5;
        }
       
    }
}
