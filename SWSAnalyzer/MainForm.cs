using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraBars.Ribbon;
using DevExpress.CodeParser;
using DevExpress.LookAndFeel;

namespace SWSAnalyzer
{
    public partial class MainForm : RibbonForm
    {
        private string authDescr;
        private string tempPalette;

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
            if (Properties.Settings.Default.UserSkin != "")
                defaultLookAndFeel1.LookAndFeel.SkinName = Properties.Settings.Default.UserSkin;

			UserLookAndFeel.Default.SetSkinStyle(Properties.Settings.Default.UserSkin, Properties.Settings.Default.UserPalette);
            tempPalette = Properties.Settings.Default.UserPalette;

			if (Program.Option.AuthLevel > 1)
            {
                bbtnCompany.Enabled = bbtnProjectRelation.Enabled = bbtnProject.Enabled = bbtnUsers.Enabled = false;
            }

            if (!Program.Option.FormBeadMasterManage)
                bbtnBeadMasterManage.Enabled = false;

            if (!Program.Option.FormBeadDetailList)
                bbtnBeadDetailList.Enabled = false;

            if (!Program.Option.FormGeneralChart1)
                bbtnGeneralChart1.Enabled = false;

            if (!Program.Option.FormPivotResult)
                bbtnPivotSummary.Enabled = false;

            if (!Program.Option.FormWeldMasterManage)
                bbtnWeldMasterManage.Enabled = false;

            if (!Program.Option.FormBeadAndWeldList)
                bbtnBeadAndWeldList.Enabled = false;

            if (!Program.Option.FormDashboard)
                bbtnDashBoard.Enabled = false;

            if (!Program.Option.FormDashboard2)
                bbtnDashBoard2.Enabled = false;

            if (!Program.Option.FormDashboard3)
                bbtnDashBoard3.Enabled = false;
        }

        /// <summary>
        /// 폼 쇼운
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Shown(object sender, EventArgs e)
        {
            SkinHelper.InitSkinGallery(rbbGallery, true);
            Application.DoEvents();
            SplashScreenManager.CloseForm(false);
            if (Program.Option.FormDashboard)
                new FormDashboard() { MdiParent = this }.Show();
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

                        FormGeneralChart1 frm = new FormGeneralChart1();
                        frm.SendToBack();
                        frm.Show();
                        frm.Close();
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

        #region 폼 열기
        /// <summary>
        /// 창이 열려있는지 검사
        /// </summary>
        /// <param name="formName"></param>
        /// <returns></returns>
        public bool isNewForm(string formName)
        {
            //foreach (Form theForm in this.MdiChildren)    // 현재 MainForm의 MdiChildren에 해당하는 폼만 체크
            foreach (Form theForm in Application.OpenForms) // 현재 프로그램에 있는 모든 열려있는 폼 체크
            {
                if (formName == theForm.Name)
                {
                    theForm.BringToFront();
                    theForm.Focus();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 폼 열기 버튼 클릭 시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenForm_ItemClick(object sender, ItemClickEventArgs e)
        {
            SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
            BarButtonItem bbtnitem = e.Item as BarButtonItem;
            SplashScreenManager.Default.SetWaitFormDescription(bbtnitem.Caption + "Ready to screen ..."); // 2018.07.20 Hwang.W.S : Caption으로 WaitForm을 띄우도록 변경

            if (isNewForm(bbtnitem.Description))
            {
                switch (bbtnitem.Description)
                {
                    case "FormBeadMasterManage":
                        new FormBeadMasterManage() { MdiParent = this }.Show();
                        break;
                    case "FormWeldMasterManage":
                        new FormWeldMasterManage() { MdiParent = this }.Show();
                        break;
                    case "FormBeadAndWeldList":
                        new FormBeadAndWeldList() { MdiParent = this }.Show();
                        break;
                    case "FormBeadDetailList":
                        new FormBeadDetailList() { MdiParent = this }.Show();
                        break;
                    case "FormGeneralChart1":
                        new FormGeneralChart1() { MdiParent = this }.Show();
                        break;
                    case "FormPivotResult":
                        new FormPivotResult() { MdiParent = this }.Show();
                        break;
                    case "FormDashboard":
                        new FormDashboard() { MdiParent = this }.Show();
                        break;
                    case "FormDashboard2":
                        new FormDashboard2(0) { MdiParent = this }.Show();
                        break;
                    case "FormDashboard3":
                        new FormDashboard3(0) { MdiParent = this }.Show();
                        break;
                    default:
                        break;
                }
            }
            SplashScreenManager.CloseForm(false);
        }

        /// <summary>
        /// 열려있는 모든 화면 새로고침 (BaseForm 상속 적용만)
        /// </summary>
        private void DoRefreshAll()
        {
            foreach (Form frm in Application.OpenForms)
            {
                BaseForm baseFrm = frm as BaseForm;
                if (baseFrm != null)
                    baseFrm.DoRefresh();
            }
        }

        /// <summary>
        /// 거래처관리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bbtnCompany_ItemClick(object sender, ItemClickEventArgs e)
        {
            FormCompany frm = new FormCompany();
            frm.ShowDialog();
        }

        /// <summary>
        /// 프로젝트 현황
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bbtnProjectRelation_ItemClick(object sender, ItemClickEventArgs e)
        {
            FormProjectRelation frm = new FormProjectRelation();
            frm.ShowDialog();
        }

        /// <summary>
        /// 프로젝트 관리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bbtnProject_ItemClick(object sender, ItemClickEventArgs e)
        {
            FormProject frm = new FormProject();
            frm.ShowDialog();
        }

        /// <summary>
        /// 암호변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bbtnPasswd_ItemClick(object sender, ItemClickEventArgs e)
        {
            FormChangePasswd frm = new FormChangePasswd();
            frm.ShowDialog();
        }

        /// <summary>
        /// 사용자 관리
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bbtnUsers_ItemClick(object sender, ItemClickEventArgs e)
        {
            FormUsers frm = new FormUsers();
            frm.ShowDialog();
        }

        /// <summary>
        /// 창이 열려있는지 검사
        /// </summary>
        /// <param name="formName"></param>
        /// <returns></returns>
        public bool isNewFormForDashBoard2(string formName, int focus)
        {
            foreach (Form theForm in Application.OpenForms) // 현재 프로그램에 있는 모든 열려있는 폼 체크
            {
                if (formName == theForm.Name)
                {
                    BaseForm basefrm = theForm as BaseForm;
                    theForm.BringToFront();
                    theForm.Focus();
                    basefrm.DoRefresh_Dashboard(focus);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 폼 열기 버튼 클릭 시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OpenDashBoard2(string formName, int focusChart)
        {
            SplashScreenManager.ShowForm(this, typeof(WaitForm1), true, true, false);
            SplashScreenManager.Default.SetWaitFormDescription("Ready to screen ..."); // 2018.07.20 Hwang.W.S : Caption으로 WaitForm을 띄우도록 변경

            if (isNewFormForDashBoard2(formName, focusChart))
            {
                if (formName == "FormDashboard2")
                {
                    new FormDashboard2(focusChart) { MdiParent = this }.Show();
                }
                else
                {
                    new FormDashboard3(focusChart) { MdiParent = this }.Show();
                }
            }
            SplashScreenManager.CloseForm(false);
        }
        #endregion

        #region 종료
        /// <summary>
        /// 프로그램 종료시
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (XtraMessageBox.Show("Are you sure you want to exit the program?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != System.Windows.Forms.DialogResult.Yes)
            {
                e.Cancel = true;
            }
            else
            {
                Program.SaveConfig();
            }
        }

        /// <summary>
        /// 프로그램 종료
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bbtnExit_ItemClick(object sender, ItemClickEventArgs e)
        {
            Close();
        }
        #endregion

        #region 스킨저장
        /// <summary>
        /// 스킨 Palette 저장
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void skinPaletteRibbonGalleryBarItem1_GalleryItemClick(object sender, GalleryItemClickEventArgs e)
        {
			Properties.Settings.Default.UserPalette = e.Item.Value.ToString();
			Properties.Settings.Default.Save();
			UserLookAndFeel.Default.SetSkinStyle(Properties.Settings.Default.UserSkin, tempPalette);

            foreach (Form frm in Application.OpenForms)
            {
                BaseForm bFrm = frm as BaseForm;
                if (bFrm != null)
                {
                    bFrm.SetSkinStyle();
                }
            }

			//if (XtraMessageBox.Show(LangResx.Main.msg_ChangeSkin, "Restart", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			//{
			//	try
			//	{
			//		Properties.Settings.Default.UserPalette = e.Item.Value.ToString();
			//		Properties.Settings.Default.Save();
			//		Application.Restart();
			//	}
			//	catch (Exception)
			//	{
			//	    UserLookAndFeel.Default.SetSkinStyle(Properties.Settings.Default.UserSkin, tempPalette);
			//		return;
			//	}
			//}
   //         else
   //         {
   //             Properties.Settings.Default.UserPalette = tempPalette;
			//	Properties.Settings.Default.Save();
			//	UserLookAndFeel.Default.SetSkinStyle(Properties.Settings.Default.UserSkin, tempPalette);
			//}
		}

        /// <summary>
        /// 스킨저장
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbbGallery_Gallery_ItemClick(object sender, DevExpress.XtraBars.Ribbon.GalleryItemClickEventArgs e)
        {
            string skinName = e.Item.Tag.ToString();

            if (skinName == "")
                return;

            try
            {
                Properties.Settings.Default.UserSkin = skinName;
                Properties.Settings.Default.Save();
            }
            catch (Exception)
            {
                return;
            }
        }
		#endregion
	}
}
