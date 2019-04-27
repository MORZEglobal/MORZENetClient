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
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbMessage.Text) == false)
            {
                string err = null;
                try
                {
                    err = m_net.SendMessage(tbMessage.Text, m_cnt);
                }
                catch (Exception exp)
                {
                    err = exp.Message;
                }
            }
        }

        
    }
}
