using SMS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace morzeui
{
    public partial class dlgMessage : Form
    {
        IMORZEContact m_cnt;
        IMORZEAccount m_acc;
        SMSNet m_net;
        DateTime? m_lastLoadedMessage;
        public dlgMessage(IMORZEContact cnt, IMORZEAccount acc, SMSNet net)
        {
            m_cnt = cnt;
            m_acc = acc;
            m_net = net;
            m_lastLoadedMessage = null;
            InitializeComponent();
        }

        private void dlgMessage_Load(object sender, EventArgs e)
        {
            Text = string.Format("{0} - {1}", m_acc.ToString(), m_cnt.ToString());
            MORZEContact mcnt = m_cnt as MORZEContact;
            if (mcnt!=null)
            {
                mcnt.OnRecvMessage += OnRecvMessage;

                MORZEMessages msgs=m_acc.GetMessages(m_cnt);
                if (msgs!=null)
                {
                    List<MORZEMessage> m;
                    if (m_lastLoadedMessage != null)
                        m = msgs.Messages.Where(x => x.Date > m_lastLoadedMessage).ToList();
                    else
                        m = msgs.Messages;
                    m = m.OrderBy(x => x.Date).ToList();

                    foreach(MORZEMessage i in m)
                    {
                        if (i.Status == MORZEMessageStatus.recived)
                            PutDisplayMessage(m_cnt.ToString(), i.ToString(),false);
                        else
                            PutDisplayMessage(m_acc.ToString(), i.ToString(),true);
                        m_lastLoadedMessage = i.Date;
                    }
                }
            }
        }

        private void OnRecvMessage(IMORZEContact sender, MORZEMessage msg)
        {
            Invoke(new Action(() =>
            {
                PutDisplayMessage(sender.ToString(), msg.ToString(), false);
            }));
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbMessage.Text) == false)
            {
                string err = null;
                try
                {
                    err = m_net.SendMessage(tbMessage.Text, m_cnt);
                    if (string.IsNullOrEmpty(err) == true)
                    {
                        PutDisplayMessage(m_acc.ToString(), tbMessage.Text, true);
                        tbMessage.Text = string.Empty;
                    }
                    else
                        MessageBox.Show(err, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception exp)
                {
                    err = exp.Message;
                }
            }
        }

        private void dlgMessage_FormClosing(object sender, FormClosingEventArgs e)
        {
            MORZEContact mcnt = m_cnt as MORZEContact;
            if (mcnt != null)
            {
                mcnt.OnRecvMessage -= OnRecvMessage;
            }
        }
        private void PutDisplayMessage(string from, string text, bool isrigth)
        {
            int begin = rb.Text.Length;
            rb.SelectionAlignment = HorizontalAlignment.Left;
            rb.AppendText(string.Format("{0} - {1}", from, DateTime.Now.ToString("dd.MM.yyyy HH:mm")));
            rb.SelectionStart = begin;
            rb.SelectionLength = rb.Text.Length - begin;
            rb.SelectionColor = Color.Blue;

            rb.AppendText("\r\n");
            int begin1 = rb.Text.Length;
            rb.AppendText(text);
            rb.SelectionStart = begin1;
            rb.SelectionLength = text.Length;
            float size = rb.Font.Size + 2;
            rb.SelectionFont = new Font(rb.Font.FontFamily,size);
            rb.AppendText("\r\n");
            rb.AppendText("\r\n");
            if (isrigth == true)
            {
                rb.SelectionStart = begin;
                rb.SelectionLength = rb.Text.Length - begin;
                rb.SelectionAlignment = HorizontalAlignment.Right;
            }
            
                


            rb.SelectionStart = rb.Text.Length;
            rb.ScrollToCaret();
            rb.SelectionLength = 0;
        }

        private void tbMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0xa)
                btnSend_Click(sender, e);
        }
    }
}
