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

            Text = "Новый контакт";
        }

        private void btnOk_Click(object sender, EventArgs e)
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
                    MessageBox.Show("Имя пустое", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        }
    }
}
