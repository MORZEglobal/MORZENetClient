using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

namespace SMS
{
    public class AddressBook : IAddressBook
    {
        List<MORZEContact> m_contacts;
        string m_path;
        public AddressBook()
        {
            m_path = SMSFileTools.SMSPath;
            m_path += "addressbook.store";
            Load();
        }
        public string AddContact(string Name, string address)
        {

            string err = null;
            Monitor.Enter(m_contacts);
            if (GetContact(address , false)==null)
            {
                try
                {
                    MORZEContact cnt = new MORZEContact(Name, address);
                    m_contacts.Add(cnt);
                }
                catch(Exception exp)
                {
                    err = exp.Message;
                }

            }
            Monitor.Exit(m_contacts);
            return err;
        }
        public List<MORZEContact> Contacts
        {
            get
            {
                return m_contacts;
            }
        }
        public string AddContact(IMORZEContact   cnt)
        {

            string err = null;
            if (m_contacts == null)
                m_contacts = new List<MORZEContact>();
            Monitor.Enter(m_contacts);
            try
            {
                m_contacts.Add(cnt as MORZEContact);
            }
            catch (Exception exp)
            {
                err = exp.Message;
            }
            Monitor.Exit(m_contacts);
            return err;
        }

        /// <summary>
        /// добавить контакт в адресную книгу
        /// </summary>
        /// <param name="address">адрес контакта</param>
        /// <param name="isaddIfnotExist">добавить контакт если не сущесвует</param>
        /// <returns></returns>
        public IMORZEContact GetContact(string address, bool isaddIfnotExist)
        {
            IMORZEContact cnt=null;
            Monitor.Enter(m_contacts);

            var cnts = m_contacts.Where(x => x.GetAddress() == address);
            if (cnts.Any() == true)
                cnt = cnts.First();
            else
            {
                if (isaddIfnotExist == true)
                {
                    try
                    {
                        MORZEContact _cnt;
                        _cnt = new MORZEContact(null, address);
                        m_contacts.Add(_cnt);
                        cnt = _cnt;
                    }
                    catch 
                    {
                        cnt = null;
                    }
                }
            }

            Monitor.Exit(m_contacts);
            return cnt;
        }
        public IMORZEContact GetContact(byte [] ext)
        {
            IMORZEContact cnt=null;
            Monitor.Enter(m_contacts);
            foreach (MORZEContact c in m_contacts)
            {
                if (c.isHasExt(ext) == true)
                {
                    if (cnt == null)
                        cnt = c;
                    else
                    {
                        throw new Exception("multiple ext");
                    }
                }
            }
            Monitor.Exit(m_contacts);
            return cnt;
        }
        /// <summary>
        /// сохранение контактов на диск
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string Save()
        {
            BinaryFormatter formatter = null;
            MemoryStream ms=null;
            string error = null;
            bool isBkCopied = false;
            string bkfile = m_path + ".bac";
            FileStream fs = null;
            try
            {
                if (m_contacts != null)
                {
                    formatter = new BinaryFormatter();
                    ms = new MemoryStream();

                    Monitor.Enter(m_contacts);
                    formatter.Serialize(ms, m_contacts);
                    Monitor.Exit(m_contacts);

                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    byte[] data;
                    byte[] encdata;
                    data = new byte[ms.Length];
                    ms.Read(data, 0, data.Length);
                    encdata = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
                    if (File.Exists(m_path) == true)
                    {

                        File.Copy(m_path, bkfile, true);
                        isBkCopied = true;
                    }
                    fs = File.Open(m_path, FileMode.Create, FileAccess.Write);
                    fs.Write(encdata, 0, encdata.Length);
                    fs.Close();

                }
            }
            catch (Exception exp)
            {
                error = exp.Message;
                if (isBkCopied == true)
                {
                    if (File.Exists(bkfile)==true)
                        File.Copy(bkfile, m_path, true);
                }
            }
            finally
            {
                if (ms != null)
                    ms.Dispose();
                if (fs != null)
                    fs.Dispose();
            }
            return error;
        }
        /// <summary>
        /// загрузка контактов с диска
        /// </summary>
        /// <returns></returns>
        private string Load()
        {
            string error = null;
            if (File.Exists(m_path) == true)
                error = Load(m_path);
            else
                m_contacts = new List<MORZEContact>();
            return error;
        }
        private string Load(string path)
        {
            BinaryFormatter formatter = null;
            MemoryStream ms = null;
            string error = null;
            
            FileStream fs = null;
            try
            {
                fs = File.Open(m_path, FileMode.Open, FileAccess.Read);

                byte[] data;
                byte[] encdata;
                encdata = new byte[fs.Length];

                fs.Read(encdata, 0, encdata.Length);
                fs.Close();
                data = ProtectedData.Unprotect(encdata, null, DataProtectionScope.CurrentUser);
                formatter = new BinaryFormatter();
                ms = new MemoryStream();
                ms.Write(data, 0, data.Length);
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                m_contacts = formatter.Deserialize(ms) as List<MORZEContact>;

            }
            catch (Exception exp)
            {
                error = exp.Message;
            }
            finally
            {
                if (ms != null)
                    ms.Dispose();
                if (fs != null)
                    fs.Dispose();
            }
            return error;
        }
    }
}
