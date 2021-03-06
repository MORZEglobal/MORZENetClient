﻿using System;
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
        /// message send
        /// </summary>
        /// <param name="msg">mesage's plain text</param>
        /// <param name="to">reciver</param>
        /// <returns>error description</returns>
        public string SendMessage (string msg, IMORZEContact to)
        {
            string err = null;
            if (to.isHasConfirmEtx()==false)
            {
                try
                {
                    SendInitialMessage(to);

                    MORZEMessages msgs;
                    msgs=m_account.GetMessages(to);
                    if (msgs!=null)
                    {
                        msgs.AddUnsendedNewMessages(msg);
                    }
                }
                catch(Exception exp)
                {
                    OnConnectChange(false, exp.Message);
                }
            }
            else
            {
                ExtKey ext;
                MORZEMessage mmsg = new MORZEMessage(msg.ToString());
                byte[] netmsg = to.getMORZENetMessage(mmsg, out ext);

                MORZESendMessage m = new MORZESendMessage();
                m.AddMessageBody(netmsg, ext.HashID, ext.Ext);
                
                m.Send(m_netStream);

                MORZEMessages msgs;
                msgs = m_account.GetMessages(to);
                if (msgs != null)
                {
                    mmsg.AddHashInfo(ext.HashID, SMSCrypt.CalcHash(ext.HashID, netmsg));
                    msgs.AddSendedNewMessages(mmsg);
                }

            }
            return err;
        }

        public string SendMessage(MORZEMessage msg, IMORZEContact to)
        {
            string err = null;
            if (to.isHasConfirmEtx() == true)
            {
                try
                {
                    ExtKey ext;
                    byte []netmsg = to.getMORZENetMessage(msg, out ext);
                    MORZESendMessage m = new MORZESendMessage();
                    m.AddMessageBody(netmsg, ext.HashID, ext.Ext);
                    m.Send(m_netStream);
                    msg.Status = MORZEMessageStatus.sended;
                    msg.AddHashInfo(ext.HashID, SMSCrypt.CalcHash(ext.HashID, netmsg));
                }
                catch (Exception exp)
                {
                    OnConnectChange(false, exp.Message);
                }
            }
            else
                SendInitialMessage(to);
            return err;
        }
        private void SendInitialMessage(IMORZEContact to)
        {
            SMSInitialMessage imsg = new SMSInitialMessage(m_account, to);

            //Monitor.Enter(this);
            ////if (m_InitalMessages == null)
            ////    m_InitalMessages = new List<MORZEInitialMessage>();
            ////m_InitalMessages.Add(imsg);
            //Monitor.Exit(this);

            imsg.Send(m_netStream);
            SMSSendExt ext = new SMSSendExt(this);
            imsg.InitExtParam(ext);
            ext.Send(m_netStream);
        }

    }
}