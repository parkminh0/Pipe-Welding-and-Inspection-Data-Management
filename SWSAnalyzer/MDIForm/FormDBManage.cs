using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using DevExpress.XtraSplashScreen;

namespace SWSAnalyzer
{
    public partial class FormDBManage : DevExpress.XtraEditors.XtraForm
    {
        public bool isInitDB;
        public FormDBManage()
        {
            InitializeComponent();
        }

        private void FormDBManage_Load(object sender, EventArgs e)
        {
            bteOrgDBPath.EditValue = Program.Option.DBFullPath;
        }
        private void FormDBManage_Shown(object sender, EventArgs e)
        {
            if (isInitDB) //최소 DB설정시에만
            {
                rdoMethd.SelectedIndex = 4;
                rdoMethd.ReadOnly = true;
            }                
        }

        /// <summary>
        /// 기능선택
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoMethd_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (rdoMethd.SelectedIndex)
            {
                case 0: //백업만
                    groupControl1.Enabled = true;
                    groupControl2.Enabled = false;
                    groupControl1.Text = string.Format("DB백업(복사) - 폴더를 선택하신 후 저장하시면 별도의 파일로 복사됩니다.");
                    bteBackupPath.Enabled = true;
                    lblDescr.Text = "현재 DB를 별도의 장소에 백업(복사) 합니다.";
                    btnRunBackup.Text = "DB백업(복사)";
                    //groupControl1.Text = string.Format("Create a copy - Please select a folder to save the copy.");
                    //bteBackupPath.Enabled = true;
                    //lblDescr.Text = "It creates a copy of the data generated in a separate DB.";
                    //btnRunBackup.Text = "Create a copy";
                    break;
                case 1: //백업 및 초기화
                    groupControl1.Enabled = true;
                    groupControl2.Enabled = false;
                    groupControl1.Text = string.Format("백업 및 초기화 - 폴더를 선택하신 후 저장하시면 별도의 파일로 복사됩니다.");
                    bteBackupPath.Enabled = true;
                    lblDescr.Text = "백업(복사) 후 DB를 초기화 합니다.";
                    btnRunBackup.Text = "백업 및 초기화";
                    //groupControl1.Text = string.Format("Backup & Initialize - Please select a folder to save the copy.");
                    //bteBackupPath.Enabled = true;
                    //lblDescr.Text = "Reset After you create a DB copy.";
                    //btnRunBackup.Text = "Backup & Initialize";
                    break;
                case 2: //초기화만
                    groupControl1.Enabled = true;
                    groupControl2.Enabled = false;
                    //groupControl1.Text = string.Format("DB Initialize");
                    //bteBackupPath.Enabled = false;
                    //lblDescr.Text = "Initializes data completely.";
                    //btnRunBackup.Text = "DB Initialize";
                    groupControl1.Text = string.Format("DB 초기화");
                    bteBackupPath.Enabled = false;
                    lblDescr.Text = "DB를 완전히 초기화 합니다.";
                    btnRunBackup.Text = "DB 초기화";
                    break;
                case 3: //최적화 작업
                    groupControl1.Enabled = true;
                    groupControl2.Enabled = false;
                    //groupControl1.Text = string.Format("DB Optimization");
                    //bteBackupPath.Enabled = false;
                    //lblDescr.Text = "Perform the DB optimization.";
                    //btnRunBackup.Text = "DB Optimization";
                    groupControl1.Text = string.Format("DB 최적화");
                    bteBackupPath.Enabled = false;
                    lblDescr.Text = "DB초기화를 수행합니다.";
                    btnRunBackup.Text = "DB 초기화";
                    break;
                case 4: //원본 DB 폴더설정
                    groupControl1.Enabled = false;
                    groupControl2.Enabled = true;
                    //groupControl2.Text = string.Format("Data source location settings");
                    groupControl2.Text = string.Format("DB 위치를 설정합니다.");
                    bteOrgDBPath.Focus();
                    break;
            }
        }

        /// <summary>
        /// 폴더 선택
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bteBackupPath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            FolderBrowserDialog openFolderDialog1 = new FolderBrowserDialog();
            openFolderDialog1.ShowNewFolderButton = false;
            openFolderDialog1.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFolderDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            bteBackupPath.EditValue = openFolderDialog1.SelectedPath;
        }

        /// <summary>
        /// 취소
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            //if (XtraMessageBox.Show(string.Format("DB관리 화면을 나가시겠습니까?"), "취소", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != System.Windows.Forms.DialogResult.Yes)
            //    return;

            this.Close();
        }

        private bool isOK = false;

        /// <summary>
        /// 실행
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRunBackup_Click(object sender, EventArgs e)
        {
            if (rdoMethd.SelectedIndex == 1)
            {
                //XtraMessageBox.Show("Unable to perform DB Optimizer.", "exception", MessageBoxButtons.OK, MessageBoxIcon.Information);  //기능 제외
                XtraMessageBox.Show("본 기능은 사용하실 수 없습니다.", "기능제외", MessageBoxButtons.OK, MessageBoxIcon.Information);  //기능 제외
                return;
            }

            if (bteBackupPath.Text.Trim() == "")
            {
                if (rdoMethd.SelectedIndex != 2 && rdoMethd.SelectedIndex != 3)
                {
                    //XtraMessageBox.Show("Please specify a folder to create a copy.", "Set Folder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    XtraMessageBox.Show("백업(복사)할 폴더를 선택해 주세요.", "폴더 선택", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            //if (XtraMessageBox.Show("This task can be executed for a long time, please initialize the data is carefully determined. \r\nDo you want to start a job you choose?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != System.Windows.Forms.DialogResult.Yes)
            if (XtraMessageBox.Show("이 작업은 시간이 오래걸릴 수 있습니다. \r\n작업을 시작하시겠습니까?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != System.Windows.Forms.DialogResult.Yes)
                return;

            SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
            //SplashScreenManager.Default.SetWaitFormDescription("It is being processed. Do not turn off the PC power supply Please wait until it completes....");
            SplashScreenManager.Default.SetWaitFormDescription("처리중이오니 PC 전원을 끄지마시고 기다려주십시요....");

            if (backgroundWorker1.IsBusy != true)
                backgroundWorker1.RunWorkerAsync();
        }

        /// <summary>
        /// 백그라운드 실행
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string copyName = string.Format("BACKUP_{0}.db", DateTime.Now.ToString("yyyyMMddHHmmss"));
            switch (rdoMethd.SelectedIndex)
            {
                case 0: //백업만
                    isOK = RunCopy(Path.Combine(bteBackupPath.Text, copyName));
                    break;
                case 1: //백업 및 초기화
                    if (RunCopy(Path.Combine(bteBackupPath.Text, copyName)))
                        isOK = RunInitial(copyName);
                    else
                        isOK = false;
                    break;
                case 2: //초기화만
                    isOK = RunInitial(copyName);
                    break;
                case 3: //최적화 작업
                    isOK = string.IsNullOrEmpty(RunVaccum());
                    break;
                default:
                    isOK = false;
                    break;
            }

        }
        /// <summary>
        /// 백그라운드 종료
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SplashScreenManager.CloseForm(false);

            if (isOK)
                //XtraMessageBox.Show("Normal has been processed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                XtraMessageBox.Show("정상처리 되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                //XtraMessageBox.Show("The error occurred during processing.", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Information);
                XtraMessageBox.Show("처리중 오류가 발생했습니다.", "실패", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 복사 수행
        /// </summary>
        private bool RunCopy(string copyName)
        {
            if (!File.Exists(copyName))
            {
                try
                {
                    File.Copy(Program.constance.DbTargetFullName, copyName);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                //XtraMessageBox.Show("[Fail] he state can not create a copy.", "Can not create", MessageBoxButtons.OK, MessageBoxIcon.Information);
                XtraMessageBox.Show("[실패] 백업(복사)중 오류가 발생했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            return true;
        }
        /// <summary>
        /// 초기화 수행 (Copy & Rename)
        /// </summary>
        private bool RunInitial(string copyName)
        {
            try
            {
                //Initial DB 가져오기
                File.Copy(Program.constance.DbInitOrgFullName, Program.constance.DbInitFullName, true);
            }
            catch (Exception)
            {
                return false;
            }

            //Inital DB를 Attach하여 작업
            if (!MakeInitialDB())
            {
                XtraMessageBox.Show("[Fail] That can not be initialized. Please try after confirmation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;                
            }

            string toName = Path.Combine(Program.constance.DbFilePath, copyName);
            //try
            //{
            //    //MainDB 임시 Rename 
            //    File.Move(Program.constance.DbTargetFullName, toName);
            //}
            //catch (Exception)
            //{
            //    //return false;
            //}
        
            try
            {
                //BaseDB를 MainDB명으로 복사
                File.Copy(Program.constance.DbInitFullName, Program.constance.DbTargetFullName, true);
            }
            catch (Exception)
            {
                return false;
            }

            try
            {
                //대상DB 삭제
                File.Delete(toName);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Initial DB에 기본 Master데이터 채우기
        /// </summary>
        private bool MakeInitialDB()
        {
            //string sql = string.Format("ATTACH database '{0}' as 'Initial' KEY '{1}'", Program.constance.DbInitFullName, DBManager.Instance.Pswd);
            //if (DBManager.Instance.ExcuteDataUpdate(sql) < 0)
            //    return false;

            //List<string> sqls = new List<string>();
            //sqls.Add("insert into Initial.[RefExonInfo] select * from [RefExonInfo]");
            //sqls.Add("insert into Initial.[RefColumns] select * from [RefColumns]");
            //sqls.Add("insert into Initial.[RefData] select * from [RefData]");
            //DBManager.Instance.MDB.ExcuteTransaction(sqls);

            //sql = "DETACH database 'Initial' ";
            //if (DBManager.Instance.ExcuteDataUpdate(sql) < 0)
            //    return false;

            //return true;
            //try
            //{
            //    DBManager.Instance.MDB.importData();
            //}
            //catch (Exception)
            //{
            //    return false;
            //}

            return true;
        }

        /// <summary>
        /// 최적화 Vaccum수행 (실패 시 오류메세지 Return)
        /// </summary>
        private string RunVaccum()
        {
            List<string> sqls = new List<string>();
            sqls.Add("VACUUM ");

            return DBManager.Instance.MDB.ExcuteTransaction(sqls);            
        }

        /// <summary>
        /// 원본DB 위치 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApplyOrgPath_Click(object sender, EventArgs e)
        {
            if (bteOrgDBPath.EditValue == null || bteOrgDBPath.EditValue.ToString() == "")
            {
                //XtraMessageBox.Show("Please specify a folder in the original DB.", "Set Folder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                XtraMessageBox.Show("원본DB가 위치하는 폴더를 선택해 주세요.", "폴더선택", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //if (XtraMessageBox.Show("Specify the location you select Database as the source folder. Please specify in consideration of the capacity of the disc.\r\nWould you like to make your selected folders?", "Specifying the source folder", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != System.Windows.Forms.DialogResult.Yes)
            if (XtraMessageBox.Show("데이터의 용량이 클 경우를 대비하여 넉넉한 드라이브의 폴더를 선택하시기 바랍니다.\r\n선택하신 위치의 DB를 설정하시겠습니까?", "설정확인", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != System.Windows.Forms.DialogResult.Yes)
                return;

            string fullPath = Path.Combine(bteOrgDBPath.EditValue.ToString(), Program.constance.DbFileName);
            if (!File.Exists(fullPath))
            {
                try
                {
                    File.Copy(Program.constance.DbBaseOrgFullName, fullPath);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("An error has occurred in the designated source DB.\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            //Program.constance.DbFilePath = bteOrgDBPath.EditValue.ToString();
            Program.Option.DBFullPath = bteOrgDBPath.EditValue.ToString();
            Program.SaveConfig(); //저장

            //XtraMessageBox.Show("The source DB folder has been set. \r\nPlease try again from the beginning after you completely shut down the program.", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            XtraMessageBox.Show("설정이 완료되었습니다. \r\n프로그램을 종료하고 다시 시작해 주시기바랍니다.", "설정완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();

        }
        /// <summary>
        /// 원본폴더 설정
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bteOrgDBPath_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            FolderBrowserDialog openFolderDialog1 = new FolderBrowserDialog();
            openFolderDialog1.ShowNewFolderButton = false;
            openFolderDialog1.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFolderDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            bteOrgDBPath.EditValue = openFolderDialog1.SelectedPath;
        }

    }
}