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
    public partial class FormCompany : DevExpress.XtraEditors.XtraForm
    {
        private int ChoiceKey;

        #region 폼 로드
        /// <summary>
        /// 
        /// </summary>
        public FormCompany()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormCompany_Load(object sender, EventArgs e)
        {
            if (!Program.Option.isAdmin)
                gridColumn1.Visible = btnNew.Visible = btnModify.Visible = btnDelete.Visible = false;
        }
        
        /// <summary>
        /// 폼 쇼운
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormCompany_Shown(object sender, EventArgs e)
        {
            GetDataList();
            GetFocusedRow();
            btnClose.Focus();
        }
        #endregion

        #region 로드/포커스
        /// <summary>
        /// 기존 목록 불러오기
        /// </summary>
        private void GetDataList()
        {
            string sql = string.Empty;
            switch (Program.Option.AuthLevel)
            {
                case 0:
                    sql += "select CompKey, CompName, Descr from Company where isDeleted = 0 ORDER BY 1 ";
                    break;
                case 1:
                    sql += "SELECT DISTINCT(c.CompKey) CompKey, c.CompName, c.Descr ";
                    sql += "  FROM Company c ";
                    sql += "  JOIN UserInfo u ";
                    sql += "    ON c.CompKey = u.CompKey AND u.isDeleted = 0 ";
                    sql += " WHERE 1 = 1 ";
                    sql += "   AND c.isDeleted = 0 ";
                    sql += $"  AND u.ParentAdminID = '{Program.Option.LoginID}' ";
                    sql += " ORDER BY 1 ";
                    break;
            }

            try
            {
                DataTable dtDataList = DBManager.Instance.GetDataTable(sql);
                grdCompany.DataSource = dtDataList;
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 데이터 건수 출력
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_RowCountChanged(object sender, EventArgs e)
        {
            //groupControl1.Text = string.Format("≡ 거래처 목록 - {0:n0}건 ", gridView1.RowCount);
            groupControl1.Text = string.Format(LangResx.Main.count_Comp, gridView1.RowCount);
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
                ChoiceKey = int.Parse(row["CompKey"].ToString());
                btnModify.Enabled = true;
                btnDelete.Enabled = true;
            }
            catch (Exception)
            {
                return;
            }
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
            FormPopCompany frm = new FormPopCompany();
            frm.EditMode = 1;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                ChoiceKey = frm.ChoiceKey;
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
            FormPopCompany frm = new FormPopCompany();
            frm.EditMode = 2;
            frm.ChoiceKey = ChoiceKey;
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
        private void grdCompany_DoubleClick(object sender, EventArgs e)
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
            FormPopCompany frm = new FormPopCompany();
            frm.EditMode = 3;
            frm.ChoiceKey = ChoiceKey;
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