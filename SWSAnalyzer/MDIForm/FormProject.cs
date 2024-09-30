using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace SWSAnalyzer
{
    public partial class FormProject : DevExpress.XtraEditors.XtraForm
    {
        private int ChoiceProjectKey;
        private int ChoiceCompKey;
        private DataTable dtProject;

        #region 폼 로드
        /// <summary>
        /// 
        /// </summary>
        public FormProject()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormProject_Load(object sender, EventArgs e)
        {
            SetCompany();
            if (!Program.Option.isAdmin)
                btnNew.Visible = btnModify.Visible = btnDelete.Visible = labelControl1.Visible = lueCompany.Visible = gridColumn1.Visible = false;
        }

        /// <summary>
        /// 폼 쇼운
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormProject_Shown(object sender, EventArgs e)
        {
            GetFocusedRow();
            lueCompany.Focus();
        }

        /// <summary>
        /// 회사코드 콤보
        /// </summary>
        private void SetCompany()
        {
            string sql = string.Empty;
            switch (Program.Option.AuthLevel)
            {
                case 0:
                    sql += "select CompKey No, CompName Company from Company where isDeleted = 0 ORDER BY 1 ";
                    break;
                case 1:
                    sql += "SELECT DISTINCT(c.CompKey) No, c.CompName Company ";
                    sql += "  FROM Company c ";
                    sql += "  JOIN UserInfo u ";
                    sql += "    ON c.CompKey = u.CompKey AND u.isDeleted = 0 ";
                    sql += " WHERE 1 = 1 ";
                    sql += "   AND c.isDeleted = 0 ";
                    sql += $"  AND u.ParentAdminID = '{Program.Option.LoginID}' ";
                    sql += " ORDER BY 1 ";
                    break;
            }

            DataTable dtCompany = DBManager.Instance.GetDataTable(sql);
            lueCompany.Properties.DataSource = dtCompany;
            lueCompany.Properties.ValueMember = "No";
            lueCompany.Properties.DisplayMember = "Company";
            lueCompany.EditValue = Program.Option.CompKey;
        }
        #endregion

        #region 조회
        /// <summary>
        /// 회사선택시 해당 프로젝트 목록 가져오기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lueCompany_EditValueChanged(object sender, EventArgs e)
        {
            ChoiceCompKey = int.Parse(lueCompany.EditValue.ToString());
            GetDataList();
        }

        /// <summary>
        /// 기존 목록 불러오기
        /// </summary>
        private void GetDataList()
        {
            string sql = string.Empty;
            sql += "select ProjectKey, ";
            sql += "       ProjectName, ";
            sql += "       Descr, ";
            sql += "       CompKey ";
            sql += "  from Project ";
            sql += " where 1 = 1 ";
            sql += "   and isDeleted = 0 ";
            sql += $"  and CompKey = {ChoiceCompKey} ";
            dtProject = DBManager.Instance.GetDataTable(sql);
            grdProject.DataSource = dtProject;
            GetFocusedRow();
        }

        /// <summary>
        /// 포커스
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            GetFocusedRow();
        }
        private void GetFocusedRow()
        {
            DataRow row = gridView1.GetFocusedDataRow();
            if (row == null)
                return;

            try
            {
                ChoiceProjectKey = int.Parse(row["ProjectKey"].ToString());
                btnModify.Enabled = btnDelete.Enabled = true;
            }
            catch (Exception)
            {
                ChoiceProjectKey = -1;
                btnModify.Enabled = btnDelete.Enabled = false;
                return;
            }
        }

        /// <summary>
        /// 데이터 건수 출력
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_RowCountChanged(object sender, EventArgs e)
        {
            groupControl1.Text = string.Format(LangResx.Main.count_Project, lueCompany.Text, gridView1.RowCount);
        }
        #endregion

        #region CRUD
        /// <summary>
        /// 신규
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNew_Click(object sender, EventArgs e)
        {
            FormPopProject frm = new FormPopProject();
            frm.EditMode = 1;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                ChoiceProjectKey = frm.ChoiceProjectKey;
                GetDataList();
            }
        }

        /// <summary>
        /// 수정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModify_Click(object sender, EventArgs e)
        {
            FormPopProject frm = new FormPopProject();
            frm.EditMode = 2;
            frm.ChoiceCompKey = ChoiceCompKey;
            frm.ChoiceProjectKey = ChoiceProjectKey;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                GetDataList();
            }
        }
        private void grdProject_DoubleClick(object sender, EventArgs e)
        {
            if (!Program.Option.isAdmin)
                return;

            GetFocusedRow();
            btnModify.PerformClick();
        }

        /// <summary>
        /// 삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            FormPopProject frm = new FormPopProject();
            frm.EditMode = 3;
            frm.ChoiceCompKey = ChoiceCompKey;
            frm.ChoiceProjectKey = ChoiceProjectKey;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                GetDataList();
            }
        }
        #endregion

        #region 닫기
        /// <summary>
        /// 닫기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}