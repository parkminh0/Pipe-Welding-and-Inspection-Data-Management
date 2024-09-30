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
using DevExpress.XtraGrid.Views.Base;
using System.IO;
using DevExpress.XtraPrinting;
using DevExpress.Export;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeMap;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid;

namespace SWSAnalyzer
{
    public partial class FormBeadMasterManage : DevExpress.XtraEditors.XtraForm
    {
        private DataTable dtBeadMaster;
        private DataTable dtBeadDetail;
        private int ChoiceBeadKey;
        private int ChoiceWeldKey; //Link용
        private int ChoiceBeadDetailKey; // 비드상세 선택 시 바인딩됨,  [상세, sunburst에 사용]

        #region 폼 로드
        /// <summary>
        /// 
        /// </summary>
        public FormBeadMasterManage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormBeadMasterManage_Load(object sender, EventArgs e)
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
                btnExportExcel.Enabled = btnExportExcellDetail.Enabled = false;

            if (!Program.Option.DeleteInspectData)
                btnDelMaster.Enabled = btnDelDetail.Enabled = false;
        }

        /// <summary>
        /// 폼 쇼운
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormBeadMasterManage_Shown(object sender, EventArgs e)
        {
            bandedGridView1.OptionsView.ShowColumnHeaders = false;
            bandedGridView1.OptionsView.ShowBands = true;
            SetCompanyCode();
            ShowSunburstPosition(0);
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
        #endregion

        #region 비드검수 목록 조회
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

        /// <summary>
        /// 검색 수행
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (chkIsAllDate.Checked)
            {
                if (string.IsNullOrWhiteSpace(txtSerialNo.Text) && string.IsNullOrWhiteSpace(txtMaterial.Text))
                {
                    XtraMessageBox.Show(LangResx.Main.msg_BeadSearchAll, LangResx.Main.msg_title_SearchAll, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSerialNo.Focus();
                    return;
                }
            }

            string sql = LogicManager.Common.GetSelectBeadMasterSql(); //Query문장 가져오기

            if (!chkIsAllCompany.Checked)
                sql += string.Format("   and c.CompKey = {0} ", lueCompany.EditValue);

            if (!chkIsAllProject.Checked)
                sql += string.Format("   and p.ProjectKey = {0} ", lueProject.EditValue);

            if (!chkIsAllDate.Checked)
            {
                if (btnSetDate.Text == "Inspect Date")
                    sql += string.Format("   and bm.InspectionDate BETWEEN '{0}' and '{1}' ", dteFrom.DateTime.ToShortDateString(), dteTo.DateTime.ToShortDateString());

                else
                    sql += string.Format("   and bm.CreateDtm BETWEEN '{0}' and '{1}' ", dteFrom.DateTime.ToShortDateString(), dteTo.DateTime.ToShortDateString());
            }

            if (!string.IsNullOrWhiteSpace(txtSerialNo.Text))
                sql += string.Format("   and SerialNo = '{0}' ", txtSerialNo.Text);

            if (!string.IsNullOrWhiteSpace(txtMaterial.Text))
                sql += string.Format("   and Material = '{0}' ", txtMaterial.Text);

            if (!Program.Option.isAdmin)
            {
                if (Program.Option.AuthLevel == 1)
                {
                    sql += $" and (wm.CreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                    sql += $"      OR bm.CreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}')) ";
                }
                else if (Program.Option.AuthLevel == 2)
                {
                    sql += $" and (wm.CreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR bm.CreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR wm.CreateID = '{Program.Option.LoginID}' OR bm.CreateID = '{Program.Option.LoginID}') ";
                }
                else
                {
                    sql += $"  and (wm.CreateID = '{Program.Option.LoginID}' or bm.CreateID = '{Program.Option.LoginID}') ";
                }
            }

            SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
            SplashScreenManager.Default.SetWaitFormDescription("The loading of data. Please wait....");

            dtBeadMaster = DBManager.Instance.GetDataTable(sql);
            grdBeadMaster.DataSource = dtBeadMaster;

            SplashScreenManager.CloseForm(false);
        }

        /// <summary>
        /// 데이터 건수 출력
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bandedGridView1_RowCountChanged(object sender, EventArgs e)
        {
            groupControl1.Text = string.Format(LangResx.Main.count_BeadMaster, bandedGridView1.RowCount);
        }
        #endregion

        #region 포커스

        /// <summary>
        /// 선택한 비드검수 목록 Key 추출
        /// </summary>
        private void GetFocusedRowBeadMaster()
        {
            ColumnView view = (ColumnView)grdBeadMaster.FocusedView;
            try
            {
                object oBeadKey = view.GetRowCellValue(view.FocusedRowHandle, "BeadKey");
                ChoiceBeadKey = int.Parse(oBeadKey.ToString());

                object oWeldKey = view.GetRowCellValue(view.FocusedRowHandle, "WeldKey");
                ChoiceWeldKey = int.Parse(oWeldKey.ToString());

                GetBeadDetailList();
                btnViewDetail.Enabled = true;
                if (Program.Option.DeleteInspectData)
                {
                    if (ChoiceWeldKey > 0)
                        btnOpenWeldMaster.Enabled = true;
                    else
                        btnOpenWeldMaster.Enabled = false;
                }
                    
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 선택한 BeadKey에 해당한 상세데이터 불러오기
        /// </summary>
        private void GetBeadDetailList()
        {
            string sql = string.Empty;
            sql += "SELECT [BeadDetailKey] ";
            sql += "      ,[LineNo] ";
            sql += "      ,[SerialNo] ";
            sql += "      ,[InspectionNo] ";
            sql += "      ,[InspectionDate] ";
            sql += "      ,[InspectionTime] ";
            sql += "      ,[Position] ";
            sql += "      ,[isPass] ";
            sql += "      ,[KValue] ";
            sql += "      ,[isKPass] ";
            sql += "      ,[BeadWidthTotal] ";
            sql += "      ,[isBWidthPass] ";
            sql += "      ,[B1Width] ";
            sql += "      ,[B2Width] ";
            sql += "      ,[BRatio] ";
            sql += "      ,[isBRatioPass] ";
            sql += "      ,[MissAlignValue] ";
            sql += "      ,[isMissAlignPass] ";
            sql += "      ,[NotchValue] ";
            sql += "      ,[isNotchPass] ";
            sql += "      ,[ContactAngle] ";
            sql += "  FROM [dbo].[BeadDetail] ";
            sql += " where BeadKey = " + ChoiceBeadKey;
            sql += " order by Position";
            dtBeadDetail = DBManager.Instance.GetDataTable(sql);
            grdBeadDetail.DataSource = dtBeadDetail;
            gridView2.BestFitColumns();
        }

        /// <summary>
        /// 데이터 건수 출력
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView2_RowCountChanged(object sender, EventArgs e)
        {
            groupControl2.Text = string.Format(LangResx.Main.count_BeadDetail, gridView2.RowCount);
        }

        /// <summary>
        /// 선택한 비드검수 상세 Key 추출
        /// </summary>
        private void GetFocusedRowBeadDetail()
        {
            ColumnView view = (ColumnView)grdBeadDetail.FocusedView;
            try
            {
                object oBeadDetailKey = view.GetRowCellValue(view.FocusedRowHandle, "BeadDetailKey");
                ChoiceBeadDetailKey = int.Parse(oBeadDetailKey.ToString());
                ChoiceSunburstBeadDetailKey = ChoiceBeadDetailKey;

                object oPosition = view.GetRowCellValue(view.FocusedRowHandle, "Position");
                int pos = int.Parse(oPosition.ToString());

                ShowBeadImage(); //Image 보여주기
                ShowSunburstPosition(pos);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 포커스
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow row = bandedGridView1.GetFocusedDataRow();
            if (row == null)
                return;

            if (Program.Option.DeleteInspectData)
                btnDelMaster.Enabled = true;
            GetFocusedRowBeadMaster();
            GetFocusedRowBeadDetail();
        }
        private void gridView2_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            DataRow row = gridView2.GetFocusedDataRow();
            if (row == null)
                return;

            GetFocusedRowBeadDetail();
        }

        /// <summary>
        /// 검수상세 팝업
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdBeadMaster_DoubleClick(object sender, EventArgs e)
        {
            ViewDetail();
        }
        private void btnViewDetail_Click(object sender, EventArgs e)
        {
            ViewDetail(); //Double Click과 동일
        }
        private void ViewDetail()
        {
            FormPopBeadMastercs frm = new FormPopBeadMastercs();
            frm.ChoiceBeadKey = ChoiceBeadKey;
            frm.isCanWeldFormOpen = true;
            frm.Show();
        }

        /// <summary>
        /// 융착상세 팝업
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenWeldMaster_Click(object sender, EventArgs e)
        {
            if (ChoiceWeldKey == 0)
                return;

            FormPopWeldMaster frm = new FormPopWeldMaster();
            frm.ChoiceWeldKey = ChoiceWeldKey;
            frm.isCanBeadFormOpen = false;
            frm.Show();
        }
        #endregion

        #region 이미지 & Sunburst
        /// <summary>
        /// 이미지 읽어와 보여주기
        /// </summary>
        private void ShowBeadImage()
        {
            byte[] beadImage = LogicManager.Common.GetBeadImage(ChoiceBeadDetailKey, rdoImageType.SelectedIndex);
            MemoryStream ms = null;
            if (beadImage != null)
            {
                ms = new MemoryStream(beadImage);
                picBeadImage.Image = new Bitmap(ms);
            }
            else
            {
                picBeadImage.Image = null;
            }
        }

        private List<Color> colorList;
        private int _AngleUnit = Program.constance.AngleUnit; //각도 단위(10으로 수정해야 함)
        private DataTable dtAngleData;
        private void ShowSunburstPosition(int pos)
        {
            //SunburstFlatDataAdapter
            colorList = new List<Color>();

            SunburstFlatDataAdapter dataAdapter = new SunburstFlatDataAdapter();
            dataAdapter.ValueDataMember = "Angle";
            dataAdapter.LabelDataMember = "Position";
            dataAdapter.GroupDataMembers.AddRange(new string[] { "Category" });
            dataAdapter.DataSource = CreateAngleInfos(pos);
            sunburstControl1.DataAdapter = dataAdapter;
            sunburstControl1.StartAngle = -90;
            sunburstControl1.HoleRadiusPercent = 60;

            //SunburstGradientColorizer // SunburstPaletteColorizer
            ((SunburstGradientColorizer)sunburstControl1.Colorizer).Palette = new Palette(colorList.ToArray());
        }

        /// <summary>
        /// Sunburst 데이터 생성
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private List<SunBurstPosition> CreateAngleInfos(int currPos)
        {
            int angleCount = 360; //각도 갯수
            double tc = 0, fc = 0, tot = angleCount / _AngleUnit; // (360/5 = 72) -> (360/10 = 36)
            List<SunBurstPosition> data = new List<SunBurstPosition>();
            int status = 0, pos = 0, idx = 0;

            //전체 각도 Data불러오기
            dtAngleData = DBManager.Instance.MDB.uspGetBeadDetailByAngle(ChoiceBeadKey, _AngleUnit, 0, 1);
            double tmp = 10;
            foreach (DataRow dr in dtAngleData.Rows)
            {
                idx++;
                status = int.Parse(dr["Status"].ToString());
                pos = int.Parse(dr["Angle"].ToString()); // 각도
                switch (status)
                {
                    case 1:  //True
                        tc++;
                        colorList.Add(Color.SteelBlue);
                        break;
                    case 2:  //False
                        fc++;
                        colorList.Add(Color.Firebrick);
                        break;
                    default: //Null
                        colorList.Add(Color.AntiqueWhite);
                        break;
                }

                SunBurstPosition one = new SunBurstPosition { Position = pos, Angle = tmp, Category = idx, Status = status }; //Angle = _AngleUnit
                data.Add(one);
                if (idx == 1)
                    choiceAngleData = one;
                tmp -= 0.0000001;
            }

            double inspRatio = (tc + fc) / tot * 100.0; // 총 검수 비율
            double passRatio = tc / (tc + fc) * 100.0; // OK 비율
            double nonPassRatio = fc / (tc + fc) * 100.0; // NOK 비율
            sunburstControl1.CenterLabel.TextPattern = string.Format("Inspected Rate: {0:N1}%\r\nOK: {1:N1}%\r\nNOK: {2:N1}%", inspRatio, passRatio, nonPassRatio); // sunburst center 라벨

            return data;
        }

        private SunBurstPosition choiceAngleData;

        /// <summary>
        /// 마우스 클릭시 해당 위치데이터값 찾기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sunburstControl1_MouseClick(object sender, MouseEventArgs e)
        {
            Color itemColor = Color.Empty;
            SunburstHitInfo shi = sunburstControl1.CalcHitInfo(sunburstControl1.PointToClient(MousePosition));

            if (shi.InSunburstItem)
            {
                try
                {
                    choiceAngleData = (SunBurstPosition)shi.SunburstItem.Tag;
                    ShowClickData();
                }
                catch (Exception ex)
                {
                    foreach (var childItem in shi.SunburstItem.Children)
                    {
                        choiceAngleData = (SunBurstPosition)childItem.Tag;
                        ShowClickData();
                    }
                }
            }
        }

        private int ChoiceSunburstBeadDetailKey;
        /// <summary>
        /// 클릭한 위치 데이터 목록
        /// </summary>
        private void ShowClickData()
        {
            int choiceAngle = choiceAngleData.Position;
            if (choiceAngle == 0)
                return;

            try
            {
                //해당 각도의 Data불러오기
                DataTable dtClickAngleData = DBManager.Instance.MDB.uspGetBeadDetailByAngle(ChoiceBeadKey, _AngleUnit, choiceAngle, 0);
                //grdBeadDetail.DataSource = dtClickAngleData;
                ChoiceSunburstBeadDetailKey = int.Parse(dtClickAngleData.Rows[dtClickAngleData.Rows.Count - 1]["BeadDetailKey"].ToString());
                int ChoicePosition = int.Parse(dtClickAngleData.Rows[dtClickAngleData.Rows.Count - 1]["Position"].ToString());
                ChoiceBeadDetailKey = ChoiceSunburstBeadDetailKey;
                ColumnView View = (ColumnView)grdBeadDetail.FocusedView;
                GridColumn column = View.Columns["Position"];
                if (column != null)
                {
                    // locating the row
                    int rhFound = View.LocateByDisplayText(0, column, ChoicePosition.ToString());
                    // focusing the cell
                    if (rhFound != GridControl.InvalidRowHandle)
                    {
                        int[] sRows = View.GetSelectedRows();
                        foreach (int handle in sRows)
                            View.UnselectRow(handle);

                        View.SelectRow(rhFound);
                        View.FocusedRowHandle = rhFound;
                        View.FocusedColumn = column;
                    }
                }
                //int getFocus =
                ShowSunburstBeadImage();
            }
            catch (Exception)
            {
                ChoiceBeadDetailKey = ChoiceSunburstBeadDetailKey = 0;
            }
        }

        /// <summary>
        /// 이미지 읽어와 보여주기
        /// </summary>
        private void ShowSunburstBeadImage()
        {
            //Image
            byte[] beadImage = LogicManager.Common.GetBeadImage(ChoiceSunburstBeadDetailKey, rdoImageType.SelectedIndex);
            MemoryStream ms = null;
            if (beadImage != null)
            {
                ms = new MemoryStream(beadImage);
                picBeadImage.Image = new Bitmap(ms);
            }
            else
            {
                picBeadImage.Image = null;
            }
        }

        /// <summary>
        /// 합성/원본 이미지 선택
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoImageType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowBeadImage(); //Image 보여주기
        }
        #endregion

        #region Excel
        /// <summary>
        /// 엑셀저장
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            bandedGridView1.Columns["CreateDtm"].Visible = false;
            bandedGridView1.Bands["gridBand100"].Visible = false;
            targetFileName = GetFileName("xlsx", LangResx.Main.excel_BeadMaster);
            //if (targetFileName.Trim() != "")
            //    grdBeadMaster.ExportToXlsx(targetFileName, new XlsxExportOptionsEx { ExportType = ExportType.WYSIWYG });
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
                grdBeadMaster.MainView.ExportToXlsx(targetFileName, options);
            }
            catch (Exception)
            {
            }

            bandedGridView1.Columns["CreateDtm"].Visible = true;
            bandedGridView1.Bands["gridBand100"].Visible = true;
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

        /// <summary>
        /// 상세데이터 엑셀저장
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcellDetail_Click(object sender, EventArgs e)
        {
            targetFileName = GetFileName("xlsx", LangResx.Main.excel_BeadDetail);
            if (targetFileName.Trim() != "")
                grdBeadDetail.ExportToXlsx(targetFileName, new XlsxExportOptionsEx { ExportType = ExportType.WYSIWYG });
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
            int delCount = bandedGridView1.SelectedRowsCount;
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

            bandedGridView1.DeleteSelectedRows(); //그리드 삭제
            QueryList = new List<string>();       //Transaction Query생성용
            DataTable dtChanged;
            try
            {
                dtChanged = dtBeadMaster.GetChanges(DataRowState.Deleted);
                foreach (DataRow dr in dtChanged.Rows)
                {
                    if (dr.RowState == DataRowState.Deleted)
                    {
                        MakeDeleteQuery(dr);
                    }
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

            dtBeadMaster.AcceptChanges();

            bandedGridView1.FocusedRowHandle = 0;
            GetFocusedRowBeadMaster();
            GetFocusedRowBeadDetail();
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
            string sql, beadKey;
            beadKey = dr["BeadKey", DataRowVersion.Original].ToString();

            //1.BeadImageInfo 지우기
            sql = string.Empty;
            sql += "DELETE BeadImageInfo ";
            sql += "  from BeadImageInfo x ";
            sql += "  join BeadDetail y ";
            sql += "    on x.BeadDetailKey = y.BeadDetailKey ";
            sql += $" where y.BeadKey = {beadKey} ";
            QueryList.Add(sql);

            //2.상세데이터 지우기
            sql = $"delete from BeadDetail where BeadKey = {beadKey} ";
            QueryList.Add(sql);

            //3.마스터 지우기
            sql = $"delete from BeadMaster where BeadKey = {beadKey} ";
            QueryList.Add(sql);
        }
        #endregion

        #region grid 설정
        /// <summary>
        ///  Show Column Header 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkShowHeader_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkShowHeader.Checked && !chkShowBands.Checked)
                chkShowBands.Checked = true;

            ShowHeader();
        }
        private void ShowHeader()
        {
            bandedGridView1.OptionsView.ShowColumnHeaders = chkShowHeader.Checked;
            bandedGridView1.OptionsView.ShowBands = chkShowBands.Checked;
        }

        private void chkShowBands_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkShowHeader.Checked && !chkShowBands.Checked)
                chkShowHeader.Checked = true;

            ShowHeader();
        }
        #endregion

        #region 닫기
        /// <summary>
        /// 나가기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        /// <summary>
        /// Inspect Date or Upload Date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetDate_Click(object sender, EventArgs e)
        {
            if (btnSetDate.Text == "Inspect Date")
                btnSetDate.Text = "Upload Date";
            else
                btnSetDate.Text = "Inspect Date";
        }

        /// <summary>
        /// 화면 사이즈 조정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormBeadMasterManage_Resize(object sender, EventArgs e)
        {
            splitContainerControl2.SplitterPosition = splitContainerControl2.Size.Height * 60 / 100;
            splitContainerControl3.SplitterPosition = splitContainerControl3.Size.Width * 60 / 100;
            splitContainerControl4.SplitterPosition = splitContainerControl4.Size.Width / 2;
        }
	}
}