using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace SWSUploader
{
    public partial class ucBeadUploadHistory : DevExpress.XtraEditors.XtraUserControl
    {
        public ucBeadUploadHistory()
        {
            InitializeComponent();
        }

        private void ucUploadHistory_Load(object sender, EventArgs e)
        {
            dteFrom.DateTime = DateTime.Now.AddMonths(-3);
            dteTo.DateTime = DateTime.Now;

            switch (Program.Option.AuthLevel)
            {
                case 2:
                    lueProject.ReadOnly = true;
                    chkIsAllProject.Visible = false;
                    break;
                case 3:
                    lueCompany.ReadOnly = lueProject.ReadOnly = true;
                    chkIsAllCompany.Visible = chkIsAllProject.Visible = false;
                    break;
                default:
                    break;
            }

            SetCompanyCode();
        }

        /// <summary>
        /// 거래처 콤보
        /// </summary>
        private void SetCompanyCode()
        {
            string sql = string.Empty;
            switch (Program.Option.AuthLevel)
            {
                case 0:
                    sql += $"select c.CompKey No, c.CompName as Company from Company c where c.isDeleted = 0 ";
                    break;
                case 1:
                    sql += "select DISTINCT(c.CompKey) No, c.CompName as Company ";
                    sql += "  from Company c ";
                    sql += "  JOIN UserInfo u ";
                    sql += "    ON c.CompKey = u.CompKey AND u.isDeleted = 0 ";
                    sql += " WHERE 1 = 1 ";
                    sql += "   AND c.isDeleted = 0 ";
                    sql += $"  AND u.ParentAdminID = '{Program.Option.LoginID}' ";
                    break;
                case 2:
                    sql += "select DISTINCT(c.CompKey) No, c.CompName as Company ";
                    sql += "  from Company c ";
                    sql += "  JOIN UserInfo u ";
                    sql += "    ON c.CompKey = u.CompKey AND u.isDeleted = 0 ";
                    sql += " WHERE 1 = 1 ";
                    sql += "   AND c.isDeleted = 0 ";
                    sql += $"  AND (u.ParentManagerID = '{Program.Option.LoginID}' or u.UserID = '{Program.Option.LoginID}') ";
                    break;
                case 3:
                    sql += "select DISTINCT(c.CompKey) No, c.CompName as Company ";
                    sql += "  from Company c ";
                    sql += "  JOIN UserInfo u ";
                    sql += "    ON c.CompKey = u.CompKey AND u.isDeleted = 0 ";
                    sql += " WHERE 1 = 1 ";
                    sql += "   AND c.isDeleted = 0 ";
                    sql += $"  AND u.UserID = '{Program.Option.LoginID}' ";
                    break;
            }

            DataTable dtCompany = DBManager.Instance.GetDataTable(sql);
            lueCompany.Properties.DataSource = dtCompany;
            lueCompany.Properties.ValueMember = "No";
            lueCompany.Properties.DisplayMember = "Company";
            lueCompany.EditValue = Program.Option.CompKey;
        }

        /// <summary>
        /// 거래처 변경시 프로젝트 코드 다시 불러오기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lueCompany_EditValueChanged(object sender, EventArgs e)
        {
            string sql = string.Empty;
            if (Program.Option.AuthLevel <= 1)
            {
                sql = string.Format("select p.ProjectKey No, p.ProjectName Project from Project p where p.isDeleted = 0 and p.CompKey = {0} ", lueCompany.EditValue);
            }
            else
            {
                sql = string.Format($"select p.ProjectKey No, p.ProjectName Project from Project p where isDeleted = 0 and ProjectKey = {Program.Option.ProjectKey} ");
            }

            DataTable dtProject = DBManager.Instance.GetDataTable(sql);
            lueProject.Properties.DataSource = dtProject;
            lueProject.Properties.ValueMember = "No";
            lueProject.Properties.DisplayMember = "Project";

            if (dtProject == null || dtProject.Rows.Count == 0)
                return;

            if (Program.Option.AuthLevel > 1)
                lueProject.EditValue = Program.Option.ProjectKey;
            else
                lueProject.EditValue = int.Parse(dtProject.Rows[0][0].ToString());
        }

        /// <summary>
        /// 회사전체 선택
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkIsAllCompany_CheckedChanged(object sender, EventArgs e)
        {
            //회사 전체를 선택하면 프로젝트는 자동으로 전체선택 상태가 되도록 함
            //즉 회사가 선택되어야 프로젝트도 선택할 수 있음
            chkIsAllProject.Checked = chkIsAllCompany.Checked;
            if (chkIsAllCompany.Checked)
            {
                lueCompany.Enabled = lueProject.Enabled = chkIsAllProject.Enabled = false;
            }
            else
            {
                lueCompany.Enabled = lueProject.Enabled = chkIsAllProject.Enabled = true;
            }
        }

        /// <summary>
        /// 전체 프로젝트 선택
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkIsAllProject_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIsAllProject.Checked)
                lueProject.Enabled = false;
            else
                lueProject.Enabled = true;
        }

        /// <summary>
        /// 조회 실행
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string sql = string.Empty;
            sql += "select bm.BeadKey ";
            sql += "	  ,bm.ProjectKey ";
            sql += "	  ,p.ProjectName ";
            sql += "	  ,bm.[LineNo] ";
            sql += "	  ,bm.SerialNo ";
            sql += "	  ,bm.InspectionNo ";
            sql += "	  ,CONVERT(VARCHAR(10), bm.InspectionDate, 120) InspectionDate ";
            sql += "	  ,bm.Material ";
            sql += "	  ,bm.OD ";
            sql += "	  ,bm.WallThickness ";
            sql += "	  ,bm.SDR ";
            sql += "	  ,bm.CreateID ";
            sql += "	  ,bm.CreateDtm ";
            sql += "  FROM BeadMaster bm ";
            sql += "  left join Project p ";
            sql += "    on bm.ProjectKey = p.ProjectKey ";
            sql += " where 1 = 1 ";
            if (rdoType.SelectedIndex == 0) //기간별
            {
                sql += string.Format("   and bm.InspectionDate between '{0}' and '{1}' ", dteFrom.DateTime.ToShortDateString(), dteTo.DateTime.ToShortDateString());
            }
            else //작업기간별
            {
                sql += string.Format("   and bm.CreateDtm between '{0} 00:00:00' and '{1} 23:59:59' ", dteFrom.DateTime.ToShortDateString(), dteTo.DateTime.ToShortDateString());
            }

            if (!chkIsAllCompany.Checked)
                sql += string.Format("   and p.CompKey = {0} ", lueCompany.EditValue);

            if (!chkIsAllProject.Checked)
                sql += string.Format("   and bm.ProjectKey = {0} ", lueProject.EditValue.ToString());

            if (!string.IsNullOrWhiteSpace(txtUserID.Text))
                sql += "   and bm.CreateID = '" + txtUserID.Text.Trim() + "' ";

            if (!string.IsNullOrWhiteSpace(txtSerialNo.Text))
                sql += "   and bm.SerialNo = '" + txtSerialNo.Text.Trim() + "' ";

            if (!Program.Option.isAdmin)
            {
                if (Program.Option.AuthLevel == 1)
                {
                    sql += $" and bm.CreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                }
                else if (Program.Option.AuthLevel == 2)
                {
                    sql += $" and (bm.CreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR bm.CreateID = '{Program.Option.LoginID}') ";
                }
                else
                {
                    sql += $"  and bm.CreateID = '{Program.Option.LoginID}' ";
                }
            }
            sql += " order by bm.InspectionDate DESC, bm.InspectionNo";

            DataTable dtData = DBManager.Instance.GetDataTable(sql);
            grdHistory.DataSource = dtData;
        }

        /// <summary>
        /// 데이터 건수 출력
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_RowCountChanged(object sender, EventArgs e)
        {
            groupControl2.Text = string.Format(LangResx.Main.count_UploadHistory, gridView1.RowCount);
        }

        /// <summary>
        /// 닫기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            Program.mainApp.fluentDesignFormContainer1.Controls.Clear();
        }
    }
}
