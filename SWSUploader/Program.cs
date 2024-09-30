using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;
using System.IO;
using System.Text;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using DevExpress.LookAndFeel;
using DevExpress.XtraSplashScreen;
using System.Drawing;
using System.Globalization;
using System.Threading;
using DevExpress.XtraEditors;

namespace SWSUploader
{
    static class Program
    {
        public static OptionInfo Option;
        public static WarningMSG WMSG;
        public static bool GRunYN;
        public static bool isBusy;
        public static int UserAuthLevel;
        public static bool isPWUpdated;

        public static int CompKey;
        public static int ProjectKey;

        /// <summary>
        /// The main entry point for the application. 
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            BonusSkins.Register();
            constance = new Constance();
            LoadConfig();
            ChangeLanguage(Option.cultureName);

            DevExpress.Utils.AppearanceObject.DefaultFont = new System.Drawing.Font(LangResx.Main.Font, 9);
            //DevExpress.Utils.AppearanceObject.DefaultFont = new System.Drawing.Font("맑은 고딕", 9);
            UserLookAndFeel.Default.SetSkinStyle("The Bezier");

            //GRunYN = true;
            GRunYN = false;
            isPWUpdated = false;
            FormLogin frm = new FormLogin();
            frm.ShowDialog();
            if (!GRunYN)
                return;

            if (!isPWUpdated)
            {
                XtraMessageBox.Show(LangResx.Main.msg_FirstLogin, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FormChangePswd changefrm = new FormChangePswd();
                changefrm.ShowDialog();
            }

            if (!isPWUpdated)
                return;

            DevExpress.Utils.AppearanceObject.DefaultFont = new System.Drawing.Font(LangResx.Main.Font, 9);
            SplashScreenManager.ShowForm(mainApp, typeof(SplashScreen2), true, true, false);
            SplashScreenManager.Default.SendCommand(SplashScreen2.SplashScreenCommand.SetStatus, "Starting....");
            SkinManager.EnableFormSkins();
            SplashScreenManager.Default.SendCommand(SplashScreen2.SplashScreenCommand.SetStatus, "Starting....");
            BonusSkins.Register();
            //UserLookAndFeel.Default.SetSkinStyle("DevExpress Style");
            SplashScreenManager.Default.SendCommand(SplashScreen2.SplashScreenCommand.SetStatus, "Starting....");
            DevExpress.Utils.AppearanceObject.DefaultFont = new System.Drawing.Font(LangResx.Main.Font, 9);
            //DevExpress.Utils.AppearanceObject.DefaultFont = new System.Drawing.Font("굴림", 9);

            if (GRunYN)
            {
                mainApp = new MainForm();
                string runCheck = mainApp.Splash();
                //ShowSplashScreen(mainApp);
                Application.Run(mainApp);
            }
            else
            {
                SaveConfig();
            }
        }

        private static void ShowSplashScreen(Form form)
        {
            // Logo image.
            Image myLogoImage = null; // Resources.Logo;

            // Show a splashscreen.
            SplashScreenManager.ShowSkinSplashScreen(
                logoImage: myLogoImage,
                title: "Automatic welding-bead inspection",
                subtitle: "SWS(SMART WELD SCANNER) System",
                footer: "Copyright© 2020 Agru Korea Welding Technic " + Environment.NewLine + "All Rights reserved.",
                loading: "Starting...",
                parentForm: form
            );

            //SplashScreenManager.Default.SendCommand(SkinSplashScreenCommand.UpdateLoadingText, "Done");
        }

        public static MainForm mainApp;
        public static Constance constance;

        /// <summary> 설정파일 xml싱크. </summary>
        public static XmlSerializer m_serializer = new XmlSerializer(typeof(OptionInfo));

        /// <summary>
        /// 설정파일 xml 전체경로
        /// </summary>
        static string cfgPath = Path.GetFileNameWithoutExtension(System.Environment.GetCommandLineArgs()[0]) + "Settings.xml";
        public static string CfgPath
        {
            get
            {
                return Path.Combine(Program.constance.CommonFilePath, cfgPath);
            }
        }

        /// <summary>
        /// 설정정보 로드
        /// </summary>
        public static void LoadConfig()
        {
            if (File.Exists(Program.CfgPath))
            {
                bool isError = false;
                using (FileStream fs = File.OpenRead(Program.CfgPath))
                {
                    try
                    {
                        Program.Option = Program.m_serializer.Deserialize(fs) as OptionInfo;
                    }
                    catch
                    {
                        isError = true;
                    }
                }

                if (isError)
                {
                    File.Delete(Program.CfgPath);
                }
            }

            if (Program.Option == null)
            {
                Program.Option = new OptionInfo();
                Option.LoginID = "admin";
                Option.ServerAddress = "112.220.22.186";
                Option.ProjectKey = Option.CompKey = 0;
                SaveConfig();
            }
            else
            {
            }
        }

        /// <summary>
        /// 설정정보 저장.
        /// </summary>
        public static void SaveConfig()
        {
            if (!File.Exists(Program.CfgPath))
            {
                File.Create(Program.CfgPath).Close();
            }

            using (XmlTextWriter xtw = new XmlTextWriter(Program.CfgPath, Encoding.UTF8))
            {
                Program.m_serializer.Serialize(xtw, Program.Option);
                xtw.Flush();
                xtw.Close();
            }
        }

        /// <summary>
        /// 현재 선택한 프로젝트 정보 보여주기
        /// </summary>
        //public static void ShowMainFormTitle()
        //{
        //    if (MasterKey == 0)
        //    {
        //        //ProjectKey = Option.ProjectKey;
        //        //ProjectName = Option.ProjectName;
        //    }

        //    mainApp.Text = string.Format("{0}  -  [{1}]: {2} ", constance.SystemTitle, MasterKey, MasterName);
        //    //if (ProjectKey == 0)
        //    //    mainApp.Text = string.Format("{0} ", constance.SystemTitle);
        //    //else
        //    //    mainApp.Text = string.Format("{0}  -  [{1}]: {2} ", constance.SystemTitle, ProjectKey, ProjectName);
        //}

        /// <summary>
        /// 언어 변경
        /// </summary>
        /// <param name="cultureName"></param>
        public static void ChangeLanguage(string cultureName)
        {
            CultureInfo culture;
            if (string.IsNullOrWhiteSpace(cultureName))
            {
                string lang = CultureInfo.InstalledUICulture.Parent.IetfLanguageTag;
                if (lang == "ko" || lang == "en" || lang == "es" || lang == "id" || lang == "fr" || lang == "")
                    Option.cultureName = lang;
                else
                    Option.cultureName = "en";
            }
            else
            {
                Option.cultureName = cultureName;
            }
            SaveConfig();

            culture = CultureInfo.CreateSpecificCulture(Option.cultureName);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            DevExpress.Utils.AppearanceObject.DefaultFont = new System.Drawing.Font(LangResx.Main.Font, 9);
        }
    }
}