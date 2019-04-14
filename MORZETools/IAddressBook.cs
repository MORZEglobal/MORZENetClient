using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS
{
    public interface IAddressBook
    {
        /// <summary>
        /// получить контакт по адресу
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        IMORZEContact GetContact(string address, bool addifNotExist);
        /// <summary>
        /// добавить контакт 
        /// </summary>
        /// <param name="address"></param>
        /// <returns>текст ошибки</returns>
        string AddContact(string Name, string address);
        string Save();
        IMORZEContact GetContact(byte[] ext);
        
    }
}
