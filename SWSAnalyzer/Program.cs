using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.XtraSplashScreen;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Globalization;
using System.Threading;
using DevExpress.XtraEditors;

namespace SWSAnalyzer
{
    static class Program
    {
        public static OptionInfo Option;
        public static WarningMSG WMSG;
        public static bool GRunYN;
        public static bool isPWUpdated;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            constance = new Constance();
            LoadConfig();
            ChangeLanguage(Option.cultureName);

            // DB설정 및 로그인 창에도 적용이 되도록 폰트 설정시점 변경
            DevExpress.Utils.AppearanceObject.DefaultFont = new System.Drawing.Font(LangResx.Main.Font, 9);
            //DevExpress.Utils.AppearanceObject.DefaultFont = new System.Drawing.Font("맑은 고딕", 9);
            // 기본스킨 Bezier로 변경 및 팔레트 설정 추가 (DB설정 및 로그인 창에도 적용이 되도록 설정시점 변경)
            if (Properties.Settings.Default.UserSkin != "")
            {
                if (Properties.Settings.Default.UserSkin == "The Bezier" && Properties.Settings.Default.UserPalette != "")
                    UserLookAndFeel.Default.SetSkinStyle(Properties.Settings.Default.UserSkin, Properties.Settings.Default.UserPalette);
                else
                    UserLookAndFeel.Default.SetSkinStyle(Properties.Settings.Default.UserSkin);
            }
            else
            {
                UserLookAndFeel.Default.SetSkinStyle("The Bezier", "Gloom Gloom");
            }

            //GRunYN = true;
            GRunYN = false;
            isPWUpdated = false;
            FormLogin logfrm = new FormLogin();
            logfrm.ShowDialog();
            if (!GRunYN)
                return;


            if (!isPWUpdated)
            {
                XtraMessageBox.Show(LangResx.Main.msg_FirstLogin, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FormChangePasswd changefrm = new FormChangePasswd();
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

            mainApp = new MainForm();
            WMSG = new WarningMSG();
            string runCheck = mainApp.Splash();

            if (string.IsNullOrEmpty(runCheck) && GRunYN)
            {
                Application.Run(mainApp);
            }
            else
            {
                SaveConfig();

                SplashScreenManager.Default.SendCommand(SplashScreen2.SplashScreenCommand.SetStatus, runCheck);
                SplashScreenManager.CloseForm(true);
                WMSG.MSG(runCheck);
            }
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
