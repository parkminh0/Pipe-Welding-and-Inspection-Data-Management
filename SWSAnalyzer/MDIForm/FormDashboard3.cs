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
using DevExpress.XtraCharts.Designer.Native;

namespace SWSAnalyzer
{
    public partial class FormDashboard3 : BaseForm
    {
        int focusChart = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="focusChart"></param>
        public FormDashboard3(int focusChart)
        {
            InitializeComponent();
            this.focusChart = focusChart;
        }

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormDashboard3_Load(object sender, EventArgs e)
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
				chartTotalInspectPoint.Titles[0].TextColor = chartInspectOKNOK.Titles[0].TextColor = chartInspectNOKReason.Titles[0].TextColor = chartInspectionMaterial.Titles[0].TextColor = chartInspector.Titles[0].TextColor = chartOutsideDiameter.Titles[0].TextColor = Color.White;
				lblNOK.ForeColor = lblNOKTotal.ForeColor = lblInspectorTotal.ForeColor = lblInspector.ForeColor = Color.White;
				radioGroup1.BackColor = lblNOK.BackColor = lblNOKTotal.BackColor = lblInspectorTotal.BackColor = lblInspector.BackColor = Color.FromArgb(35, 35, 35);
				txtNOKcnt.ForeColor = txtInspectorcnt.ForeColor = Color.Black;
			}
            else
            {
				chartTotalInspectPoint.Titles[0].TextColor = chartInspectOKNOK.Titles[0].TextColor = chartInspectNOKReason.Titles[0].TextColor = chartInspectionMaterial.Titles[0].TextColor = chartInspector.Titles[0].TextColor = chartOutsideDiameter.Titles[0].TextColor = Color.Black;
				lblNOK.ForeColor = lblNOKTotal.ForeColor = lblInspectorTotal.ForeColor = lblInspector.ForeColor = Color.Black;
				radioGroup1.BackColor = lblNOK.BackColor = lblNOKTotal.BackColor = lblInspectorTotal.BackColor = lblInspector.BackColor = Color.White;
				txtNOKcnt.ForeColor = txtInspectorcnt.ForeColor = Color.Black;
			}
		}

		/// <summary>
		/// 폼 쇼운
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormDashboard3_Shown(object sender, EventArgs e)
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
        /// 새로고침
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
            SplashScreenManager.Default.SetWaitFormDescription("The loading of data. Please wait....");

            ShowChartMain();

            SplashScreenManager.CloseForm(false);
        }

        /// <summary>
        /// 챠트 출력
        /// </summary>
        private void ShowChartMain()
        {
            ShowChartTotalInspectionPoint();
            ShowChartInspectOKNOK();
            ShowChartNOKReason();
            ShowChartMaterial();
            ShowChartInspectorTop10();
            ShowChartOutsideDiameter();
            ShowChartMachineSeries();
        }

        /// <summary>
        /// Chart: Total Inspection Point
        /// </summary>
        private void ShowChartTotalInspectionPoint()
        {
            DataTable dtChart1 = null;
            XYDiagram diagram = chartTotalInspectPoint.Diagram as XYDiagram;
            string sql = string.Empty;
            switch (radioGroup2.SelectedIndex)
            {
                case 0: // 연단위
                    sql += "SELECT   FORMAT(CAST(DataDate AS DATE), 'yyyy') YEAR, ";
                    sql += "         SUM(ItemValue)                         points ";
                    sql += "FROM     ChartTable ";
                    sql += "WHERE    1 = 1 ";
                    sql += "         AND chartkey = 2 ";
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
                    sql += "         AND chartkey = 2 ";
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
                    sql += "         AND chartkey = 2 ";
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
                    sql += "         AND chartkey = 2 ";
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

            Series series1 = chartTotalInspectPoint.Series[0];
            series1.ArgumentDataMember = "Year";
            series1.ValueDataMembers.AddRange("points");
            series1.DataSource = dtChart1;
        }
        private void radioGroup2_SelectedIndexChanged(object sender, EventArgs e)
        {
            SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
            SplashScreenManager.Default.SetWaitFormDescription("The loading of data. Please wait....");

            ShowChartTotalInspectionPoint();

            SplashScreenManager.CloseForm(false);
        }

        /// <summary>
        /// Chart: Inspect OK, NOK
        /// </summary>
        private void ShowChartInspectOKNOK()
        {
            string sql = string.Empty;
            sql += "select case when UPPER(ItemName) = 'TRUE' THEN 'OK' else 'NOK' end ItemName, sum(ItemValue) ItemValue ";
            sql += "from ChartTable ";
            sql += "where ChartKey = 4 ";
            sql += $" and DataDate BETWEEN '{dteTo.DateTime.ToString("yyyy-MM-dd")}' AND '{dteFrom.DateTime.ToString("yyyy-MM-dd")}' ";
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
            sql += "group by ItemName ";
            sql += "order by ItemValue DESC ";
            DataTable dtChart = DBManager.Instance.GetDataTable(sql);
            chartInspectOKNOK.Series["Series1"].DataSource = dtChart;
            chartInspectOKNOK.Series["Series1"].ArgumentScaleType = ScaleType.Auto;
            chartInspectOKNOK.Series["Series1"].ArgumentDataMember = "ItemName";
            chartInspectOKNOK.Series["Series1"].ValueScaleType = ScaleType.Numerical;
            chartInspectOKNOK.Series["Series1"].ValueDataMembers.AddRange(new string[] { "ItemValue" });
        }

        /// <summary>
        /// Chart: Welding NOK Reason 
        /// </summary>
        private void ShowChartNOKReason()
        {
            string sql = string.Empty;
            sql += "select ItemName, sum(x.ItemValue) ItemValue, rn ";
            sql += "  from ( ";
            sql += "	select CASE WHEN x.rn > 19 THEN 'ETC' ";
            sql += "					ELSE ItemName END ItemName, ";
            sql += "			   x.ItemValue, ";
            sql += "			   CASE WHEN x.rn > 19 THEN 20 ";
            sql += "					ELSE rn END rn ";
            sql += "	  from ( ";
            sql += "		select x.*, ROW_NUMBER() over(order by x.ItemValue desc) rn ";
            sql += "		  from ( ";
            sql += "			select ItemName, SUM(ItemValue) ItemValue ";
            sql += "			  from ChartTable ";
            sql += "			 where 1 = 1 ";
            sql += "			   and ChartKey = 8 ";
            sql += $"              AND DataDate BETWEEN '{dteTo.DateTime.ToString("yyyy-MM-dd")}' AND '{dteFrom.DateTime.ToString("yyyy-MM-dd")}' ";
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
            sql += "			 group by ItemName ";
            sql += "			) x ";
            sql += "		) x ";
            sql += ") x ";
            sql += "group by ItemName, rn ";
            sql += "order by rn DESC ";
            DataTable dtChart = DBManager.Instance.GetDataTable(sql);
            Series series = chartInspectNOKReason.Series[0];
            series.ArgumentDataMember = "ItemName";
            series.ValueDataMembers.AddRange("ItemValue");
            series.DataSource = dtChart;

            sql = string.Empty;
            sql += "SELECT COUNT(ItemName) ";
            sql += "  FROM ( SELECT ItemName ";
            sql += "            FROM ChartTable ";
            sql += "        WHERE 1 = 1 ";
            sql += "            AND ChartKey = 8 ";
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
            sql += "        GROUP BY ItemName ) x";
            int cnt = DBManager.Instance.GetIntScalar(sql);
            try
            {
                lblNOKTotal.Visible = lblNOK.Visible = txtNOKcnt.Visible = true;
                txtNOKcnt.Text = string.Format($"{cnt:n0}");
            }
            catch
            {
                lblNOKTotal.Visible = lblNOK.Visible = txtNOKcnt.Visible = false;
            }
        }

        /// <summary>
        /// Chart: Material
        /// </summary>
        private void ShowChartMaterial()
        {
            string sql = string.Empty;
            sql += "select ItemName, sum(ItemValue) ItemValue ";
            sql += "  from ChartTable ";
            sql += " where 1 = 1 ";
            sql += "   and ChartKey = 6 ";
            sql += $"  AND DataDate BETWEEN '{dteTo.DateTime.ToString("yyyy-MM-dd")}' AND '{dteFrom.DateTime.ToString("yyyy-MM-dd")}' ";
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
            sql += " group by ItemName ";
            sql += " ORDER BY ItemName DESC";
            DataTable dtChart = DBManager.Instance.GetDataTable(sql);

            chartInspectionMaterial.Series["Series1"].DataSource = dtChart;
            chartInspectionMaterial.Series["Series1"].ArgumentScaleType = ScaleType.Auto;
            chartInspectionMaterial.Series["Series1"].ArgumentDataMember = "ItemName";
            chartInspectionMaterial.Series["Series1"].ValueScaleType = ScaleType.Numerical;
            chartInspectionMaterial.Series["Series1"].ValueDataMembers.AddRange(new string[] { "ItemValue" });
        }

        /// <summary>
        /// Chart: Total Welding Point
        /// </summary>
        private void ShowChartInspectorTop10()
        {
            string sql = string.Empty;
            sql += "SELECT * ";
            sql += "  FROM ( SELECT TOP 10 ItemName, ";
            sql += "                      SUM(ItemValue) ItemValue ";
            sql += "           FROM ChartTable ";
            sql += "		  WHERE 1 = 1 ";
            sql += "            AND ChartKey = 10 ";
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
            Series series = chartInspector.Series[0];
            series.ArgumentDataMember = "ItemName";
            series.ValueDataMembers.AddRange("ItemValue");
            series.DataSource = dtChart;

            sql = string.Empty;
            sql += "SELECT COUNT(*) ";
            sql += "  FROM ( SELECT ItemName ";
            sql += "           FROM ChartTable ";
            sql += "          WHERE 1 = 1 ";
            sql += "            AND ChartKey = 10 ";
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
            sql += "          GROUP BY ItemName) x";
            int cnt = DBManager.Instance.GetIntScalar(sql);
            try
            {
                lblInspectorTotal.Visible = lblInspector.Visible = txtInspectorcnt.Visible = true;
                txtInspectorcnt.Text = string.Format($"{cnt:n0}");
            }
            catch
            {
                lblInspectorTotal.Visible = lblInspector.Visible = txtInspectorcnt.Visible = false;
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
            sql += "				IsNull([PVDF], 0) PVDF, ";
            sql += "				IsNull([PP], 0) PP ";
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
            sql += "                     AND c.ChartKey = 16 ";
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
        /// Chart: Machine Series
        /// </summary>
        private void ShowChartMachineSeries()
        {
            string sql = string.Empty;
            sql += "select ItemName, SUM(ItemValue) ItemValue, rn ";
            sql += "  from ( ";
            sql += "	select CASE WHEN x.Ratio < 0.05 THEN 'ETC' ";
            sql += "		   ELSE ItemName ";
            sql += "		   END ItemName, ";
            sql += "		   ItemValue, ";
            sql += "		   CASE WHEN x.Ratio < 0.05 THEN 10000 ";
            sql += "		   ELSE rn ";
            sql += "		   END rn ";
            sql += "	  from ( ";
            sql += "		select x.*, CONVERT(float, x.ItemValue) / SUM(x.ItemValue) OVER() AS Ratio, ROW_NUMBER() over(order by ItemValue DESC) rn ";
            sql += "		  from ( ";
            sql += "			select ItemName, ";
            sql += "				   SUM(ItemValue) ItemValue ";
            sql += "			  from ChartTable ";
            sql += "			 where 1 = 1 ";
            sql += "               and ChartKey = 12 ";
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
            sql += "			 group by ItemName ";
            sql += "			) x ";
            sql += "		) x ";
            sql += "	) x ";
            sql += "group by ItemName, rn ";
            sql += "order by ItemValue DESC ";

            DataTable dtChart = DBManager.Instance.GetDataTable(sql);
            TreeMapFlatDataAdapter DataAdapter = (TreeMapFlatDataAdapter)treeMapInspect.DataAdapter;
            DataAdapter.DataSource = dtChart;
        }

        /// <summary>
        /// 요약화면 1 에서 넘어왔을 시 포커스 적용
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
                    chartTotalInspectPoint.Focus();
                    break;
                case 2:
                    chartInspectOKNOK.Focus();
                    break;
                case 3:
                    chartInspector.Focus();
                    break;
                case 4:
                    chartOutsideDiameter.Focus();
                    break;
                case 5:
                    treeMapInspect.Focus();
                    break;
            }
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
    }
}