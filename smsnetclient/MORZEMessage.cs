using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SMS
{

    public partial class SMSNet :
        IDisposable
    {
        /// <summary>
        /// отправка сообщения
        /// </summary>
        /// <param name="msg">текст сообщения</param>
        /// <param name="to">контакт получателя</param>
        /// <returns>описание ошибки</returns>
        public string SendMessage (string msg, IMORZEContact to)
        {
            string err = null;
            if (to.isHasConfirmEtx()==false)
            {
                try
                {
                    SendInitialMessage(to);
                }
                catch(Exception exp)
                {
                    OnConnectChange(false, exp.Message);
                }
            }
            return err;
        }
        private void SendInitialMessage(IMORZEContact to)
        {
            SMSInitialMessage imsg = new SMSInitialMessage(m_account, to);

            Monitor.Enter(this);
            //if (m_InitalMessages == null)
            //    m_InitalMessages = new List<MORZEInitialMessage>();
            //m_InitalMessages.Add(imsg);
            Monitor.Exit(this);

            imsg.Send(m_netStream);
            SMSSendExt ext = new SMSSendExt(this);
            imsg.InitExtParam(ext);
            ext.Send(m_netStream);
        }

    }
}