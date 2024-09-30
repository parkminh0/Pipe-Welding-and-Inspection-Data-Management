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
using DevExpress.XtraBars;
using DevExpress.Internal;

namespace SWSAnalyzer
{
    public partial class FormUsers : DevExpress.XtraEditors.XtraForm
    {
        private string ChoiceUserID;
        private int ChoiceCompKey;
        private int ChoiceAuthLevel;

        #region 폼 로드
        /// <summary>
        /// 
        /// </summary>
        public FormUsers()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormUsers_Load(object sender, EventArgs e)
        {
            SetCompany();
            SetAuthLevelCode();
            if (!Program.Option.isAdmin)
            {
                chkDeleted.Visible = btnNew.Visible = btnModify.Visible = btnUserAuth.Visible = btnDelete.Visible = false;
                gridColumn8.Visible = false;
            }
        }

        /// <summary>
        /// 폼 쇼운
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormUsers_Shown(object sender, EventArgs e)
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
                    sql += $"select c.CompKey No, c.CompName as Company from Company c where c.isDeleted = 0 ORDER BY 1 ";
                    break;
                case 1:
                    sql += "select DISTINCT(c.CompKey) No, c.CompName as Company ";
                    sql += "  from Company c ";
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

        /// <summary>
        /// 사용자권한코드
        /// </summary>
        private void SetAuthLevelCode()
        {
            string sql = string.Empty;
            sql += "select cast(DetailCode as int) Level, CodeDescr as Auth ";
            sql += "  from CodeInfomation ";
            sql += " where Category = 'AuthLevel' ";
            sql += " order by OrderNo";
            DataTable dtAuthLevel = DBManager.Instance.GetDataTable(sql);

            repositoryItemLookUpEdit1.DataSource = dtAuthLevel;
            repositoryItemLookUpEdit1.ValueMember = "Level";
            repositoryItemLookUpEdit1.DisplayMember = "Auth";
        }

        /// <summary>
        /// 회사선택시 해당 사용자 목록 가져오기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lueCompany_EditValueChanged(object sender, EventArgs e)
        {
            ChoiceCompKey = int.Parse(lueCompany.EditValue.ToString());
            GetDataList();
        }
        #endregion

        #region 조회
        /// <summary>
        /// 삭제된 유저 보기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkDeleted_CheckedChanged(object sender, EventArgs e)
        {
            GetDataList();
        }

        /// <summary>
        /// 기존 목록 불러오기
        /// </summary>
        private void GetDataList()
        {
            string sql = string.Empty;
            sql += "select ui.UserID ";
            sql += "	  ,ui.UserName ";
            sql += "	  ,ui.Passwd ";
            sql += "	  ,ui.AuthLevel ";
            sql += "	  ,ui.PhoneNo ";
            sql += "	  ,ui.Descr ";
            sql += "      ,CASE WHEN ui.AuthLevel = 1 THEN null else ui.ParentAdminId END ParentAdminID ";
            sql += "      ,ui.ParentManagerID ";
            sql += "      ,p.ProjectName ";
            sql += "      ,ui.ExpireDtm ";
            sql += "      ,c.CompName ";
            sql += "      ,ui.CreateDtm ";
            sql += "  from UserInfo ui ";
            sql += "  join Company c ";
            sql += "    on c.CompKey = ui.CompKey ";
            sql += "  left join project p ";
            sql += "    on p.projectkey = ui.projectkey ";
            sql += $"where ui.CompKey = {ChoiceCompKey} ";
            if (chkDeleted.Checked)
            {
                sql += "   and ui.isDeleted = 1 ";
            }
            else
            {
                sql += "   and ui.isDeleted = 0 ";
            }
            if (!Program.Option.isAdmin)
                sql += $"  and ui.ParentAdminID = '{Program.Option.LoginID}' ";
            sql += "   and ui.UserID != 'WooriBnc' ";
            sql += " order by AuthLevel, CompName ";

            DataTable dtDataList = DBManager.Instance.GetDataTable(sql);
            grdUsers.DataSource = dtDataList;
            gridView1.BestFitColumns();
        }

        /// <summary>
        /// 데이터 건수 출력
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_RowCountChanged(object sender, EventArgs e)
        {
            groupControl1.Text = string.Format(LangResx.Main.count_User, lueCompany.Text, gridView1.RowCount);
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

            ChoiceUserID = row["UserID"].ToString();
            ChoiceAuthLevel = int.Parse(row["AuthLevel"].ToString());
            btnUserAuth.Enabled = true;
            btnModify.Enabled = true;
            if (ChoiceAuthLevel != 0)
                btnDelete.Enabled = true;
            else
                btnDelete.Enabled = false;
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
            FormPopUsers frm = new FormPopUsers();
            frm.EditMode = 1;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                ChoiceUserID = frm.ChoiceUserID;
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
            FormPopUsers frm = new FormPopUsers();
            frm.EditMode = 2;
            frm.ChoiceCompKey = ChoiceCompKey;
            frm.ChoiceAuthLevel = ChoiceAuthLevel;
            frm.ChoiceUserID = ChoiceUserID;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                GetDataList();
            }
        }

        /// <summary>
        /// 더블클릭시 수정창 자동열기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdUsers_DoubleClick(object sender, EventArgs e)
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
            FormPopUsers frm = new FormPopUsers();
            frm.EditMode = 3;
            frm.ChoiceCompKey = ChoiceCompKey;
            frm.ChoiceUserID = ChoiceUserID;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                GetDataList();
            }
        }
        #endregion

        #region 유저 권한 설정
        /// <summary>
        /// 유저 권한 설정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUserAuth_Click(object sender, EventArgs e)
        {
            FormPopUserAuth frm = new FormPopUserAuth();
            frm.ChoiceCompKey = ChoiceCompKey;
            frm.ChoiceUserID = ChoiceUserID;
            frm.ChoiceAuthLevel = ChoiceAuthLevel;
            frm.ShowDialog();
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