using DevExpress.XtraBars;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using System.Threading;

namespace SWSUploader
{
    public partial class MainForm : DevExpress.XtraBars.FluentDesignSystem.FluentDesignForm
    {
        private string authDescr;

        #region 폼 로드
        /// <summary>
        /// 
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!Program.Option.UploadInspectData)
                elmNewFile1.Enabled = false;

            if (!Program.Option.UploadWeldData)
                elmNewWeldMaster1.Enabled = false;

            if (!Program.Option.ShowInspectUploadRecord)
                elmBeadHistory1.Enabled = false;

            if (!Program.Option.ShowWeldUploadRecord)
                elmWeldHistory1.Enabled = false;
        }

        /// <summary>
        /// 폼 쇼운
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void MainForm_Shown(object sender, EventArgs e)
        {
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);

            if (Program.Option.UploadInspectData)
            {
                ShowNewBeadMaster();
                xtraTabControl1.SelectedTabPageIndex = 1;
            }
        }

        /// <summary>
        /// 시작준비
        ///  1. DB Connection
        ///  2. Ready Screen
        /// </summary>
        public string Splash()
        {
            switch (Program.Option.AuthLevel)
            {
                case 0:
                    authDescr = "Super Admin";
                    break;
                case 1:
                    authDescr = "Admin";
                    break;
                case 2:
                    authDescr = "Manager";
                    break;
                case 3:
                    authDescr = "Operator";
                    break;

            }

            if (Program.Option.AuthLevel <= 1)
                this.Text = $"{Program.constance.SystemTitle} [{Program.Option.LoginID} ({Program.Option.CompName} - {authDescr})] ";
            else
                this.Text = $"{Program.constance.SystemTitle} [{Program.Option.LoginID} ({Program.Option.ProjectName} - {authDescr})] ";

            string InitLoadComplete = string.Empty;
            SplashScreenManager.Default.SendCommand(SplashScreen2.SplashScreenCommand.SetProgress, 30);
            Thread.Sleep(500);

            if (Program.constance.DBConnectInSplash) // 스플래쉬에서 DB 커넥션
            {
                string networkMsg = string.Empty;
                //string networkMsg = NetUtil.Connect(JKBConstance.ServerIP);
                if (string.IsNullOrEmpty(networkMsg))
                {
                    SplashScreenManager.Default.SendCommand(SplashScreen2.SplashScreenCommand.SetStatus, "Starting....");
                    DataTable dt = DBManager.Instance.GetDataTable(Program.constance.DBTestQuery);  //DB Connection
                    Thread.Sleep(500);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        SplashScreenManager.Default.SendCommand(SplashScreen2.SplashScreenCommand.SetProgress, 50);
                        Thread.Sleep(500);

                        SplashScreenManager.Default.SendCommand(SplashScreen2.SplashScreenCommand.SetProgress, 80);
                        Thread.Sleep(500);

                        SplashScreenManager.Default.SendCommand(SplashScreen2.SplashScreenCommand.SetProgress, 100);
                        Thread.Sleep(500);
                    }
                    else
                    {
                        InitLoadComplete = "Error in connecting to database.\r\n" + DBManager.DBMErrString;
                        return InitLoadComplete;
                    }
                }
                else
                {
                    InitLoadComplete = networkMsg;
                    return InitLoadComplete;
                }
            }

            return InitLoadComplete;
        }
        #endregion

        /// <summary>
        /// 프로젝트 신규
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void elmNewFile_Click(object sender, EventArgs e)
        {
            ShowNewBeadMaster();
        }
        private void ShowNewBeadMaster()
        {
            if (Program.isBusy) return;

            //Program.MasterKey = 0;
            fluentDesignFormContainer1.Controls.Clear();
            ucNewBeadMaster frm = new ucNewBeadMaster();
            fluentDesignFormContainer1.Controls.Add(frm);
            frm.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// 융착기록 업로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void elmNewWeldMaster_Click(object sender, EventArgs e)
        {
            ShowNewWeldMaster();
        }
        private void ShowNewWeldMaster()
        {
            if (Program.isBusy) return;

            fluentDesignFormContainer1.Controls.Clear();
            ucNewWeldMaster frm = new ucNewWeldMaster();
            fluentDesignFormContainer1.Controls.Add(frm);
            frm.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// 비드검수 데이터 업로드 기록 보기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void elmBeadHistory_Click(object sender, EventArgs e)
        {
            if (Program.isBusy) return;

            fluentDesignFormContainer1.Controls.Clear();
            ucBeadUploadHistory frm = new ucBeadUploadHistory();
            fluentDesignFormContainer1.Controls.Add(frm);
            frm.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// 융착데이터 업로드 기록보기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void elmWeldHistory_Click(object sender, EventArgs e)
        {
            if (Program.isBusy) return;

            fluentDesignFormContainer1.Controls.Clear();
            ucWeldUploadHistory frm = new ucWeldUploadHistory();
            fluentDesignFormContainer1.Controls.Add(frm);
            frm.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// 비밀번호 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void elmChangePasswd1_Click(object sender, EventArgs e)
        {
            FormChangePswd frm = new FormChangePswd();
            frm.isChangePassword = true;
            frm.ShowDialog();
        }
        /// <summary>
        /// 비밀번호 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void elmChangePasswd2_Click(object sender, EventArgs e)
        {
            FormChangePswd frm = new FormChangePswd();
            frm.isChangePassword = true;
            frm.ShowDialog();
        }

        /// <summary>
        /// 프로그램 종료시
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (Program.isBusy)
            {
                XtraMessageBox.Show("The program is running!!!", "Busy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true;
            }

            if (XtraMessageBox.Show("Are you sure you want to exit the program?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != System.Windows.Forms.DialogResult.Yes)
                e.Cancel = true;
            else
                Program.SaveConfig();
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            switch (xtraTabControl1.SelectedTabPageIndex)
            {
                case 0:
                    if (Program.Option.UploadWeldData)
                        ShowNewWeldMaster();
                    break;
                case 1:
                    if (Program.Option.UploadInspectData)
                        ShowNewBeadMaster();
                    break;
            }
        }
    }
}
