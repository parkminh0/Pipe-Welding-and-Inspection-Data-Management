using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;

namespace SWSUploader
{
    public partial class SplashScreen2 : SplashScreen
    {
        public SplashScreen2()
        {
            InitializeComponent();
            labelControl1.Text = Program.constance.SystemTitle;
            labelControl6.Text = Program.Option.LoginID;
            labelControl8.Text = Program.Option.CompName;

            switch (Program.Option.AuthLevel)
            {
                case 0:
                    labelControl11.Text = "Super Admin";
                    labelControl10.Text = "";
                    break;
                case 1:
                    labelControl11.Text = "Admin";
                    labelControl10.Text = "";
                    break;
                case 2:
                    labelControl11.Text = "Manager";
                    labelControl10.Text = Program.Option.ProjectName;
                    break;
                case 3:
                    labelControl11.Text = "Operator";
                    labelControl10.Text = Program.Option.ProjectName;
                    break;
                default:
                    break;
            }
        }

        #region Overrides

        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
            SplashScreenCommand command = (SplashScreenCommand)cmd;
            if (command == SplashScreenCommand.SetProgress)
            {
                int pos = (int)arg;
                progressBarControl1.Position = pos;
            }
        }

        #endregion

        public enum SplashScreenCommand
        {
            SetProgress,
            DbConnect,
            SetStatus
        }
    }
}