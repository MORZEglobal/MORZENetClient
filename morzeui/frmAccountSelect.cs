using SMS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace morzeui
{
    public partial class frmAccountSelect : Form
    {
        private string m_selectedAccount;
        public frmAccountSelect()
        {
            InitializeComponent();
        }

        private void frmAccountSelect_Load(object sender, EventArgs e)
        {
            const string p1 = "SMS-";
            const string p2 = ".key";
            DirectoryInfo di = new DirectoryInfo(SMSFileTools.SMSPath);
            FileInfo[] fis;
            try
            {
                fis = di.GetFiles(string.Format("{0}*{1}", p1, p2));

                if (fis.Length == 1)
                {
                    string name = fis[0].Name.Substring(p1.Length, fis[0].Name.Length - p1.Length);
                    m_selectedAccount = name.Substring(0, name.Length - p2.Length);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    foreach (FileInfo i in fis)
                    {
                        string name = i.Name.Substring(p1.Length, i.Name.Length - p1.Length);
                        name = name.Substring(0, name.Length - p2.Length);
                        lv.Items.Add(name);
                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                m_selectedAccount = null;
                Close();
            }
        }
        public string SelectedAccount
        {
            get
            {
                return m_selectedAccount;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            if (lv.SelectedItems.Count == 1)
            {
                m_selectedAccount = lv.SelectedItems[0].Text;
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
