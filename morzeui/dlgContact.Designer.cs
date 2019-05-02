namespace morzeui
{
    partial class dlgContact
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.gb = new System.Windows.Forms.GroupBox();
            this.tbAddress = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.gbExt = new System.Windows.Forms.GroupBox();
            this.lbUnconfCount = new System.Windows.Forms.Label();
            this.lbConfCount = new System.Windows.Forms.Label();
            this.lbun = new System.Windows.Forms.Label();
            this.lbcfrm = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.gb.SuspendLayout();
            this.gbExt.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Display name";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(139, 10);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(340, 20);
            this.tbName.TabIndex = 1;
            // 
            // gb
            // 
            this.gb.Controls.Add(this.tbAddress);
            this.gb.Location = new System.Drawing.Point(13, 51);
            this.gb.Name = "gb";
            this.gb.Size = new System.Drawing.Size(436, 182);
            this.gb.TabIndex = 2;
            this.gb.TabStop = false;
            this.gb.Text = "MORZE address";
            // 
            // tbAddress
            // 
            this.tbAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbAddress.Location = new System.Drawing.Point(3, 16);
            this.tbAddress.MaxLength = 512;
            this.tbAddress.Multiline = true;
            this.tbAddress.Name = "tbAddress";
            this.tbAddress.Size = new System.Drawing.Size(430, 163);
            this.tbAddress.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(401, 254);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(308, 254);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = global::morzeui.Properties.Resources.refresh;
            this.btnRefresh.Location = new System.Drawing.Point(456, 67);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(23, 23);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // gbExt
            // 
            this.gbExt.Controls.Add(this.btnClear);
            this.gbExt.Controls.Add(this.lbUnconfCount);
            this.gbExt.Controls.Add(this.lbConfCount);
            this.gbExt.Controls.Add(this.lbun);
            this.gbExt.Controls.Add(this.lbcfrm);
            this.gbExt.Location = new System.Drawing.Point(16, 240);
            this.gbExt.Name = "gbExt";
            this.gbExt.Size = new System.Drawing.Size(267, 62);
            this.gbExt.TabIndex = 6;
            this.gbExt.TabStop = false;
            this.gbExt.Text = "Session keys";
            // 
            // lbUnconfCount
            // 
            this.lbUnconfCount.AutoSize = true;
            this.lbUnconfCount.Location = new System.Drawing.Point(99, 39);
            this.lbUnconfCount.Name = "lbUnconfCount";
            this.lbUnconfCount.Size = new System.Drawing.Size(13, 13);
            this.lbUnconfCount.TabIndex = 3;
            this.lbUnconfCount.Text = "0";
            // 
            // lbConfCount
            // 
            this.lbConfCount.AutoSize = true;
            this.lbConfCount.Location = new System.Drawing.Point(99, 19);
            this.lbConfCount.Name = "lbConfCount";
            this.lbConfCount.Size = new System.Drawing.Size(13, 13);
            this.lbConfCount.TabIndex = 2;
            this.lbConfCount.Text = "0";
            // 
            // lbun
            // 
            this.lbun.AutoSize = true;
            this.lbun.Location = new System.Drawing.Point(7, 39);
            this.lbun.Name = "lbun";
            this.lbun.Size = new System.Drawing.Size(65, 13);
            this.lbun.TabIndex = 1;
            this.lbun.Text = "unconfirmed";
            // 
            // lbcfrm
            // 
            this.lbcfrm.AutoSize = true;
            this.lbcfrm.Location = new System.Drawing.Point(6, 19);
            this.lbcfrm.Name = "lbcfrm";
            this.lbcfrm.Size = new System.Drawing.Size(53, 13);
            this.lbcfrm.TabIndex = 0;
            this.lbcfrm.Text = "confirmed";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(183, 14);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 4;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // dlgContact
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(511, 304);
            this.Controls.Add(this.gbExt);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gb);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "dlgContact";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "dlgContact";
            this.Load += new System.EventHandler(this.dlgContact_Load);
            this.gb.ResumeLayout(false);
            this.gb.PerformLayout();
            this.gbExt.ResumeLayout(false);
            this.gbExt.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        protected System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.GroupBox gb;
        protected System.Windows.Forms.TextBox tbAddress;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        protected System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.GroupBox gbExt;
        private System.Windows.Forms.Label lbUnconfCount;
        private System.Windows.Forms.Label lbConfCount;
        private System.Windows.Forms.Label lbun;
        private System.Windows.Forms.Label lbcfrm;
        private System.Windows.Forms.Button btnClear;
    }
}