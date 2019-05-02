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
        public dlgMessage(IMORZEContact cnt, IMORZEAccount acc, SMSNet net)
        {
            m_cnt = cnt;
            m_acc = acc;
            m_net = net;
            InitializeComponent();
        }

        private void dlgMessage_Load(object sender, EventArgs e)
        {
            Text = string.Format("{0} - {1}", m_acc.ToString(), m_cnt.ToString());
            MORZEContact mcnt = m_cnt as MORZEContact;
            if (mcnt!=null)
            {
                mcnt.OnRecvMessage += OnRecvMessage;
            }
        }

        private void OnRecvMessage(IMORZEContact sender, string message, uint param)
        {
            PutDisplayMessage(sender.ToString(), message);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbMessage.Text) == false)
            {
                string err = null;
                try
                {
                    err = m_net.SendMessage(tbMessage.Text, m_cnt);
                    if (string.IsNullOrEmpty(err) == false)
                    {
                        PutDisplayMessage(m_acc.ToString(), tbMessage.Text);
                        tbMessage.Text = string.Empty;
                    }
                    else
                        MessageBox.Show(Text, err, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                //mcnt.OnRecvMessage -= OnRecvMessage;
            }
        }
        private void PutDisplayMessage(string from, string text)
        {
            rb.AppendText(string.Format("{0} - {1}", from, DateTime.Now.ToString("DD.MM.YYYY:MM:HH")));
            rb.AppendText(text);
            rb.AppendText("\r\n");
        }

        private void tbMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0xa)
                btnSend_Click(sender, e);
        }
    }
}
