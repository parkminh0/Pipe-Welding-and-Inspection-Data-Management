namespace SWSAnalyzer
{
    partial class FormProject
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProject));
			this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.lueCompany = new DevExpress.XtraEditors.LookUpEdit();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.btnClose = new DevExpress.XtraEditors.SimpleButton();
			this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
			this.btnModify = new DevExpress.XtraEditors.SimpleButton();
			this.btnNew = new DevExpress.XtraEditors.SimpleButton();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.grdProject = new DevExpress.XtraGrid.GridControl();
			this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel1)).BeginInit();
			this.splitContainerControl1.Panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel2)).BeginInit();
			this.splitContainerControl1.Panel2.SuspendLayout();
			this.splitContainerControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lueCompany.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.grdProject)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
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
			this.splitContainerControl1.Panel2.Controls.Add(this.groupControl1);
			resources.ApplyResources(this.splitContainerControl1.Panel2, "splitContainerControl1.Panel2");
			this.splitContainerControl1.SplitterPosition = 44;
			// 
			// panelControl1
			// 
			this.panelControl1.Controls.Add(this.lueCompany);
			this.panelControl1.Controls.Add(this.labelControl1);
			this.panelControl1.Controls.Add(this.btnClose);
			this.panelControl1.Controls.Add(this.btnDelete);
			this.panelControl1.Controls.Add(this.btnModify);
			this.panelControl1.Controls.Add(this.btnNew);
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			// 
			// lueCompany
			// 
			resources.ApplyResources(this.lueCompany, "lueCompany");
			this.lueCompany.Name = "lueCompany";
			this.lueCompany.Properties.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lueCompany.Properties.Appearance.Font")));
			this.lueCompany.Properties.Appearance.Options.UseFont = true;
			this.lueCompany.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
			this.lueCompany.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("lueCompany.Properties.Buttons"))))});
			this.lueCompany.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("lueCompany.Properties.Columns"), resources.GetString("lueCompany.Properties.Columns1"), ((int)(resources.GetObject("lueCompany.Properties.Columns2"))), ((DevExpress.Utils.FormatType)(resources.GetObject("lueCompany.Properties.Columns3"))), resources.GetString("lueCompany.Properties.Columns4"), ((bool)(resources.GetObject("lueCompany.Properties.Columns5"))), ((DevExpress.Utils.HorzAlignment)(resources.GetObject("lueCompany.Properties.Columns6"))), ((DevExpress.Data.ColumnSortOrder)(resources.GetObject("lueCompany.Properties.Columns7"))), ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("lueCompany.Properties.Columns8")))),
            new DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("lueCompany.Properties.Columns9"), resources.GetString("lueCompany.Properties.Columns10"), ((int)(resources.GetObject("lueCompany.Properties.Columns11"))), ((DevExpress.Utils.FormatType)(resources.GetObject("lueCompany.Properties.Columns12"))), resources.GetString("lueCompany.Properties.Columns13"), ((bool)(resources.GetObject("lueCompany.Properties.Columns14"))), ((DevExpress.Utils.HorzAlignment)(resources.GetObject("lueCompany.Properties.Columns15"))), ((DevExpress.Data.ColumnSortOrder)(resources.GetObject("lueCompany.Properties.Columns16"))), ((DevExpress.Utils.DefaultBoolean)(resources.GetObject("lueCompany.Properties.Columns17"))))});
			this.lueCompany.Properties.NullText = resources.GetString("lueCompany.Properties.NullText");
			this.lueCompany.EditValueChanged += new System.EventHandler(this.lueCompany_EditValueChanged);
			// 
			// labelControl1
			// 
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			// 
			// btnClose
			// 
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
			this.btnClose.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.ImageOptions.Image")));
			this.btnClose.Name = "btnClose";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
			resources.ApplyResources(this.btnDelete, "btnDelete");
			this.btnDelete.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.ImageOptions.Image")));
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnModify
			// 
			this.btnModify.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
			resources.ApplyResources(this.btnModify, "btnModify");
			this.btnModify.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnModify.ImageOptions.Image")));
			this.btnModify.Name = "btnModify";
			this.btnModify.Click += new System.EventHandler(this.btnModify_Click);
			// 
			// btnNew
			// 
			this.btnNew.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
			this.btnNew.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("btnNew.ImageOptions.Image")));
			resources.ApplyResources(this.btnNew, "btnNew");
			this.btnNew.Name = "btnNew";
			this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
			// 
			// groupControl1
			// 
			this.groupControl1.AppearanceCaption.Font = ((System.Drawing.Font)(resources.GetObject("groupControl1.AppearanceCaption.Font")));
			this.groupControl1.AppearanceCaption.Options.UseFont = true;
			this.groupControl1.Controls.Add(this.grdProject);
			resources.ApplyResources(this.groupControl1, "groupControl1");
			this.groupControl1.Name = "groupControl1";
			// 
			// grdProject
			// 
			resources.ApplyResources(this.grdProject, "grdProject");
			this.grdProject.MainView = this.gridView1;
			this.grdProject.Name = "grdProject";
			this.grdProject.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
			this.grdProject.DoubleClick += new System.EventHandler(this.grdProject_DoubleClick);
			// 
			// gridView1
			// 
			this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3});
			this.gridView1.GridControl = this.grdProject;
			this.gridView1.Name = "gridView1";
			this.gridView1.OptionsBehavior.Editable = false;
			this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.gridView1.OptionsSelection.MultiSelect = true;
			this.gridView1.OptionsView.EnableAppearanceEvenRow = true;
			this.gridView1.OptionsView.ShowAutoFilterRow = true;
			this.gridView1.OptionsView.ShowGroupPanel = false;
			this.gridView1.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridView1_FocusedRowChanged);
			this.gridView1.RowCountChanged += new System.EventHandler(this.gridView1_RowCountChanged);
			// 
			// gridColumn1
			// 
			this.gridColumn1.AppearanceCell.Options.UseTextOptions = true;
			this.gridColumn1.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
			this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			resources.ApplyResources(this.gridColumn1, "gridColumn1");
			this.gridColumn1.FieldName = "ProjectKey";
			this.gridColumn1.Name = "gridColumn1";
			// 
			// gridColumn2
			// 
			this.gridColumn2.AppearanceHeader.Options.UseTextOptions = true;
			this.gridColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			resources.ApplyResources(this.gridColumn2, "gridColumn2");
			this.gridColumn2.FieldName = "ProjectName";
			this.gridColumn2.Name = "gridColumn2";
			// 
			// gridColumn3
			// 
			this.gridColumn3.AppearanceHeader.Options.UseTextOptions = true;
			this.gridColumn3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			resources.ApplyResources(this.gridColumn3, "gridColumn3");
			this.gridColumn3.FieldName = "Descr";
			this.gridColumn3.Name = "gridColumn3";
			// 
			// FormProject
			// 
			this.Appearance.Options.UseFont = true;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.splitContainerControl1);
			this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("FormProject.IconOptions.Icon")));
			this.Name = "FormProject";
			this.Load += new System.EventHandler(this.FormProject_Load);
			this.Shown += new System.EventHandler(this.FormProject_Shown);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel1)).EndInit();
			this.splitContainerControl1.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel2)).EndInit();
			this.splitContainerControl1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
			this.splitContainerControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			this.panelControl1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.lueCompany.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.grdProject)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.SimpleButton btnDelete;
        private DevExpress.XtraEditors.SimpleButton btnModify;
        private DevExpress.XtraEditors.SimpleButton btnNew;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraGrid.GridControl grdProject;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraEditors.LookUpEdit lueCompany;
    }
}