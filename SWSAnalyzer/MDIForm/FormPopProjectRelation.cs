using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SWSAnalyzer
{
    public partial class FormPopProjectRelation : DevExpress.XtraEditors.XtraForm
    {
        private int formMode;
        private int transferCount;
        public string transferID;
        public int transferProject;
        public string toProjName;
        public FormPopProjectRelation(int formMode, int transferCount)
        {
            InitializeComponent();
            this.formMode = formMode;
            this.transferCount = transferCount;
        }

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormPopProjectRelation_Load(object sender, EventArgs e)
        {
            SetData();

            if (formMode == 1)
            {
                btnExportExcel.Visible = false;
            }
        }

        /// <summary>
        /// 데이터 로드
        /// </summary>
        private void SetData()
        {
            string sql = string.Empty;
            sql += "SELECT x.ProjectKey, x.ProjectName, x.AdminComp, x.AdminID, y.ManagerComp, y.ManagerID, z.OperatorComp, z.OperatorID ";
            sql += "  FROM ( ";
            sql += "	SELECT p.ProjectName, ";
            sql += "		   c.CompName AdminComp, ";
            sql += "		   u.UserID AdminID, ";
            sql += "		   p.ProjectKey ";
            sql += "	  FROM UserInfo u ";
            sql += "	  JOIN Company c ";
            sql += "		ON c.CompKey = u.CompKey ";
            sql += "	  JOIN Project p ";
            sql += "		ON p.CompKey = c.CompKey ";
            sql += "	 WHERE 1 = 1 ";
            sql += "	   AND u.AuthLevel = 1 ";
            sql += "	   AND u.isDeleted = 0 ";
            sql += "	   AND p.isDeleted = 0 ";
            sql += " ) x ";
            sql += " LEFT JOIN (SELECT u.UserID ManagerID, ";
            sql += "                   c.CompName ManagerComp, ";
            sql += "				   u.ParentAdminID ParentAdminID, ";
            sql += "				   u.ProjectKey ";
            sql += "              FROM UserInfo u ";
            sql += "			  JOIN Company c ";
            sql += "			    ON c.CompKey = u.CompKey ";
            sql += "			 WHERE 1 = 1 ";
            sql += "			   AND u.AuthLevel = 2 ";
            sql += "			   ANd u.isDeleted = 0 ";
            sql += "		    ) y ";
            sql += "	     ON y.ParentAdminID = x.AdminID ";
            sql += "		AND y.ProjectKey = x.ProjectKey ";
            sql += "  LEFT JOIN (SELECT u.UserID OperatorID, ";
            sql += "                   c.CompName OperatorComp, ";
            sql += "				   u.ParentAdminID, ";
            sql += "				   u.ParentManagerID, ";
            sql += "				   u.ProjectKey ";
            sql += "              FROM UserInfo u ";
            sql += "			  JOIN Company c ";
            sql += "			    ON c.CompKey = u.CompKey ";
            sql += "			 WHERE 1 = 1 ";
            sql += "			   AND u.AuthLevel = 3 ";
            sql += "			   ANd u.isDeleted = 0 ";
            sql += "		    ) z ";
            sql += "	     ON z.ParentAdminID = x.AdminID ";
            sql += "		AND z.ParentManagerID = y.ManagerID ";
            sql += "		AND y.ProjectKey = x.ProjectKey";
            DataTable dt = DBManager.Instance.GetDataTable(sql);
            grdProject.DataSource = dt;
            grdViewProject.BestFitColumns();
        }

        /// <summary>
        /// 엑셀 저장
        /// </summary>
        static string targetFileName;
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            targetFileName = GetFileName("xlsx", LangResx.Main.excel_ProjRealation);
            if (targetFileName.Trim() != "")
                grdProject.ExportToXlsx(targetFileName, new XlsxExportOptionsEx { ExportType = ExportType.WYSIWYG });
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

        /// <summary>
        /// 닫기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 융착 데이터 프로젝트 전환용
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void grdViewProject_DoubleClick(object sender, EventArgs e)
		{
            if (formMode != 1)
                return;

			DXMouseEventArgs ea = e as DXMouseEventArgs;
			GridView view = sender as GridView;
			GridHitInfo info = view.CalcHitInfo(ea.Location);
			if (info.InRow || info.InRowCell)
			{
                if (info.Column == null)
                    return;
				
                string colCaption = info.Column.FieldName;
				
                if (colCaption != "AdminID" && colCaption != "ManagerID" && colCaption != "OperatorID")
                    return;

				transferID = view.GetRowCellDisplayText(info.RowHandle, colCaption);
                if (string.IsNullOrWhiteSpace(transferID)) return;

				transferProject = int.Parse(view.GetRowCellValue(info.RowHandle, "ProjectKey").ToString());
				string selectedProject = view.GetRowCellValue(info.RowHandle, "ProjectName").ToString();

				if (XtraMessageBox.Show(string.Format(LangResx.Main.msg_MasterTransfer1, toProjName, transferCount, selectedProject, transferID), LangResx.Main.msg_title_MasterTransfer, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
					this.DialogResult = DialogResult.OK;
                }
			}
		}
	}
}