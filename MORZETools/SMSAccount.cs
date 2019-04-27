using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Threading;

namespace SMS
{
    public class SMSAccount : IMORZEAccount
    {
        byte[] m_publicKey;
        byte[] m_ppKey;
        string m_address;
        string m_KeyName;
        byte[] m_Entropy;
        IAddressBook m_addressBook;

        List<MORZEMessages> m_Messages;
        
        /// <summary>
        /// генерация адреса абонента
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        
        public SMSAccount(string keyName)
        {
            m_KeyName = keyName;
            m_Entropy=Encoding.Unicode.GetBytes(keyName.ToLower());
            
        }
        public string GenerateKey()
        {
            
            
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider(1024);
            m_publicKey = csp.ExportCspBlob(false);
            m_address = "MRZR" + Convert.ToBase64String(m_publicKey);
            m_ppKey =csp.ExportCspBlob(true);
            m_ppKey=ProtectedData.Protect(m_ppKey, m_Entropy, DataProtectionScope.CurrentUser);
            
            csp.Dispose();
            return m_address;
        }

        private string getKeyFileName()
        {
            string path = string.Format("{0}SMS-{1}.key", SMSFileTools.SMSPath,  m_KeyName);
            return path;
        }
        /// <summary>
        /// сохранение ключа
        /// </summary>
        /// <param name="password">опционально</param>
        /// <returns>текст ошибки</returns>
        public string SaveKey(string password)
        {
            string err = null;
            MemoryStream ms=null;
            
            string filename = getKeyFileName();
           

            try
            {
                if (File.Exists(filename) == true)
                    err = "Ключ с таким именем уже существует, перезапись невозможна";
                else
                {
                    byte[] data = new byte[4096];

                    byte[] sz;
                    byte[] un;
                    ms = new MemoryStream();
                    
                    

                    un = Encoding.Unicode.GetBytes(m_KeyName);

                    sz = BitConverter.GetBytes(un.Length);
                    ms.Write(sz, 0, sz.Length);
                    ms.Write(un, 0, un.Length);

                    sz = BitConverter.GetBytes(m_ppKey.Length);
                    ms.Write(sz,0, sz.Length);
                    ms.Write(m_ppKey, 0 , m_ppKey.Length);

                    
                    sz = BitConverter.GetBytes(m_publicKey.Length);
                    ms.Write(sz, 0, sz.Length);
                    ms.Write(m_publicKey,0, m_publicKey.Length);

                    un = Encoding.Unicode.GetBytes(m_address);

                    sz = BitConverter.GetBytes(un.Length);
                    ms.Write(sz, 0, sz.Length);
                    ms.Write(un, 0, un.Length);
                    
                    
                    
                    
                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    data = new byte[ms.Length];
                    ms.Read(data, 0, data.Length);
                    data=ProtectedData.Protect(data, m_Entropy, DataProtectionScope.CurrentUser);
                    using (FileStream fs = File.Open(filename, FileMode.CreateNew, FileAccess.Write))
                    {
                        fs.Write(data, 0, data.Length);
                        fs.Close();
                    }
                }
              
            }
            catch(Exception exp)
            {
                err = exp.Message;
            }
            finally
            {
                if (ms != null)
                    ms.Dispose();
                if (ms != null)
                    ms.Dispose();
            }
            return err;
        }
        /// <summary>
        /// загрузка сохраненного ключа
        /// </summary>
        /// <param name="password">опционально</param>
        /// <returns>текст ошибки</returns>
        public string LoadKey(string password)
        {
            string err=null;
            FileStream fs=null;
            MemoryStream ms = null;
           
            
            try
            {
                byte []data;
                byte[] sz = new byte[4];
                byte[] vl;
                fs = File.Open(getKeyFileName(), FileMode.Open, FileAccess.Read);
                data = new byte[fs.Length];
                fs.Read(data, 0,(int) fs.Length);
                fs.Close();
                data=ProtectedData.Unprotect(data, m_Entropy, DataProtectionScope.CurrentUser);
                ms = new MemoryStream(data);

                ms.Read(sz, 0, sz.Length);
                vl = new byte[BitConverter.ToInt32(sz, 0)];
                ms.Read(vl, 0, vl.Length);

                string name = null;
                name = Encoding.Unicode.GetString(vl);
                if (name.ToLower() == m_KeyName.ToLower())
                {
                    byte []ppkey;
                    byte[] pkey;
                    string pkeyname;
                    
                    ms.Read(sz, 0, sz.Length);
                    ppkey = new byte[BitConverter.ToInt32(sz, 0)];
                    ms.Read(ppkey, 0, ppkey.Length);
                    ms.Read(sz, 0, sz.Length);
                    pkey = new byte[BitConverter.ToInt32(sz, 0)];
                    ms.Read(pkey, 0, pkey.Length);
                    ms.Read(sz, 0, sz.Length);
                    vl = new byte[BitConverter.ToInt32(sz, 0)];
                    ms.Read(vl, 0, vl.Length);
                    pkeyname = Encoding.Unicode.GetString(vl);

                    //byte[] key = ProtectedData.Unprotect(ppkey, m_Entropy, DataProtectionScope.CurrentUser);

                    //ProtectedMemory.Protect(key, MemoryProtectionScope.CrossProcess);

                    m_ppKey = ppkey;

                    m_publicKey = pkey;
                    m_address = pkeyname;

                }
                else
                    err = "Неверное имя ключа";

                
            }
            catch(Exception exp)
            {
                err = exp.Message;
            }
            finally
            {
                if (ms != null)
                    ms.Dispose();
                if (fs != null)
                    fs.Dispose();
            }
            return err;

        }
        /// <summary>
        /// создание резеврной копии ключа
        /// </summary>
        /// <param name="pathTo">путь сохранения</param>
        /// <param name="pass">пароль - обязательно</param>
        /// <returns>текст ошибки</returns>
        public  string BackCopy(string pathTo, string pass)
        {
            string err = null;

            return err;
        }

        public string GetMyAccount()
        {
            return m_address;
        }
        /// <summary>
        /// расшифровка данных закрытым ключем аккаунта
        /// </summary>
        /// <param name="enc"></param>
        /// <param name="dec"></param>
        /// <returns></returns>
        public bool decodeAsync(byte[] enc, out byte[] dec)
        {
            RSACryptoServiceProvider rsa = null;
            bool bres = false;
            try
            {
                rsa = new RSACryptoServiceProvider();
                
                byte[] key = ProtectedData.Unprotect(m_ppKey, m_Entropy, DataProtectionScope.CurrentUser);

                rsa.ImportCspBlob(key);
                dec = rsa.Decrypt(enc, false);
                key = null;
                bres = true;
            }
            catch(Exception exp)
            {
                dec = null;
                bres = false;
            }
            finally
            {
                if (rsa != null)
                    rsa.Dispose();
            }
            return bres;
        }

        public bool updateSynKey(SMSHash hash, byte[] ext, SMSSyncAlgo sync, byte[] key, byte[] iv, string contact)
        {
            IMORZEContact cnt;
            bool bres = false;
            cnt=m_addressBook.GetContact(contact, true);
            if (cnt!=null)
            {
                try
                {
                    bres = cnt.updateSynKey(hash, ext, sync, key, iv);
                    if (bres == true)
                    {
                        if (string.IsNullOrEmpty(m_addressBook.Save()) == true)
                            bres = true;
                    }
                }
                catch
                {
                    bres = false;
                }
            }
            return bres;
        }

        public IAddressBook AddressBook
        {
            set
            {
                m_addressBook = value;
            }
            get
            {
                return m_addressBook;
            }
        }

        public IAddressBook GetAddressBook()
        {
            return AddressBook;
        }
        public override string ToString()
        {
            return m_KeyName;
        }

        public MORZEMessages GetMessages(IMORZEContact contact)
        {

            Monitor.Enter(this);

            MORZEMessages msgs=null;
            if (m_Messages == null)
                m_Messages = new List<MORZEMessages>();
            else
                msgs = m_Messages.Where(x => x.Contact.ToString() == contact.ToString()).FirstOrDefault();
            if (msgs == null)
            {
                msgs = new MORZEMessages(this, contact);
                m_Messages.Add(msgs);
            }
            Monitor.Exit(this);

            return msgs;
        }
    }
}
