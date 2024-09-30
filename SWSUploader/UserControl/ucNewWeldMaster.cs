using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using DevExpress.XtraSplashScreen;

namespace SWSUploader
{
    public partial class ucNewWeldMaster : DevExpress.XtraEditors.XtraUserControl
    {
        /// <summary>
        /// 화면에 진행로그 출력용
        /// </summary>
        /// <param name="one"></param>
        delegate void dlgLogMessage(string one);
        delegate void dlgSetRateValue(int beadKey, int value);

        private bool isCancel;

        private ExcelWeldMasterData excel;
        private int cntErrorDate;
        public ucNewWeldMaster()
        {
            InitializeComponent();
        }

        private void ucNewWeldMaster_Load(object sender, EventArgs e)
        {
            SetCompanyCode();
            mmeAnalysisResult.Text = string.Empty;
            ShowMessage(string.Format("Program Started...."));

            switch (Program.Option.AuthLevel)
            {
                case 2:
                    lueProject.ReadOnly = true;
                    break;
                case 3:
                    lueCompany.ReadOnly = lueProject.ReadOnly = true;
                    break;
                default:
                    break;
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
                    sql += $"select c.CompKey No, c.CompName as Company from Company c where c.isDeleted = 0 ";
                    break;
                case 1:
                    sql += "select DISTINCT(c.CompKey) No, c.CompName as Company ";
                    sql += "  from Company c ";
                    sql += "  JOIN UserInfo u ";
                    sql += "    ON c.CompKey = u.CompKey AND u.isDeleted = 0 ";
                    sql += " WHERE 1 = 1 ";
                    sql += "   AND c.isDeleted = 0 ";
                    sql += $"  AND u.ParentAdminID = '{Program.Option.LoginID}' ";
                    break;
                case 2:
                    sql += "select DISTINCT(c.CompKey) No, c.CompName as Company ";
                    sql += "  from Company c ";
                    sql += "  JOIN UserInfo u ";
                    sql += "    ON c.CompKey = u.CompKey AND u.isDeleted = 0 ";
                    sql += " WHERE 1 = 1 ";
                    sql += "   AND c.isDeleted = 0 ";
                    sql += $"  AND (u.ParentManagerID = '{Program.Option.LoginID}' or u.UserID = '{Program.Option.LoginID}') ";
                    break;
                case 3:
                    sql += "select DISTINCT(c.CompKey) No, c.CompName as Company ";
                    sql += "  from Company c ";
                    sql += "  JOIN UserInfo u ";
                    sql += "    ON c.CompKey = u.CompKey AND u.isDeleted = 0 ";
                    sql += " WHERE 1 = 1 ";
                    sql += "   AND c.isDeleted = 0 ";
                    sql += $"  AND u.UserID = '{Program.Option.LoginID}' ";
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

        /// <summary>
        /// 융착정보 파일 선택
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            //test only
            //txtFileName.Text = @"V:\아그루코리아_20200513\4.테스트\아그루코리아 - 2차\[Uploader] 융착데이터 - 02. 파일 업로드 전 엑셀데이터 검사에서 .Net Framework 오류, 오류 이후 멈춰 있는 현상, 30분지나도 진행 X\2020(03-04) 24345 point.xlsx";
            //btnRunAnalysis.Enabled = true;
            //btnRunAnalysis.PerformClick();
            //----------------------------------

            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.DefaultExt = "xlsx";
            openFileDlg.Filter = "Excel Files(*.xlsx)|*.xlsx";
            if (openFileDlg.ShowDialog() == DialogResult.OK)
            {
                txtFileName.Text = openFileDlg.FileName;
                btnRunAnalysis.Enabled = true;
                btnRunAnalysis.PerformClick();
            }
        }

        /// <summary>
        /// 백그라운드 실행취소
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ShowMessage(string.Format("You have requested to stop running......."));
            backgroundWorker1.CancelAsync();
            progressBarControl1.EditValue = 0;
        }

        private DialogResult dialogDup;

        /// <summary>
        /// 업로드 수행
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRunAnalysis_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFileName.Text))
            {
                XtraMessageBox.Show(LangResx.Main.msg_NullWeldFile, "Select File", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (XtraMessageBox.Show(LangResx.Main.msg_UploadData, "Upload", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
                return;

            ShowLogMessage("Ready to Start...");
            progressBarControl1.EditValue = 10;
            Application.DoEvents();

            btnRunAnalysis.Enabled = false;
            SplashScreenManager.ShowForm(Program.mainApp, typeof(WaitForm1), true, true, false);
            SplashScreenManager.Default.SetWaitFormDescription(LangResx.Main.msg_LoadFile);

            // Excel File Loading 
            ShowMessage(string.Format("Excel File loading start.... "));
            excel = new ExcelWeldMasterData(txtFileName.Text, int.Parse(lueProject.EditValue.ToString()));  //Excel File Load
            if (!excel.isOK)
            {
                SplashScreenManager.CloseForm(false);
                XtraMessageBox.Show(LangResx.Main.msg_LoadFileError + "\r\n" + excel.ResultMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SplashScreenManager.CloseForm(false);

            // Excel데이터 생성
            SplashScreenManager.ShowForm(Program.mainApp, typeof(WaitForm1), true, true, false);
            SplashScreenManager.Default.SetWaitFormDescription(LangResx.Main.msg_CreateData);
            ShowMessage(string.Format("Create Excel data.... "));
            progressBarControl1.EditValue = 30;
            Application.DoEvents();

            if (!excel.MakeData())
            {
                SplashScreenManager.CloseForm(false);
                XtraMessageBox.Show(LangResx.Main.msg_CreateDataError + "\r\n" + excel.ResultMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //SplashScreenManager.CloseForm(false);

            //Total Count 
            totCount = excel.dtExcelData.Rows.Count;
            progressBarControl1.EditValue = 50;
            ShowMessage(string.Format("Success TempWeldMaster Uploaded...."));

            stepProgressBar1.SelectedItemIndex = 1; //  2/4 데이터 전송
            ShowMessage(string.Format("Total line count: {0:n0}", totCount));
            lblStatus.Text = "It's running.";

            //검수 목록용 Master File 업로드
            try
            {
                excel.UpdateDupStatus(); //Temp BeadMaster 중복상태 Update
                progressBarControl1.EditValue = 60;

                int dupCount = excel.GetDupCount(); //중복건수 산출
                progressBarControl1.EditValue = 70;

                if (dupCount > 0)
                {
                    ShowMessage(string.Format(LangResx.Main.msg_ExistData1, dupCount));
                    string dupRow = excel.GetDupFirstRow();
                    string msg = string.Format(LangResx.Main.msg_ExistData2, dupCount);
                    SplashScreenManager.CloseForm(false);
                    dialogDup = XtraMessageBox.Show(msg, LangResx.Main.msg_title_Exist, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                }
                else if (dupCount < 0)
                {
                    ShowMessage(LangResx.Main.msg_title_ExistDataError);
                    dialogDup = DialogResult.No;
                    SplashScreenManager.CloseForm(false);
                    XtraMessageBox.Show(LangResx.Main.msg_title_ExistDataError, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                //진행 취소선택시.
                if (dialogDup == DialogResult.No)
                {
                    progressBarControl1.EditValue = 100;
                    btnRunAnalysis.Enabled = true;
                    ShowMessage(LangResx.Main.msg_CancleUpload);
                    SplashScreenManager.CloseForm(false);
                    return;
                }
            }
            catch (Exception ex)
            {
                SplashScreenManager.CloseForm(false);
                ShowMessage(string.Format(LangResx.Main.msg_UploadError, ex.Message));
                return;
            }
            SplashScreenManager.CloseForm(false);

            //그리드 갱신(하위 삭제후 갱신기능 포함)
            //if (chkOverwrite.Checked) csv.DeleteDetailData();
            btnCancel.Visible = true;
            isCancel = false;
            btnOpenFile.Enabled = false;

            //ProcCount = 0;
            progressBarControl1.EditValue = 80;
            stepProgressBar1.SelectedItemIndex = 2; //서버전송

            try
            {
                ShowMessage(string.Format("Started Excel file upload......[{0}]", totCount));
                Program.isBusy = true;
                if (backgroundWorker1.IsBusy != true)
                    backgroundWorker1.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Waiting.....";
                ShowMessage(string.Format("Background execution error occurred:......"));
                XtraMessageBox.Show("Background execution error occurred:" + ex.Message);
            }
        }

        /// <summary>
        /// Background 실행
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            ShowMessage(string.Format(LangResx.Main.msg_Uploading));

            //WeldMaster 생성
            if (!excel.InsertWeldMaster())
            {
                XtraMessageBox.Show("[Error] " + excel.ResultMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            cntErrorDate = ExcelWeldMasterData.cntErrorDate;
            //progressBarControl1.EditValue = 90;
        }

        /// <summary>
        /// 처리 메시지 출력
        /// </summary>
        /// <param name="msg"></param>
        private void ShowMessage(string msg)
        {
            mmeAnalysisResult.Invoke(new dlgLogMessage(ShowLogMessage), new string[] { msg });
        }

        /// <summary>
        /// 충돌 방지
        /// </summary>
        /// <param name="one"></param>
        private void ShowLogMessage(string msg)
        {
            string message = string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg);
            mmeAnalysisResult.Text += string.Format("{0}\r\n", message);
        }

        private double totCount;

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Program.isBusy = false;
            //progressPanel1.Visible = false;
            btnCancel.Visible = false;
            progressBarControl1.EditValue = 100;

            btnOpenFile.Enabled = true;
            btnRunAnalysis.Enabled = true;

            if (isCancel)
            {
                ShowMessage(string.Format("You have requested to stop running......."));
                XtraMessageBox.Show(string.Format("[Stop] Analysis operation has been stopped."), "Stopped", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            stepProgressBar1.SelectedItemIndex = 3; //  4/4 완료
            if (cntErrorDate > 0)
            {
                XtraMessageBox.Show(string.Format(LangResx.Main.msg_DateError, cntErrorDate), "Date Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowMessage(string.Format(LangResx.Main.msg_DateError, cntErrorDate));
            }
            lblStatus.Text = "It's done.";
            ShowMessage(string.Format("[Finished] Upload completed successfully!"));
            XtraMessageBox.Show(string.Format("[Finished] Analysis completed successfully!"), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowMessage(string.Format("Done."));
        }
    }
}
