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

namespace SWSAnalyzer
{
    public partial class FormPopBeadMastercs : DevExpress.XtraEditors.XtraForm
    {
        public int ChoiceBeadKey;
        public bool isCanWeldFormOpen;
        private DataTable dtBeadMaster;
        private DataRow row;
        private int WeldKey;

        public FormPopBeadMastercs()
        {
            InitializeComponent();
        }

        private void FormPopBeadMastercs_Load(object sender, EventArgs e)
        {
            WeldKey = 0;
            if (!Program.Option.SaveExcel)
                btnSaveExcel.Enabled = false;
        }

        private void FormPopBeadMastercs_Shown(object sender, EventArgs e)
        {
            GetData();
            ShowData();
        }

        private int isSelectedInt;
        /// <summary>
        /// BeadMaster 한건 불러오기
        /// </summary>
        private void GetData()
        {
            string sql = LogicManager.Common.GetSelectBeadMasterSql(); //Query문장 가져오기
            sql += string.Format("   and bm.BeadKey = {0} ", ChoiceBeadKey);
            
            try
            {
                dtBeadMaster = DBManager.Instance.GetDataTable(sql);
                isSelectedInt = dtBeadMaster.Rows.Count;
            }
            catch (Exception)
            {
                isSelectedInt = 0;
            }
        }

        /// <summary>
        /// 화면에 데이터 출력
        /// </summary>
        private void ShowData()
        {
            if (isSelectedInt == 0)
            {
                XtraMessageBox.Show(LangResx.Main.msg_PopDataEmpty, LangResx.Main.msg_title_PopDataEmpty, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            row = dtBeadMaster.Rows[0];
            txtBeadKey.Text = row["BeadKey"].ToString();
            txtCompName.Text = row["CompName"].ToString();
            txtProjectName.Text = row["ProjectName"].ToString();
            txtLineNo.Text = row["LineNo"].ToString();
            txtSerialNo.Text = row["SerialNo"].ToString();
            txtInspectionNo.Text = row["InspectionNo"].ToString();
            try
            {
                txtInspectionDate.Text = DateTime.Parse(row["InspectionDate"].ToString()).ToShortDateString();
            }
            catch (Exception)
            {
                txtInspectionDate.Text = row["InspectionDate"].ToString();
            }
            txtInspectionTime.Text = row["InspectionTime"].ToString();
            txtInspectorID.Text = row["InspectorID"].ToString();
            txtSoftwareVersion.Text = row["SoftwareVersion"].ToString();
            txtMaterial.Text = row["Material"].ToString();
            txtOD.Text = row["OD"].ToString();
            txtWallThickness.Text = row["WallThickness"].ToString();
            txtSDR.Text = row["SDR"].ToString();
            txtProdCode.Text = row["ProdCode"].ToString();
            txtBatchNo.Text = row["BatchNo"].ToString();
            txtIDNo.Text = row["IDNo"].ToString();
            txtProdCode2.Text = row["ProdCode2"].ToString();
            txtBatchNo2.Text = row["BatchNo2"].ToString();
            txtIDNo2.Text = row["IDNo2"].ToString();
            txtManufacturer.Text = row["Manufacturer"].ToString();
            txtMachineSerialNo.Text = row["MachineSerialNo"].ToString();
            txtWeldNo.Text = row["WeldNo"].ToString();
            string wd = row["WeldingDate"].ToString();
            try
            {
                if (!string.IsNullOrWhiteSpace(wd))
                    txtWeldingDate.Text = DateTime.Parse(wd).ToShortDateString();
                else
                    txtWeldingDate.Text = string.Empty;
            }
            catch (Exception)
            {
                txtWeldingDate.Text = wd;
            }
            txtWeldedTime.Text = row["WeldedTime"].ToString();
            txtWelder.Text = row["Welder"].ToString();
            txtProject.Text = row["Project"].ToString();
            txtFloor.Text = row["Floor"].ToString();
            txtColumn.Text = row["Column"].ToString();
            txtLocation.Text = row["Location"].ToString();
            txtLineSeries.Text = row["LineSeries"].ToString();
            txtMedium.Text = row["Medium"].ToString();
            txtOtherInfo1.Text = row["OtherInfo1"].ToString();
            txtAmbTemp.Text = row["AmbTemp"].ToString();
            txtPipeTemp.Text = row["PipeTemp"].ToString();
            txtOperatingPressure.Text = row["OperatingPressure"].ToString();
            txtConstructor.Text = row["Constructor"].ToString();
            txtOtherInfo2.Text = row["OtherInfo2"].ToString();
            //txtTestPoints.Text = row["TestPoints"].ToString();
            //txtCompletionRate.Text = row["CompletionRate"].ToString();
            //txtPassRate.Text = row["PassRate"].ToString();
            txtK_C.Text = row["K_C"].ToString();
            txtKMin.Text = string.Format("{0:n2}", double.Parse(row["KMin"].ToString()));
            txtKMax.Text = string.Format("{0:n2}", double.Parse(row["KMax"].ToString()));
            txtKAvg.Text = string.Format("{0:n2}", double.Parse(row["KAvg"].ToString()));
            txtisKPass.Text = row["isKPass"].ToString();
            txtBMin_C.Text = row["BMin_C"].ToString();
            txtBMax_C.Text = row["BMax_C"].ToString();
            txtBMin.Text = string.Format("{0:n2}", double.Parse(row["BMin"].ToString()));
            txtBMax.Text = string.Format("{0:n2}", double.Parse(row["BMax"].ToString()));
            txtBAvg.Text = string.Format("{0:n2}", double.Parse(row["BAvg"].ToString()));
            txtisBPass.Text = row["isBPass"].ToString();
            txtBRatio_C.Text = row["BRatio_C"].ToString();
            txtB1Min.Text = string.Format("{0:n2}", double.Parse(row["B1Min"].ToString()));
            txtB2Min.Text = string.Format("{0:n2}", double.Parse(row["B2Min"].ToString()));
            txtBRatioMin.Text = string.Format("{0:n2}", double.Parse(row["BRatioMin"].ToString()));
            txtB1Max.Text = string.Format("{0:n2}", double.Parse(row["B1Max"].ToString()));
            txtB2Max.Text = string.Format("{0:n2}", double.Parse(row["B2Max"].ToString()));
            txtBRatioMax.Text = string.Format("{0:n2}", double.Parse(row["BRatioMax"].ToString()));
            txtB1Avg.Text = string.Format("{0:n2}", double.Parse(row["B1Avg"].ToString()));
            txtB2Avg.Text = string.Format("{0:n2}", double.Parse(row["B2Avg"].ToString()));
            txtBRatioAvg.Text = string.Format("{0:n2}", double.Parse(row["BRatioAvg"].ToString()));
            txtisBRatioPass.Text = row["isBRatioPass"].ToString();
            txtMissAlign_C.Text = row["MissAlign_C"].ToString();
            txtMissAlignMin.Text = string.Format("{0:n2}", double.Parse(row["MissAlignMin"].ToString()));
            txtMissAlignMax.Text = string.Format("{0:n2}", double.Parse(row["MissAlignMax"].ToString()));
            txtMissAlignAvg.Text = string.Format("{0:n2}", double.Parse(row["MissAlignAvg"].ToString()));
            txtisMissAlignPass.Text = row["isMissAlignPass"].ToString();
            txtNotch_C.Text = row["Notch_C"].ToString();
            txtNotchMin.Text = string.Format("{0:n2}", double.Parse(row["NotchMin"].ToString()));
            txtNotchMax.Text = string.Format("{0:n2}", double.Parse(row["NotchMax"].ToString()));
            txtNotchAvg.Text = string.Format("{0:n2}", double.Parse(row["NotchAvg"].ToString()));
            txtisNotchPass.Text = row["isNotchPass"].ToString();
            txtisNotchEyes.Text = row["isNotchEyes"].ToString();
            txtNotchNote.Text = row["NotchNote"].ToString();
            txtisAngularDevEyes.Text = row["isAngularDevEyes"].ToString();
            txtAngularDevNote.Text = row["AngularDevNote"].ToString();
            txtisCrackEyes.Text = row["isCrackEyes"].ToString();
            txtCrackNote.Text = row["CrackNote"].ToString();
            txtisVoidEyes.Text = row["isVoidEyes"].ToString();
            txtVoidNote.Text = row["VoidNote"].ToString();
            txtisSupportPadEyes.Text = row["isSupportPadEyes"].ToString();
            txtSupportPadNote.Text = row["SupportPadNote"].ToString();
            txtisInterruptionEyes.Text = row["isInterruptionEyes"].ToString();
            txtInterruptionNote.Text = row["InterruptionNote"].ToString();
            txtisOverheatingEyes.Text = row["isOverheatingEyes"].ToString();
            txtOverheatingNote.Text = row["OverheatingNote"].ToString();
            //txtDVS.Text = row["DVS"].ToString();
            //txtVIScore.Text = row["VIScore"].ToString();
            //txtVIGrade.Text = row["VIGrade"].ToString();
            txtCode.Text = row["Code"].ToString();
            txtResult.Text = row["Result"].ToString();
            txtEvaluationScore.Text = row["EvaluationScore"].ToString();
            txtGrade.Text = row["Grade"].ToString();
            WeldKey = int.Parse(row["WeldKey"].ToString());

            if (WeldKey > 0 && isCanWeldFormOpen)
                btnOpenWeldMaster.Enabled = true;
            else
                btnOpenWeldMaster.Enabled = false;
        }

        /// <summary>
        /// 창닫기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Export Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            targetFileName = GetFileName("xlsx", string.Format(LangResx.Main.excel_PopBeadMaster, ChoiceBeadKey));
            if (targetFileName.Trim() != "")
                layoutControl1.ExportToXlsx(targetFileName);
        }
        /// <summary>
        /// Export PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSavePdf_Click(object sender, EventArgs e)
        {
            targetFileName = GetFileName("pdf", string.Format(LangResx.Main.excel_PopBeadMaster, ChoiceBeadKey));
            if (targetFileName.Trim() != "")
                layoutControl1.ExportToPdf(targetFileName);
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
            else if (extName == "pdf")
            {
                fileDialog.DefaultExt = "pdf";
                fileDialog.Filter = "PDF File(*.pdf)|*.pdf";
                fileDialog.FileName = string.Format("{0}_{1}.pdf", defaultName, DateTime.Now.ToString("yyyy-MM-dd"));
            }
            else
            {
                return "Export_" + DateTime.Now.ToString("yyyyMMdd");
            }

            if (fileDialog.ShowDialog() != DialogResult.OK)
                return "";

            string fileName = fileDialog.FileName;
            if (fileName.StartsWith("*."))
                fileName = "";

            return fileName;
        }
        static string targetFileName;

        /// <summary>
        /// 융착정보 창열기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenWeldMaster_Click(object sender, EventArgs e)
        {
            if (WeldKey == 0)
                return;

            FormPopWeldMaster frm = new FormPopWeldMaster();
            frm.ChoiceWeldKey = WeldKey;
            frm.Show();
        }
    }
}