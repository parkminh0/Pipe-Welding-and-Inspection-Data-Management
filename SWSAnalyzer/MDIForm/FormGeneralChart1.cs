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
using DevExpress.XtraCharts;
using DevExpress.PivotGrid.PivotQuery;
using DevExpress.XtraCharts.Native;
using DevExpress.Office.Utils;
using System.Collections;
//using DevExpress.XtraGrid.Views.Grid;

namespace SWSAnalyzer
{
    public partial class FormGeneralChart1 : BaseForm
    {
        private int ChoiceCompKey;
        private int ChoiceBeadKey;
        private int ChoiceWeldKey; //Link용
        private DataTable dtBeadMaster;
        private int ArgumentNumber = 13;

        private string StartDate;
        private string EndDate;
        private int ProjectKey;

        private string CompareValue = "";
        private bool isClicked = false;

        #region 폼 로드
        /// <summary>
        /// 
        /// </summary>
        public FormGeneralChart1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormGeneralChart1_Load(object sender, EventArgs e)
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


			SetSkinStyle();
		}

		public override void SetSkinStyle()
		{
			if (Properties.Settings.Default.UserPalette.Contains("Dark"))
			{
				radioGroup1.BackColor = radioGroup2.BackColor = Color.FromArgb(35, 35, 35);
				chart2StackedBar.Titles[0].TextColor = chart4StackedBar.Titles[0].TextColor = System.Drawing.Color.White;
			}
			else
			{
                radioGroup1.BackColor = radioGroup2.BackColor = Color.White;
				chart2StackedBar.Titles[0].TextColor = chart4StackedBar.Titles[0].TextColor = System.Drawing.Color.Black;
			}
		}

		/// <summary>
		/// 폼 쇼운
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormGeneralChart1_Shown(object sender, EventArgs e)
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
        /// 회사선택시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lueCompany_EditValueChanged(object sender, EventArgs e)
        {
            ChoiceCompKey = int.Parse(lueCompany.EditValue.ToString());
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

        #region 조회/차트 셋팅
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
        /// 조회
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            StartDate = dteFrom.DateTime.ToString("yyyy-MM-dd");
            EndDate = dteTo.DateTime.ToString("yyyy-MM-dd");
            ProjectKey = int.Parse(lueProject.EditValue.ToString());

            SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
            SplashScreenManager.Default.SetWaitFormDescription("The loading of data. Please wait....");

            ShowChart1();
            ShowChart2();
            ShowChart3();
            ShowChart4(false);
            radioGroup1.SelectedIndex = 0;

            SplashScreenManager.CloseForm(false);
        }

        /// <summary>
        /// 도넛차트 1
        /// </summary>
        private void ShowChart1()
        {
            string sql = string.Empty;
            sql += "select v.Material, COUNT(*) ItemCount ";
            sql += "  from ViewBeadMasterForSummary v ";
            sql += " where 1 = 1 ";
            if (!chkIsAllCompany.Checked)
                sql += $"   and CompKey = {ChoiceCompKey} ";

            if (!chkIsAllProject.Checked)
                sql += $"   and ProjectKey = {ProjectKey} ";

            if (!chkIsAllDate.Checked)
                sql += $"   and v.InspectionDate BETWEEN '{StartDate}' and '{EndDate}' ";

            if (!Program.Option.isAdmin)
            {
                if (Program.Option.AuthLevel == 1)
                {
                    sql += $" and (v.WeldCreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                    sql += $"      OR v.InspectCreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}')) ";
                }
                else if (Program.Option.AuthLevel == 2)
                {
                    sql += $" and (v.WeldCreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR v.InspectCreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR v.WeldCreateID = '{Program.Option.LoginID}' OR v.InspectCreateID = '{Program.Option.LoginID}') ";
                }
                else if (Program.Option.AuthLevel == 3)
                {
                    sql += $"  and (v.WeldCreateID = '{Program.Option.LoginID}' or v.InspectCreateID = '{Program.Option.LoginID}') ";
                }
            }

            sql += " group by v.Material";

            DataTable dtChart1 = DBManager.Instance.GetDataTable(sql);
            chart1Doughnut.Series["Series1"].DataSource = dtChart1;
            chart1Doughnut.Series["Series1"].ArgumentScaleType = ScaleType.Auto;
            chart1Doughnut.Series["Series1"].ArgumentDataMember = "Material";
            chart1Doughnut.Series["Series1"].ValueScaleType = ScaleType.Numerical;
            chart1Doughnut.Series["Series1"].ValueDataMembers.AddRange(new string[] { "ItemCount" });
        }

        /// <summary>
        /// 두번째 챠트 출력
        /// </summary>
        /// <param name="isClicked">true: 파이그래프 클릭 시, false: Search 버튼 클릭 시</param>
        private void ShowChart2()
        {
            //2
            chart2StackedBar.Series["NOK"].DataSource = GetChart2Data("FALSE", isClicked, CompareValue);
            chart2StackedBar.Series["NOK"].ArgumentScaleType = ScaleType.Auto;
            chart2StackedBar.Series["NOK"].ArgumentDataMember = "OD";
            chart2StackedBar.Series["NOK"].ValueScaleType = ScaleType.Numerical;
            chart2StackedBar.Series["NOK"].ValueDataMembers.AddRange(new string[] { "ODCount" });
            //1
            chart2StackedBar.Series["OK"].DataSource = GetChart2Data("TRUE", isClicked, CompareValue);
            chart2StackedBar.Series["OK"].ArgumentScaleType = ScaleType.Auto;
            chart2StackedBar.Series["OK"].ArgumentDataMember = "OD";
            chart2StackedBar.Series["OK"].ValueScaleType = ScaleType.Numerical;
            chart2StackedBar.Series["OK"].ValueDataMembers.AddRange(new string[] { "ODCount" });

            XYDiagram diagram = chart2StackedBar.Diagram as XYDiagram;
            if (diagram == null)
                return;

            diagram.AxisX.QualitativeScaleComparer = new ODComparer();
        }
        private DataTable GetChart2Data(string tfString, bool isClick, string Value = "")
        {
            if (isClick)
            {
                chart2StackedBar.Titles[0].Text = Value;
            }

            string sql = string.Empty;
            sql += "select v.OD, ";
            sql += "       count(*) ODCount ";
            sql += "  from ViewBeadMasterForSummary v ";
            if (radioGroup2.SelectedIndex == 0)
            {
                sql += "                    JOIN CodeInfomation ci ";
                sql += "                      ON ci.Category = 'Diameter' ";
                sql += "   AND CAST(ci.DetailCode AS INT) = CAST(SUBSTRING(v.OD, 3, 10) AS INT) ";
            }
            sql += " where 1 = 1 ";
            if (!chkIsAllCompany.Checked)
                sql += $"   and CompKey = {ChoiceCompKey} ";
            if (!chkIsAllProject.Checked)
                sql += $"	and ProjectKey = {ProjectKey} ";
            if (!chkIsAllDate.Checked)
                sql += $"	and v.InspectionDate BETWEEN '{StartDate}' and '{EndDate}' ";
            if (isClick)
                sql += $"	and v.Material = '{Value}' ";
            sql += $"	and UPPER(v.DVS) = '{tfString}' ";

            if (!Program.Option.isAdmin)
            {
                if (Program.Option.AuthLevel == 1)
                {
                    sql += $" and (v.WeldCreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                    sql += $"      OR v.InspectCreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}')) ";
                }
                else if (Program.Option.AuthLevel == 2)
                {
                    sql += $" and (v.WeldCreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR v.InspectCreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR v.WeldCreateID = '{Program.Option.LoginID}' OR v.InspectCreateID = '{Program.Option.LoginID}') ";
                }
                else if (Program.Option.AuthLevel == 3)
                {
                    sql += $"  and (v.WeldCreateID = '{Program.Option.LoginID}' or v.InspectCreateID = '{Program.Option.LoginID}') ";
                }
            }
            sql += " group by v.OD ";
            DataTable dtData = DBManager.Instance.GetDataTable(sql);
            return dtData;
        }
        private void radioGroup2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowChart2();
        }

        /// <summary>
        /// 세번째 차트 출력
        /// </summary>
        /// <param name="isClicked">true: 파이그래프 클릭 시, false: Search 버튼 클릭 시</param>
        private void ShowChart3()
        {
            string sql = string.Empty;
            sql += "select case when UPPER(v.DVS) = 'TRUE' THEN 'Weld OK' ";
            sql += "            when UPPER(v.DVS) = 'FALSE' THEN 'Weld NOK' ";
            sql += "			ELSE 'etc.' END DVS, ";
            sql += "       count(*) tfCount ";
            sql += "  from ViewBeadMasterForSummary v ";
            sql += " where 1 = 1 ";
            if (!chkIsAllCompany.Checked)
                sql += $"   and CompKey = {ChoiceCompKey} ";

            if (!chkIsAllProject.Checked)
                sql += $"	and ProjectKey = {ProjectKey} ";

            if (!chkIsAllDate.Checked)
                sql += $"	and v.InspectionDate BETWEEN '{StartDate}' and '{EndDate}' ";

            if (!Program.Option.isAdmin)
            {
                if (Program.Option.AuthLevel == 1)
                {
                    sql += $" and (v.WeldCreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                    sql += $"      OR v.InspectCreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}')) ";
                }
                else if (Program.Option.AuthLevel == 2)
                {
                    sql += $" and (v.WeldCreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR v.InspectCreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR v.WeldCreateID = '{Program.Option.LoginID}' OR v.InspectCreateID = '{Program.Option.LoginID}') ";
                }
                else if (Program.Option.AuthLevel == 3)
                {
                    sql += $"  and (v.WeldCreateID = '{Program.Option.LoginID}' or v.InspectCreateID = '{Program.Option.LoginID}') ";
                }
            }
            sql += " group by v.DVS ";
            sql += " ORDER BY DVS DESC ";
            DataTable dtChart3 = DBManager.Instance.GetDataTable(sql);

            chart3Pie.Series["Series1"].DataSource = dtChart3;
            chart3Pie.Series["Series1"].ArgumentScaleType = ScaleType.Auto;
            chart3Pie.Series["Series1"].ArgumentDataMember = "DVS";
            chart3Pie.Series["Series1"].ValueScaleType = ScaleType.Numerical;
            chart3Pie.Series["Series1"].ValueDataMembers.AddRange(new string[] { "tfCount" });
        }

        /// <summary>
        /// 4번째 차트 출력
        /// </summary>
        private void ShowChart4(bool isClick, string Value = "")
        {
            //1
            chart4StackedBar.Series["OK"].DataSource = GetChart4Data("TRUE", isClick, Value);
            chart4StackedBar.Series["OK"].ArgumentScaleType = ScaleType.Auto;
            chart4StackedBar.Series["OK"].ArgumentDataMember = "Item";
            chart4StackedBar.Series["OK"].ValueScaleType = ScaleType.Numerical;
            chart4StackedBar.Series["OK"].ValueDataMembers.AddRange(new string[] { "tfCount" });
            //2
            chart4StackedBar.Series["NOK"].DataSource = GetChart4Data("FALSE", isClick, Value);
            chart4StackedBar.Series["NOK"].ArgumentScaleType = ScaleType.Auto;
            chart4StackedBar.Series["NOK"].ArgumentDataMember = "Item";
            chart4StackedBar.Series["NOK"].ValueScaleType = ScaleType.Numerical;
            chart4StackedBar.Series["NOK"].ValueDataMembers.AddRange(new string[] { "tfCount" });

            chart4StackedBar.Series["NOK"].SeriesPointsSorting = SortingMode.Descending;
            chart4StackedBar.Series["NOK"].SeriesPointsSortingKey = SeriesPointKey.Value_1;
        }
        private DataTable GetChart4Data(string tfString, bool isClick, string Value = "")
        {
            string sql = string.Empty;
            sql += "select Item, sum(tfCount) tfCount ";
            sql += "  from ( ";
            sql += "		SELECT  ProjectKey, ";
            sql += "				Item, ";
            sql += "				COUNT(tfCount) tfCount ";
            sql += "		  FROM  ViewBeadMasterForSummary v ";
            sql += "       UNPIVOT (tfCount FOR Item IN ([K-Value], ";
            sql += "				[Bead Width], ";
            sql += "				[Uneven Bead], ";
            sql += "				[Mis-Al], ";
            sql += "				[Notch], ";
            sql += "				[NotchEye], ";
            sql += "				[Ang-Dev], ";
            sql += "				[Crack] , ";
            sql += "				[Void], ";
            sql += "				[Support], ";
            sql += "				[Interrupt], ";
            sql += "				[Overheat], ";
            sql += "                [OtherInfo2]) ";
            sql += "			) AS x ";
            sql += "		WHERE 1 = 1 ";
            if (!chkIsAllDate.Checked)
                sql += $"     and InspectionDate BETWEEN '{StartDate}' and '{EndDate}' ";
            if (isClick)
                sql += $"	  and DVS = '{Value}' ";
            sql += $"         and UPPER(x.tfCount) = '{tfString}' ";
            if (!chkIsAllCompany.Checked)
                sql += $"   and CompKey = {ChoiceCompKey} ";
            if (!chkIsAllProject.Checked)
                sql += $"	and ProjectKey = {ProjectKey} ";
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
            sql += "		GROUP BY ProjectKey, Item ";
            sql += "	 ) x ";
            sql += " where 1 = 1 ";
            sql += " group by x.Item ";

            DataTable dtData = DBManager.Instance.GetDataTable(sql);
            if (tfString == "FALSE")
                ArgumentNumber = dtData.Rows.Count;
            return dtData;
        }
        #endregion

        #region 차트 데이터 클릭 시 상세 데이터 출력
        /// <summary>
        /// 상세데이터 가져오기
        /// </summary>
        /// <param name="ShowType">1:Material, 2:OD, 3:DVS, 4:by Item</param>
        private void ShowBeadMasterList(int showType, string colName, string colValue, string tfString = "")
        {
            string sql = LogicManager.Common.GetSelectBeadMasterSql(); //Query문장 가져오기
            if (!chkIsAllCompany.Checked)
                sql += $"   and c.CompKey = {ChoiceCompKey} ";

            if (!chkIsAllProject.Checked)
                sql += $"   and p.ProjectKey = {ProjectKey} ";

            if (!chkIsAllDate.Checked)
                sql += $"   and bm.InspectionDate BETWEEN '{StartDate}' and '{EndDate}' ";

            if (!string.IsNullOrWhiteSpace(txtSerialNo.Text))
                sql += $"   and SerialNo = '{txtSerialNo.Text}' ";

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

            switch (showType)
            {
                case 2: //OD Numeric
                    sql += $"   and {colName} = {colValue} ";
                    if (colName == "OtherInfo2")
                    {
                        if (colValue == "FALSE")
                        {
                            sql += $"   and {colName} IS NOT NULL ";
                            sql += $"   and {colName} != '' ";
                        }
                        else
                        {
                            sql += $"   and ({colName} IS NULL ";
                            sql += $"        OR {colName} = '') ";
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(tfString))
                        sql += $"   and DVS = '{tfString}' ";
                    break;
                case 1:
                case 3:
                case 4:
                    if (colName == "OtherInfo2")
                    {
                        if (colValue == "FALSE")
                        {
                            sql += $"   and {colName} IS NOT NULL ";
                            sql += $"   and {colName} != '' ";
                        }
                        else
                        {
                            sql += $"   and ({colName} IS NULL ";
                            sql += $"        OR {colName} = '') ";
                        }
                    }
                    else
                        sql += $"   and {colName} = '{colValue}' ";
                    
                    if (showType == 4)
                    {
                        if (chart4StackedBar.Titles[0].Text == "Weld NOK")
                        {
                            sql += $"   and DVS = 'FALSE' ";
                        }
                        else if (chart4StackedBar.Titles[0].Text == "Weld OK")
                        {
                            sql += $"   and DVS = 'TRUE' ";
                        }
                        else
                        {
                        }
                    }
                    break;
                default:
                    break;
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
        private void gridView1_RowCountChanged(object sender, EventArgs e)
        {
            groupControl1.Text = string.Format(LangResx.Main.count_General, gridView1.RowCount);
        }

        /// <summary>
        /// Material을 조건으로 상세조회
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chart1Doughnut_Click(object sender, EventArgs e)
        {
            ChartHitInfo hi = chart1Doughnut.CalcHitInfo(chart1Doughnut.PointToClient(MousePosition));

            if (hi.InSeriesPoint)
            {
                isClicked = true;
                CompareValue = hi.SeriesPoint.Argument;
                ShowChart2();
                ShowBeadMasterList(1, "Material", CompareValue);
            }
            else
            {
                isClicked = false;
                CompareValue = "";
                ShowChart2();
                chart2StackedBar.Titles[0].Text = "All";
                grdBeadMaster.DataSource = null;
            }
        }

        /// <summary>
        /// OD를 조건으로 상세조회
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chart2StackedBar_Click(object sender, EventArgs e)
        {
            ChartHitInfo hi = chart2StackedBar.CalcHitInfo(chart2StackedBar.PointToClient(MousePosition));
            Series s = (Series)hi.Series;
            if (s == null)
                return;

            string tfString;
            try
            {
                tfString = (s.Name == "OK") ? "TRUE" : "FALSE";
            }
            catch (Exception)
            {
                return;
            }

            if (hi.SeriesPoint == null)
                return;

            string condString = hi.SeriesPoint.Argument.Replace("OD", "");
            if (hi.InSeriesPoint)
            {
                ShowBeadMasterList(2, "OD", condString, tfString);
            }
        }

        /// <summary>
        /// DVS값을 조건으로 상세조회
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chart3Pie_Click(object sender, EventArgs e)
        {
            ChartHitInfo hi = chart3Pie.CalcHitInfo(chart3Pie.PointToClient(MousePosition));
            if (hi.InSeriesPoint)
            {
                string condString;
                chart4StackedBar.Titles[0].Text = hi.SeriesPoint.Argument.ToString();
                if (hi.SeriesPoint.Argument.EndsWith("Weld NOK"))
                    condString = "FALSE";
                else
                    condString = "TRUE";

                ShowChart4(true, condString);
                ShowBeadMasterList(3, "DVS", condString);
            }
            else
            {
                ShowChart4(false);
                chart4StackedBar.Titles[0].Text = "All";
                grdBeadMaster.DataSource = null;
            }
        }

        /// <summary>
        /// 검수 Item별로 상세조회
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chart4StackedBar_Click(object sender, EventArgs e)
        {
            ChartHitInfo hi = chart4StackedBar.CalcHitInfo(chart4StackedBar.PointToClient(MousePosition));
            Series s = (Series)hi.Series;
            string colName, condString;
            if (s == null)
                return;

            try
            {
                condString = (s.Name == "OK") ? "TRUE" : "FALSE";
            }
            catch (Exception)
            {
                return;
            }

            if (hi.SeriesPoint == null)
                return;

            switch (hi.SeriesPoint.Argument)
            {
                case "K-Value":
                    colName = "isKPass";
                    break;
                case "Bead Width":
                    colName = "isBPass";
                    break;
                case "Uneven Bead":
                    colName = "isBRatioPass";
                    break;
                case "Mis-Al":
                    colName = "isMissAlignPass";
                    break;
                case "Notch":
                    colName = "isNotchPass";
                    break;
                case "NotchEye":
                    colName = "isNotchEyes";
                    break;
                case "Ang-Dev":
                    colName = "isAngularDevEyes";
                    break;
                case "Crack":
                    colName = "isCrackEyes";
                    break;
                case "Void":
                    colName = "isVoidEyes";
                    break;
                case "Support":
                    colName = "isSupportPadEyes";
                    break;
                case "Interrupt":
                    colName = "isInterruptionEyes";
                    break;
                case "Overheat":
                    colName = "isOverheatingEyes";
                    break;
                case "OtherInfo2":
                    colName = "OtherInfo2";
                    break;
                default:
                    colName = "";
                    break;
            }

            if (hi.InSeriesPoint)
            {
                ShowBeadMasterList(4, colName, condString);
            }
        }
        #endregion

        #region 그리드 선택 시 FormPopBeadMaster
        /// <summary>
        /// 더블클릭시 상세창 띄우기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdBeadMaster_DoubleClick(object sender, EventArgs e)
        {
            FormPopBeadMastercs frm = new FormPopBeadMastercs();
            frm.ChoiceBeadKey = ChoiceBeadKey;
            frm.isCanWeldFormOpen = true;
            frm.Show();
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

            ChoiceBeadKey = int.Parse(row["BeadKey"].ToString());
            ChoiceWeldKey = int.Parse(row["WeldKey"].ToString());
        }
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

        /// <summary>
        /// chart4 X축 설정
        /// </summary>
        /// <param name="sender">0: 에러율순, 1: 이름순</param>
        /// <param name="e"></param>
        private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
        {
            XYDiagram diagram = chart4StackedBar.Diagram as XYDiagram;
            if (diagram == null)
                return;

            diagram.AxisX.QualitativeScaleComparer = null;

            switch (radioGroup1.SelectedIndex)
            {
                case 0:
                    Series series = chart4StackedBar.Series["NOK"];
                    var argTotalDict = new Dictionary<string, double>();
                    for (int i = 0; i < ArgumentNumber; i++)
                    {
                        string argument = series.Points[i].Argument;
                        double total = GetTotalByArg(argument);
                        argTotalDict.Add(argument, total);
                    }
                    chart4StackedBar.Series["NOK"].SeriesPointsSorting = SortingMode.Descending;
                    chart4StackedBar.Series["NOK"].SeriesPointsSortingKey = SeriesPointKey.Value_1;
                    break;
                case 1:
                    diagram.AxisX.QualitativeScaleComparer = new NumberComparer();
                    break;
            }
        }
        double GetTotalByArg(object arg)
        {
            double total = 0;
            foreach (Series series in chart4StackedBar.Series)
                foreach (SeriesPoint point in series.Points)
                    if (Equals(point.Argument, arg))
                        total += point.Values[0];
            return total;
        }
    }

    /// <summary>
    /// Chart4 이름순 정렬
    /// 'K-Value, Bead Width, Uneven bead, Mis-Al, Notch, NotchEye, Ang-Dev, Crack, Void, Support, Interrupt, Overheat, OtherInfo2'
    /// </summary>
    class NumberComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            int ix = NumberConverter.ToInt(x);
            int iy = NumberConverter.ToInt(y);
            return ix - iy;
        }
    }
    class NumberConverter
    {
        public static int ToInt(object tmp)
        {
            string stringNumber = tmp as string;
            int number = 0;
            if (string.IsNullOrEmpty(stringNumber)) return -1;
            if (Int32.TryParse(stringNumber, out number))
                return number;
            switch (stringNumber.ToLower())
            {
                case "k-value":
                    return 1;
                case "bead width":
                    return 2;
                case "uneven bead":
                    return 3;
                case "mis-al":
                    return 4;
                case "notch":
                    return 5;
                case "notcheye":
                    return 6;
                case "ang-dev":
                    return 7;
                case "crack":
                    return 8;
                case "void":
                    return 9;
                case "support":
                    return 10;
                case "interrupt":
                    return 11;
                case "overheat":
                    return 12;
                case "otherinfo2":
                    return 13;
            }
            return number;
        }
    }

    /// <summary>
    /// Chart2 OD순 정렬
    /// </summary>
    class ODComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            int ix = ODConverter.ToInt(x);
            int iy = ODConverter.ToInt(y);
            return ix - iy;
        }
    }
    class ODConverter
    {
        public static int ToInt(object tmp)
        {
            string stringNumber = tmp as string;
            int number = 0;
            if (string.IsNullOrEmpty(stringNumber)) 
                return -1;

            if (Int32.TryParse(stringNumber, out number))
                return number;

            return int.Parse(stringNumber.Substring(2, stringNumber.Length - 2));
        }
    }
}