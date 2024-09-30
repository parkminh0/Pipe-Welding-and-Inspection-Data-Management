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
using DevExpress.XtraSplashScreen;
using DevExpress.XtraPrinting;
using DevExpress.Export;

namespace SWSAnalyzer
{
    public partial class FormPivotResult : DevExpress.XtraEditors.XtraForm
    {
        #region 폼 로드
        /// <summary>
        /// 
        /// </summary>
        public FormPivotResult()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormPivotResult_Load(object sender, EventArgs e)
        {
            dteFrom.DateTime = DateTime.Now.AddMonths(-1);
            dteTo.DateTime = DateTime.Now;

            if (!Program.Option.SaveExcel)
                btnExportExcel.Enabled = false;

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
        }

        /// <summary>
        /// 폼 쇼운
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormPivotResult_Shown(object sender, EventArgs e)
        {
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
                    sql += " ORDER BY 1 ";
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
                case 2:
                    sql += "select DISTINCT(c.CompKey) No, c.CompName as Company ";
                    sql += "  from Company c ";
                    sql += "  JOIN UserInfo u ";
                    sql += "    ON c.CompKey = u.CompKey AND u.isDeleted = 0 ";
                    sql += " WHERE 1 = 1 ";
                    sql += "   AND c.isDeleted = 0 ";
                    sql += $"  AND (u.ParentManagerID = '{Program.Option.LoginID}' or u.UserID = '{Program.Option.LoginID}') ";
                    sql += " ORDER BY 1 ";
                    break;
                case 3:
                    sql += "select DISTINCT(c.CompKey) No, c.CompName as Company ";
                    sql += "  from Company c ";
                    sql += "  JOIN UserInfo u ";
                    sql += "    ON c.CompKey = u.CompKey AND u.isDeleted = 0 ";
                    sql += " WHERE 1 = 1 ";
                    sql += "   AND c.isDeleted = 0 ";
                    sql += $"  AND u.UserID = '{Program.Option.LoginID}' ";
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
        /// 회사선택시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lueCompany_EditValueChanged(object sender, EventArgs e)
        {
            string sql = string.Empty;
            if (Program.Option.AuthLevel <= 1)
            {
                sql = string.Format("select p.ProjectKey No, p.ProjectName Project from Project p where p.isDeleted = 0 and p.CompKey = {0} ORDER BY 1 ", lueCompany.EditValue);
            }
            else
            {
                sql = string.Format($"select p.ProjectKey No, p.ProjectName Project from Project p where isDeleted = 0 and ProjectKey = {Program.Option.ProjectKey} ORDER BY 1 ");
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
        #endregion

        #region 조회
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
        /// 전체기간 선택
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkIsAllDate_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIsAllDate.Checked)
            {
                dteFrom.Enabled = false;
                dteTo.Enabled = false;
            }
            else
            {
                dteFrom.Enabled = true;
                dteTo.Enabled = true;
            }
        }

        private string StartDate;
        private string EndDate;
        private int ProjectKey;
        private DataTable dtPivotData;

        /// <summary>
        /// 조회
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            StartDate = dteFrom.DateTime.ToString("yyyy-MM-dd");
            EndDate = dteTo.DateTime.ToString("yyyy-MM-dd");
            ProjectKey = int.Parse(lueProject.EditValue.ToString());

            string sql = string.Empty;
            sql += "select CompKey, ProjectKey, BeadKey, YearMonth, Year, SerialNo, InspectionDate, InspectionNo, InspectorID, Material, OD, SDR, InspectCreateID, WeldCreateID, ";
            sql += "        y.Item, y.TfString, 1 as DataCount ";
            sql += "  from ViewBeadMasterForSummary ";
            sql += "       UNPIVOT (TfString FOR Item in ([K-Value],[Bead Width],[Uneven Bead],[Mis-Al],[Notch],[NotchEye],[Ang-Dev],[Crack],[Void],[Support],[Interrupt],[Overheat])) as y ";
            sql += " where 1 = 1 ";

            if (!chkIsAllCompany.Checked)
                sql += string.Format("   and CompKey = {0} ", lueCompany.EditValue);

            if (!chkIsAllProject.Checked)
                sql += $"   and ProjectKey = {ProjectKey} ";

            if (!chkIsAllDate.Checked)
                sql += $"   and InspectionDate BETWEEN '{StartDate}' and '{EndDate}' ";

            if (!Program.Option.isAdmin)
            {
                if (Program.Option.AuthLevel == 1)
                {
                    sql += $" and (WeldCreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                    sql += $"      OR InspectCreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}')) ";
                }
                else if (Program.Option.AuthLevel == 2)
                {
                    sql += $" and (WeldCreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR InspectCreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR WeldCreateID = '{Program.Option.LoginID}' OR InspectCreateID = '{Program.Option.LoginID}') ";
                }
                else if (Program.Option.AuthLevel == 3)
                {
                    sql += $"  and (WeldCreateID = '{Program.Option.LoginID}' or InspectCreateID = '{Program.Option.LoginID}') ";
                }
            }

            SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
            SplashScreenManager.Default.SetWaitFormDescription("The loading of data. Please wait....");

            dtPivotData = DBManager.Instance.GetDataTable(sql);
            pgrdPivotSummary.DataSource = dtPivotData;

            SplashScreenManager.CloseForm(false);

        }
        #endregion

        #region 엑셀 저장
        /// <summary>
        /// 엑셀저장
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            targetFileName = GetFileName("xlsx", string.Format("Pivot"));
            if (targetFileName.Trim() != "")
                pgrdPivotSummary.ExportToXlsx(targetFileName, new XlsxExportOptionsEx { ExportType = ExportType.WYSIWYG });
        }

        /// <summary>
        /// 파일명 가져오기
        /// </summary>
        /// <param name="extName"></param>
        /// <returns></returns>
        private string GetFileName(string extName, string defaultName)
        {
            FileDialog fileDialog = new SaveFileDialog();
            if (extName == "xlsx")
            {
                fileDialog.DefaultExt = "xlsx";
                fileDialog.Filter = "Excel File(*.xlsx)|*.xlsx";
                fileDialog.FileName = string.Format("{0}_{1}.xlsx", defaultName, DateTime.Now.ToString("yyyy-MM-dd"));
            }
            else
            {
                return "Export_" + DateTime.Now.ToString("yyyyMMdd");
            }

            if (fileDialog.ShowDialog() != DialogResult.OK)
                return "";

            string fileName = fileDialog.FileName;
            if (fileName.ToLower() == "*.xlsx")
                fileName = "";

            return fileName;
        }
        static string targetFileName;
        #endregion

        /// <summary>
        /// 닫기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}