namespace SWSAnalyzer
{
    partial class FormPopProjectRelation
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPopProjectRelation));
			this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.btnExportExcel = new DevExpress.XtraEditors.SimpleButton();
			this.btnClose = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
			this.grdProject = new DevExpress.XtraGrid.GridControl();
			this.grdViewProject = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel1)).BeginInit();
			this.splitContainerControl1.Panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel2)).BeginInit();
			this.splitContainerControl1.Panel2.SuspendLayout();
			this.splitContainerControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
			this.groupControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.grdProject)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grdViewProject)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			this.SuspendLayout();
			// 
			// splitContainerControl1
			// 
			resources.ApplyResources(this.splitContainerControl1, "splitContainerControl1");
			this.splitContainerControl1.Horizontal = false;
			this.splitContainerControl1.IsSplitterFixed = true;
			this.splitContainerControl1.Name = "splitContainerControl1";
			// 
			// splitContainerControl1.Panel1
			// 
			this.splitContainerControl1.Panel1.Controls.Add(this.panelControl1);
			resources.ApplyResources(this.splitContainerControl1.Panel1, "splitContainerControl1.Panel1");
			// 
			// splitContainerControl1.Panel2
			// 
			this.splitContainerControl1.Panel2.Controls.Add(this.layoutControl1);
			resources.ApplyResources(this.splitContainerControl1.Panel2, "splitContainerControl1.Panel2");
			this.splitContainerControl1.SplitterPosition = 44;
			// 
			// panelControl1
			// 
			this.panelControl1.Controls.Add(this.btnExportExcel);
			this.panelControl1.Controls.Add(this.btnClose);
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			// 
			// btnExportExcel
			// 
			resources.ApplyResources(this.btnExportExcel, "btnExportExcel");
			this.btnExportExcel.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("btnExportExcel.Appearance.Font")));
			this.btnExportExcel.Appearance.Options.UseFont = true;
			this.btnExportExcel.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
			this.btnExportExcel.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnExportExcel.ImageOptions.Image")));
			this.btnExportExcel.Name = "btnExportExcel";
			this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
			// 
			// btnClose
			// 
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("btnClose.Appearance.Font")));
			this.btnClose.Appearance.Options.UseFont = true;
			this.btnClose.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
			this.btnClose.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.ImageOptions.Image")));
			this.btnClose.Name = "btnClose";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// layoutControl1
			// 
			this.layoutControl1.Controls.Add(this.groupControl2);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.Root = this.Root;
			// 
			// groupControl2
			// 
			this.groupControl2.AppearanceCaption.Font = ((System.Drawing.Font)(resources.GetObject("groupControl2.AppearanceCaption.Font")));
			this.groupControl2.AppearanceCaption.Options.UseFont = true;
			this.groupControl2.Controls.Add(this.grdProject);
			this.groupControl2.CustomHeaderButtonsLocation = DevExpress.Utils.GroupElementLocation.AfterText;
			resources.ApplyResources(this.groupControl2, "groupControl2");
			this.groupControl2.Name = "groupControl2";
			// 
			// grdProject
			// 
			resources.ApplyResources(this.grdProject, "grdProject");
			this.grdProject.MainView = this.grdViewProject;
			this.grdProject.Name = "grdProject";
			this.grdProject.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdViewProject});
			// 
			// grdViewProject
			// 
			this.grdViewProject.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn3,
            this.gridColumn2,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8});
			this.grdViewProject.GridControl = this.grdProject;
			this.grdViewProject.Name = "grdViewProject";
			this.grdViewProject.OptionsBehavior.Editable = false;
			this.grdViewProject.OptionsView.AllowCellMerge = true;
			this.grdViewProject.OptionsView.ShowAutoFilterRow = true;
			this.grdViewProject.OptionsView.ShowFooter = true;
			this.grdViewProject.OptionsView.ShowGroupPanel = false;
			this.grdViewProject.DoubleClick += new System.EventHandler(this.grdViewProject_DoubleClick);
			// 
			// gridColumn1
			// 
			this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
			this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			resources.ApplyResources(this.gridColumn1, "gridColumn1");
			this.gridColumn1.FieldName = "ProjectName";
			this.gridColumn1.Name = "gridColumn1";
			this.gridColumn1.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
			// 
			// gridColumn3
			// 
			this.gridColumn3.AppearanceHeader.Options.UseTextOptions = true;
			this.gridColumn3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			resources.ApplyResources(this.gridColumn3, "gridColumn3");
			this.gridColumn3.FieldName = "AdminComp";
			this.gridColumn3.Name = "gridColumn3";
			this.gridColumn3.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
			// 
			// gridColumn2
			// 
			this.gridColumn2.AppearanceHeader.Options.UseTextOptions = true;
			this.gridColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			resources.ApplyResources(this.gridColumn2, "gridColumn2");
			this.gridColumn2.FieldName = "AdminID";
			this.gridColumn2.Name = "gridColumn2";
			this.gridColumn2.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
			// 
			// gridColumn4
			// 
			this.gridColumn4.AppearanceHeader.Options.UseTextOptions = true;
			this.gridColumn4.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			resources.ApplyResources(this.gridColumn4, "gridColumn4");
			this.gridColumn4.FieldName = "ManagerComp";
			this.gridColumn4.Name = "gridColumn4";
			this.gridColumn4.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
			// 
			// gridColumn5
			// 
			this.gridColumn5.AppearanceHeader.Options.UseTextOptions = true;
			this.gridColumn5.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			resources.ApplyResources(this.gridColumn5, "gridColumn5");
			this.gridColumn5.FieldName = "ManagerID";
			this.gridColumn5.Name = "gridColumn5";
			this.gridColumn5.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.True;
			// 
			// gridColumn6
			// 
			this.gridColumn6.AppearanceHeader.Options.UseTextOptions = true;
			this.gridColumn6.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			resources.ApplyResources(this.gridColumn6, "gridColumn6");
			this.gridColumn6.FieldName = "OperatorComp";
			this.gridColumn6.Name = "gridColumn6";
			this.gridColumn6.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
			// 
			// gridColumn7
			// 
			this.gridColumn7.AppearanceHeader.Options.UseTextOptions = true;
			this.gridColumn7.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			resources.ApplyResources(this.gridColumn7, "gridColumn7");
			this.gridColumn7.FieldName = "OperatorID";
			this.gridColumn7.Name = "gridColumn7";
			this.gridColumn7.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
			this.gridColumn7.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(((DevExpress.Data.SummaryItemType)(resources.GetObject("gridColumn7.Summary"))), resources.GetString("gridColumn7.Summary1"), resources.GetString("gridColumn7.Summary2"))});
			// 
			// gridColumn8
			// 
			resources.ApplyResources(this.gridColumn8, "gridColumn8");
			this.gridColumn8.FieldName = "ProjectKey";
			this.gridColumn8.Name = "gridColumn8";
			// 
			// Root
			// 
			this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.Root.GroupBordersVisible = false;
			this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2});
			this.Root.Name = "Root";
			this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 3);
			this.Root.Size = new System.Drawing.Size(1092, 514);
			this.Root.TextVisible = false;
			// 
			// layoutControlItem2
			// 
			this.layoutControlItem2.Control = this.groupControl2;
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Size = new System.Drawing.Size(1092, 511);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			// 
			// FormPopProjectRelation
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.splitContainerControl1);
			this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("FormPopProjectRelation.IconOptions.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPopProjectRelation";
			this.Load += new System.EventHandler(this.FormPopProjectRelation_Load);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel1)).EndInit();
			this.splitContainerControl1.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel2)).EndInit();
			this.splitContainerControl1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
			this.splitContainerControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
			this.groupControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.grdProject)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grdViewProject)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnExportExcel;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraGrid.GridControl grdProject;
        private DevExpress.XtraGrid.Views.Grid.GridView grdViewProject;
        private DevExpress.XtraLayout.LayoutControlGroup Root;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
		private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
	}
}