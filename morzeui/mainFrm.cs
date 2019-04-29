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
    public partial class mainFrm : Form
    {
        AddressBook m_book;
        SMSNet m_net;
        SMSAccount m_account;
        string m_wndTitle;
        List<dlgMessage> m_dlgMsgs;
        public mainFrm()
        {
            InitializeComponent();
            m_wndTitle = Text;
        }

        private void mainFrm_Load(object sender, EventArgs e)
        {
            
            m_book = new AddressBook();
            if (m_book != null)
            {
                foreach (MORZEContact contact in m_book.Contacts)
                {
                    AddContactToList(contact);
                }
            }
            LoadAccount();
            m_book.OnNewAccountRecive += book_OnNewAccountRecive;
        }

        private void book_OnNewAccountRecive(IMORZEContact contact)
        {
            if (contact != null)
            {
                Invoke(new Action(() =>
                {
                    AddContactToList(contact);
                }));
            }
        }

        private void newContact_Click(object sender, EventArgs e)
        {
            dlgContact dlg=new dlgContact();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                IMORZEContact contact = dlg.MORZEContact;
                string err = m_book.AddContact(contact);
                if (string.IsNullOrEmpty(err) == true)
                {
                    m_book.Save();
                    AddContactToList(contact);
                }
                else
                {
                    MessageBox.Show(err, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                    
            }
            dlg.Dispose();
        }

        private void mainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (m_book != null)
                m_book.Save();

            foreach (dlgMessage dlg in m_dlgMsgs)
                dlg.Dispose();
        }
        private void LoadAccount()
        {
            frmAccountSelect acsel = new frmAccountSelect();
            if (acsel.ShowDialog() == DialogResult.OK)
            {
                
                try
                {
                    m_account = new SMSAccount(acsel.SelectedAccount);
                    string err;
                    err=m_account.LoadKey(null);
                    if (string.IsNullOrEmpty(err) == false)
                        throw new Exception(err);
                    Text=string.Format("{0} - {1}", m_wndTitle, acsel.SelectedAccount);
                    Connect();
                }
                catch(Exception exp)
                {
                    MessageBox.Show(exp.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    m_account = null;
                }
            }
            acsel.Dispose();

        }
        private void Connect()
        {
            if (m_net==null)
            {
                m_net = new SMSNet(m_account);
                m_net.OnConnected += OnConnected;
                m_net.OnDisconnected += OnDisconnected;
                tsConnect.Image = Properties.Resources.yellow;

                Cursor = Cursors.WaitCursor;
                m_net.Connect("127.0.0.1", 5555);
            }
        }
        private void Disconnect()
        {
            if (m_net != null)
            {
                m_net.Dispose();
                m_net = null;
                tsConnect.Image = Properties.Resources.red;
            }
        }
        private void OnDisconnected(string msg)
        {

            Invoke(new Action(() =>
            {
                tsConnect.Image = Properties.Resources.red;
                MessageBox.Show(msg, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor = Cursors.Default;
            }));

            m_net.Dispose();
            m_net = null;
        }
        private void OnConnected()
        {
            Invoke(new Action(() =>
                {
                    tsConnect.Image = Properties.Resources.Green;
                    Cursor = Cursors.Default;
                    m_account.AddressBook = m_book;
            }));
        }

        private void mnConnect_Click(object sender, EventArgs e)
        {
            Connect();
        }

        private void mnDisconnect_Click(object sender, EventArgs e)
        {
            Disconnect();
        }
        private void AddContactToList(IMORZEContact contact)
        {
            ListViewItem lvi = new ListViewItem(contact.ToString());
            lvi.Tag = contact;
            lvi.ToolTipText = contact.GetAddress();
            lvContact.Items.Add(lvi);

            MORZEContact mrz = contact as MORZEContact;
            mrz.OnRecvNotifyAcceptecExtKey += OnRecvNotifyAcceptecExtKey;
            mrz.OnRecvMessage += OnRecvMessage;
        }

        private void lvContact_DoubleClick(object sender, EventArgs e)
        {
            if (lvContact.SelectedItems.Count == 1)
            {
                IMORZEContact cnt = lvContact.SelectedItems[0].Tag as IMORZEContact;


                dlgMessage msg = null;
                if (m_dlgMsgs != null)
                {
                    for (int i = 0; i < m_dlgMsgs.Count && msg == null; i++)
                    {
                        if (m_dlgMsgs[i].Tag == cnt)
                            msg = m_dlgMsgs[i];
                    }
                }
                else
                    m_dlgMsgs = new List<dlgMessage>();

                if (msg == null)
                {
                    msg=new dlgMessage(cnt, m_account, m_net);
                    msg.Tag = cnt;
                    msg.FormClosed += Msg_FormClosed;
                    m_dlgMsgs.Add(msg);
                    
                }
                if (msg != null)
                {
                    if (msg.Visible == false)
                        msg.Show();
                    msg.Focus();
                    
                }
            }
        }

        private void Msg_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_dlgMsgs != null)
            {
                dlgMessage dlg = sender as dlgMessage;
                m_dlgMsgs.Remove(dlg);
                dlg.Dispose();
            }

        }

        private void параметрыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvContact.SelectedItems.Count == 1)
            {
                dlgContact dlg = new dlgContact();
                dlg.MORZEContact = lvContact.SelectedItems[0].Tag as IMORZEContact;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    IMORZEContact contact = dlg.MORZEContact;
                    string err = m_book.UpdateContact(contact);
                    if (string.IsNullOrEmpty(err) == true)
                    {
                        m_book.Save();
                        lvContact.SelectedItems[0].Text = contact.ToString();
                    }
                    else
                    {
                        MessageBox.Show(err, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                dlg.Dispose();
            }
        }
        private void OnRecvMessage(IMORZEContact sender, string message)
        {
            
        }

        private void OnRecvNotifyAcceptecExtKey(IMORZEContact sender)
        {
            List<MORZEMessage> msgs;
            msgs=m_account.GetUnsendedNewMessages(sender);
            if (msgs!=null)
            {
                string err;
                foreach (MORZEMessage msg in msgs)
                {
                    err = m_net.SendMessage(msg, sender);
                    if (string.IsNullOrEmpty(err) == false)
                        msg.Status = MORZEMessageStatus.sended;
                }
            }
        }
    }
}
