using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SWSAnalyzer
{
    public partial class FormProjectRelation : DevExpress.XtraEditors.XtraForm
    {
        #region 폼 로드
        /// <summary>
        /// 
        /// </summary>
        public FormProjectRelation()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormProjectRelation_Load(object sender, EventArgs e)
        {
            SetProject();
            if (!Program.Option.isAdmin)
            {
                groupControl2.CustomHeaderButtons[0].Properties.Visible = groupControl2.CustomHeaderButtons[1].Properties.Visible = groupControl2.CustomHeaderButtons[2].Properties.Visible = groupControl2.CustomHeaderButtons[3].Properties.Visible = false;
                groupControl3.CustomHeaderButtons[0].Properties.Visible = groupControl3.CustomHeaderButtons[1].Properties.Visible = groupControl3.CustomHeaderButtons[2].Properties.Visible = groupControl3.CustomHeaderButtons[3].Properties.Visible = false;
                btnProjectRealation.Visible = false;
            }
        }

        /// <summary>
        /// 폼 쇼운
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormProjectRelation_Shown(object sender, EventArgs e)
        {
            lueProject.Focus();
        }

        /// <summary>
        /// 프로젝트 세팅
        /// </summary>
        private void SetProject()
        {
            string sql = string.Empty;
            sql += "select p.ProjectKey No, p.ProjectName ProjectName, c.CompName ";
            sql += "  from project p ";
            sql += "  LEFT JOIN Company c ON c.CompKey = p.CompKey ";
            sql += " WHERE p.isDeleted = 0 ";
            if (!Program.Option.isAdmin)
                sql += $" AND p.CompKey = {Program.Option.CompKey} ";
            sql += " ORDER BY 1 ";

            DataTable dtProject = DBManager.Instance.GetDataTable(sql);
            lueProject.Properties.DataSource = dtProject;
            lueProject.Properties.ValueMember = "No";
            lueProject.Properties.DisplayMember = "ProjectName";
        }
        #endregion

        #region Data 로드
        /// <summary>
        /// ID 세팅
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lueProject_EditValueChanged(object sender, EventArgs e)
        {
            grdManager.DataSource = null;
            grdOperator.DataSource = null;

            SetAdmin();
            SetManager();
        }

        /// <summary>
        /// Admin ID 세팅
        /// </summary>
        private void SetAdmin()
        {
            string sql = string.Empty;
            sql += "SELECT u.UserID, ";
            sql += "       u.UserName, ";
            sql += "       u.CompKey, ";
            sql += "       c.CompName, ";
            sql += "       u.AuthLevel ";
            sql += "  FROM UserInfo u ";
            sql += "  JOIN Company c ";
            sql += "    ON c.CompKey = u.CompKey ";
            sql += "  JOIN Project p ";
            sql += "    ON p.CompKey = c.CompKey ";
            sql += " WHERE 1 = 1 ";
            sql += $"  AND p.ProjectKey = {lueProject.EditValue} ";
            sql += "   AND u.AuthLevel = 1 ";
            sql += "   AND u.isDeleted = 0 ";
            sql += " ORDER BY UserID ";
            DataTable dtAdmin = DBManager.Instance.GetDataTable(sql);
            if (dtAdmin == null || dtAdmin.Rows.Count == 0)
            {
                lblComp.Visible = lblID.Visible = lblUser.Visible = txtCompName.Visible = txtID.Visible = txtUserName.Visible = false;
                return;
            }
            
            DataRow row = dtAdmin.Rows[0];
            lblComp.Visible = lblID.Visible = lblUser.Visible = txtCompName.Visible = txtID.Visible = txtUserName.Visible = true;
            txtCompName.Text = row["CompName"].ToString();
            txtID.Text = row["UserID"].ToString();
            txtUserName.Text = row["UserName"].ToString();
        }

        /// <summary>
        /// Manager ID 세팅
        /// </summary>
        private void SetManager()
        {
            string sql = string.Empty;
            sql += "SELECT u.UserID, ";
            sql += "       u.UserName, ";
            sql += "       u.CompKey, ";
            sql += "       c.CompName, ";
            sql += "       u.ParentAdminID, ";
            sql += "       u.AuthLevel ";
            sql += "  FROM UserInfo u ";
            sql += "  JOIN Company c ";
            sql += "    ON c.CompKey = u.CompKey ";
            sql += "  JOIN Project p ";
            sql += "    ON p.ProjectKey = u.ProjectKey ";
            sql += " WHERE 1 = 1 ";
            sql += $"  AND u.ProjectKey = {lueProject.EditValue} ";
            sql += "   AND u.AuthLevel = 2 ";
            sql += "   AND u.isDeleted = 0 ";
            sql += " ORDER BY u.ParentAdminID, u.UserID ";
            DataTable dtManager = DBManager.Instance.GetDataTable(sql);
            grdManager.DataSource = dtManager;
            grdViewManager.BestFitColumns();
        }

        /// <summary>
        /// Operator ID 세팅
        /// </summary>
        private void grdViewManager_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            SetOperator();
        }
        private void SetOperator()
        {
            DataRow row = grdViewManager.GetFocusedDataRow();
            DataTable dtOperator = null;
            try
            {
                string sql = string.Empty;
                sql += "SELECT u.UserID, ";
                sql += "       u.UserName, ";
                sql += "       u.CompKey, ";
                sql += "       c.CompName, ";
                sql += "       u.ParentAdminID, ";
                sql += "       u.ParentManagerID, ";
                sql += "       u.AuthLevel ";
                sql += "  FROM UserInfo u ";
                sql += "  JOIN Company c ";
                sql += "    ON c.CompKey = u.CompKey ";
                sql += "  JOIN Project p ";
                sql += "    ON p.ProjectKey = u.ProjectKey ";
                sql += " WHERE 1 = 1 ";
                sql += $"  AND u.ProjectKey = {lueProject.EditValue} ";
                sql += "   AND u.AuthLevel = 3 ";
                sql += "   AND u.isDeleted = 0 ";
                sql += $"  AND u.ParentManagerID = '{row["UserID"].ToString()}' ";
                sql += " ORDER BY u.ParentAdminID, u.ParentManagerID, u.UserID ";
                dtOperator = DBManager.Instance.GetDataTable(sql);
            }
            catch (Exception e)
            {
            }
            grdOperator.DataSource = dtOperator;
            grdViewOperator.BestFitColumns();   
        }
        #endregion

        #region 권한/신규/수정/삭제
        /// <summary>
        /// Manager => 권한/신규/수정/삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupControl2_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            DataRow row = grdViewManager.GetFocusedDataRow();

            switch (e.Button.Properties.Tag)
            {
                case "New":
                    if (lueProject.EditValue == null)
                        return;

                    FormPopUsers frmNew = new FormPopUsers();
                    frmNew.EditMode = 1;
                    frmNew.ChoiceProjectKey = int.Parse(lueProject.EditValue.ToString());
                    frmNew.ChoiceAuthLevel = 2;
                    if (frmNew.ShowDialog() == DialogResult.OK)
                    {
                        SetAdmin();
                        SetManager();
                        SetOperator();
                    }
                    break;
                case "Auth":
                    if (row == null)
                        return;

                    FormPopUserAuth frmAuth = new FormPopUserAuth();
                    frmAuth.ChoiceCompKey = int.Parse(row["CompKey"].ToString());
                    frmAuth.ChoiceUserID = row["userID"].ToString();
                    frmAuth.ChoiceAuthLevel = int.Parse(row["AuthLevel"].ToString());
                    frmAuth.ShowDialog();
                    break;
                case "Modify":
                    if (row == null)
                        return;

                    FormPopUsers frmModify = new FormPopUsers();
                    frmModify.EditMode = 2;
                    frmModify.ChoiceCompKey = int.Parse(row["CompKey"].ToString());
                    frmModify.ChoiceUserID = row["userID"].ToString();
                    if (frmModify.ShowDialog() == DialogResult.OK)
                    {
                        SetAdmin();
                        SetManager();
                        SetOperator();
                    }
                    break;
                case "Delete":
                    if (row == null)
                        return;

                    FormPopUsers frmDelete = new FormPopUsers();
                    frmDelete.EditMode = 3;
                    frmDelete.ChoiceCompKey = int.Parse(row["CompKey"].ToString());
                    frmDelete.ChoiceUserID = row["userID"].ToString();
                    if (frmDelete.ShowDialog() == DialogResult.OK)
                    {
                        SetAdmin();
                        SetManager();
                        SetOperator();
                    }
                    break;
            }
        }

        /// <summary>
        /// Operator => 권한/수정/삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupControl3_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            DataRow row = grdViewOperator.GetFocusedDataRow();
            switch (e.Button.Properties.Tag)
            {
                case "New":
                    if (lueProject.EditValue == null)
                        return;

                    FormPopUsers frmNew = new FormPopUsers();
                    frmNew.EditMode = 1;
                    frmNew.ChoiceProjectKey = int.Parse(lueProject.EditValue.ToString());
                    frmNew.ChoiceAuthLevel = 3;
                    if (frmNew.ShowDialog() == DialogResult.OK)
                    {
                        SetAdmin();
                        SetManager();
                        SetOperator();
                    }
                    break;
                case "Auth":
                    FormPopUserAuth frmAuth = new FormPopUserAuth();
                    frmAuth.ChoiceCompKey = int.Parse(row["CompKey"].ToString());
                    frmAuth.ChoiceUserID = row["userID"].ToString();
                    frmAuth.ChoiceAuthLevel = int.Parse(row["AuthLevel"].ToString());
                    frmAuth.ShowDialog();
                    break;
                case "Modify":
                    if (row == null)
                        return;

                    FormPopUsers frmModify = new FormPopUsers();
                    frmModify.EditMode = 2;
                    frmModify.ChoiceCompKey = int.Parse(row["CompKey"].ToString());
                    frmModify.ChoiceUserID = row["userID"].ToString();
                    if (frmModify.ShowDialog() == DialogResult.OK)
                    {
                        SetAdmin();
                        SetManager();
                        SetOperator();
                    }
                    break;
                case "Delete":
                    if (row == null)
                        return;

                    FormPopUsers frmDelete = new FormPopUsers();
                    frmDelete.EditMode = 3;
                    frmDelete.ChoiceCompKey = int.Parse(row["CompKey"].ToString());
                    frmDelete.ChoiceUserID = row["userID"].ToString();
                    if (frmDelete.ShowDialog() == DialogResult.OK)
                    {
                        SetAdmin();
                        SetManager();
                        SetOperator();
                    }
                    break;
            }
        }
        #endregion

        /// <summary>
        /// 조직도 팝업
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProjectRealation_Click(object sender, EventArgs e)
        {
            FormPopProjectRelation frm = new FormPopProjectRelation(0, 0);
            frm.ShowDialog();
        }        
        
        /// <summary>
        /// 닫기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}