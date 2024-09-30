using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraPrinting;
using DevExpress.Export;

namespace SWSUploader
{
    public partial class ucNewBeadMaster : DevExpress.XtraEditors.XtraUserControl
    {
        /// <summary>
        /// 화면에 진행로그 출력용
        /// </summary>
        /// <param name="one"></param>
        delegate void dlgLogMessage(string one);
        delegate void dlgSetRateValue(int beadKey, int value);
        delegate void dlgSetRateStatusValue(int beadKey, string status);

        //private int ProcCount;
        private bool isCancel;

        //private string LogDate;
        private string MasterfileFullName;
        private string MasterfileName;
        //private List<string> sqls;

        private CsvMasterListData csv;
        private DataTable dtMasterList;
        private DataTable dtGrdMasterList;

        //private int ChoiceCompKey;

        public ucNewBeadMaster()
        {
            InitializeComponent();
        }       
        private void ucNewAnalysis_Load(object sender, EventArgs e)
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
                sql = string.Format("select ProjectKey No, ProjectName Project from Project where isDeleted = 0 and CompKey = {0} ", lueCompany.EditValue);
            }
            else
            {
                sql = string.Format($"select ProjectKey No, ProjectName Project from Project where isDeleted = 0 and ProjectKey = {Program.Option.ProjectKey} ");
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
        /// 대상파일 열기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            //folderBrowserDialog.Description
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;

            txtFileName.Text = folderBrowserDialog.SelectedPath;
            MasterfileFullName = string.Empty;
            foreach (string file in Directory.GetFiles(txtFileName.Text))
            {
                string ext = Path.GetExtension(file);
                if (ext == ".csv")
                {
                    MasterfileFullName = file;
                    break; //하나만 가져오고 끝내자.
                }
            }

            if (string.IsNullOrWhiteSpace(MasterfileFullName))
                return;

            MasterfileName = Path.GetFileName(MasterfileFullName);
            if (XtraMessageBox.Show(string.Format(LangResx.Main.msg_LoadBeadFile, MasterfileName), "Load", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
                return;

            
            GetMasterFile();
            btnRunAnalysis.Enabled = true;
            //chkDelDetail.Visible = true;

            ShowMessage(string.Format(LangResx.Main.msg_FileLoad1, MasterfileName));
            ShowMessage(string.Format(LangResx.Main.msg_FileLoad2));
        }

        /// <summary>
        /// 목록용 마스터파일 불러오기
        /// </summary>
        private void GetMasterFile()
        {
            csv = new CsvMasterListData(MasterfileFullName);
            if (!csv.isOK)
            {
                XtraMessageBox.Show(LangResx.Main.msg_FileLoadError + "\r\n" + csv.ResultMessage, "File error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //빈테이블 생성
            dtGrdMasterList = csv.GetTempBeadMaster(true); 
            csv.ProjectKey = int.Parse(lueProject.EditValue.ToString());

            //목록용 데이터 생성
            DataRow row;
            dtMasterList = csv.dtCSVFile;
            foreach (DataRow dr in dtMasterList.Rows)
            {
                row = dtGrdMasterList.NewRow();
                row["BeadKey"] = 0;
                row["ProjectKey"] = csv.ProjectKey; // int.Parse(lueProject.EditValue.ToString());
                row["LineNo"] = dr[0];
                row["SerialNo"] = dr[1];
                row["InspectionNo"] = int.Parse(dr[2].ToString());
                row["InspectionDate"] = dr[3];
                row["Material"] = dr[7];

                if (string.IsNullOrWhiteSpace(dr[8].ToString()))
                    row["OD"] = 0;
                else
                    row["OD"] = double.Parse(dr[8].ToString());

                if (string.IsNullOrWhiteSpace(dr[9].ToString()))
                    row["WallThickness"] = 0;
                else
                    row["WallThickness"] = double.Parse(dr[9].ToString());

                row["SDR"] = dr[10];
                row["Rate"] = 0;
                row["Status"] = 0;
                row["CreateID"] = Program.Option.LoginID;
                row["CreateDtm"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                dtGrdMasterList.Rows.Add(row);
                totCount++;
            }
            dtGrdMasterList.AcceptChanges();
            grdCsvList.DataSource = dtGrdMasterList;
        }

        private DialogResult dialogDup;

        /// <summary>
        /// 분석 시작
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRunAnalysis_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFileName.Text))
            {
                XtraMessageBox.Show(LangResx.Main.msg_NullBeadFile, "Select File", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            progressBarControl1.EditValue = 0;
            ShowLogMessage("Ready to Start...");
            Application.DoEvents();

            if (XtraMessageBox.Show(LangResx.Main.msg_UploadData, "Upload", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
                return;

            Program.CompKey = int.Parse(lueCompany.EditValue.ToString());
            Program.ProjectKey = int.Parse(lueProject.EditValue.ToString());

            btnRunAnalysis.Enabled = false;
            stepProgressBar1.SelectedItemIndex = 1; //  2/4 데이터 전송
            //sqls = new List<string>();
            ShowMessage(string.Format("Total line count: {0:n0}", totCount));
            lblStatus.Text = "It's running.";

            //검수 목록용 Master File 업로드
            try
            {
                //Temp Data Upload
                ShowMessage(LangResx.Main.msg_isExistData);
                DBManager.Instance.MDB.BulkCopyDI(dtGrdMasterList, "TempBeadMaster");
                ShowMessage(string.Format("Success TempBeadMaster Uploaded...."));

                //Temp BeadMaster 중복상태 Update
                csv.UpdateDupStatus();

                //중복건수 산출
                int dupCount = csv.GetDupCount();
                if (dupCount > 0)
                {
                    ShowMessage(string.Format(LangResx.Main.msg_ExistData1, dupCount));
                    string msg = string.Format(LangResx.Main.msg_ExistData2, dupCount);
                    dialogDup = XtraMessageBox.Show(msg, LangResx.Main.msg_title_Exist, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                } 
                else if (dupCount < 0)
                {
                    ShowMessage(LangResx.Main.msg_title_ExistDataError);
                    dialogDup = DialogResult.No;
                    XtraMessageBox.Show(LangResx.Main.msg_title_ExistDataError, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (dialogDup == DialogResult.No)
                {
                    progressBarControl1.EditValue = 100;
                    btnRunAnalysis.Enabled = true;
                    ShowMessage(LangResx.Main.msg_CancleUpload);
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowMessage(string.Format(LangResx.Main.msg_UploadError, ex.Message));
                return;
            }

            //BeadMaster 생성
            if (!csv.InsertBeadMaster())
            {
                XtraMessageBox.Show("[Error] " + csv.ResultMessage, LangResx.Main.msg_title_UploadError, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //그리드 갱신(하위 삭제후 갱신기능 포함)
            if (chkDelDetail.Checked)
                csv.DeleteDetailData();

            dtGrdMasterList = csv.GetTempBeadMaster(false);
            grdCsvList.DataSource = dtGrdMasterList;

            btnCancel.Visible = true;
            isCancel = false;
            btnOpenFile.Enabled = false;
            //ProcCount = 0;

            try
            {
                ShowMessage(string.Format("Started csv file analysis......[{0}]", totCount));
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
            bool[] options = new bool[3];
            options[0] = chkIsOed.Checked;
            options[1] = chkIsRaw.Checked;
            options[2] = chkIsXyLine.Checked;

            currLineNo = 0;
            foreach (DataRow dr in dtGrdMasterList.Rows) //이미 완료된 대상도 포함된 목록임
            {
                CountStatus();
                if ((backgroundWorker1.CancellationPending == true))
                {
                    e.Cancel = isCancel = true;
                    return;
                }

                //Console.WriteLine("The number of processors " + "on this computer is {0}.", Environment.ProcessorCount);
                int rate = int.Parse(dr["Rate"].ToString());
                int beadCount = int.Parse(dr["Status"].ToString()); //비드검수상세(BeadDetail)에 데이터가 몇건이 들어있는지 확인용

                if (rate < 100 || beadCount == 0)
                {
                    //업로드 상세목록 검사 
                    string foldName = string.Format("{0}_{1}-{2}", dr["InspectionDate"].ToString().Substring(0, 10), dr["SerialNo"].ToString(), dr["InspectionNo"].ToString().PadLeft(6, '0'));
                    CsvDetailData csvDtl = new CsvDetailData(txtFileName.Text, foldName, dr, options);
                    SetRateValue(csvDtl.BeadKey, 0); //1%부터 시작
                    if (!csvDtl.isOK)
                    {
                        ShowMessage(string.Format(LangResx.Main.msg_FolderError, csvDtl.BeadKey, foldName, csvDtl.ResultMessage));
                        XtraMessageBox.Show("[Error] " + csvDtl.ResultMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    //하위 폴더 업로드
                    if (csvDtl.RunQuery())
                    {
                        SetRateValue(csvDtl.BeadKey, 100);
                        ShowMessage(string.Format(LangResx.Main.msg_UploadSuccess, csvDtl.BeadKey, foldName));
                    }
                    else
                    {
                        SetRateValue(csvDtl.BeadKey, -1);
                        ShowMessage(string.Format(LangResx.Main.msg_UploadFail, csvDtl.BeadKey, foldName, csvDtl.ResultMessage));
                    }
                }
                currLineNo++;
            }
        }

        /// <summary>
        /// 그리드상의 진행율 표시용
        /// </summary>
        /// <param name="beadKey"></param>
        /// <param name="value"></param>
        private void SetRateValue(int beadKey, int value)
        {
            //mmeAnalysisResult.Invoke(new dlgLogMessage(ShowLogMessage), new string[] { msg });
            grdCsvList.Invoke(new dlgSetRateValue(SetRateValueInvoke), beadKey, value);
        }
        /// <summary>
        /// 업로드 완료 표시
        /// </summary>
        private void SetRateValueInvoke(int beadKey, int value)
        {
            DataRow row = dtGrdMasterList.Rows.Find(beadKey);
            row.BeginEdit();
            row["Rate"] = value;
            row.EndEdit();
            row.AcceptChanges();
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
        private int currLineNo;

        /// <summary>
        /// 진행바 
        /// </summary>
        /// <param name="currentNo"></param>
        private void CountStatus()
        {
            double currCount = currLineNo;
            int rto = (int)(currCount / totCount * 100);
            backgroundWorker1.ReportProgress(rto);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarControl1.EditValue = e.ProgressPercentage;
        }

        /// <summary>
        /// 완료시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Program.isBusy = false;
            btnCancel.Visible = false;
            progressBarControl1.EditValue = 100;

            btnOpenFile.Enabled = true;
            btnRunAnalysis.Enabled = true;

            if (isCancel)
            {
                ShowMessage(string.Format("You have requested to stop running.......{0:n0}", currLineNo));
                XtraMessageBox.Show(string.Format("[Stop] Analysis operation has been stopped."), "Stopped", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SplashScreenManager.ShowForm(Program.mainApp, typeof(WaitForm1), true, true, false);
            SplashScreenManager.Default.SetWaitFormDescription("Saving log analysis results....");

            SplashScreenManager.CloseForm(false);
            stepProgressBar1.SelectedItemIndex = 3; //  4/4 완료
            lblStatus.Text = "It's done.";

            ShowMessage(string.Format("Analysis count: {0:n0}", currLineNo));
            ShowMessage(string.Format("[Finished] Upload completed successfully!"));
            XtraMessageBox.Show(string.Format("[Finished] Analysis completed successfully!"), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowMessage(string.Format("Done."));
        }

        /// <summary>
        /// 실행중 취소
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ShowMessage(string.Format("You have requested to stop running......."));
            backgroundWorker1.CancelAsync();
            progressBarControl1.EditValue = 0;
        }

        /// <summary>
        /// 폼 자동 조절
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ucNewAnalysis_Resize(object sender, EventArgs e)
        {
            splitContainerControl1.SplitterPosition = this.Size.Width * 65 / 100;
        }
    }
}
