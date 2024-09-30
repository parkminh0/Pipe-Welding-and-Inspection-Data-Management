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
using DevExpress.XtraCharts;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeMap;

namespace SWSAnalyzer
{
    public partial class FormDashboard : BaseForm
    {
        #region 폼 로드
        /// <summary>
        /// 
        /// </summary>
        public FormDashboard()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormDashboard_Load(object sender, EventArgs e)
        {
            if (!Program.Option.isAdmin)
                btnDataLoad.Visible = false;

            SetSkinStyle();
		}

        public override void SetSkinStyle()
        {
			if (Properties.Settings.Default.UserPalette.Contains("Dark"))
			{
				panelWeld.BackColor = panelInspection.BackColor = txtWeldingPoints.BackColor = txtInspectionPoints.BackColor = Color.FromArgb(35, 35, 35);
				chartWeldOKNOK.Titles[0].TextColor = chartInspectOKNOK.Titles[0].TextColor = chartWeldingMaterial.Titles[0].TextColor = chartInspectionMaterial.Titles[0].TextColor = Color.White;
				chartWeldingNOKReason.Titles[0].TextColor = chartInspectNOKReason.Titles[0].TextColor = chartWelder.Titles[0].TextColor = chartInspector.Titles[0].TextColor = chartWeldingOutsideDiameter.Titles[0].TextColor = chartInspectionOutsideDiameter.Titles[0].TextColor = Color.White;
				txtWeldingPoints.ForeColor = txtInspectionPoints.ForeColor = Color.White;
			}
            else
            {
				panelWeld.BackColor = panelInspection.BackColor = txtWeldingPoints.BackColor = txtInspectionPoints.BackColor = Color.White;
				chartWeldOKNOK.Titles[0].TextColor = chartInspectOKNOK.Titles[0].TextColor = chartWeldingMaterial.Titles[0].TextColor = chartInspectionMaterial.Titles[0].TextColor = Color.Black;
				chartWeldingNOKReason.Titles[0].TextColor = chartInspectNOKReason.Titles[0].TextColor = chartWelder.Titles[0].TextColor = chartInspector.Titles[0].TextColor = chartWeldingOutsideDiameter.Titles[0].TextColor = chartInspectionOutsideDiameter.Titles[0].TextColor = Color.Black;
				txtWeldingPoints.ForeColor = txtInspectionPoints.ForeColor = Color.Black;
			}
		}

        /// <summary>
        /// 폼 쇼운
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormDashboard_Shown(object sender, EventArgs e)
        {
            SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
            SplashScreenManager.Default.SetWaitFormDescription("The loading of data. Please wait....");

            try
            {
                ShowChartMain();
            }
            catch (Exception ex)
            {
            }

            SplashScreenManager.CloseForm(false);
        }
        #endregion

        #region 차트 셋팅
        /// <summary>
        /// 새로고침(ChartTable 생성)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDataLoad_Click(object sender, EventArgs e)
        {
            SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
            SplashScreenManager.Default.SetWaitFormDescription("The loading of data. Please wait....");

            try
            {
                DataTable tmp = DBManager.Instance.MDB.uspusp_GetChartTable();
                ShowChartMain();
            }
            catch (Exception ex)
            {
            }

            SplashScreenManager.CloseForm(false);
        }

        /// <summary>
        /// 데이터 새로고침(업체용)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
            SplashScreenManager.Default.SetWaitFormDescription("The loading of data. Please wait....");

            try
            {
                ShowChartMain();
            }
            catch(Exception ex)
            {
            }

            SplashScreenManager.CloseForm(false);
        }

        /// <summary>
        /// 챠트 출력
        /// </summary>
        private void ShowChartMain()
        {
            ShowPoints();
            ShowChartWeldingMaterial();
            ShowChartInspectionMaterial();
            ShowChartWeldOKNOK();
            ShowInspectOKNOK();
            ShowWeldingNOKReason();
            ShowInspectionNOKReason();
            ShowWelderTop();
            ShowInspectorTop();
            ShowWeldingMachine();
            ShowInspectionMachine();
            ShowWeldingOutsideDiameter();
            ShowInspectionOutsideDiameter();
        }

        /// <summary>
        /// 7. Last Update (최근30일)
        /// </summary>
        private void ShowPoints()
        {
            string sql = string.Empty;
            sql += "select x.WeldingPoints, y.BeadPoints ";
            sql += "   from ";
            sql += " ( ";
            sql += "	 select sum(ItemValue) WeldingPoints ";
            sql += "	   from ChartTable c1 ";
            sql += "	  where 1 = 1 ";
            sql += "        and ChartKey = 1 ";
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
            sql += ") x ";
            sql += "	cross join ( ";
            sql += "	 select sum(ItemValue) BeadPoints ";
            sql += "	   from ChartTable c2 ";
            sql += "	  where 1 = 1 ";
            sql += "        and ChartKey = 2 ";
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
            sql += "	) y";
            DataTable dtPoinsts = DBManager.Instance.GetDataTable(sql);
            
            // Total Welding
            try
            {
                double wp = double.Parse(dtPoinsts.Rows[0]["WeldingPoints"].ToString());
                if (wp >= 1000000)
                {
                    txtWeldingPoints.Text = string.Format("{0} M", Math.Round(wp / 1000000, 1));
                    lbcWeldPoint.Text = "(million)";
                }
                else if (wp >= 1000)
                {
                    txtWeldingPoints.Text = string.Format("{0} T", Math.Round(wp / 1000, 1));
                    lbcWeldPoint.Text = "(thousand)";
                }
                else
                {
                    txtWeldingPoints.Text = string.Format("{0}", wp);
                    lbcWeldPoint.Text = "";
                }
            }
            catch (Exception e)
            {
                lbcWeldPoint.Visible = false;
                txtWeldingPoints.Visible = false;
            }

            // Total Inspection
            try
            {
                double ip = double.Parse(dtPoinsts.Rows[0]["BeadPoints"].ToString());
                if (ip >= 1000000)
                {
                    txtInspectionPoints.Text = string.Format("{0} M", Math.Round(ip / 1000000, 1));
                    lbcBeadPoint.Text = "(million)";
                }
                else if (ip >= 1000)
                {
                    txtInspectionPoints.Text = string.Format("{0} T", Math.Round(ip / 1000, 1));
                    lbcBeadPoint.Text = "(thousand)";
                }
                else
                {
                    txtInspectionPoints.Text = string.Format("{0}", ip);
                    lbcBeadPoint.Text = "";
                }
            }
            catch (Exception e)
            {
                lbcBeadPoint.Visible = false;
                txtInspectionPoints.Visible = false;
            }

            // Time
            try
            {
                sql = string.Empty;
                sql += "select * from ChartTime ";
                DataTable dtTime = DBManager.Instance.GetDataTable(sql);
                lbcWeldTime.Text = DateTime.Parse(dtTime.Rows[0][0].ToString()).ToString("(" + "yyyy.MM.dd. HH:mm" + ")");
                lbcBeadTime.Text = DateTime.Parse(dtTime.Rows[0][0].ToString()).ToString("(" + "yyyy.MM.dd. HH:mm" + ")");
            }
            catch (Exception e)
            {
                lbcWeldTime.Visible = false;
                lbcBeadTime.Visible = false;
            }

            sql = string.Empty;
            sql += "select FORMAT(CAST(DataDate AS DATE), 'yyyy') year, sum(ItemValue) points ";
            sql += " from ChartTable ";
            sql += " where 1 = 1 ";
            sql += "   and chartkey = 1 ";
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
            sql += " group by FORMAT(CAST(DataDate AS DATE), 'yyyy') ";
            sql += " order by FORMAT(CAST(DataDate AS DATE), 'yyyy')";
            DataTable dtChart1 = DBManager.Instance.GetDataTable(sql);
            Series series1 = chartWeldPoints.Series[0];
            series1.ArgumentDataMember = "year";
            series1.ValueDataMembers.AddRange("points");
            series1.DataSource = dtChart1;

            sql = string.Empty;
            sql += "select FORMAT(CAST(DataDate AS DATE), 'yyyy-MM') yearMonth, sum(ItemValue) points ";
            sql += " from ChartTable ";
            sql += " where 1 = 1 ";
            sql += "   and chartkey = 2 ";
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
            sql += " group by FORMAT(CAST(DataDate AS DATE), 'yyyy-MM') ";
            sql += " order by FORMAT(CAST(DataDate AS DATE), 'yyyy-MM')";
            DataTable dtChart2 = DBManager.Instance.GetDataTable(sql);
            Series series2 = chartInspectPoints.Series[0];
            series2.ArgumentDataMember = "yearMonth";
            series2.ValueDataMembers.AddRange("Points");
            series2.DataSource = dtChart2;
        }

        /// <summary>
        /// chart: Weld OK, NOK
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
            sql += "					  on p.BaseYmd = ct.DataDate ";
            sql += "					where 1 = 1 ";
            sql += "                      and ct.ChartKey = 3 ";
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
        /// chart: Inspect OK, NOK
        /// </summary>
        private void ShowInspectOKNOK()
        {
            string sql = string.Empty;
            sql += "select case when UPPER(ItemName) = 'TRUE' THEN 'OK' else 'NOK' end ItemName, sum(ItemValue) ItemValue ";
            sql += "from ChartTable ";
            sql += "where 1 = 1 ";
            sql += "  and ChartKey = 4 ";
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
        /// chart: Welding Material
        /// </summary>
        private void ShowChartWeldingMaterial()
        {
            string sql = string.Empty;
            sql += "select ItemName, sum(ItemValue) ItemValue ";
            sql += "  from ( ";
            sql += " select case when ItemName != 'PVDF' and ItemName != 'PP' THEN 'ETC' ELSE ItemName END ItemName, sum(ItemValue) ItemValue ";
            sql += "   from ChartTable ";
            sql += "  where 1 = 1 ";
            sql += "    and ChartKey = 5 ";
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
        /// chart: Inspection Material
        /// </summary>
        private void ShowChartInspectionMaterial()
        {
            string sql = string.Empty;
            sql += "SELECT x.ItemName, SUM(x.ItemValue) ItemValue ";
            sql += "  FROM ( ";
            sql += "	select CASE WHEN ItemName != 'PP' AND ItemName != 'PVDF' THEN 'ETC' ELSE ItemName END ItemName, sum(ItemValue) ItemValue ";
            sql += "	  from ChartTable ";
            sql += "	 where 1 = 1 ";
            sql += "       and ChartKey = 6 ";
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
            sql += "	 group by ItemName ";
            sql += "	 ) x ";
            sql += " GROUP BY x.ItemName ";
            sql += " ORDER BY x.ItemName DESC";
            DataTable dtChart = DBManager.Instance.GetDataTable(sql);

            chartInspectionMaterial.Series["Series1"].DataSource = dtChart;
            chartInspectionMaterial.Series["Series1"].ArgumentScaleType = ScaleType.Auto;
            chartInspectionMaterial.Series["Series1"].ArgumentDataMember = "ItemName";
            chartInspectionMaterial.Series["Series1"].ValueScaleType = ScaleType.Numerical;
            chartInspectionMaterial.Series["Series1"].ValueDataMembers.AddRange(new string[] { "ItemValue" });
        }

        /// <summary>
        /// Welding NOK Reason 
        /// </summary>
        private void ShowWeldingNOKReason()
        {
            string sql = string.Empty;
            sql += "select x.ItemName, SUM(x.ItemValue) ItemValue, x.rn ";
            sql += "  from ( ";
            sql += "	select CASE WHEN x.rn > 5 THEN 'ETC' ";
            sql += "				ELSE ItemName END ItemName, ";
            sql += "			x.ItemValue, ";
            sql += "			CASE WHEN x.rn > 5 THEN 6 ";
            sql += "				ELSE rn END rn ";
            sql += "	  FROM ( ";
            sql += "		SELECT x.*, ROW_NUMBER() over(order by x.ItemValue desc) rn ";
            sql += "		  FROM ( ";
            sql += "			SELECT ItemName, SUM(ItemValue) ItemValue ";
            sql += "			  FROM ( ";
            sql += "				SELECT CASE WHEN ct.ItemName LIKE '0%' THEN CONCAT('ERROR ', ct.ItemName) ";
            sql += "						 ELSE ct.ItemName ";
            sql += "					   END ItemName, ";
            sql += "					   ItemValue ";
            sql += "				  FROM ChartTable ct ";
            sql += "				 WHERE 1 = 1 ";
            sql += "                   AND ct.ChartKey = 3 ";
            sql += "				   AND LEFT(ct.ItemName, 6) != 'WELDP.' ";
            sql += "				   AND LEFT(ct.ItemName, 7) != 'PH-WELD' ";
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
            sql += "				 ) x ";
            sql += "			 GROUP BY ItemName ";
            sql += "			) x ";
            sql += "		) x ";
            sql += "	) x ";
            sql += "	GROUP BY ItemName, rn ";
            sql += "    ORDER BY rn DESC ";
            DataTable dtChart = DBManager.Instance.GetDataTable(sql);
            Series series = chartWeldingNOKReason.Series[0];
            series.ArgumentDataMember = "ItemName";
            series.ValueDataMembers.AddRange("ItemValue");
            series.DataSource = dtChart;
        }

        /// <summary>
        /// Inspection NOK Reason 
        /// </summary>
        private void ShowInspectionNOKReason()
        {
            string sql = string.Empty;
            sql += "select ItemName, sum(x.ItemValue) ItemValue, rn ";
            sql += "  from ( ";
            sql += "	select CASE WHEN x.rn > 5 THEN 'ETC' ";
            sql += "					ELSE ItemName END ItemName, ";
            sql += "			   x.ItemValue, ";
            sql += "			   CASE WHEN x.rn > 5 THEN 6 ";
            sql += "					ELSE rn END rn ";
            sql += "	  from ( ";
            sql += "		select x.*, ROW_NUMBER() over(order by x.ItemValue desc) rn ";
            sql += "		  from ( ";
            sql += "			select ItemName, SUM(ItemValue) ItemValue ";
            sql += "			  from ChartTable ";
            sql += "			 where 1 = 1 ";
            sql += "			   and ChartKey = 8 ";
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
        }

        /// <summary>
        /// chart: Welder Top3
        /// </summary>
        private void ShowWelderTop()
        {
            string sql = string.Empty;
            sql += "SELECT * ";
            sql += "  FROM ( SELECT TOP 3 ItemName, ";
            sql += "                      SUM(ItemValue) AS ItemValue ";
            sql += "           FROM ChartTable ";
            sql += "		  WHERE 1 = 1 ";
            sql += "            AND ChartKey = 9 ";
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
            sql += " ORDER BY x.ItemValue DESC ";
            DataTable dtChart = DBManager.Instance.GetDataTable(sql);
            Series series = chartWelder.Series[0];
            series.ArgumentDataMember = "ItemName";
            series.ValueDataMembers.AddRange("ItemValue");
            series.DataSource = dtChart;
        }

        /// <summary>
        /// chart: Inspector Top3
        /// </summary>
        private void ShowInspectorTop()
        {
            string sql = string.Empty;
            sql += "SELECT * ";
            sql += "  FROM ( SELECT TOP 3 ItemName, ";
            sql += "                      SUM(ItemValue) ItemValue ";
            sql += "           FROM ChartTable ";
            sql += "		  WHERE 1 = 1 ";
            sql += "            AND ChartKey = 10 ";
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
            sql += " ORDER BY x.ItemValue DESC ";

            DataTable dtChart = DBManager.Instance.GetDataTable(sql);
            Series series = chartInspector.Series[0];
            series.ArgumentDataMember = "ItemName";
            series.ValueDataMembers.AddRange("ItemValue");
            series.DataSource = dtChart;
        }

        /// <summary>
        /// Welding Machine TreeMap
        /// </summary>
        private void ShowWeldingMachine()
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
            sql += "						 where 1 = 1 ";
            sql += "                           and chartkey = 11 ";
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
        /// Inspection Machine TreeMap
        /// </summary>
        private void ShowInspectionMachine()
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
        /// Weld Outside Diameter Chart
        /// </summary>
        private void ShowWeldingOutsideDiameter()
        {
            string sql = string.Empty;
            sql += "select c.ItemName, SUM(c.ItemValue) ItemValue ";
            sql += "  from ChartTable c ";
            sql += "  JOIN CodeInfomation ci ";
            sql += "	ON ci.Category = 'Diameter' and ci.DetailCode = c.ItemName ";
            sql += " WHERE 1 = 1 ";
            sql += "   AND c.ChartKey = 13 ";
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
            sql += " GROUP BY ItemName";
            DataTable dtChart = DBManager.Instance.GetDataTable(sql);
            Series series = chartWeldingOutsideDiameter.Series[0];
            series.ArgumentDataMember = "ItemName";
            series.ValueDataMembers.AddRange("ItemValue");
            series.DataSource = dtChart;

            XYDiagram diagram = chartWeldingOutsideDiameter.Diagram as XYDiagram;
            if (diagram == null)
                return;
            diagram.AxisX.QualitativeScaleComparer = new ODComparer();
        }

        /// <summary>
        /// Inspect Outside Diameter Chart
        /// </summary>
        private void ShowInspectionOutsideDiameter()
        {
            string sql = string.Empty;
            sql += "select ItemName, SUM(ItemValue) ItemValue ";
            sql += "   from ChartTable ";
            sql += "  where 1 = 1 ";
            sql += "    and ChartKey = 14 ";
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
            sql += "  group by ItemName";
            DataTable dtChart = DBManager.Instance.GetDataTable(sql);
            Series series = chartInspectionOutsideDiameter.Series[0];
            series.ArgumentDataMember = "ItemName";
            series.ValueDataMembers.AddRange("ItemValue");
            series.DataSource = dtChart;

            XYDiagram diagram = chartInspectionOutsideDiameter.Diagram as XYDiagram;
            if (diagram == null)
                return;
            diagram.AxisX.QualitativeScaleComparer = new ODComparer();
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
        /// 통계화면 이동
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Chart_DoubleClick(object sender, EventArgs e)
        {
            ChartControl chart = (ChartControl)sender;
            switch (chart.Name)
            {
                // 융착통계
                case "chartWeldPoints":
                    if (!Program.Option.FormDashboard2)
                        return;

                    Program.mainApp.OpenDashBoard2("FormDashboard2", 1);
                    break;
                case "chartWeldOKNOK":
                    if (!Program.Option.FormDashboard2)
                        return;

                    Program.mainApp.OpenDashBoard2("FormDashboard2", 2);
                    break;
                case "chartWeldingNOKReason":
                    if (!Program.Option.FormDashboard2)
                        return;

                    Program.mainApp.OpenDashBoard2("FormDashboard2", 2);
                    break;
                case "chartWeldingMaterial":
                    if (!Program.Option.FormDashboard2)
                        return;

                    Program.mainApp.OpenDashBoard2("FormDashboard2", 3);
                    break;
                case "chartWelder":
                    if (!Program.Option.FormDashboard2)
                        return;

                    Program.mainApp.OpenDashBoard2("FormDashboard2", 3);
                    break;
                case "chartWeldingOutsideDiameter":
                    if (!Program.Option.FormDashboard2)
                        return;

                    Program.mainApp.OpenDashBoard2("FormDashboard2", 4);
                    break;
                // 검수통계
                case "chartInspectPoints":
                    if (!Program.Option.FormDashboard3)
                        return;

                    Program.mainApp.OpenDashBoard2("FormDashboard3", 1);
                    break;
                case "chartInspectOKNOK":
                    if (!Program.Option.FormDashboard3)
                        return;

                    Program.mainApp.OpenDashBoard2("FormDashboard3", 2);
                    break;
                case "chartInspectNOKReason":
                    if (!Program.Option.FormDashboard3)
                        return;

                    Program.mainApp.OpenDashBoard2("FormDashboard3", 2);
                    break;
                case "chartInspectionMaterial":
                    if (!Program.Option.FormDashboard3)
                        return;

                    Program.mainApp.OpenDashBoard2("FormDashboard3", 3);
                    break;
                case "chartInspector":
                    if (!Program.Option.FormDashboard3)
                        return;

                    Program.mainApp.OpenDashBoard2("FormDashboard3", 3);
                    break;
                case "chartInspectionOutsideDiameter":
                    if (!Program.Option.FormDashboard3)
                        return;

                    Program.mainApp.OpenDashBoard2("FormDashboard3", 4);
                    break;

            }
        }
        private void panelWeld_DoubleClick(object sender, EventArgs e)
        {
            if (!Program.Option.FormDashboard2)
                return;

            Program.mainApp.OpenDashBoard2("FormDashboard2", 1);
        }
        private void treeMapWeld_DoubleClick(object sender, EventArgs e)
        {
            if (!Program.Option.FormDashboard2)
                return;

            Program.mainApp.OpenDashBoard2("FormDashboard2", 5);
        }
        private void panelInspection_DoubleClick(object sender, EventArgs e)
        {
            if (!Program.Option.FormDashboard3)
                return;

            Program.mainApp.OpenDashBoard2("FormDashboard3", 1);
        }
        private void treeMapInspect_DoubleClick(object sender, EventArgs e)
        {
            if (!Program.Option.FormDashboard3)
                return;

            Program.mainApp.OpenDashBoard2("FormDashboard3", 5);
        }
    }
}