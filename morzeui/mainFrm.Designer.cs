namespace morzeui
{
    partial class mainFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainFrm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lvContact = new System.Windows.Forms.ListView();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.delContact = new System.Windows.Forms.ToolStripMenuItem();
            this.propertyContact = new System.Windows.Forms.ToolStripMenuItem();
            this.newContact = new System.Windows.Forms.ToolStripMenuItem();
            this.tsConnect = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.mnDisconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.tsConnect});
            this.statusStrip1.Location = new System.Drawing.Point(0, 594);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(284, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lvContact
            // 
            this.lvContact.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvContact.Location = new System.Drawing.Point(0, 0);
            this.lvContact.Name = "lvContact";
            this.lvContact.Size = new System.Drawing.Size(284, 594);
            this.lvContact.TabIndex = 1;
            this.lvContact.UseCompatibleStateImageBehavior = false;
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.delContact,
            this.propertyContact,
            this.newContact});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(63, 20);
            this.toolStripDropDownButton1.Text = "Контакт";
            // 
            // delContact
            // 
            this.delContact.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.delContact.Name = "delContact";
            this.delContact.Size = new System.Drawing.Size(134, 22);
            this.delContact.Text = "Удалить...";
            // 
            // propertyContact
            // 
            this.propertyContact.Name = "propertyContact";
            this.propertyContact.Size = new System.Drawing.Size(134, 22);
            this.propertyContact.Text = "Свойства...";
            // 
            // newContact
            // 
            this.newContact.Name = "newContact";
            this.newContact.Size = new System.Drawing.Size(134, 22);
            this.newContact.Text = "Новый...";
            this.newContact.Click += new System.EventHandler(this.newContact_Click);
            // 
            // tsConnect
            // 
            this.tsConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsConnect.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnConnect,
            this.mnDisconnect});
            this.tsConnect.Image = global::morzeui.Properties.Resources.red;
            this.tsConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsConnect.Name = "tsConnect";
            this.tsConnect.Size = new System.Drawing.Size(29, 20);
            this.tsConnect.Text = "toolStripDropDownButton2";
            // 
            // mnConnect
            // 
            this.mnConnect.Name = "mnConnect";
            this.mnConnect.Size = new System.Drawing.Size(152, 22);
            this.mnConnect.Text = "В сети";
            this.mnConnect.Click += new System.EventHandler(this.mnConnect_Click);
            // 
            // mnDisconnect
            // 
            this.mnDisconnect.Name = "mnDisconnect";
            this.mnDisconnect.Size = new System.Drawing.Size(152, 22);
            this.mnDisconnect.Text = "Не в сети";
            this.mnDisconnect.Click += new System.EventHandler(this.mnDisconnect_Click);
            // 
            // mainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 616);
            this.Controls.Add(this.lvContact);
            this.Controls.Add(this.statusStrip1);
            this.Name = "mainFrm";
            this.Text = "MORZE.global";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.mainFrm_FormClosing);
            this.Load += new System.EventHandler(this.mainFrm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ListView lvContact;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem delContact;
        private System.Windows.Forms.ToolStripMenuItem propertyContact;
        private System.Windows.Forms.ToolStripMenuItem newContact;
        private System.Windows.Forms.ToolStripDropDownButton tsConnect;
        private System.Windows.Forms.ToolStripMenuItem mnConnect;
        private System.Windows.Forms.ToolStripMenuItem mnDisconnect;
    }
}

