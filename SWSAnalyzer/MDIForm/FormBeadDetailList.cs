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
using DevExpress.XtraTreeMap;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraGrid.Views.Base;
using System.IO;
using System.Collections;

namespace SWSAnalyzer
{
    public partial class FormBeadDetailList : DevExpress.XtraEditors.XtraForm
    {
        private DataTable dtBeadMaster;

        #region 폼 로드
        /// <summary>
        /// 
        /// </summary>
        public FormBeadDetailList()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormBeadDetailList_Load(object sender, EventArgs e)
        {
            dteFrom.DateTime = DateTime.Now.AddMonths(-1);
            dteTo.DateTime = DateTime.Now;
            CreateGridDataTable();

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
        /// 결과 출력용 그리드 테이블 생성
        /// </summary>
        private DataTable CreateGridDataTable()
        {
            DataTable dtTemplete = new DataTable();
            DataColumn col;
            col = new DataColumn("Cat", typeof(string));
            dtTemplete.Columns.Add(col);
            col = new DataColumn("KValue", typeof(string));
            dtTemplete.Columns.Add(col);
            col = new DataColumn("BWidth", typeof(string));
            dtTemplete.Columns.Add(col);
            col = new DataColumn("UnevenB1", typeof(string));
            dtTemplete.Columns.Add(col);
            col = new DataColumn("UnevenB2", typeof(string));
            dtTemplete.Columns.Add(col);
            col = new DataColumn("UnevenRatio", typeof(string));
            dtTemplete.Columns.Add(col);
            col = new DataColumn("MissAlign", typeof(string));
            dtTemplete.Columns.Add(col);
            col = new DataColumn("Notch", typeof(string));
            dtTemplete.Columns.Add(col);
            col = new DataColumn("Status", typeof(int));
            dtTemplete.Columns.Add(col);

            return dtTemplete;
        }

        /// <summary>
        /// 폼 쇼운
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormBeadDetailList_Shown(object sender, EventArgs e)
        {
            SetCompanyCode();
            ShowSunburstPosition();
        }

        /// <summary>
        /// 화면 사이즈 조정.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormBeadDetailList_Resize(object sender, EventArgs e)
        {
            splitContainerControl3.SplitterPosition = splitContainerControl3.Size.Width * 38 / 100;
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
        /// 회사 선택에 따른 프로젝트 룩업 셋팅
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

        /// <summary>
        /// 검색
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
                sql += string.Format("   and p.ProjectKey = {0} ", lueProject.EditValue.ToString());

            if (!chkIsAllDate.Checked)
                sql += string.Format("   and bm.InspectionDate BETWEEN '{0}' and '{1}' ", dteFrom.DateTime.ToShortDateString(), dteTo.DateTime.ToShortDateString());

            if (!string.IsNullOrWhiteSpace(txtSerialNo.Text))
                sql += string.Format("   and SerialNo = '{0}' ", txtSerialNo.Text);

            if (!string.IsNullOrWhiteSpace(txtMaterial.Text))
                sql += string.Format("   and Material = '{0}' ", txtMaterial.Text);

            if (!string.IsNullOrWhiteSpace(txtInspectionNo.Text))
                sql += string.Format("   and InspectionNo = {0} ", txtInspectionNo.Text);

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
        /// 데이터 건수
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_RowCountChanged(object sender, EventArgs e)
        {
            groupControl1.Text = string.Format(LangResx.Main.count_BeadMaster, gridView1.RowCount);
        }
        #endregion

        #region 데이터 선택 시 상세내역 출력
        private int ChoiceBeadKey;
        private DataRow ChoiceRow;

        /// <summary>
        /// 선택한 BeadMaster Key추출
        /// </summary>
        private void GetFocusedRowBeadMaster()
        {
            ColumnView view = (ColumnView)grdBeadMaster.FocusedView;
            try
            {
                object oBeadKey = view.GetRowCellValue(view.FocusedRowHandle, "BeadKey");
                ChoiceBeadKey = int.Parse(oBeadKey.ToString());

                ShowSunburstPosition();
                ChoiceRow = view.GetFocusedDataRow(); //DataRow 
                ShowMasterInfo();
                ShowMasterInfoGridData();

                rdoImageType.Enabled = true;
            }
            catch (Exception)
            {
                rdoImageType.Enabled = false;
            }
        }
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataRow row = gridView1.GetFocusedDataRow();
            if (row == null)
                return;

            GetFocusedRowBeadMaster();
        }

        #region Sunburst
        private List<Color> colorList;
        private int _AngleUnit = Program.constance.AngleUnit; //각도 단위(10으로 수정해야 함)
        private DataTable dtAngleData;
        /// <summary>
        /// 각도별 데이터 보여주기
        /// </summary>
        /// <param name="v"></param>
        private void ShowSunburstPosition()
        {
            //SunburstFlatDataAdapter
            colorList = new List<Color>();

            SunburstFlatDataAdapter dataAdapter = sunburstControl1.DataAdapter as SunburstFlatDataAdapter;
            dataAdapter.ValueDataMember = "Angle"; //10
            dataAdapter.LabelDataMember = "Position";
            dataAdapter.GroupDataMembers.AddRange(new string[] { "Category" });
            dataAdapter.DataSource = CreateAngleInfos();
            sunburstControl1.DataAdapter = dataAdapter;
            sunburstControl1.StartAngle = -90;
            sunburstControl1.HoleRadiusPercent = 60;
            ((SunburstGradientColorizer)sunburstControl1.Colorizer).Palette = new Palette(colorList.ToArray());

            ShowClickData(); //Sunburst 첫번째 각도를 클릭한 효과주기
        }
        private List<SunBurstPosition> CreateAngleInfos() // position category project angle status
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

        private int ChoiceBeadDetailKey;
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
                grdBeadDetail.DataSource = dtClickAngleData;
                gridView2.BestFitColumns();
                ChoiceBeadDetailKey = int.Parse(dtClickAngleData.Rows[dtClickAngleData.Rows.Count - 1]["BeadDetailKey"].ToString());
                ShowBeadImage();
            }
            catch (Exception)
            {
                ChoiceBeadDetailKey = 0;
            }
        }

        /// <summary>
        /// 이미지 읽어와 보여주기
        /// </summary>
        private void ShowBeadImage()
        {
            //Text 출력
            txtOutAngle.Text = choiceAngleData.Position.ToString();
            if (choiceAngleData.Status == 1)
                txtOutIsPass.Text = "True";
            else if (choiceAngleData.Status == 2)
                txtOutIsPass.Text = "False";
            else
                txtOutIsPass.Text = string.Empty;

            if (choiceAngleData.Status == 2)
                txtOutIsPass.ForeColor = Color.Firebrick;
            else
                txtOutIsPass.ForeColor = Color.DarkGreen;

            //Image
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

        private void rdoImageType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowBeadImage();
        }
        #endregion

        /// <summary>
        /// 검수 상세내역 텍스트 채우기
        /// </summary>
        private void ShowMasterInfo()
        {
            txtOutInpsectionNo.Text = string.Format("{0}-{1}-{2}", ChoiceRow["SerialNo"], ChoiceRow["InspectionDate"].ToString().Substring(0,10).Replace("-",""), ChoiceRow["InspectionNo"]);
            txtOutMaterial.Text = string.Format("{0}", ChoiceRow["Material"]);
            txtOutOD.Text = string.Format("{0}", ChoiceRow["OD"]);
            txtOutSDR.Text = string.Format("{0}", ChoiceRow["SDR"]);
            txtOutWallThickness.Text = string.Format("{0}", ChoiceRow["WallThickness"]);
            txtOutDVS.Text = string.Format("{0}", ChoiceRow["DVS"]);
            txtOutScore.Text = string.Format("{0}", ChoiceRow["VIScore"]);
            txtOutGrade.Text = string.Format("{0}", ChoiceRow["VIGrade"]);
            txtOutIsNotchEyes.Text = string.Format("{0}", ChoiceRow["isNotchEyes"]);
            txtOutNotchNote.Text = string.Format("{0}", ChoiceRow["NotchNote"]);
            txtOutIsAngularDevEyes.Text = string.Format("{0}", ChoiceRow["isAngularDevEyes"]);
            txtOutAngularDevNote.Text = string.Format("{0}", ChoiceRow["AngularDevNote"]);
            txtOutIsCrackEyes.Text = string.Format("{0}", ChoiceRow["isCrackEyes"]);
            txtOutCrackNote.Text = string.Format("{0}", ChoiceRow["CrackNote"]);
            txtOutIsVoidEyes.Text = string.Format("{0}", ChoiceRow["isVoidEyes"]);
            txtOutVoidNote.Text = string.Format("{0}", ChoiceRow["VoidNote"]);
            txtOutIsSupportPadEyes.Text = string.Format("{0}", ChoiceRow["isSupportPadEyes"]);
            txtOutSupportPadNote.Text = string.Format("{0}", ChoiceRow["SupportPadNote"]);
            txtOutIsInterruptionEyes.Text = string.Format("{0}", ChoiceRow["isInterruptionEyes"]);
            txtOutInterruptionNote.Text = string.Format("{0}", ChoiceRow["InterruptionNote"]);
            txtOutIsOverheatingEyes.Text = string.Format("{0}", ChoiceRow["isOverheatingEyes"]);
            txtOutOverheatingNote.Text = string.Format("{0}", ChoiceRow["OverheatingNote"]);
            txtOutIsEtc.Text = txtOutEtcNote.Text = string.Empty;
        }

        /// <summary>
        /// 마스터정보를 그리드 형태로 보여주기
        /// </summary>
        private void ShowMasterInfoGridData()
        {
            DataTable dtGridData = CreateGridDataTable();
            DataRow row;

            //Div. (구분)
            row = dtGridData.NewRow();
            row["Cat"] = string.Format("{0}", "Std.Value");
            row["KValue"] = string.Format("K ≥ {0}", ChoiceRow["K_C"]);
            row["BWidth"] = string.Format("{0}~{1}", ChoiceRow["BMin_C"], ChoiceRow["BMax_C"]);
            row["UnevenB1"] = string.Format("{0}", "-");
            row["UnevenB2"] = string.Format("{0}", "-");
            row["UnevenRatio"] = string.Format("{0:P2}", ChoiceRow["BRatio_C"]);
            row["MissAlign"] = string.Format("{0}", ChoiceRow["MissAlign_C"]);
            row["Notch"] = string.Format("{0}", ChoiceRow["Notch_C"]);
            row["Status"] = 1;
            dtGridData.Rows.Add(row);

            //Min 
            row = dtGridData.NewRow();
            row["Cat"] = string.Format("{0}", "Min");
            row["KValue"] = string.Format("{0:F2}", ChoiceRow["KMin"]);
            row["BWidth"] = string.Format("{0:F2}", ChoiceRow["BMin"]);
            row["UnevenB1"] = string.Format("{0:F2}", ChoiceRow["B1Min"]);
            row["UnevenB2"] = string.Format("{0:F2}", ChoiceRow["B2Min"]);
            row["UnevenRatio"] = string.Format("{0:P2}", ChoiceRow["BRatioMin"]);
            row["MissAlign"] = string.Format("{0:F2}", ChoiceRow["MissAlignMin"]);
            row["Notch"] = string.Format("{0:F2}", ChoiceRow["NotchMin"]);
            row["Status"] = 0;
            dtGridData.Rows.Add(row);

            //Max
            row = dtGridData.NewRow();
            row["Cat"] = string.Format("{0}", "Max");
            row["KValue"] = string.Format("{0:F2}", ChoiceRow["KMax"]);
            row["BWidth"] = string.Format("{0:F2}", ChoiceRow["BMax"]);
            row["UnevenB1"] = string.Format("{0:F2}", ChoiceRow["B1Max"]);
            row["UnevenB2"] = string.Format("{0:F2}", ChoiceRow["B2Max"]);
            row["UnevenRatio"] = string.Format("{0:P2}", ChoiceRow["BRatioMax"]);
            row["MissAlign"] = string.Format("{0:F2}", ChoiceRow["MissAlignMax"]);
            row["Notch"] = string.Format("{0:F2}", ChoiceRow["NotchMax"]);
            row["Status"] = 0;
            dtGridData.Rows.Add(row);

            //Avg
            row = dtGridData.NewRow();
            row["Cat"] = string.Format("{0}", "Avg");
            row["KValue"] = string.Format("{0:F2}", ChoiceRow["KAvg"]);
            row["BWidth"] = string.Format("{0:F2}", ChoiceRow["BAvg"]);
            row["UnevenB1"] = string.Format("{0:F2}", ChoiceRow["B1Avg"]);
            row["UnevenB2"] = string.Format("{0:F2}", ChoiceRow["B2Avg"]);
            row["UnevenRatio"] = string.Format("{0:P2}", ChoiceRow["BRatioAvg"]);
            row["MissAlign"] = string.Format("{0:F2}", ChoiceRow["MissAlignAvg"]);
            row["Notch"] = string.Format("{0:F2}", ChoiceRow["NotchAvg"]);
            row["Status"] = 0;
            dtGridData.Rows.Add(row);

            //Pass
            row = dtGridData.NewRow();
            row["Cat"] = string.Format("{0}", "Pass");
            row["KValue"] = string.Format("{0}", ChoiceRow["isKPass"]);
            row["BWidth"] = string.Format("{0}", ChoiceRow["isBPass"]);
            row["UnevenB1"] = string.Format("{0}", "-");
            row["UnevenB2"] = string.Format("{0}", "-");
            row["UnevenRatio"] = string.Format("{0}", ChoiceRow["isBRatioPass"]);
            row["MissAlign"] = string.Format("{0}", ChoiceRow["isMissAlignPass"]);
            row["Notch"] = string.Format("{0}", ChoiceRow["isNotchPass"]);
            row["Status"] = 0;
            dtGridData.Rows.Add(row);

            bandedGridView1.OptionsView.ShowColumnHeaders = false;
            grdOutBeadItems.DataSource = dtGridData;
        }
        #endregion

        /// <summary>
        /// Close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}