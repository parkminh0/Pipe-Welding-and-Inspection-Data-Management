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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.CodeParser;
using DevExpress.DataProcessing.InMemoryDataProcessor;
using static DevExpress.XtraEditors.Mask.MaskSettings;
using DevExpress.XtraRichEdit.Import.Html;
using static DevExpress.XtraBars.Docking2010.Views.BaseRegistrator;
using System.Windows.Media.Media3D;
using DevExpress.XtraRichEdit.API.Layout;

namespace SWSAnalyzer
{
    public partial class FormBeadAndWeldList : BaseForm
    {
        private DataTable dtBeadAndWeldList;
        private int ChoiceBeadKey;
        private int ChoiceWeldKey;

		private string SummaryName = "SummaryFormBeadAndWeldList.xml";
		private string FullLayoutName = "FullFormBeadAndWeldList.xml";

		#region 폼 로드
		/// <summary>
		/// 
		/// </summary>
		public FormBeadAndWeldList()
        {
            InitializeComponent();
		}

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormBeadAndWeldList_Load(object sender, EventArgs e)
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

            if (!Program.Option.SaveExcel) // 엑셀 권한
                btnExportExcel.Enabled = false;

			SetSkinStyle();
			CheckLayoutFile();
		}

		public override void SetSkinStyle()
		{
			if (Properties.Settings.Default.UserPalette.Contains("Dark"))
			{
			}
			else
			{
			}
		}

		/// <summary>
		/// 폼 쇼운
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormBeadAndWeldList_Shown(object sender, EventArgs e)
        {
            SetCompanyCode();
		}

		private void CheckLayoutFile()
        {
			string SummaryLayoutPath = Path.Combine(Program.constance.CommonFilePath, SummaryName);
			string FullLayoutPath = Path.Combine(Program.constance.CommonFilePath, FullLayoutName);

			if (File.Exists(SummaryLayoutPath))
                return;

            try
            {
				File.Copy(Path.Combine(Application.StartupPath, SummaryName), SummaryLayoutPath);
				File.Copy(Path.Combine(Application.StartupPath, FullLayoutName), FullLayoutPath);
			}
			catch (Exception)
            {
                throw;
            }
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

            if (dtCompany == null || dtCompany.Rows.Count == 0)
                return;

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
        /// 전체회사 선택
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
        /// 전체 기간 선택
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
                if (string.IsNullOrWhiteSpace(txtMachineSerialNo.Text) && string.IsNullOrWhiteSpace(txtMaterial.Text) && string.IsNullOrWhiteSpace(txtSerialNo.Text))
                {
                    XtraMessageBox.Show(LangResx.Main.msg_BeadWeldSearchAll, LangResx.Main.msg_title_SearchAll, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtMachineSerialNo.Focus();
                    return;
                }
            }

            SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
            SplashScreenManager.Default.SetWaitFormDescription("The loading of data. Please wait....");

            string sql = string.Empty;
            sql += "select * from ViewAllMasterList vm ";
            sql += "  left join project p ";
            if (btnSetDate.Text == "Inspect Date")
                sql += "    on vm.ProjectKey = p.ProjectKey ";
            else
                sql += "    on vm.wProjectKey = p.ProjectKey ";
            sql += " where 1 = 1 ";
            if (!chkIsAllCompany.Checked)
                sql += string.Format("   and p.CompKey = {0} ", lueCompany.EditValue);

            if (!chkIsAllProject.Checked)
                sql += string.Format("   and (vm.ProjectKey = {0} OR vm.wProjectKey = {0}) ", lueProject.EditValue.ToString());

            if (!chkIsAllDate.Checked)
            {
                if (btnSetDate.Text == "Inspect Date")
                    sql += string.Format("   and vm.InspectionDate BETWEEN '{0}' and '{1}' ", dteFrom.DateTime.ToString("yyyy-MM-dd"), dteTo.DateTime.ToString("yyyy-MM-dd"));
                else
                    sql += string.Format("   and vm.wWeldingDate BETWEEN '{0}' and '{1}' ", dteFrom.DateTime.ToString("yyyy-MM-dd"), dteTo.DateTime.ToString("yyyy-MM-dd"));
            }

            if (!string.IsNullOrWhiteSpace(txtSerialNo.Text))
                sql += string.Format("  and vm.SerialNo = '{0}' ", txtSerialNo.Text);

            if (!string.IsNullOrWhiteSpace(txtMaterial.Text))
                sql += string.Format("  and (vm.Material = '{0}' OR vm.WeldMaterial = '{0}') ", txtMaterial.Text);

            if (!string.IsNullOrWhiteSpace(txtMachineSerialNo.Text))
                sql += string.Format("  and (vm.MachineSerialNo = '{0}' OR vm.wMachineSerialNo = '{0}') ", txtMachineSerialNo.Text);

            if (!Program.Option.isAdmin)
            {
                if (Program.Option.AuthLevel == 1)
                {
                    sql += $" and (vm.WeldCreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                    sql += $"      OR vm.InspectCreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}')) ";
                }
                else if (Program.Option.AuthLevel == 2)
                {
                    sql += $" and (vm.WeldCreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR vm.InspectCreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR vm.WeldCreateID = '{Program.Option.LoginID}' OR vm.InspectCreateID = '{Program.Option.LoginID}') ";
                }
                else if (Program.Option.AuthLevel == 3)
                {
                    sql += $"  and (vm.WeldCreateID = '{Program.Option.LoginID}' or vm.InspectCreateID = '{Program.Option.LoginID}') ";
                }
            }

            dtBeadAndWeldList = DBManager.Instance.GetDataTable(sql);
            grdWeldMaster.DataSource = dtBeadAndWeldList;
            gridView1.BestFitColumns();
            gridView1.BestFitMaxRowCount = 100;

            SplashScreenManager.CloseForm(false);
        }

        /// <summary>
        /// 데이터 건수 표기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_RowCountChanged(object sender, EventArgs e)
        {
            GridView view = sender as GridView;
            int WeldQty = 0;
            int BeadQty = 0;
            int MatchQty = 0;
            for (int r = 0; r < view.RowCount; r++)
            {
                if (!string.IsNullOrEmpty(view.GetRowCellValue(r, "WeldKey").ToString()))
                {
                    WeldQty++;
                }

                if (!string.IsNullOrEmpty(view.GetRowCellValue(r, "BeadKey").ToString()))
                {
                    BeadQty++;
                }

                if (!string.IsNullOrEmpty(view.GetRowCellValue(r, "WeldKey").ToString()) && !string.IsNullOrEmpty(view.GetRowCellValue(r, "BeadKey").ToString()))
                {
                    MatchQty++;
                }
            }

            groupControl2.Text = string.Format(LangResx.Main.count_BeadWeld, gridView1.RowCount);
            txtWeld.Text = string.Format($"{WeldQty:n0}");
            txtBead.Text = string.Format($"{BeadQty:n0}");
            txtMatching.Text = string.Format($"{MatchQty:n0}");
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
            targetFileName = GetFileName("xlsx", LangResx.Main.excel_BeadWeld);
            if (targetFileName.Trim() != "")
                grdWeldMaster.ExportToXlsx(targetFileName, new XlsxExportOptionsEx { ExportType = ExportType.WYSIWYG });
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
        /// 선택한 BeadMaster Key추출
        /// </summary>
        private void GetFocusedRowWeldMaster()
        {
            ColumnView view = (ColumnView)grdWeldMaster.FocusedView;
            try
            {
                object oBeadKey = view.GetRowCellValue(view.FocusedRowHandle, "BeadKey");
                ChoiceBeadKey = int.Parse(oBeadKey.ToString());

                if (ChoiceBeadKey > 0)
                    btnOpenBeadMaster.Enabled = true;
                else
                    btnOpenBeadMaster.Enabled = false;
            }
            catch (Exception)
            {
                btnOpenBeadMaster.Enabled = false;
                ChoiceBeadKey = 0;
            }

            try
            {
                object oWeldKey = view.GetRowCellValue(view.FocusedRowHandle, "WeldKey");
                ChoiceWeldKey = int.Parse(oWeldKey.ToString());

                if (ChoiceWeldKey > 0)
                    btnOpenWeldMaster.Enabled = true;
                else
                    btnOpenWeldMaster.Enabled = false;
            }
            catch (Exception)
            {
                btnOpenWeldMaster.Enabled = false;
                ChoiceWeldKey = 0;
            }

        }

        /// <summary>
        /// 그리드 선택시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow row = gridView1.GetFocusedDataRow();
            if (row == null)
                return;

            GetFocusedRowWeldMaster();
        }

        /// <summary>
        /// 검수상세보기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenBeadMaster_Click(object sender, EventArgs e)
        {
            if (ChoiceBeadKey == 0)
                return;

            FormPopBeadMastercs frm = new FormPopBeadMastercs();
            frm.ChoiceBeadKey = ChoiceBeadKey;
            frm.isCanWeldFormOpen = true;
            frm.Show();
        }

        /// <summary>
        /// 융착상세보기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenWeldMaster_Click(object sender, EventArgs e)
        {
            if (ChoiceWeldKey == 0)
                return;

            FormPopWeldMaster frm = new FormPopWeldMaster();
            frm.ChoiceWeldKey = ChoiceWeldKey;
            frm.isCanBeadFormOpen = true;
            frm.Show();
        }

        /// <summary>
        /// 구분선 생성
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
			if (btnGridSummary.Text == LangResx.Main.btn_viewall)
			{
                return;
			}

			if (gridView1.GetDataRow(e.RowHandle) == null)
                return;

            if (!(e.Column.FieldName == "InspectCreateID"))
                return;

            GridView view = sender as GridView;

           
            CellDrawHelper.DoDefaultDrawCell(view, e);
            
            if (Properties.Settings.Default.UserPalette.Contains("Dark"))
				CellDrawHelper.DrawCellBorderWhite(e);
            else
			    CellDrawHelper.DrawCellBorderBlack(e);

			e.Handled = true;
        }

        /// <summary>
        /// 요구사항 - 버튼 클릭 시 LineNo(검수데이터) 컬럼으로 이동
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            gridView1.FocusedColumn = gridView1.Columns["LineNo"];
            gridView1.FocusedColumn = gridView1.Columns["WelderCode"];
        }

        /// <summary>
        /// Inspect Date or Upload Date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetDate_Click(object sender, EventArgs e)
        {
            if (btnSetDate.Text == "Inspect Date")
                btnSetDate.Text = "Welding Date";
            else
                btnSetDate.Text = "Inspect Date";
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
		/// 간략보기
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnGridSummary_Click(object sender, EventArgs e)
		{
		    string SummaryLayoutPath = Path.Combine(Program.constance.CommonFilePath, SummaryName);
		    string FullLayoutPath = Path.Combine(Program.constance.CommonFilePath, FullLayoutName);
			//gridView1.SaveLayoutToXml(LayoutPath);
			if (btnGridSummary.Text == LangResx.Main.btn_viewsummary)
            {
                btnGridSummary.Text = LangResx.Main.btn_viewall;
				try
				{
					if (File.Exists(SummaryLayoutPath))
						gridView1.RestoreLayoutFromXml(SummaryLayoutPath); //간략보기
				}
				catch (Exception)
				{
				}		   
			}
            else
            {
                btnGridSummary.Text = LangResx.Main.btn_viewsummary;
				try
				{
					if (File.Exists(FullLayoutPath))
						gridView1.RestoreLayoutFromXml(FullLayoutPath); //전체보기
				}
				catch (Exception)
				{
				}
			}

			gridView1.BestFitColumns();
			gridView1.BestFitMaxRowCount = 100;
		}
	}
}