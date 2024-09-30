namespace SWSAnalyzer
{
    partial class FormLogin
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLogin));
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
			this.lblConnStatus = new DevExpress.XtraEditors.LabelControl();
			this.cboLanguage = new DevExpress.XtraEditors.ComboBoxEdit();
			this.btnExit = new DevExpress.XtraEditors.SimpleButton();
			this.btnLogIn = new DevExpress.XtraEditors.SimpleButton();
			this.chkSave = new DevExpress.XtraEditors.CheckEdit();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.txtPassword = new DevExpress.XtraEditors.TextEdit();
			this.txtUserID = new DevExpress.XtraEditors.TextEdit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cboLanguage.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkSave.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtUserID.Properties)).BeginInit();
			this.SuspendLayout();
			// 
			// panelControl1
			// 
			this.panelControl1.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.panelControl1.Appearance.Options.UseBackColor = true;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.panelControl1.ContentImage = global::SWSAnalyzer.Properties.Resources.Keychain;
			this.panelControl1.ContentImageAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.panelControl1.Controls.Add(this.pictureEdit1);
			this.panelControl1.Controls.Add(this.lblConnStatus);
			this.panelControl1.Controls.Add(this.cboLanguage);
			this.panelControl1.Controls.Add(this.btnExit);
			this.panelControl1.Controls.Add(this.btnLogIn);
			this.panelControl1.Controls.Add(this.chkSave);
			this.panelControl1.Controls.Add(this.labelControl3);
			this.panelControl1.Controls.Add(this.labelControl2);
			this.panelControl1.Controls.Add(this.labelControl1);
			this.panelControl1.Controls.Add(this.txtPassword);
			this.panelControl1.Controls.Add(this.txtUserID);
			this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelControl1.Location = new System.Drawing.Point(0, 0);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Size = new System.Drawing.Size(464, 196);
			this.panelControl1.TabIndex = 4;
			// 
			// pictureEdit1
			// 
			this.pictureEdit1.EditValue = global::SWSAnalyzer.Properties.Resources.UploaderWeld;
			this.pictureEdit1.Location = new System.Drawing.Point(3, 3);
			this.pictureEdit1.Margin = new System.Windows.Forms.Padding(1);
			this.pictureEdit1.Name = "pictureEdit1";
			this.pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
			this.pictureEdit1.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
			this.pictureEdit1.Size = new System.Drawing.Size(78, 43);
			this.pictureEdit1.TabIndex = 19;
			// 
			// lblConnStatus
			// 
			this.lblConnStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblConnStatus.Appearance.Font = new System.Drawing.Font("Arial", 10F);
			this.lblConnStatus.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.lblConnStatus.Appearance.Options.UseFont = true;
			this.lblConnStatus.Appearance.Options.UseForeColor = true;
			this.lblConnStatus.Location = new System.Drawing.Point(169, 160);
			this.lblConnStatus.Name = "lblConnStatus";
			this.lblConnStatus.Size = new System.Drawing.Size(51, 16);
			this.lblConnStatus.TabIndex = 17;
			this.lblConnStatus.Text = "Login.....";
			this.lblConnStatus.Visible = false;
			// 
			// cboLanguage
			// 
			this.cboLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cboLanguage.EditValue = "Korean";
			this.cboLanguage.Location = new System.Drawing.Point(275, 12);
			this.cboLanguage.Name = "cboLanguage";
			this.cboLanguage.Properties.Appearance.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
			this.cboLanguage.Properties.Appearance.Options.UseFont = true;
			this.cboLanguage.Properties.Appearance.Options.UseTextOptions = true;
			this.cboLanguage.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.cboLanguage.Properties.AutoHeight = false;
			this.cboLanguage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cboLanguage.Properties.Items.AddRange(new object[] {
            "Korean",
            "English"});
			this.cboLanguage.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cboLanguage.Size = new System.Drawing.Size(177, 34);
			this.cboLanguage.TabIndex = 2;
			// 
			// btnExit
			// 
			this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnExit.Appearance.Font = new System.Drawing.Font("Arial", 12F);
			this.btnExit.Appearance.Options.UseFont = true;
			this.btnExit.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.ImageOptions.Image")));
			this.btnExit.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
			this.btnExit.ImageOptions.ImageToTextIndent = 8;
			this.btnExit.Location = new System.Drawing.Point(352, 146);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(100, 38);
			this.btnExit.TabIndex = 1;
			this.btnExit.Text = "Exit";
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// btnLogIn
			// 
			this.btnLogIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnLogIn.Appearance.Font = new System.Drawing.Font("Arial", 12F);
			this.btnLogIn.Appearance.Options.UseFont = true;
			this.btnLogIn.Enabled = false;
			this.btnLogIn.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnLogIn.ImageOptions.Image")));
			this.btnLogIn.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
			this.btnLogIn.ImageOptions.ImageToTextIndent = 8;
			this.btnLogIn.Location = new System.Drawing.Point(226, 146);
			this.btnLogIn.Name = "btnLogIn";
			this.btnLogIn.Size = new System.Drawing.Size(120, 38);
			this.btnLogIn.TabIndex = 0;
			this.btnLogIn.Text = "Login";
			this.btnLogIn.Click += new System.EventHandler(this.btnLogIn_Click);
			// 
			// chkSave
			// 
			this.chkSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.chkSave.Location = new System.Drawing.Point(398, 57);
			this.chkSave.Name = "chkSave";
			this.chkSave.Properties.Appearance.Font = new System.Drawing.Font("Arial", 9F);
			this.chkSave.Properties.Appearance.Options.UseFont = true;
			this.chkSave.Properties.Caption = "Save";
			this.chkSave.Size = new System.Drawing.Size(54, 22);
			this.chkSave.TabIndex = 3;
			// 
			// labelControl3
			// 
			this.labelControl3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelControl3.Appearance.Font = new System.Drawing.Font("Arial", 10F);
			this.labelControl3.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.labelControl3.Appearance.Options.UseFont = true;
			this.labelControl3.Appearance.Options.UseForeColor = true;
			this.labelControl3.Location = new System.Drawing.Point(213, 21);
			this.labelControl3.Name = "labelControl3";
			this.labelControl3.Size = new System.Drawing.Size(56, 16);
			this.labelControl3.TabIndex = 1;
			this.labelControl3.Text = "Language";
			// 
			// labelControl2
			// 
			this.labelControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelControl2.Appearance.Font = new System.Drawing.Font("Arial", 10F);
			this.labelControl2.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.labelControl2.Appearance.Options.UseFont = true;
			this.labelControl2.Appearance.Options.UseForeColor = true;
			this.labelControl2.Location = new System.Drawing.Point(226, 61);
			this.labelControl2.Name = "labelControl2";
			this.labelControl2.Size = new System.Drawing.Size(43, 16);
			this.labelControl2.TabIndex = 1;
			this.labelControl2.Text = "User ID";
			// 
			// labelControl1
			// 
			this.labelControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelControl1.Appearance.Font = new System.Drawing.Font("Arial", 10F);
			this.labelControl1.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
			this.labelControl1.Appearance.Options.UseFont = true;
			this.labelControl1.Appearance.Options.UseForeColor = true;
			this.labelControl1.Location = new System.Drawing.Point(212, 99);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(57, 16);
			this.labelControl1.TabIndex = 1;
			this.labelControl1.Text = "User P/W";
			// 
			// txtPassword
			// 
			this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtPassword.EditValue = "";
			this.txtPassword.Location = new System.Drawing.Point(275, 90);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.Properties.Appearance.Font = new System.Drawing.Font("Arial", 9F);
			this.txtPassword.Properties.Appearance.Options.UseFont = true;
			this.txtPassword.Properties.Appearance.Options.UseTextOptions = true;
			this.txtPassword.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.txtPassword.Properties.AutoHeight = false;
			this.txtPassword.Properties.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(177, 32);
			this.txtPassword.TabIndex = 5;
			this.txtPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPassword_KeyPress);
			// 
			// txtUserID
			// 
			this.txtUserID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtUserID.Location = new System.Drawing.Point(275, 52);
			this.txtUserID.Name = "txtUserID";
			this.txtUserID.Properties.Appearance.Font = new System.Drawing.Font("Arial", 9F);
			this.txtUserID.Properties.Appearance.Options.UseFont = true;
			this.txtUserID.Properties.Appearance.Options.UseTextOptions = true;
			this.txtUserID.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.txtUserID.Properties.AutoHeight = false;
			this.txtUserID.Size = new System.Drawing.Size(117, 32);
			this.txtUserID.TabIndex = 4;
			// 
			// FormLogin
			// 
			this.Appearance.Options.UseFont = true;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(464, 196);
			this.Controls.Add(this.panelControl1);
			this.Font = new System.Drawing.Font("Arial", 9F);
			this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("FormLogin.IconOptions.Icon")));
			this.LookAndFeel.SkinName = "WXI";
			this.LookAndFeel.UseDefaultLookAndFeel = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormLogin";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Login";
			this.Load += new System.EventHandler(this.FormLogin_Load);
			this.Shown += new System.EventHandler(this.FormLogin_Shown);
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			this.panelControl1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cboLanguage.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkSave.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtUserID.Properties)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.TextEdit txtPassword;
        private DevExpress.XtraEditors.TextEdit txtUserID;
        private DevExpress.XtraEditors.SimpleButton btnExit;
        private DevExpress.XtraEditors.SimpleButton btnLogIn;
        private DevExpress.XtraEditors.ComboBoxEdit cboLanguage;
        private DevExpress.XtraEditors.CheckEdit chkSave;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl lblConnStatus;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.PictureEdit pictureEdit1;
    }
}