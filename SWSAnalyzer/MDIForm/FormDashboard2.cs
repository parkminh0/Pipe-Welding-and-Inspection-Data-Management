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
using DevExpress.XtraTreeMap;

namespace SWSAnalyzer
{
    public partial class FormDashboard2 : BaseForm
    {
        int focusChart = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="focusChart"></param>
        public FormDashboard2(int focusChart)
        {
            InitializeComponent();
            this.focusChart = focusChart;
        }

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormDashboard2_Load(object sender, EventArgs e)
        {
            dteTo.DateTime = DateTime.Parse("2000-01-01");
            dteFrom.DateTime = DateTime.Now;
            if (Program.Option.AuthLevel <= 1)
                chkIsAllCompany.Checked = chkIsAllProject.Checked = true;

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
				chartTotalWeldingPoint.Titles[0].TextColor = chartWeldOKNOK.Titles[0].TextColor = chartWeldingNOKReason.Titles[0].TextColor = chartWelder.Titles[0].TextColor = chartOutsideDiameter.Titles[0].TextColor = Color.White;
				chartWeldMaterialETC.Legend.TextColor = labelControl2.ForeColor = labelControl6.ForeColor = lblWelder.ForeColor = lblWelderTotal.ForeColor = lblNOKTotal.ForeColor = lblIR.ForeColor = lblEF.ForeColor = Color.White;
				panelControl7.BackColor = radioGroup1.BackColor = labelControl2.BackColor = labelControl6.BackColor = lblWelder.BackColor = lblWelderTotal.BackColor = lblNOKTotal.BackColor = lblIR.BackColor = lblEF.BackColor = Color.FromArgb(35, 35, 35);
				txtWeldercnt.ForeColor = txtEFcnt.ForeColor = txtIRcnt.ForeColor = Color.Black;
			}
			else
			{
				chartTotalWeldingPoint.Titles[0].TextColor = chartWeldOKNOK.Titles[0].TextColor = chartWeldingNOKReason.Titles[0].TextColor = chartWelder.Titles[0].TextColor = chartOutsideDiameter.Titles[0].TextColor = Color.Black;
				chartWeldMaterialETC.Legend.TextColor = labelControl2.ForeColor = labelControl6.ForeColor = lblWelder.ForeColor = lblWelderTotal.ForeColor = lblNOKTotal.ForeColor = lblIR.ForeColor = lblEF.ForeColor = Color.Black;
				panelControl7.BackColor = radioGroup1.BackColor = labelControl2.BackColor = labelControl6.BackColor = lblWelder.BackColor = lblWelderTotal.BackColor = lblNOKTotal.BackColor = lblIR.BackColor = lblEF.BackColor = Color.White;
				txtWeldercnt.ForeColor = txtEFcnt.ForeColor = txtIRcnt.ForeColor = Color.Black;
			}
		}

		/// <summary>
		/// 폼 쇼운
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormDashboard2_Shown(object sender, EventArgs e)
        {
            // lue 세팅
            SetCompanyCode();

            // 차트 세팅
            btnSearch.PerformClick();

            // 포커스 적용
            setFocus(focusChart);
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
        /// 회사 선택에 따른 프로젝트 룩업 셋팅
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

        /// <summary>
        /// 새로고침
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
            SplashScreenManager.Default.SetWaitFormDescription("The loading of data. Please wait....");

            ShowChartTotalWeldingPoint();
            ShowChartWeldOKNOK();
            ShowChartNOKReason();
            ShowChartMaterial();
            ShowChartETC();
            ShowChartWelderTop10();
            ShowChartOutsideDiameter();
            ShowChartMachineSeries();

            SplashScreenManager.CloseForm(false);
        }

        /// <summary>
        /// 요약화면에서 넘어왔을 시 포커스 적용
        /// </summary>
        public override void DoRefresh_Dashboard(int focus)
        {
            setFocus(focus);
        }
        public void setFocus(int focus)
        {
            switch (focus)
            {
                case 1:
                    chartTotalWeldingPoint.Focus();
                    break;
                case 2:
                    chartWeldOKNOK.Focus();
                    break;
                case 3:
                    chartWelder.Focus();
                    break;
                case 4:
                    chartOutsideDiameter.Focus();
                    break;
                case 5:
                    treeMapWeld.Focus();
                    break;
            }
        }

        /// <summary>
        /// Chart: Total Welding Point
        /// </summary>
        private void ShowChartTotalWeldingPoint()
        {
            DataTable dtChart1 = null;
            XYDiagram diagram = chartTotalWeldingPoint.Diagram as XYDiagram;
            string sql = string.Empty;
            switch (radioGroup2.SelectedIndex)
            {
                case 0: // 연단위
                    sql += "SELECT   FORMAT(CAST(DataDate AS DATE), 'yyyy') YEAR, ";
                    sql += "         SUM(ItemValue)                         points ";
                    sql += "FROM     ChartTable ";
                    sql += "WHERE    1 = 1 ";
                    sql += "         AND chartkey = 1 ";
                    sql += $"  and DataDate BETWEEN '{dteTo.DateTime.ToString("yyyy-MM-dd")}' AND '{dteFrom.DateTime.ToString("yyyy-MM-dd")}' ";
                    if (!chkIsAllProject.Checked)
                        sql += string.Format("   and ProjectKey = {0} ", lueProject.EditValue.ToString());
                    if (!chkIsAllCompany.Checked)
                        sql += string.Format("   and CompKey = {0} ", lueCompany.EditValue.ToString());
                    if (!Program.Option.isAdmin)
                    {
                        if (Program.Option.AuthLevel == 1)
                        {
                            sql += $" and CreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                        }
                        else if (Program.Option.AuthLevel == 2)
                        {
                            sql += $" and (CreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                            sql += $"      OR CreateID = '{Program.Option.LoginID}') ";
                        }
                        else
                        {
                            sql += $"  and CreateID = '{Program.Option.LoginID}' ";
                        }
                    }
                    sql += "GROUP BY FORMAT(CAST(DataDate AS DATE), 'yyyy') ";
                    sql += "ORDER BY FORMAT(CAST(DataDate AS DATE), 'yyyy')";
                    dtChart1 = DBManager.Instance.GetDataTable(sql);
                    diagram.AxisX.Title.Text = "Year";
                    break;
                case 1: // 월단위
                    sql += "SELECT   FORMAT(CAST(DataDate AS DATE), 'yyyy-MM') YEAR, ";
                    sql += "         SUM(ItemValue)                         points ";
                    sql += "FROM     ChartTable ";
                    sql += "WHERE    1 = 1 ";
                    sql += "         AND chartkey = 1 ";
                    sql += $"  and DataDate BETWEEN '{dteTo.DateTime.ToString("yyyy-MM-dd")}' AND '{dteFrom.DateTime.ToString("yyyy-MM-dd")}' ";
                    if (!chkIsAllProject.Checked)
                        sql += string.Format("   and ProjectKey = {0} ", lueProject.EditValue.ToString());
                    if (!chkIsAllCompany.Checked)
                        sql += string.Format("   and CompKey = {0} ", lueCompany.EditValue.ToString());
                    if (!Program.Option.isAdmin)
                    {
                        if (Program.Option.AuthLevel == 1)
                        {
                            sql += $" and CreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                        }
                        else if (Program.Option.AuthLevel == 2)
                        {
                            sql += $" and (CreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                            sql += $"      OR CreateID = '{Program.Option.LoginID}') ";
                        }
                        else
                        {
                            sql += $"  and CreateID = '{Program.Option.LoginID}' ";
                        }
                    }
                    sql += "GROUP BY FORMAT(CAST(DataDate AS DATE), 'yyyy-MM') ";
                    sql += "ORDER BY FORMAT(CAST(DataDate AS DATE), 'yyyy-MM')";
                    dtChart1 = DBManager.Instance.GetDataTable(sql);
                    diagram.AxisX.Title.Text = "Month";
                    break;
                case 2: // 주단위
                    sql += "SELECT   CONCAT(DATEPART(yyyy, DataDate), '-', FORMAT(DATEPART(ww, DataDate), '00')) YEAR, ";
                    sql += "         SUM(ItemValue)                         points ";
                    sql += "FROM     ChartTable ";
                    sql += "WHERE    1 = 1 ";
                    sql += "         AND chartkey = 1 ";
                    sql += $"  and DataDate BETWEEN '{dteTo.DateTime.ToString("yyyy-MM-dd")}' AND '{dteFrom.DateTime.ToString("yyyy-MM-dd")}' ";
                    if (!chkIsAllProject.Checked)
                        sql += string.Format("   and ProjectKey = {0} ", lueProject.EditValue.ToString());
                    if (!chkIsAllCompany.Checked)
                        sql += string.Format("   and CompKey = {0} ", lueCompany.EditValue.ToString());
                    if (!Program.Option.isAdmin)
                    {
                        if (Program.Option.AuthLevel == 1)
                        {
                            sql += $" and CreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                        }
                        else if (Program.Option.AuthLevel == 2)
                        {
                            sql += $" and (CreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                            sql += $"      OR CreateID = '{Program.Option.LoginID}') ";
                        }
                        else
                        {
                            sql += $"  and CreateID = '{Program.Option.LoginID}' ";
                        }
                    }
                    sql += "GROUP BY  CONCAT(DATEPART(yyyy, DataDate), '-',  FORMAT(DATEPART(ww, DataDate), '00')) ";
                    sql += "ORDER BY  CONCAT(DATEPART(yyyy, DataDate), '-',  FORMAT(DATEPART(ww, DataDate), '00'))";
                    dtChart1 = DBManager.Instance.GetDataTable(sql);
                    diagram.AxisX.Title.Text = "Week";
                    break;
                case 3: // 일단위
                    sql += "SELECT   FORMAT(CAST(DataDate AS DATE), 'yyyy-MM-dd') Year, ";
                    sql += "         SUM(ItemValue)                         points ";
                    sql += "FROM     ChartTable ";
                    sql += "WHERE    1 = 1 ";
                    sql += "         AND chartkey = 1 ";
                    sql += $"        AND DataDate BETWEEN '{dteTo.DateTime.ToString("yyyy-MM-dd")}' AND '{dteFrom.DateTime.ToString("yyyy-MM-dd")}' ";
                    if (!chkIsAllProject.Checked)
                        sql += string.Format("   and ProjectKey = {0} ", lueProject.EditValue.ToString());
                    if (!chkIsAllCompany.Checked)
                        sql += string.Format("   and CompKey = {0} ", lueCompany.EditValue.ToString());
                    if (!Program.Option.isAdmin)
                    {
                        if (Program.Option.AuthLevel == 1)
                        {
                            sql += $" and CreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                        }
                        else if (Program.Option.AuthLevel == 2)
                        {
                            sql += $" and (CreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                            sql += $"      OR CreateID = '{Program.Option.LoginID}') ";
                        }
                        else
                        {
                            sql += $"  and CreateID = '{Program.Option.LoginID}' ";
                        }
                    }
                    sql += "GROUP BY FORMAT(CAST(DataDate AS DATE), 'yyyy-MM-dd') ";
                    sql += "ORDER BY FORMAT(CAST(DataDate AS DATE), 'yyyy-MM-dd')";
                    dtChart1 = DBManager.Instance.GetDataTable(sql);
                    diagram.AxisX.Title.Text = "Day";
                    break;
            }
            Series series1 = chartTotalWeldingPoint.Series[0];
            series1.ArgumentDataMember = "Year";
            series1.ValueDataMembers.AddRange("points");
            series1.DataSource = dtChart1;
        }
        private void radioGroup2_SelectedIndexChanged(object sender, EventArgs e)
        {
            SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
            SplashScreenManager.Default.SetWaitFormDescription("The loading of data. Please wait....");
            
            ShowChartTotalWeldingPoint();
            
            SplashScreenManager.CloseForm(false);
        }

        /// <summary>
        /// Chart: Weld OK, NOK
        /// </summary>
        private void ShowChartWeldOKNOK()
        {
            string sql = string.Empty;
            sql += "select x.Status, sum(x.ItemValue) ItemValue ";
            sql += "  from ( ";
            sql += "		select CASE ";
            sql += "				   WHEN (LEFT(x.ItemName, 6) = 'WELDP.') THEN 'WELDP.OK' ";
            sql += "				   WHEN (LEFT(x.ItemName, 7) = 'PH-WELD') THEN 'PH-WELDP.OK' ";
            sql += "				   ELSE 'ERROR' ";
            sql += "			   END Status, ";
            sql += "			   x.ItemValue ";
            sql += "		  from ( ";
            sql += "				select ct.ItemName, ";
            sql += "				        sum(ItemValue) ItemValue ";
            sql += "					from ChartTable ct ";
            sql += "					join PeriodDay p ";
            sql += "					on p.BaseYmd = ct.DataDate ";
            sql += "					where 1 = 1 ";
            sql += "                      and ct.ChartKey = 3 ";
            sql += $"                     and DataDate BETWEEN '{dteTo.DateTime.ToString("yyyy-MM-dd")}' AND '{dteFrom.DateTime.ToString("yyyy-MM-dd")}' ";
            if (!chkIsAllProject.Checked)
                sql += string.Format("   and ProjectKey = {0} ", lueProject.EditValue.ToString());
            if (!chkIsAllCompany.Checked)
                sql += string.Format("   and CompKey = {0} ", lueCompany.EditValue.ToString());
            if (!Program.Option.isAdmin)
            {
                if (Program.Option.AuthLevel == 1)
                {
                    sql += $" and CreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                }
                else if (Program.Option.AuthLevel == 2)
                {
                    sql += $" and (CreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR CreateID = '{Program.Option.LoginID}') ";
                }
                else
                {
                    sql += $"  and CreateID = '{Program.Option.LoginID}' ";
                }
            }
            sql += "					group by ct.ItemName ";
            sql += "			) x ";
            sql += "	) x ";
            sql += " group by x.Status ";
            sql += " order by ItemValue DESC ";
            DataTable dtChart = DBManager.Instance.GetDataTable(sql);
            chartWeldOKNOK.Series["Series1"].DataSource = dtChart;
            chartWeldOKNOK.Series["Series1"].ArgumentScaleType = ScaleType.Auto;
            chartWeldOKNOK.Series["Series1"].ArgumentDataMember = "Status";
            chartWeldOKNOK.Series["Series1"].ValueScaleType = ScaleType.Numerical;
            chartWeldOKNOK.Series["Series1"].ValueDataMembers.AddRange(new string[] { "ItemValue" });
        }

        /// <summary>
        /// Chart: Welding NOK Reason 
        /// </summary>
        private void ShowChartNOKReason()
        {
            string sql = string.Empty;
            sql += "SELECT ItemName, ItemValue, ROW_NUMBER() OVER (ORDER BY ItemValue) rn ";
            sql += "FROM ( ";
            sql += "	SELECT Top 10 ItemName, ";
            sql += "					SUM(ItemValue) itemvalue ";
            sql += "				FROM ( SELECT CASE WHEN ct.ItemName LIKE '0%' THEN CONCAT('ERROR ', ct.ItemName) ";
            sql += "							ELSE ct.ItemName ";
            sql += "							END itemname, ";
            sql += "							ItemValue ";
            sql += "						FROM ChartTable ct ";
            sql += "						WHERE 1 = 1 ";
            sql += "						AND ct.ChartKey = 3 ";
            sql += $"                     and DataDate BETWEEN '{dteTo.DateTime.ToString("yyyy-MM-dd")}' AND '{dteFrom.DateTime.ToString("yyyy-MM-dd")}' ";
            if (!chkIsAllProject.Checked)
                sql += string.Format("   and ProjectKey = {0} ", lueProject.EditValue.ToString());
            if (!chkIsAllCompany.Checked)
                sql += string.Format("   and CompKey = {0} ", lueCompany.EditValue.ToString());
            if (!Program.Option.isAdmin)
            {
                if (Program.Option.AuthLevel == 1)
                {
                    sql += $" and CreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                }
                else if (Program.Option.AuthLevel == 2)
                {
                    sql += $" and (CreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR CreateID = '{Program.Option.LoginID}') ";
                }
                else
                {
                    sql += $"  and CreateID = '{Program.Option.LoginID}' ";
                }
            }
            sql += "							) x ";
            sql += "			WHERE 1 = 1 ";
            sql += "			  AND LEFT(ItemName, 5) = 'ERROR' ";
            sql += "			  AND LEN(ItemName) = 14 ";
            sql += "			GROUP BY ItemName ";
            sql += "			ORDER BY ItemValue DESC ";
            sql += "		) x";
            DataTable dtChartIR = DBManager.Instance.GetDataTable(sql);
            Series series1 = chartWeldingNOKReason.Series[0];
            series1.ArgumentDataMember = "ItemName";
            series1.ValueDataMembers.AddRange("ItemValue");
            series1.DataSource = dtChartIR;

            sql = string.Empty;
            sql += "SELECT ItemName, ItemValue, ROW_NUMBER() OVER (ORDER BY ItemValue) rn ";
            sql += "	FROM ( ";
            sql += "	SELECT Top 10 ItemName, ";
            sql += "					SUM(ItemValue) itemvalue ";
            sql += "				FROM ( SELECT CASE WHEN ct.ItemName LIKE '0%' THEN CONCAT('ERROR ', ct.ItemName) ";
            sql += "							ELSE ct.ItemName ";
            sql += "							END itemname, ";
            sql += "							ItemValue ";
            sql += "						FROM ChartTable ct ";
            sql += "						WHERE 1 = 1 ";
            sql += "                      and ct.ChartKey = 3 ";
            sql += $"                     and DataDate BETWEEN '{dteTo.DateTime.ToString("yyyy-MM-dd")}' AND '{dteFrom.DateTime.ToString("yyyy-MM-dd")}' ";
            if (!chkIsAllProject.Checked)
                sql += string.Format("   and ProjectKey = {0} ", lueProject.EditValue.ToString());
            if (!chkIsAllCompany.Checked)
                sql += string.Format("   and CompKey = {0} ", lueCompany.EditValue.ToString());
            if (!Program.Option.isAdmin)
            {
                if (Program.Option.AuthLevel == 1)
                {
                    sql += $" and CreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                }
                else if (Program.Option.AuthLevel == 2)
                {
                    sql += $" and (CreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR CreateID = '{Program.Option.LoginID}') ";
                }
                else
                {
                    sql += $"  and CreateID = '{Program.Option.LoginID}' ";
                }
            }
            sql += "							) x ";
            sql += "			WHERE 1 = 1 ";
            sql += "				AND LEFT(ItemName, 5) != 'ERROR' ";
            sql += "				AND LEFT(ItemName, 5) != 'WELDP' ";
            sql += "				AND LEFT(ItemName, 8) != 'PH-WELDP' ";
            sql += "			GROUP BY ItemName ";
            sql += "			ORDER BY ItemValue DESC ";
            sql += "	) x ";
            DataTable dtChartEF = DBManager.Instance.GetDataTable(sql);
            Series series2 = chartWeldingNOKReason.Series[1];
            series2.ArgumentDataMember = "ItemName";
            series2.ValueDataMembers.AddRange("ItemValue");
            series2.DataSource = dtChartEF;

            sql = string.Empty;
            sql += "SELECT COUNT(CASE WHEN LEFT(ItemName, 5) = 'ERROR' THEN 1 END) IR, COUNT(CASE WHEN LEFT(ItemName, 5) != 'ERROR' THEN 1 END) EF ";
            sql += "  FROM ( ";
            sql += "	SELECT DISTINCT(ItemName) ";
            sql += "	FROM ( ";
            sql += "		SELECT CASE WHEN ct.ItemName LIKE '0%' THEN CONCAT('ERROR ', ct.ItemName) ";
            sql += "								ELSE ct.ItemName ";
            sql += "								END itemname ";
            sql += "						FROM ChartTable ct ";
            sql += "						WHERE 1 = 1 ";
            sql += "							AND ct.ChartKey = 3 ";
            sql += "							AND LEFT(ItemName, 5) != 'WELDP' ";
            sql += "							AND LEFT(ItemName, 8) != 'PH-WELDP' ";
            sql += $"           AND DataDate BETWEEN '{dteTo.DateTime.ToString("yyyy-MM-dd")}' AND '{dteFrom.DateTime.ToString("yyyy-MM-dd")}' ";
            if (!chkIsAllProject.Checked)
                sql += string.Format("   and ProjectKey = {0} ", lueProject.EditValue.ToString());
            if (!chkIsAllCompany.Checked)
                sql += string.Format("   and CompKey = {0} ", lueCompany.EditValue.ToString());
            if (!Program.Option.isAdmin)
            {
                if (Program.Option.AuthLevel == 1)
                {
                    sql += $" and CreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                }
                else if (Program.Option.AuthLevel == 2)
                {
                    sql += $" and (CreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR CreateID = '{Program.Option.LoginID}') ";
                }
                else
                {
                    sql += $"  and CreateID = '{Program.Option.LoginID}' ";
                }
            }
            sql += "	) x ";
            sql += ") x";
            DataTable dtCount = DBManager.Instance.GetDataTable(sql);

            try
            {
                int temp = Math.Max(int.Parse(dtChartIR.Rows[dtChartIR.Rows.Count - 1][1].ToString()), int.Parse(dtChartEF.Rows[dtChartEF.Rows.Count - 1][1].ToString()));
                XYDiagram diagram = chartWeldingNOKReason.Diagram as XYDiagram;
                if (int.Parse(diagram.AxisY.WholeRange.MaxValue.ToString()) == temp)
                {
                    diagram.SecondaryAxesY[0].WholeRange.Auto = false;
                    diagram.SecondaryAxesY[0].WholeRange.SetMinMaxValues(0, temp);
                    diagram.SecondaryAxesY[0].WholeRange.EndSideMargin = diagram.AxisY.WholeRange.EndSideMargin;
                }
                else
                {
                    diagram.AxisY.WholeRange.Auto = false;
                    diagram.AxisY.WholeRange.SetMinMaxValues(0, temp);
                    diagram.AxisY.WholeRange.EndSideMargin = diagram.SecondaryAxesY[0].WholeRange.EndSideMargin;
                }

                lblNOKTotal.Visible = lblIR.Visible = lblEF.Visible = true;
                txtIRcnt.Visible = txtEFcnt.Visible = true;
                txtIRcnt.Text = string.Format($"{int.Parse(dtCount.Rows[0]["IR"].ToString()):n0}");
                txtEFcnt.Text = string.Format($"{int.Parse(dtCount.Rows[0]["EF"].ToString()):n0}");
            }
            catch (Exception e)
            {
                lblNOKTotal.Visible = lblIR.Visible = lblEF.Visible = false;
                txtIRcnt.Visible = txtEFcnt.Visible = false;
            }
        }

        /// <summary>
        /// Chart: Material
        /// </summary>
        private void ShowChartMaterial()
        {
            string sql = string.Empty;
            sql += "select ItemName, sum(ItemValue) ItemValue ";
            sql += "  from ( ";
            sql += " select case when ItemName != 'PVDF' and ItemName != 'PP' THEN 'ETC' ELSE ItemName END ItemName, sum(ItemValue) ItemValue ";
            sql += "   from ChartTable ";
            sql += "  where ChartKey = 5 ";
            sql += $"   AND DataDate BETWEEN '{dteTo.DateTime.ToString("yyyy-MM-dd")}' AND '{dteFrom.DateTime.ToString("yyyy-MM-dd")}' ";
            if (!chkIsAllProject.Checked)
                sql += string.Format("   and ProjectKey = {0} ", lueProject.EditValue.ToString());
            if (!chkIsAllCompany.Checked)
                sql += string.Format("   and CompKey = {0} ", lueCompany.EditValue.ToString());
            if (!Program.Option.isAdmin)
            {
                if (Program.Option.AuthLevel == 1)
                {
                    sql += $" and CreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                }
                else if (Program.Option.AuthLevel == 2)
                {
                    sql += $" and (CreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR CreateID = '{Program.Option.LoginID}') ";
                }
                else
                {
                    sql += $"  and CreateID = '{Program.Option.LoginID}' ";
                }
            }
            sql += "  group by ItemName ";
            sql += "  ) x ";
            sql += " group by ItemName ";
            sql += " order by ItemName DESC";
            DataTable dtChart = DBManager.Instance.GetDataTable(sql);

            chartWeldingMaterial.Series["Series1"].DataSource = dtChart;
            chartWeldingMaterial.Series["Series1"].ArgumentScaleType = ScaleType.Auto;
            chartWeldingMaterial.Series["Series1"].ArgumentDataMember = "ItemName";
            chartWeldingMaterial.Series["Series1"].ValueScaleType = ScaleType.Numerical;
            chartWeldingMaterial.Series["Series1"].ValueDataMembers.AddRange(new string[] { "ItemValue" });
        }

        /// <summary>
        /// Chart: ETC
        /// </summary>
        private void ShowChartETC()
        {
            string sql = string.Empty;
            sql += "select itemname, sum(x.itemvalue) itemvalue, rn ";
            sql += "from ( ";
            sql += "SELECT CASE WHEN x.rn > 5 THEN 'ETC' ";
            sql += "            ELSE ItemName ";
            sql += "			END ItemName, ";
            sql += "		x.ItemValue, ";
            sql += "		CASE WHEN x.rn > 5 THEN 6 ";
            sql += "			ELSE rn END rn ";
            sql += "			from ( ";
            sql += "SELECT x.*, ROW_NUMBER() over (order by x.ItemValue desc) rn ";
            sql += "  from ( ";
            sql += "	SELECT CASE WHEN LTRIM(RTRIM(ItemName)) = '' ";
            sql += "						   OR ItemName IS NULL THEN 'EMPTY' ";
            sql += "					ELSE ItemName ";
            sql += "					END Itemname, ";
            sql += "					SUM(ItemValue) ItemValue ";
            sql += "			   FROM ChartTable ";
            sql += "			  WHERE 1 = 1 ";
            sql += "				AND ChartKey = 5 ";
            sql += "				AND ItemName != 'PVDF' ";
            sql += "				AND ItemName != 'PP' ";
            sql += $"                  AND DataDate BETWEEN '{dteTo.DateTime.ToString("yyyy-MM-dd")}' AND '{dteFrom.DateTime.ToString("yyyy-MM-dd")}' ";
            if (!chkIsAllProject.Checked)
                sql += string.Format("   and ProjectKey = {0} ", lueProject.EditValue.ToString());
            if (!chkIsAllCompany.Checked)
                sql += string.Format("   and CompKey = {0} ", lueCompany.EditValue.ToString());
            if (!Program.Option.isAdmin)
            {
                if (Program.Option.AuthLevel == 1)
                {
                    sql += $" and CreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                }
                else if (Program.Option.AuthLevel == 2)
                {
                    sql += $" and (CreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR CreateID = '{Program.Option.LoginID}') ";
                }
                else
                {
                    sql += $"  and CreateID = '{Program.Option.LoginID}' ";
                }
            }
            sql += "			  GROUP BY ItemName ";
            sql += "				) x ";
            sql += "				) x ";
            sql += "				) x ";
            sql += "				group by itemname, rn ";
            sql += "				order by itemvalue desc";

            DataTable dtChart = DBManager.Instance.GetDataTable(sql);

            chartWeldMaterialETC.Series["Series1"].DataSource = dtChart;
            chartWeldMaterialETC.Series["Series1"].ArgumentScaleType = ScaleType.Auto;
            chartWeldMaterialETC.Series["Series1"].ArgumentDataMember = "ItemName";
            chartWeldMaterialETC.Series["Series1"].ValueScaleType = ScaleType.Numerical;
            chartWeldMaterialETC.Series["Series1"].ValueDataMembers.AddRange(new string[] { "ItemValue" });
        }

        /// <summary>
        /// Chart: Total Welding Point
        /// </summary>
        private void ShowChartWelderTop10()
        {
            string sql = string.Empty;
            sql += "SELECT * ";
            sql += "  FROM ( SELECT TOP 10 ItemName, ";
            sql += "                      SUM(ItemValue) AS ItemValue ";
            sql += "           FROM ChartTable ";
            sql += "		  WHERE 1 = 1 ";
            sql += "            AND ChartKey = 9 ";
            sql += $"           AND DataDate BETWEEN '{dteTo.DateTime.ToString("yyyy-MM-dd")}' AND '{dteFrom.DateTime.ToString("yyyy-MM-dd")}' ";
            if (!chkIsAllProject.Checked)
                sql += string.Format("   and ProjectKey = {0} ", lueProject.EditValue.ToString());
            if (!chkIsAllCompany.Checked)
                sql += string.Format("   and CompKey = {0} ", lueCompany.EditValue.ToString());
            if (!Program.Option.isAdmin)
            {
                if (Program.Option.AuthLevel == 1)
                {
                    sql += $" and CreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                }
                else if (Program.Option.AuthLevel == 2)
                {
                    sql += $" and (CreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR CreateID = '{Program.Option.LoginID}') ";
                }
                else
                {
                    sql += $"  and CreateID = '{Program.Option.LoginID}' ";
                }
            }
            sql += "          GROUP BY ItemName ";
            sql += "          ORDER BY ItemValue DESC ) x ";
            sql += " ORDER BY ItemValue ";
            DataTable dtChart = DBManager.Instance.GetDataTable(sql);
            Series series = chartWelder.Series[0];
            series.ArgumentDataMember = "ItemName";
            series.ValueDataMembers.AddRange("ItemValue");
            series.DataSource = dtChart;

            sql = string.Empty;
            sql += "SELECT COUNT(ItemName) Weldercnt ";
            sql += "  FROM ( SELECT ItemName ";
            sql += "           FROM ChartTable ";
            sql += "          WHERE 1 = 1 ";
            sql += "            AND ChartKey = 9 ";
            sql += $"           AND DataDate BETWEEN '{dteTo.DateTime.ToString("yyyy-MM-dd")}' AND '{dteFrom.DateTime.ToString("yyyy-MM-dd")}' ";
            if (!chkIsAllProject.Checked)
                sql += string.Format("   and ProjectKey = {0} ", lueProject.EditValue.ToString());
            if (!chkIsAllCompany.Checked)
                sql += string.Format("   and CompKey = {0} ", lueCompany.EditValue.ToString());
            if (!Program.Option.isAdmin)
            {
                if (Program.Option.AuthLevel == 1)
                {
                    sql += $" and CreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                }
                else if (Program.Option.AuthLevel == 2)
                {
                    sql += $" and (CreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR CreateID = '{Program.Option.LoginID}') ";
                }
                else
                {
                    sql += $"  and CreateID = '{Program.Option.LoginID}' ";
                }
            }
            sql += "          GROUP BY ItemName ) x";
            int cnt = DBManager.Instance.GetIntScalar(sql);
            if (cnt > 0)
            {
                lblWelderTotal.Visible = lblWelder.Visible = txtWeldercnt.Visible = true;
                txtWeldercnt.Text = string.Format($"{cnt:n0}");
            }
            else
            {
                lblWelderTotal.Visible = lblWelder.Visible = txtWeldercnt.Visible = false;
            }
        }

        /// <summary>
        /// Chart: OutsideDiameter
        /// </summary>
        private void ShowChartOutsideDiameter()
        {
            string sql = string.Empty;
            sql += "SELECT x.*, ";
            sql += "       x.PVDF + x.PP AS total ";
            sql += "  FROM ( SELECT ItemName, ";
            sql += "                IsNull([PVDF], 0) PVDF, ";
            sql += "                IsNull([PP], 0) PP ";
            sql += "           FROM ( SELECT c.ItemName, ";
            sql += "                         SUM(c.ItemValue) itemvalue, ";
            sql += "                         CASE WHEN RIGHT(chartname, 1) = 'F' THEN 'PVDF' ";
            sql += "                         ELSE 'PP' ";
            sql += "                         END selection ";
            sql += "                    FROM ChartTable c ";
            if (radioGroup1.SelectedIndex == 0)
            {
                sql += "                    JOIN CodeInfomation ci ";
                sql += "                      ON ci.Category = 'Diameter' ";
                sql += "                         AND CAST(ci.DetailCode AS INT) = c.ItemName ";
            }
            sql += "                   WHERE 1 = 1 ";
            sql += "                     AND c.ChartKey = 15 ";
            sql += $"                    AND DataDate BETWEEN '{dteTo.DateTime.ToString("yyyy-MM-dd")}' AND '{dteFrom.DateTime.ToString("yyyy-MM-dd")}' ";
            if (!chkIsAllProject.Checked)
                sql += string.Format("   and ProjectKey = {0} ", lueProject.EditValue.ToString());
            if (!chkIsAllCompany.Checked)
                sql += string.Format("   and CompKey = {0} ", lueCompany.EditValue.ToString());
            if (!Program.Option.isAdmin)
            {
                if (Program.Option.AuthLevel == 1)
                {
                    sql += $" and CreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                }
                else if (Program.Option.AuthLevel == 2)
                {
                    sql += $" and (CreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR CreateID = '{Program.Option.LoginID}') ";
                }
                else
                {
                    sql += $"  and CreateID = '{Program.Option.LoginID}' ";
                }
            }
            sql += "                   GROUP BY ItemName, ";
            sql += "                            RIGHT(chartname, 1) ) AS result ";
            sql += "                PIVOT (SUM(itemvalue) ";
            sql += "                      FOR Selection IN ([PVDF], ";
            sql += "                                        [PP])) AS pivot_result ) x";
            DataTable dtChart = DBManager.Instance.GetDataTable(sql);
            Series series1 = chartOutsideDiameter.Series[0];
            series1.Name = "PVDF";
            series1.ArgumentDataMember = "ItemName";
            series1.ValueDataMembers.AddRange("PVDF");
            series1.DataSource = dtChart;

            Series series2 = chartOutsideDiameter.Series[1];
            series2.Name = "PP";
            series2.ArgumentDataMember = "ItemName";
            series2.ValueDataMembers.AddRange("PP");
            series2.DataSource = dtChart;

            Series series3 = chartOutsideDiameter.Series[2];
            series3.Name = "Total";
            series3.ArgumentDataMember = "ItemName";
            series3.ValueDataMembers.AddRange("Total");
            series3.DataSource = dtChart;

            XYDiagram diagram = chartOutsideDiameter.Diagram as XYDiagram;
            if (diagram == null)
                return;
            diagram.AxisX.QualitativeScaleComparer = new ODComparer();
        }

        /// <summary>
        /// 정규 OD or All 선택
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
            SplashScreenManager.Default.SetWaitFormDescription("The loading of data. Please wait....");

            ShowChartOutsideDiameter();

            SplashScreenManager.CloseForm(false);
        }

        /// <summary>
        /// Chart: MachineSeries
        /// </summary>
        private void ShowChartMachineSeries()
        {
            string sql = string.Empty;
            sql += "select x.ItemName, sum(x.ItemValue) ItemValue, rn ";
            sql += "	  from ( ";
            sql += "		   select CASE WHEN x.Ratio < 0.05 THEN 'ETC' ";
            sql += "				   ELSE ItemName ";
            sql += "				   END ItemName, ";
            sql += "				   ItemValue, ";
            sql += "				   CASE WHEN x.Ratio < 0.05 THEN 10000 ";
            sql += "				   ELSE rn ";
            sql += "				   END rn ";
            sql += "			  from ( ";
            sql += "					select x.*, CONVERT(float, x.ItemValue) / SUM(x.ItemValue) OVER() AS Ratio, ROW_NUMBER() over(order by ItemValue desc) rn ";
            sql += "					  from ( ";
            sql += "						select ItemName, sum(ItemValue) ItemValue ";
            sql += "						  from ChartTable ";
            sql += "						 where chartkey = 11 ";
            sql += $"                          AND DataDate BETWEEN '{dteTo.DateTime.ToString("yyyy-MM-dd")}' AND '{dteFrom.DateTime.ToString("yyyy-MM-dd")}' ";
            if (!chkIsAllProject.Checked)
                sql += string.Format("         and ProjectKey = {0} ", lueProject.EditValue.ToString());
            if (!chkIsAllCompany.Checked)
                sql += string.Format("         and CompKey = {0} ", lueCompany.EditValue.ToString());
            if (!Program.Option.isAdmin)
            {
                if (Program.Option.AuthLevel == 1)
                {
                    sql += $" and CreateID IN (SELECT UserID FROM UserInfo WHERE ParentAdminID = '{Program.Option.LoginID}') ";
                }
                else if (Program.Option.AuthLevel == 2)
                {
                    sql += $" and (CreateID IN (SELECT UserID FROM UserInfo WHERE ParentManagerID = '{Program.Option.LoginID}') ";
                    sql += $"      OR CreateID = '{Program.Option.LoginID}') ";
                }
                else
                {
                    sql += $"  and CreateID = '{Program.Option.LoginID}' ";
                }
            }
            sql += "						 group by ItemName ";
            sql += "					 ) x ";
            sql += " ";
            sql += "				) x ";
            sql += "		) x ";
            sql += "	 group by x.ItemName, rn ";
            sql += "order by ItemValue DESC ";

            DataTable dtChart = DBManager.Instance.GetDataTable(sql);
            TreeMapFlatDataAdapter DataAdapter = (TreeMapFlatDataAdapter)treeMapWeld.DataAdapter;
            DataAdapter.DataSource = dtChart;
        }

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
        /// 이미지 팝업
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void pictureEdit1_Click(object sender, EventArgs e)
		{
            FormPopDashboard2 frm = new FormPopDashboard2();
            frm.ShowDialog();
		}
	}
}