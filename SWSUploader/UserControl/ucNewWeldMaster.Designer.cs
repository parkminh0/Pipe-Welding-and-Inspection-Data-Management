namespace SWSUploader
{
    partial class ucNewWeldMaster
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucNewWeldMaster));
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.groupControl3 = new DevExpress.XtraEditors.GroupControl();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.stepProgressBar1 = new DevExpress.XtraEditors.StepProgressBar();
			this.stepProgressBarItem1 = new DevExpress.XtraEditors.StepProgressBarItem();
			this.stepProgressBarItem2 = new DevExpress.XtraEditors.StepProgressBarItem();
			this.stepProgressBarItem3 = new DevExpress.XtraEditors.StepProgressBarItem();
			this.stepProgressBarItem4 = new DevExpress.XtraEditors.StepProgressBarItem();
			this.chkOverwrite = new DevExpress.XtraEditors.CheckEdit();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.progressBarControl1 = new DevExpress.XtraEditors.ProgressBarControl();
			this.btnRunAnalysis = new DevExpress.XtraEditors.SimpleButton();
			this.lblStatus = new DevExpress.XtraEditors.LabelControl();
			this.groupControl4 = new DevExpress.XtraEditors.GroupControl();
			this.mmeAnalysisResult = new DevExpress.XtraEditors.MemoEdit();
			this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
			this.lueProject = new DevExpress.XtraEditors.LookUpEdit();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.lueCompany = new DevExpress.XtraEditors.LookUpEdit();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.lblComment = new DevExpress.XtraEditors.LabelControl();
			this.btnOpenFile = new DevExpress.XtraEditors.SimpleButton();
			this.txtFileName = new DevExpress.XtraEditors.TextEdit();
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.excelDataSource1 = new DevExpress.DataAccess.Excel.ExcelDataSource();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl3)).BeginInit();
			this.groupControl3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.stepProgressBar1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkOverwrite.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.progressBarControl1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl4)).BeginInit();
			this.groupControl4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.mmeAnalysisResult.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
			this.groupControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lueProject.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lueCompany.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtFileName.Properties)).BeginInit();
			this.SuspendLayout();
			// 
			// groupControl1
			// 
			this.groupControl1.Controls.Add(this.groupControl3);
			this.groupControl1.Controls.Add(this.groupControl4);
			this.groupControl1.Controls.Add(this.groupControl2);
			resources.ApplyResources(this.groupControl1, "groupControl1");
			this.groupControl1.Name = "groupControl1";
			// 
			// groupControl3
			// 
			resources.ApplyResources(this.groupControl3, "groupControl3");
			this.groupControl3.AppearanceCaption.Font = ((System.Drawing.Font)(resources.GetObject("groupControl3.AppearanceCaption.Font")));
			this.groupControl3.AppearanceCaption.Options.UseFont = true;
			this.groupControl3.Controls.Add(this.panelControl1);
			this.groupControl3.LookAndFeel.SkinName = "Seven Classic";
			this.groupControl3.LookAndFeel.UseDefaultLookAndFeel = false;
			this.groupControl3.Name = "groupControl3";
			// 
			// panelControl1
			// 
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.panelControl1.Controls.Add(this.stepProgressBar1);
			this.panelControl1.Controls.Add(this.chkOverwrite);
			this.panelControl1.Controls.Add(this.btnCancel);
			this.panelControl1.Controls.Add(this.progressBarControl1);
			this.panelControl1.Controls.Add(this.btnRunAnalysis);
			this.panelControl1.Controls.Add(this.lblStatus);
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			// 
			// stepProgressBar1
			// 
			this.stepProgressBar1.Items.Add(this.stepProgressBarItem1);
			this.stepProgressBar1.Items.Add(this.stepProgressBarItem2);
			this.stepProgressBar1.Items.Add(this.stepProgressBarItem3);
			this.stepProgressBar1.Items.Add(this.stepProgressBarItem4);
			resources.ApplyResources(this.stepProgressBar1, "stepProgressBar1");
			this.stepProgressBar1.Name = "stepProgressBar1";
			this.stepProgressBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.stepProgressBar1.SelectedItemIndex = 0;
			// 
			// stepProgressBarItem1
			// 
			this.stepProgressBarItem1.ContentBlock2.Caption = resources.GetString("stepProgressBarItem1.ContentBlock2.Caption");
			this.stepProgressBarItem1.Name = "stepProgressBarItem1";
			this.stepProgressBarItem1.State = DevExpress.XtraEditors.StepProgressBarItemState.Active;
			// 
			// stepProgressBarItem2
			// 
			this.stepProgressBarItem2.ContentBlock2.Caption = resources.GetString("stepProgressBarItem2.ContentBlock2.Caption");
			this.stepProgressBarItem2.Name = "stepProgressBarItem2";
			// 
			// stepProgressBarItem3
			// 
			this.stepProgressBarItem3.ContentBlock2.Caption = resources.GetString("stepProgressBarItem3.ContentBlock2.Caption");
			this.stepProgressBarItem3.Name = "stepProgressBarItem3";
			// 
			// stepProgressBarItem4
			// 
			this.stepProgressBarItem4.ContentBlock2.Caption = resources.GetString("stepProgressBarItem4.ContentBlock2.Caption");
			this.stepProgressBarItem4.Name = "stepProgressBarItem4";
			// 
			// chkOverwrite
			// 
			resources.ApplyResources(this.chkOverwrite, "chkOverwrite");
			this.chkOverwrite.Name = "chkOverwrite";
			this.chkOverwrite.Properties.Caption = resources.GetString("chkOverwrite.Properties.Caption");
			// 
			// btnCancel
			// 
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.ImageOptions.Image")));
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// progressBarControl1
			// 
			resources.ApplyResources(this.progressBarControl1, "progressBarControl1");
			this.progressBarControl1.Name = "progressBarControl1";
			this.progressBarControl1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
			this.progressBarControl1.Properties.FlowAnimationEnabled = true;
			this.progressBarControl1.Properties.ShowTitle = true;
			// 
			// btnRunAnalysis
			// 
			resources.ApplyResources(this.btnRunAnalysis, "btnRunAnalysis");
			this.btnRunAnalysis.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("btnRunAnalysis.Appearance.Font")));
			this.btnRunAnalysis.Appearance.Options.UseFont = true;
			this.btnRunAnalysis.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnRunAnalysis.ImageOptions.Image")));
			this.btnRunAnalysis.Name = "btnRunAnalysis";
			this.btnRunAnalysis.Click += new System.EventHandler(this.btnRunAnalysis_Click);
			// 
			// lblStatus
			// 
			resources.ApplyResources(this.lblStatus, "lblStatus");
			this.lblStatus.Name = "lblStatus";
			// 
			// groupControl4
			// 
			resources.ApplyResources(this.groupControl4, "groupControl4");
			this.groupControl4.AppearanceCaption.Font = ((System.Drawing.Font)(resources.GetObject("groupControl4.AppearanceCaption.Font")));
			this.groupControl4.AppearanceCaption.Options.UseFont = true;
			this.groupControl4.Controls.Add(this.mmeAnalysisResult);
			this.groupControl4.LookAndFeel.SkinName = "Seven Classic";
			this.groupControl4.LookAndFeel.UseDefaultLookAndFeel = false;
			this.groupControl4.Name = "groupControl4";
			// 
			// mmeAnalysisResult
			// 
			resources.ApplyResources(this.mmeAnalysisResult, "mmeAnalysisResult");
			this.mmeAnalysisResult.Name = "mmeAnalysisResult";
			this.mmeAnalysisResult.Properties.AllowFocused = false;
			this.mmeAnalysisResult.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
			this.mmeAnalysisResult.Properties.ReadOnly = true;
			// 
			// groupControl2
			// 
			resources.ApplyResources(this.groupControl2, "groupControl2");
			this.groupControl2.AppearanceCaption.Font = ((System.Drawing.Font)(resources.GetObject("groupControl2.AppearanceCaption.Font")));
			this.groupControl2.AppearanceCaption.Options.UseFont = true;
			this.groupControl2.Controls.Add(this.lueProject);
			this.groupControl2.Controls.Add(this.labelControl2);
			this.groupControl2.Controls.Add(this.lueCompany);
			this.groupControl2.Controls.Add(this.labelControl1);
			this.groupControl2.Controls.Add(this.lblComment);
			this.groupControl2.Controls.Add(this.btnOpenFile);
			this.groupControl2.Controls.Add(this.txtFileName);
			this.groupControl2.LookAndFeel.SkinName = "Seven Classic";
			this.groupControl2.LookAndFeel.UseDefaultLookAndFeel = false;
			this.groupControl2.Name = "groupControl2";
			// 
			// lueProject
			// 
			resources.ApplyResources(this.lueProject, "lueProject");
			this.lueProject.Name = "lueProject";
			this.lueProject.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
			this.lueProject.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("lueProject.Properties.Buttons"))))});
			this.lueProject.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("lueProject.Properties.Columns"), resources.GetString("lueProject.Properties.Columns1"), ((int)(resources.GetObject("lueProject.Properties.Columns2"))), ((DevExpress.Utils.FormatType)(resources.GetObject("lueProject.Properties.Columns3"))), resources.GetString("lueProject.Properties.Columns4"), ((bool)(resources.GetObject("lueProject.Properties.Columns5"))), ((DevExpress.Utils.HorzAlignment)(resources.GetObject("lueProject.Properties.Columns6"))), ((DevExpress.Data.ColumnSortOrder)(resources.GetObject("lueProject.Properties.Columns7"))), ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("lueProject.Properties.Columns8")))),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("lueProject.Properties.Columns9"), resources.GetString("lueProject.Properties.Columns10"), ((int)(resources.GetObject("lueProject.Properties.Columns11"))), ((DevExpress.Utils.FormatType)(resources.GetObject("lueProject.Properties.Columns12"))), resources.GetString("lueProject.Properties.Columns13"), ((bool)(resources.GetObject("lueProject.Properties.Columns14"))), ((DevExpress.Utils.HorzAlignment)(resources.GetObject("lueProject.Properties.Columns15"))), ((DevExpress.Data.ColumnSortOrder)(resources.GetObject("lueProject.Properties.Columns16"))), ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("lueProject.Properties.Columns17"))))});
			this.lueProject.Properties.PopupWidthMode = DevExpress.XtraEditors.PopupWidthMode.UseEditorWidth;
			// 
			// labelControl2
			// 
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.Name = "labelControl2";
			// 
			// lueCompany
			// 
			resources.ApplyResources(this.lueCompany, "lueCompany");
			this.lueCompany.Name = "lueCompany";
			this.lueCompany.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
			this.lueCompany.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("lueCompany.Properties.Buttons"))))});
			this.lueCompany.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("lueCompany.Properties.Columns"), resources.GetString("lueCompany.Properties.Columns1"), ((int)(resources.GetObject("lueCompany.Properties.Columns2"))), ((DevExpress.Utils.FormatType)(resources.GetObject("lueCompany.Properties.Columns3"))), resources.GetString("lueCompany.Properties.Columns4"), ((bool)(resources.GetObject("lueCompany.Properties.Columns5"))), ((DevExpress.Utils.HorzAlignment)(resources.GetObject("lueCompany.Properties.Columns6"))), ((DevExpress.Data.ColumnSortOrder)(resources.GetObject("lueCompany.Properties.Columns7"))), ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("lueCompany.Properties.Columns8")))),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("lueCompany.Properties.Columns9"), resources.GetString("lueCompany.Properties.Columns10"), ((int)(resources.GetObject("lueCompany.Properties.Columns11"))), ((DevExpress.Utils.FormatType)(resources.GetObject("lueCompany.Properties.Columns12"))), resources.GetString("lueCompany.Properties.Columns13"), ((bool)(resources.GetObject("lueCompany.Properties.Columns14"))), ((DevExpress.Utils.HorzAlignment)(resources.GetObject("lueCompany.Properties.Columns15"))), ((DevExpress.Data.ColumnSortOrder)(resources.GetObject("lueCompany.Properties.Columns16"))), ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("lueCompany.Properties.Columns17"))))});
			this.lueCompany.Properties.PopupWidthMode = DevExpress.XtraEditors.PopupWidthMode.UseEditorWidth;
			this.lueCompany.EditValueChanged += new System.EventHandler(this.lueCompany_EditValueChanged);
			// 
			// labelControl1
			// 
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			// 
			// lblComment
			// 
			this.lblComment.Appearance.ForeColor = System.Drawing.Color.DimGray;
			this.lblComment.Appearance.Options.UseFont = true;
			this.lblComment.Appearance.Options.UseForeColor = true;
			resources.ApplyResources(this.lblComment, "lblComment");
			this.lblComment.Name = "lblComment";
			// 
			// btnOpenFile
			// 
			this.btnOpenFile.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenFile.ImageOptions.Image")));
			resources.ApplyResources(this.btnOpenFile, "btnOpenFile");
			this.btnOpenFile.Name = "btnOpenFile";
			this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
			// 
			// txtFileName
			// 
			resources.ApplyResources(this.txtFileName, "txtFileName");
			this.txtFileName.Name = "txtFileName";
			this.txtFileName.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
			this.txtFileName.Properties.ReadOnly = true;
			// 
			// backgroundWorker1
			// 
			this.backgroundWorker1.WorkerReportsProgress = true;
			this.backgroundWorker1.WorkerSupportsCancellation = true;
			this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
			this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
			// 
			// excelDataSource1
			// 
			this.excelDataSource1.Name = "excelDataSource1";
			// 
			// ucNewWeldMaster
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupControl1);
			this.Name = "ucNewWeldMaster";
			this.Load += new System.EventHandler(this.ucNewWeldMaster_Load);
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.groupControl3)).EndInit();
			this.groupControl3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			this.panelControl1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.stepProgressBar1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkOverwrite.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.progressBarControl1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl4)).EndInit();
			this.groupControl4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.mmeAnalysisResult.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
			this.groupControl2.ResumeLayout(false);
			this.groupControl2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.lueProject.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lueCompany.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtFileName.Properties)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.GroupControl groupControl3;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.CheckEdit chkOverwrite;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.ProgressBarControl progressBarControl1;
        private DevExpress.XtraEditors.SimpleButton btnRunAnalysis;
        private DevExpress.XtraEditors.LabelControl lblStatus;
        private DevExpress.XtraEditors.GroupControl groupControl4;
        private DevExpress.XtraEditors.MemoEdit mmeAnalysisResult;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.LookUpEdit lueCompany;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl lblComment;
        private DevExpress.XtraEditors.SimpleButton btnOpenFile;
        private DevExpress.XtraEditors.TextEdit txtFileName;
        private DevExpress.XtraEditors.StepProgressBar stepProgressBar1;
        private DevExpress.XtraEditors.StepProgressBarItem stepProgressBarItem1;
        private DevExpress.XtraEditors.StepProgressBarItem stepProgressBarItem2;
        private DevExpress.XtraEditors.StepProgressBarItem stepProgressBarItem3;
        private DevExpress.XtraEditors.StepProgressBarItem stepProgressBarItem4;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private DevExpress.DataAccess.Excel.ExcelDataSource excelDataSource1;
        private DevExpress.XtraEditors.LookUpEdit lueProject;
        private DevExpress.XtraEditors.LabelControl labelControl2;
    }
}
