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
using DevExpress.XtraPrinting;
using DevExpress.Export;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraGrid.Views.Base;
using System.IO;

namespace SWSAnalyzer
{
    public partial class FormWeldMasterManage : DevExpress.XtraEditors.XtraForm
    {
        private DataTable dtWeldMaster;
        private string transferID;
        private int transferProject;
        //private string LayoutPath;

        #region 폼 로드
        /// <summary>
        /// 
        /// </summary>
        public FormWeldMasterManage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormWeldMasterManage_Load(object sender, EventArgs e)
        {
            dteFrom.DateTime = DateTime.Now.AddMonths(-1);
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

            if (!Program.Option.SaveExcel)
                btnExportExcel.Enabled = false;

            if (!Program.Option.DeleteWeldData)
                btnDelMaster.Enabled = false;

            if (!Program.Option.isAdmin)
                btnTransfer.Visible = false;
        }
        
        /// <summary>
        /// 폼 쇼운
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormWeldMasterManage_Shown(object sender, EventArgs e)
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
                    sql += $"select c.CompKey No, c.CompName as Company from Company c where c.isDeleted = 0 ORDER BY 1";
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
        /// 거래처 변경시 프로젝트 코드 다시 불러오기
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
        /// 기간 전체선택
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

        /// <summary>
        /// 검색
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (chkIsAllDate.Checked)
            {
                if (string.IsNullOrWhiteSpace(txtMachineSerialNo.Text) && string.IsNullOrWhiteSpace(txtMaterial.Text) && chkDiameter.Checked)
                {
                    XtraMessageBox.Show(LangResx.Main.msg_WeldSearchAll, LangResx.Main.msg_title_SearchAll, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtMachineSerialNo.Focus();
                    return;
                }
            }

            string sql = LogicManager.Common.GetSelectWeldMasterSql();

            if (!chkIsAllCompany.Checked)
                sql += string.Format("   and c.CompKey = {0} ", lueCompany.EditValue);

            if (!chkIsAllProject.Checked)
                sql += string.Format("   and wm.ProjectKey = {0} ", lueProject.EditValue.ToString());

            if (!chkIsAllDate.Checked)
            {
                if (btnSetDate.Text == "Welding Date")
                    sql += string.Format("  and wm.WeldingDate between '{0}' and '{1}' ", dteFrom.DateTime.ToShortDateString(), dteTo.DateTime.ToShortDateString());
                else
                    sql += string.Format("  and wm.CreateDtm between '{0}' and '{1}' ", dteFrom.DateTime.ToShortDateString(), dteTo.DateTime.ToShortDateString());
            }

            if (!string.IsNullOrWhiteSpace(txtMachineSerialNo.Text))
                sql += string.Format("  and wm.MachineSerialNo = '{0}' ", txtMachineSerialNo.Text);

            if (!string.IsNullOrWhiteSpace(txtMaterial.Text))
                sql += string.Format("  and wm.Material = '{0}' ", txtMaterial.Text);

            if (!chkDiameter.Checked)
                sql += string.Format("  and ISNULL(wm.Diameter, 0) = {0} ", spnDiameter.Value);

            if (!Program.Option.isAdmin)
            {
                if (Program.Option.AuthLevel == 1)
                {
                    sql += $" and wm.WeldCreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                }
                else if (Program.Option.AuthLevel == 2)
                {
                    sql += $" and (wm.WeldCreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR wm.WeldCreateID = '{Program.Option.LoginID}') ";
                }
                else
                {
                    sql += $"  and wm.WeldCreateID = '{Program.Option.LoginID}' ";
                }
            }

            SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
            SplashScreenManager.Default.SetWaitFormDescription("The loading of data. Please wait....");

            dtWeldMaster = DBManager.Instance.GetDataTable(sql);
            grdWeldMaster.DataSource = dtWeldMaster;
            gridView1.BestFitColumns();
            gridView1.BestFitMaxRowCount = 100;

            SplashScreenManager.CloseForm(false);
        }

        /// <summary>
        /// 데이터 수
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_RowCountChanged(object sender, EventArgs e)
        {
            groupControl2.Text = string.Format(LangResx.Main.count_WeldMaster, gridView1.RowCount);
        }
        #endregion

        #region 엑셀저장
        /// <summary>
        /// 엑셀저장
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            targetFileName = GetFileName("xlsx", LangResx.Main.excel_WeldMaster);
            //if (targetFileName.Trim() != "")
            //    grdWeldMaster.ExportToXlsx(targetFileName, new XlsxExportOptionsEx { ExportType = ExportType.WYSIWYG });
            if (targetFileName.Trim() == "")
                return;

            //Export Excel (DataAware)
            XlsxExportOptionsEx options = new XlsxExportOptionsEx();
            options.ExportType = DevExpress.Export.ExportType.DataAware;
            options.AllowFixedColumnHeaderPanel = DevExpress.Utils.DefaultBoolean.False;
            options.ApplyFormattingToEntireColumn = DevExpress.Utils.DefaultBoolean.False;
            //options.ShowGridLines = false;

            try
            {
                grdWeldMaster.MainView.ExportToXlsx(targetFileName, options);
            }
            catch (Exception)
            {
            }
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

        private int ChoiceWeldKey;
        private int ChoiceBeadKey;
        /// <summary>
        /// 선택한 BeadMaster Key추출
        /// </summary>
        private void GetFocusedRowWeldMaster()
        {
            ColumnView view = (ColumnView)grdWeldMaster.FocusedView;
            try
            {
                object oWeldKey = view.GetRowCellValue(view.FocusedRowHandle, "WeldKey");
                ChoiceWeldKey = int.Parse(oWeldKey.ToString());

                object oBeadKey = view.GetRowCellValue(view.FocusedRowHandle, "BeadKey");
                ChoiceBeadKey = int.Parse(oBeadKey.ToString());

                btnOpenWeldMaster.Enabled = true;
                if (ChoiceBeadKey > 0)
                    btnOpenBeadMaster.Enabled = true;
                else
                    btnOpenBeadMaster.Enabled = false;
            }
            catch (Exception)
            {
            }
        }
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow row = gridView1.GetFocusedDataRow();
            if (row == null)
                return;

            if (Program.Option.DeleteWeldData)
                btnDelMaster.Enabled = true;
            GetFocusedRowWeldMaster();
        }

        #region 상세보기
        /// <summary>
        /// 더블클릭시 상세보기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdWeldMaster_DoubleClick(object sender, EventArgs e)
        {
            ViewDetail();
        }

        /// <summary>
        /// 상세보기
        /// </summary>
        private void ViewDetail()
        {
            FormPopWeldMaster frm = new FormPopWeldMaster();
            frm.ChoiceWeldKey = ChoiceWeldKey;
            frm.isCanBeadFormOpen = true;
            frm.Show();
        }

        /// <summary>
        /// 상세보기 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewDetail_Click(object sender, EventArgs e)
        {
            ViewDetail();
        }

        /// <summary>
        /// 검수정보 보기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenBeadMaster_Click(object sender, EventArgs e)
        {
            if (ChoiceBeadKey == 0)
                return;

            FormPopBeadMastercs frm = new FormPopBeadMastercs();
            frm.ChoiceBeadKey = ChoiceBeadKey;
            frm.isCanWeldFormOpen = false;
            frm.Show();
        }
        #endregion

        #region 삭제
        /// <summary>
        /// Master 삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelMaster_Click(object sender, EventArgs e)
        {
            if (!Program.Option.DeleteWeldData)
                return;

            int delCount = gridView1.SelectedRowsCount;
            if (delCount == 0)
                return;

            if (XtraMessageBox.Show(string.Format(LangResx.Main.msg_MasterDel1, delCount), "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            DeleteSelectedData(); //삭제
        }

        private List<string> QueryList;

        /// <summary>
        /// 선택된 Master 및 하위데이터 DB삭제
        /// </summary>
        private void DeleteSelectedData()
        {
            SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
            SplashScreenManager.Default.SetWaitFormDescription(LangResx.Main.msg_MasterDel2);

            gridView1.DeleteSelectedRows(); //그리드 삭제
            QueryList = new List<string>();       //Transaction Query생성용
            DataTable dtChanged;
            try
            {
                dtChanged = dtWeldMaster.GetChanges(DataRowState.Deleted);
                foreach (DataRow dr in dtChanged.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted)
                        MakeDeleteQuery(dr);
                }
            }
            catch (Exception)
            {
                return;
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(DBManager.Instance.MDB.ExcuteTransaction(QueryList.ToArray())))
                {
                    SplashScreenManager.CloseForm(false);
                    XtraMessageBox.Show(LangResx.Main.msg_MasterDelError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm(false);
                XtraMessageBox.Show(LangResx.Main.msg_MasterDelError + "\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            dtWeldMaster.AcceptChanges();

            gridView1.FocusedRowHandle = 0;
            GetFocusedRowWeldMaster();
            SplashScreenManager.CloseForm(false);
            XtraMessageBox.Show(LangResx.Main.msg_MasterDel3, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        /// <summary>
        /// Update Query생성
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="isCUD"></param>
        private void MakeDeleteQuery(DataRow dr)
        {
            string sql, weldKey;
            weldKey = dr["WeldKey", DataRowVersion.Original].ToString();

            //마스터 지우기
            sql = $"delete from WeldMaster where WeldKey = {weldKey} ";
            QueryList.Add(sql);
        }
        #endregion

        /// <summary>
        /// Diameter 셋팅
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkDiameter_CheckedChanged(object sender, EventArgs e)
        {
            spnDiameter.Enabled = !chkDiameter.Checked;
            spnDiameter.ReadOnly = chkDiameter.Checked;
        }

        /// <summary>
        /// Welding Date or Upload Date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetDate_Click(object sender, EventArgs e)
        {
            if (btnSetDate.Text == "Welding Date")
                btnSetDate.Text = "Upload Date";
            else
                btnSetDate.Text = "Welding Date";
        }

        /// <summary>
        /// 나가기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

		/// <summary>
		/// 프로젝트 전환
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnTransfer_Click(object sender, EventArgs e)
		{
			int transferCount = gridView1.SelectedRowsCount;
			if (transferCount == 0)
				return;

			FormPopProjectRelation frm = new FormPopProjectRelation(1, transferCount);
            frm.toProjName = lueProject.Text;
			if (frm.ShowDialog() == DialogResult.OK)
			{
				SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
				SplashScreenManager.Default.SetWaitFormDescription(LangResx.Main.msg_MasterTransfer2);

                transferID = frm.transferID;
                transferProject = frm.transferProject;
				gridView1.DeleteSelectedRows(); //그리드 삭제

				QueryList = new List<string>();       //Transaction Query생성용
				DataTable dtChanged;
				try
				{
					dtChanged = dtWeldMaster.GetChanges(DataRowState.Deleted);
					foreach (DataRow dr in dtChanged.Rows)
					{
						if (dr.RowState == DataRowState.Deleted)
							transferDataQuery(dr);
					}
				}
				catch (Exception)
				{
					return;
				}

				try
				{
					if (!string.IsNullOrWhiteSpace(DBManager.Instance.MDB.ExcuteTransaction(QueryList.ToArray())))
					{
						SplashScreenManager.CloseForm(false);
						XtraMessageBox.Show(LangResx.Main.msg_MasterTransferError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
						return;
					}
				}
				catch (Exception ex)
				{
					SplashScreenManager.CloseForm(false);
					XtraMessageBox.Show(LangResx.Main.msg_MasterTransferError + "\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				dtWeldMaster.AcceptChanges();
				gridView1.FocusedRowHandle = 0;
				GetFocusedRowWeldMaster();
				SplashScreenManager.CloseForm(false);
				XtraMessageBox.Show(LangResx.Main.msg_MasterTransfer3, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
		/// <summary>
		/// Update Query생성
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="isCUD"></param>
		private void transferDataQuery(DataRow dr)
		{
			string sql, weldKey;
			weldKey = dr["WeldKey", DataRowVersion.Original].ToString();

            //마스터 옮기기
            sql = "update WeldMaster ";
            sql += $" set CreateID = '{transferID}', ";
            sql += $"     projectKey = {transferProject} ";
            sql += $"where WeldKey = {weldKey} ";
			QueryList.Add(sql);
		}
	}
}