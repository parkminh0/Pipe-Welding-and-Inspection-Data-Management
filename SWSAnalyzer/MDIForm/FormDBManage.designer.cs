namespace SWSAnalyzer
{
    partial class FormDBManage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDBManage));
            this.rdoMethd = new DevExpress.XtraEditors.RadioGroup();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.btnCancel2 = new DevExpress.XtraEditors.SimpleButton();
            this.bteOrgDBPath = new DevExpress.XtraEditors.ButtonEdit();
            this.btnApplyOrgPath = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnRunBackup = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.lblDescr = new DevExpress.XtraEditors.LabelControl();
            this.bteBackupPath = new DevExpress.XtraEditors.ButtonEdit();
            this.separatorControl1 = new DevExpress.XtraEditors.SeparatorControl();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.rdoMethd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bteOrgDBPath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bteBackupPath.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // rdoMethd
            // 
            this.rdoMethd.Location = new System.Drawing.Point(70, 12);
            this.rdoMethd.Name = "rdoMethd";
            this.rdoMethd.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            this.rdoMethd.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "DB백업(복사)"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "백업 및 초기화"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "DB 초기화"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "DB 최적화"),
            new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "원본DB 위치설정")});
            this.rdoMethd.Properties.ItemsLayout = DevExpress.XtraEditors.RadioGroupItemsLayout.Flow;
            this.rdoMethd.Size = new System.Drawing.Size(552, 28);
            this.rdoMethd.TabIndex = 0;
            this.rdoMethd.SelectedIndexChanged += new System.EventHandler(this.rdoMethd_SelectedIndexChanged);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.groupControl2);
            this.panelControl1.Controls.Add(this.labelControl2);
            this.panelControl1.Controls.Add(this.groupControl1);
            this.panelControl1.Controls.Add(this.rdoMethd);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(634, 396);
            this.panelControl1.TabIndex = 1;
            // 
            // groupControl2
            // 
            this.groupControl2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupControl2.Controls.Add(this.btnCancel2);
            this.groupControl2.Controls.Add(this.bteOrgDBPath);
            this.groupControl2.Controls.Add(this.btnApplyOrgPath);
            this.groupControl2.Enabled = false;
            this.groupControl2.Location = new System.Drawing.Point(12, 271);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(610, 113);
            this.groupControl2.TabIndex = 5;
            this.groupControl2.Text = "Data source location settings";
            // 
            // btnCancel2
            // 
            this.btnCancel2.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.btnCancel2.Appearance.Options.UseFont = true;
            this.btnCancel2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel2.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel2.Image")));
            this.btnCancel2.Location = new System.Drawing.Point(350, 70);
            this.btnCancel2.Margin = new System.Windows.Forms.Padding(3, 3, 30, 7);
            this.btnCancel2.Name = "btnCancel2";
            this.btnCancel2.Size = new System.Drawing.Size(87, 34);
            this.btnCancel2.TabIndex = 6;
            this.btnCancel2.Text = "나가기";
            this.btnCancel2.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // bteOrgDBPath
            // 
            this.bteOrgDBPath.EditValue = "";
            this.bteOrgDBPath.Location = new System.Drawing.Point(5, 28);
            this.bteOrgDBPath.Margin = new System.Windows.Forms.Padding(3, 3, 10, 6);
            this.bteOrgDBPath.Name = "bteOrgDBPath";
            this.bteOrgDBPath.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
            this.bteOrgDBPath.Properties.Appearance.Options.UseFont = true;
            this.bteOrgDBPath.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            this.bteOrgDBPath.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.bteOrgDBPath.Size = new System.Drawing.Size(593, 24);
            this.bteOrgDBPath.TabIndex = 2;
            this.bteOrgDBPath.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bteOrgDBPath_ButtonClick);
            // 
            // btnApplyOrgPath
            // 
            this.btnApplyOrgPath.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.btnApplyOrgPath.Appearance.Options.UseFont = true;
            this.btnApplyOrgPath.Image = ((System.Drawing.Image)(resources.GetObject("btnApplyOrgPath.Image")));
            this.btnApplyOrgPath.Location = new System.Drawing.Point(152, 70);
            this.btnApplyOrgPath.Margin = new System.Windows.Forms.Padding(3, 3, 30, 7);
            this.btnApplyOrgPath.Name = "btnApplyOrgPath";
            this.btnApplyOrgPath.Size = new System.Drawing.Size(144, 34);
            this.btnApplyOrgPath.TabIndex = 7;
            this.btnApplyOrgPath.Text = "원본위치 저장";
            this.btnApplyOrgPath.Click += new System.EventHandler(this.btnApplyOrgPath_Click);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(12, 17);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(50, 14);
            this.labelControl2.TabIndex = 4;
            this.labelControl2.Text = "Selection";
            // 
            // groupControl1
            // 
            this.groupControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupControl1.Controls.Add(this.btnCancel);
            this.groupControl1.Controls.Add(this.btnRunBackup);
            this.groupControl1.Controls.Add(this.labelControl1);
            this.groupControl1.Controls.Add(this.lblDescr);
            this.groupControl1.Controls.Add(this.bteBackupPath);
            this.groupControl1.Controls.Add(this.separatorControl1);
            this.groupControl1.Location = new System.Drawing.Point(12, 59);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(610, 206);
            this.groupControl1.TabIndex = 3;
            this.groupControl1.Text = "Create a copy - Please select a folder to save the copy.";
            // 
            // btnCancel
            // 
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.Location = new System.Drawing.Point(350, 151);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 3, 30, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 34);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "나가기";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRunBackup
            // 
            this.btnRunBackup.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.btnRunBackup.Appearance.Options.UseFont = true;
            this.btnRunBackup.Image = ((System.Drawing.Image)(resources.GetObject("btnRunBackup.Image")));
            this.btnRunBackup.Location = new System.Drawing.Point(152, 151);
            this.btnRunBackup.Margin = new System.Windows.Forms.Padding(3, 3, 30, 7);
            this.btnRunBackup.Name = "btnRunBackup";
            this.btnRunBackup.Size = new System.Drawing.Size(144, 34);
            this.btnRunBackup.TabIndex = 7;
            this.btnRunBackup.Text = "DB백업(복사)";
            this.btnRunBackup.Click += new System.EventHandler(this.btnRunBackup_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(12, 40);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(93, 14);
            this.labelControl1.TabIndex = 5;
            this.labelControl1.Text = "Backup Location:";
            // 
            // lblDescr
            // 
            this.lblDescr.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.lblDescr.Appearance.Options.UseForeColor = true;
            this.lblDescr.Location = new System.Drawing.Point(62, 119);
            this.lblDescr.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
            this.lblDescr.Name = "lblDescr";
            this.lblDescr.Size = new System.Drawing.Size(241, 14);
            this.lblDescr.TabIndex = 1;
            this.lblDescr.Text = "현재 DB를 별도의 장소에 백업(복사) 합니다.";
            // 
            // bteBackupPath
            // 
            this.bteBackupPath.EditValue = "";
            this.bteBackupPath.Location = new System.Drawing.Point(5, 60);
            this.bteBackupPath.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.bteBackupPath.Name = "bteBackupPath";
            this.bteBackupPath.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
            this.bteBackupPath.Properties.Appearance.Options.UseFont = true;
            this.bteBackupPath.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
            this.bteBackupPath.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.bteBackupPath.Size = new System.Drawing.Size(593, 24);
            this.bteBackupPath.TabIndex = 2;
            this.bteBackupPath.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bteBackupPath_ButtonClick);
            // 
            // separatorControl1
            // 
            this.separatorControl1.Location = new System.Drawing.Point(5, 90);
            this.separatorControl1.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.separatorControl1.Name = "separatorControl1";
            this.separatorControl1.Size = new System.Drawing.Size(593, 23);
            this.separatorControl1.TabIndex = 4;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // FormDBManage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(634, 396);
            this.Controls.Add(this.panelControl1);
            this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Shadow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormDBManage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DB Manage";
            this.Load += new System.EventHandler(this.FormDBManage_Load);
            this.Shown += new System.EventHandler(this.FormDBManage_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.rdoMethd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bteOrgDBPath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.groupControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bteBackupPath.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl lblDescr;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.ButtonEdit bteBackupPath;
        private DevExpress.XtraEditors.SeparatorControl separatorControl1;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnRunBackup;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.SimpleButton btnCancel2;
        private DevExpress.XtraEditors.ButtonEdit bteOrgDBPath;
        private DevExpress.XtraEditors.SimpleButton btnApplyOrgPath;
        public DevExpress.XtraEditors.RadioGroup rdoMethd;
    }
}