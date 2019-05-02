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
    #region делегаты событий
    public delegate void connected();
    public delegate void disconnected(string msg);
    #endregion

    public interface ISMSNet
    {
        byte[] Encrypt(byte[] input);
        byte[] Decrypt(byte[] input);
    }

    public partial class SMSNet :
        ISMSNet,
        IDisposable
    {

        TcpClient m_morze = null;
        Thread m_treadReadingCmd =null;
        byte []m_helloSign;
        const ushort m_clientversion=0x1;
        Random m_rnd;
        bool m_stop;
        IMORZEAccount m_account;
        NetworkStream m_netStream =null;
        //SMSRecvHello m_rspHello;
        MORZESendAsync m_sendAsyngCmd;

        SMSSync m_sync;

        #region События
        public event connected OnConnected;
        public event disconnected OnDisconnected;
        #endregion
        public SMSNet(IMORZEAccount account)
        {

            m_stop = false;
            //m_rspHello = null;
            m_rnd = new Random(); 
            m_netStream = null;
            m_morze = null;
            m_helloSign = new byte[4];
            m_sendAsyngCmd = null;
            m_rnd.NextBytes(m_helloSign);
    //        m_InitalMessages = null;
            m_account = account;

        }
        /// <summary>
        /// установить соединение с сервером
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="tcpport"></param>
        public string Connect(string server, ushort tcpport)
        {
            string err = null;
       
            try
            {

                m_morze = new TcpClient(server, tcpport);
                m_netStream = m_morze.GetStream();
                err = HelloCmd();
                if (string.IsNullOrEmpty(err)==true)
                {//запуск потока приема команд от Morze сервера
                    m_treadReadingCmd = new Thread((ParameterizedThreadStart)thRecvCommand, 0);
                    m_treadReadingCmd.Start(this);
                }
                else
                {
                    OnConnectChange(false, err);
                }
                
            }
            catch(Exception exp)
            {
                
                err = exp.Message;
                OnConnectChange(false, err);
            }
            return err;
        }

        public string setExtParam(ExtKey key)
        {
            string err = null;
            //set server filter setting
            try
            {
                SMSSendExt setext = new SMSSendExt();
                setext.pushExt(key.HashID, key.Ext);
                setext.SMSNet = this;
                setext.Send(m_netStream);
            }
            catch (Exception exp)
            {

                err = exp.Message;
                OnConnectChange(false, err);
            }
            return err;

        }
        private static void thRecvCommand (Object smsobj)
        {
            if ((smsobj is SMSNet) == true)
            {
                SMSNet sms = smsobj as SMSNet;
                sms.thRecvCommand();
            }
        }
        private void thRecvCommand()
        {
            MORZERecvCommand rcmd;
            MORZECommand cmd;
            try
            {
                do
                {
                    rcmd = new MORZERecvCommand(m_account, m_netStream);
                    cmd = rcmd.recvCmd();
                    if (cmd != null)
                    {
                        SMSRecvHello rcvhl= null;
                        if (rcvhl == null && cmd is SMSRecvHello == true)
                        {
                            rcvhl = cmd as SMSRecvHello;
                            if (rcvhl.isValid(m_helloSign) == true)
                                OnConnectChange(true, string.Empty);
                            else
                                OnConnectChange(false, string.Empty);
                        }
                        //else if (rcvhl == null && cmd is SMSRecvHello == false)
                        //    OnConnectChange(false , "Неверный ответ сервера");
                    }
                    if (cmd!=null && cmd is MORZERecvSynKey ==true)
                    {
                        MORZERecvSynKey srvSync = cmd as MORZERecvSynKey;
                        srvSync.SetAsync(m_sendAsyngCmd, m_helloSign);
                        m_sendAsyngCmd.Dispose();
                        m_sendAsyngCmd = null;
                        m_sync = srvSync.SyncCryptor;

                        if (OnConnected != null)
                            OnConnected();
                        
                    }
                    if (cmd!= null && cmd is MORZERecvMessages == true)
                    {//принято сообщение
                        MORZERecvMessages rm = cmd as MORZERecvMessages;
                        if (rm.isResponses==true)
                        {//есть ответные сообщения
                            List<SMSSendCommand> res = rm.Responses;
                            foreach (SMSSendCommand cm in res)
                            {
                                SMSSendExt setext = cm as SMSSendExt;
                                if (setext != null)
                                {
                                    setext.SMSNet = this;
                                    
                                }
                                cm.Send(m_netStream);
                            }
                        }
                    }
                } while (m_stop == false);
            }
            catch (Exception exp)
            {
                if (m_stop==false)
                    OnConnectChange(false, exp.Message);
            }
            
        }
        private  void OnConnectChange(bool isConnected, string err)
        {
            if (isConnected == true && string.IsNullOrEmpty(err)==true)
            {//отправить открытый ключ , для шифрования ключа симметричного алгоритма
                m_sendAsyngCmd = new MORZESendAsync(m_helloSign);
                m_sendAsyngCmd.Send(m_netStream);
            }
            if (isConnected==false)
            {
                ReleaseSock();
                if (OnDisconnected != null)
                    OnDisconnected(err);
            }
        }
        private void ReleaseSock ()
        {
            m_stop = true;
            if (m_morze != null)
            {

                if (m_morze != null)
                {
                    try
                    {
                        Monitor.Enter(m_morze);
                        m_morze.Close();
                    }
                    finally
                    {
                        Monitor.Exit(m_morze);

                        m_netStream.Dispose();
                        m_netStream = null;
                        m_morze = null;
                    }
                }
                
            }

            if (m_sync != null)
            {
                
                try
                {
                    IDisposable d;
                    Monitor.Enter(m_sync);
                    d = m_sync as IDisposable;
                    if (d != null)
                        d.Dispose();
                }
                finally
                {
                    Monitor.Exit(m_sync);
                    m_sync = null;
                }
            }
        }
        //отправка команды hello
        private string HelloCmd()
        {
            string err = null;
            SMSSendCommand mrz = null;
            try
            {
                mrz = new SMSSendCommand(0x01);
                mrz.WriteBytes(m_helloSign);
                mrz.Write2Bytes(m_clientversion);
                mrz.Send(m_netStream);
            }
            catch (Exception exp)
            {
                err = exp.Message;
            }
            finally
            {
                if (mrz != null)
                    mrz.Dispose();
            }

            return err;
        }
        
       

        public void Dispose()
        {
            m_stop = true;
            ReleaseSock();
            if (m_netStream != null)
            {
                try
                {
                    m_netStream.Close();
                }
                finally
                {
                    m_netStream.Dispose();
                    m_netStream = null;
                }
                
            }
            if (m_morze != null)
            {
                try
                {
                    m_morze.Close();
                }
                finally
                {
                    m_morze = null;
                }
            }
            if (m_sendAsyngCmd != null)
            {
                m_sendAsyngCmd.Dispose();
                m_sendAsyngCmd = null;
            }
        }

        public byte[] Encrypt(byte[] input)
        {
            byte[] res = null;
            if (m_sync != null)
            {
                res = m_sync.Encrypt(input);
            }
            else
                OnConnectChange(false, "Нет ключи шифрования");
            return res;
        }

        public byte[] Decrypt(byte[] input)
        {
            byte[] res = null;
            if (m_sync != null)
            {
                res = m_sync.Decrypt(input);
            }
            else
                OnConnectChange(false, "Нет ключи дешифрования");
            return res;
        }
    }
  
    
}
