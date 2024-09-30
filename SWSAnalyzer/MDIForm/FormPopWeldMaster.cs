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
    public partial class FormPopWeldMaster : DevExpress.XtraEditors.XtraForm
    {
        public int ChoiceWeldKey;
        public bool isCanBeadFormOpen; //BeadMaster 열기 가능여부

        private DataTable dtWeldMaster;
        private DataRow row;
        private int BeadKey;

        public FormPopWeldMaster()
        {
            InitializeComponent();
        }

        private void FormPopWeldMaster_Load(object sender, EventArgs e)
        {
            BeadKey = 0;
            if (!Program.Option.SaveExcel)
                btnSaveExcel.Enabled = false;
        }

        private void FormPopWeldMaster_Shown(object sender, EventArgs e)
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
            string sql = LogicManager.Common.GetSelectWeldMasterSql(); //Query문장 가져오기
            sql += string.Format("   and WeldKey = {0} ", ChoiceWeldKey);

            try
            {
                dtWeldMaster = DBManager.Instance.GetDataTable(sql);
                isSelectedInt = dtWeldMaster.Rows.Count;
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

            row = dtWeldMaster.Rows[0];
            txtWeldKey.Text = row["WeldKey"].ToString();
            txtMachineSerialNo.Text = row["MachineSerialNo"].ToString();
            txtMachineLine.Text = row["MachineLine"].ToString();
            txtMachineType.Text = row["MachineType"].ToString();
            txtSoftwareVersion.Text = row["SoftwareVersion"].ToString();
            try
            {
                txtLastMaintDate.Text = DateTime.Parse(row["LastMaintDate"].ToString()).ToShortDateString();
            }
            catch (Exception)
            {
                txtLastMaintDate.Text = row["LastMaintDate"].ToString();
            }
            try
            {
                txtNextMaintDate.Text = DateTime.Parse(row["NextMaintDate"].ToString()).ToShortDateString();
            }
            catch (Exception)
            {
                txtNextMaintDate.Text = row["NextMaintDate"].ToString();
            }
            txtWeldProjectDescr.Text = row["WeldProjectDescr"].ToString();
            txtAdditionalData.Text = row["AdditionalData"].ToString();
            txtInstallingCompany.Text = row["InstallingCompany"].ToString();
            txtWelderCode.Text = row["WelderCode"].ToString();
            txtWelderName.Text = row["WelderName"].ToString();
            txtWeldersCompany.Text = row["WeldersCompany"].ToString();
            try
            {
                txtWelderCertExpireDate.Text = DateTime.Parse(row["WelderCertExpireDate"].ToString()).ToShortDateString();
            }
            catch (Exception)
            {
                txtWelderCertExpireDate.Text = row["WelderCertExpireDate"].ToString();
            }
            txtWeldNo.Text = row["WeldNo"].ToString();
            txtDataSeqNo.Text = row["DataSeqNo"].ToString();

            try
            {
                txtWeldingDate.Text = DateTime.Parse(row["WeldingDate"].ToString()).ToShortDateString();
            }
            catch (Exception)
            {
                txtWeldingDate.Text = row["WeldingDate"].ToString();
            }
            try
            {
                txtWeldingDateTime.Text = DateTime.Parse(row["WeldingDateTime"].ToString()).ToShortTimeString();
            }
            catch (Exception)
            {
                txtWeldingDateTime.Text = row["WeldingDateTime"].ToString();
            }
            txtAmbientTemperature.Text = row["AmbientTemperature"].ToString();
            txtStatus.Text = row["Status"].ToString();
            txtWeather.Text = row["Weather"].ToString();
            txtMaterial.Text = row["Material"].ToString();
            txtDiameter.Text = row["Diameter"].ToString();
            txtDiameter2.Text = row["Diameter2"].ToString();
            txtSDR.Text = row["SDR"].ToString();
            txtWallThickness.Text = row["WallThickness"].ToString();
            txtAngle.Text = row["Angle"].ToString();
            txtFittingManufacturer.Text = row["FittingManufacturer"].ToString();
            txtDesign.Text = row["Design"].ToString();
            txtOperatingMode.Text = row["OperatingMode"].ToString();
            txtStandard.Text = row["Standard"].ToString();
            txtMode.Text = row["Mode"].ToString();
            txtFittingCode.Text = row["FittingCode"].ToString();
            txtWeldingBeadLeft.Text = row["WeldingBeadLeft"].ToString();
            txtWeldingBeadRight.Text = row["WeldingBeadRight"].ToString();
            txtWeldingBeadTotal.Text = row["WeldingBeadTotal"].ToString();
            txtWeldingBeadLeft2.Text = row["WeldingBeadLeft2"].ToString();
            txtWeldingBeadRight2.Text = row["WeldingBeadRight2"].ToString();
            txtWeldingBeadTotal2.Text = row["WeldingBeadTotal2"].ToString();
            txtWeldingBeadLeft3.Text = row["WeldingBeadLeft3"].ToString();
            txtWeldingBeadRight3.Text = row["WeldingBeadRight3"].ToString();
            txtWeldingBeadTotal3.Text = row["WeldingBeadTotal3"].ToString();
            txtLatitude.Text = row["Latitude"].ToString();
            txtLongitude.Text = row["Longitude"].ToString();
            txtDragForceNominalValue.Text = row["DragForceNominalValue"].ToString();
            txtDragForceNominalUnit.Text = row["DragForceNominalUnit"].ToString();
            txtDragForceActualValue.Text = row["DragForceActualValue"].ToString();
            txtDragForceActualUnit.Text = row["DragForceActualUnit"].ToString();
            txtMirrorTempNominalValue.Text = row["MirrorTempNominalValue"].ToString();
            txtMirrorTempActualValue.Text = row["MirrorTempActualValue"].ToString();
            txtPreheatingTimeNominalValue.Text = row["PreheatingTimeNominalValue"].ToString();
            txtPreheatingTimeActualValue.Text = row["PreheatingTimeActualValue"].ToString();
            txtBeadBuidupTimeNominalValue.Text = row["BeadBuidupTimeNominalValue"].ToString();
            txtBeadBuidupTimeActualValue.Text = row["BeadBuidupTimeActualValue"].ToString();
            txtBeadBuildupForceNominalValue.Text = row["BeadBuildupForceNominalValue"].ToString();
            txtBeadBuildupForceNominalUnit.Text = row["BeadBuildupForceNominalUnit"].ToString();
            txtBeadBuildupForceActualValue.Text = row["BeadBuildupForceActualValue"].ToString();
            txtBeadBuildupForceActualUnit.Text = row["BeadBuildupForceActualUnit"].ToString();
            txtHeatingTimeNominaValue.Text = row["HeatingTimeNominaValue"].ToString();
            txtHeatingTimeActualValue.Text = row["HeatingTimeActualValue"].ToString();
            txtHeatingForceNominalValue.Text = row["HeatingForceNominalValue"].ToString();
            txtHeatingForceNominalUnit.Text = row["HeatingForceNominalUnit"].ToString();
            txtHeatingForceActualValue.Text = row["HeatingForceActualValue"].ToString();
            txtHeatingForceActualUnit.Text = row["HeatingForceActualUnit"].ToString();
            txtChangeOverTimeNominalValue.Text = row["ChangeOverTimeNominalValue"].ToString();
            txtChangeOverTimeActualValue.Text = row["ChangeOverTimeActualValue"].ToString();
            txtJoiningPressRampNominalValue.Text = row["JoiningPressRampNominalValue"].ToString();
            txtJoiningPressRampActualValue.Text = row["JoiningPressRampActualValue"].ToString();
            txtJoiningForceNominalValue.Text = row["JoiningForceNominalValue"].ToString();
            txtJoiningForceNominalUnit.Text = row["JoiningForceNominalUnit"].ToString();
            txtJoiningForceActualValue.Text = row["JoiningForceActualValue"].ToString();
            txtJoiningForceActualUnit.Text = row["JoiningForceActualUnit"].ToString();
            txtCoolingTimeNominalValue.Text = row["CoolingTimeNominalValue"].ToString();
            txtCoolingTimeActualValue.Text = row["CoolingTimeActualValue"].ToString();
            txtTwoLevelCoolingTimeNominalValue.Text = row["TwoLevelCoolingTimeNominalValue"].ToString();
            txtTwoLevelCoolingTimeActualValue.Text = row["TwoLevelCoolingTimeActualValue"].ToString();
            txtTwoLevelCoolingForceNominalValue.Text = row["TwoLevelCoolingForceNominalValue"].ToString();
            txtTwoLevelCoolingForceNominalUnit.Text = row["TwoLevelCoolingForceNominalUnit"].ToString();
            txtTwoLevelCoolingForceActualValue.Text = row["TwoLevelCoolingForceActualValue"].ToString();
            txtTwoLevelCoolingForceActualUnit.Text = row["TwoLevelCoolingForceActualUnit"].ToString();
            txtWeldingDistance.Text = row["WeldingDistance"].ToString();
            txtWeldingVoltageNominalValue.Text = row["WeldingVoltageNominalValue"].ToString();
            txtWeldingVoltageActualValue.Text = row["WeldingVoltageActualValue"].ToString();
            txtResistanceNominalValue.Text = row["ResistanceNominalValue"].ToString();
            txtResistanceActualValue.Text = row["ResistanceActualValue"].ToString();
            txtWorkNominalValue.Text = row["WorkNominalValue"].ToString();
            txtWorkActualValue.Text = row["WorkActualValue"].ToString();
            txtTotalTimeNominalValue.Text = row["TotalTimeNominalValue"].ToString();
            txtTotalTimeActualValue.Text = row["TotalTimeActualValue"].ToString();
            BeadKey = int.Parse(row["BeadKey"].ToString());
            if (BeadKey > 0 && isCanBeadFormOpen)
                btnOpenBeadMaster.Enabled = true;
            else
                btnOpenBeadMaster.Enabled = false;
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
        private void btnSaveExcel_Click(object sender, EventArgs e)
        {
            targetFileName = GetFileName("xlsx", string.Format(LangResx.Main.excel_PopWeldMaster, ChoiceWeldKey));
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
            targetFileName = GetFileName("pdf", string.Format(LangResx.Main.excel_PopWeldMaster, ChoiceWeldKey));
            if (targetFileName.Trim() != "")
            {
                layoutControl1.ExportToPdf(targetFileName);
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
        /// 검수정보 열기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenBeadMaster_Click(object sender, EventArgs e)
        {
            if (BeadKey == 0)
                return;

            FormPopBeadMastercs frm = new FormPopBeadMastercs();
            frm.ChoiceBeadKey = BeadKey;
            frm.Show();
        }
    }
}