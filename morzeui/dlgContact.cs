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
    public partial class dlgContact : Form
    {
        IMORZEContact m_cnt;
        public dlgContact()
        {
            m_cnt = null;
            
            InitializeComponent();

            Text = "New contact";
            btnRefresh.Enabled = false;
        }

        protected virtual void btnOk_Click(object sender, EventArgs e)
        {
            IMORZEContact cnt=null;
            try
            {
                if (string.IsNullOrEmpty(tbName.Text) == false)
                {
                    if (m_cnt == null)
                    {
                        cnt = new MORZEContact(tbName.Text, tbAddress.Text);
                        m_cnt = cnt;
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                }
                else
                    MessageBox.Show("Name is empty", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public IMORZEContact MORZEContact
        {
            get
            {
                return m_cnt;
            }
            set
            {
                m_cnt = value;
            }
        }
        protected virtual void btnRefresh_Click(object sender, EventArgs e)
        {

        }

        private void dlgContact_Load(object sender, EventArgs e)
        {
            if (m_cnt != null)
            {
                MORZEContact mcnt;
                Text = m_cnt.ToString();
                mcnt = m_cnt as MORZEContact;
                if (mcnt != null)
                {
                    lbConfCount.Text = mcnt.ConfirmedKeysCount.ToString();
                    lbUnconfCount.Text = mcnt.UnconfirmedKeysCount.ToString();
                    tbName.Text = mcnt.ToString();
                    tbAddress.Text = mcnt.GetAddress();
                }
            }
        }
    }

    public class dlgAccount : dlgContact
    {
        SMSAccount m_Acc;
        string m_accName;
        public dlgAccount () : base()
        {
            Text = "New Account";
            btnRefresh.Enabled = true;
            tbAddress.ReadOnly = true;
            
        }
        public dlgAccount(IMORZEAccount acc) : base()
        {
            Text = acc.ToString();
            btnRefresh.Enabled = false;
            tbName.ReadOnly = true;
            tbAddress.ReadOnly = true;
            tbName.Text = acc.ToString();
            tbAddress.Text = acc.GetMyAccount();
            
        }

        protected override void btnOk_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(tbAddress.Text)==false)
            {
                if (m_Acc != null)
                {
                    if (tbName.Text == m_Acc.ToString())
                    {
                        if (m_Acc != null)
                        {
                            m_accName = m_Acc.ToString();
                            m_Acc.SaveKey(null);
                            m_Acc = null;
                        }
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                        MessageBox.Show("Name is invalid. You should regenerate address", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
            }
            else
                MessageBox.Show("Address is empty", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        protected override void btnRefresh_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbName.Text)==false)
            {
                m_Acc = new SMSAccount(tbName.Text);
                tbAddress.Text=m_Acc.GenerateKey();
                
            }
            else
                MessageBox.Show("Name is empty", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public string AccountName
        {
            get
            {
                return m_accName;
            }
        }
    }
}
