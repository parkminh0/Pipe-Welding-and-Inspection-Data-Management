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
using DevExpress.XtraSplashScreen;

namespace SWSUploader
{
    public partial class FormLogin : DevExpress.XtraEditors.XtraForm
    {
        private const string IDNotFound = "IDNotFound!";
        private int UserCompKey;
        private int UserAuthLevel;
        private int UserProjectKey;

        #region 폼 로드
        /// <summary>
        /// 
        /// </summary>
        public FormLogin()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormLogin_Load(object sender, EventArgs e)
        {
            Program.GRunYN = false;

            txtUserID.Text = Program.Option.LoginID;

            chkSave.Checked = Properties.Settings.Default.isSave;
            if (chkSave.Checked)
                txtPassword.Text = Properties.Settings.Default.Passwd;
            else
                txtPassword.Text = string.Empty;

            if (Program.Option.cultureName == "" || Program.Option.cultureName == "ko")
            {
                cboLanguage.SelectedIndex = 0;
            }
            else
            {
                cboLanguage.SelectedIndex = 1;
            }
        }
        
        /// <summary>
        /// 폼 쇼운
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormLogin_Shown(object sender, EventArgs e)
        {
            connectServer();
        }
        #endregion

        #region 서버 접속
        /// <summary>
        /// Connect Server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectServer()
        {
            Program.Option.ServerAddress = "112.220.22.186";
            try
            {
                DataTable dt = DBManager.Instance.GetDataTable(Program.constance.DBTestQuery, true);  //DB Connection
                if (dt != null && dt.Rows.Count > 0)
                {
                    Program.GRunYN = true;
                }
                else
                {
                    XtraMessageBox.Show(LangResx.Main.msg_LoginError1, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Program.GRunYN = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                Program.GRunYN = false;
                XtraMessageBox.Show(LangResx.Main.msg_LoginError1 + "\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            finally
            {
            }

            btnLogIn.Enabled = true;

            if (string.IsNullOrWhiteSpace(txtUserID.Text))
                txtUserID.Focus();
            else
                txtPassword.Focus();
        }
        #endregion

        /// <summary>
        /// 로그인
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            lblConnStatus.Visible = true;
            Application.DoEvents();
            DBConnect();
            lblConnStatus.Visible = false;
            if (!Program.GRunYN)
            {
                XtraMessageBox.Show(LangResx.Main.msg_LoginError2, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            LoginProcess();
        }

        /// <summary>
        /// DB Connect
        /// </summary>
        /// <returns></returns>
        private void DBConnect()
        {
            Program.GRunYN = false;
            try
            {
                DataTable dt = DBManager.Instance.GetDataTable(Program.constance.DBTestQuery);  //DB Connection
                if (dt != null && dt.Rows.Count > 0)
                    Program.GRunYN = true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(LangResx.Main.msg_LoginError2 + "\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        /// <summary>
        /// 로그인 로직
        /// </summary>
        private void LoginProcess()
        {
            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                XtraMessageBox.Show(LangResx.Main.msg_EmptyPW, "Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //if (AES.AESDecrypt256(usr, Program.constance.compName) != txtPassword.Text)
            string usr = GetPassword();
            if (usr == IDNotFound)
            {
                XtraMessageBox.Show(LangResx.Main.msg_IDError, "User ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUserID.Text = string.Empty;
                txtUserID.Focus();
                return;
            }

            if (usr.Trim() != txtPassword.Text)
            {
                XtraMessageBox.Show(LangResx.Main.msg_PWError, "Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtPassword.Text = string.Empty;
                txtPassword.Focus();
                return;
            }

            //권한 적용
            string sql2 = string.Empty;
            sql2 += $"select * from UserAuth WHERE UserID = '{txtUserID.Text.Trim()}' ";
            DataTable dtAuth = DBManager.Instance.GetDataTable(sql2);
            Program.Option.UploadInspectData = bool.Parse(dtAuth.Rows[0]["UploadInspectData"].ToString());
            Program.Option.UploadWeldData = bool.Parse(dtAuth.Rows[0]["UploadWeldData"].ToString());
            Program.Option.ShowInspectUploadRecord = bool.Parse(dtAuth.Rows[0]["ShowInspectUploadRecord"].ToString());
            Program.Option.ShowWeldUploadRecord = bool.Parse(dtAuth.Rows[0]["ShowWeldUploadRecord"].ToString());
            Program.Option.FormBeadMasterManage = bool.Parse(dtAuth.Rows[0]["FormBeadMasterManage"].ToString());
            Program.Option.FormWeldMasterManage = bool.Parse(dtAuth.Rows[0]["FormWeldMasterManage"].ToString());
            Program.Option.FormBeadDetailList = bool.Parse(dtAuth.Rows[0]["FormBeadDetailList"].ToString());
            Program.Option.FormBeadAndWeldList = bool.Parse(dtAuth.Rows[0]["FormBeadAndWeldList"].ToString());
            Program.Option.FormGeneralChart1 = bool.Parse(dtAuth.Rows[0]["FormGeneralChart1"].ToString());
            Program.Option.FormPivotResult = bool.Parse(dtAuth.Rows[0]["FormPivotResult"].ToString());
            Program.Option.FormDashboard = bool.Parse(dtAuth.Rows[0]["FormDashboard"].ToString());
            Program.Option.FormDashboard2 = bool.Parse(dtAuth.Rows[0]["FormDashboard2"].ToString());
            Program.Option.FormDashboard3 = bool.Parse(dtAuth.Rows[0]["FormDashboard3"].ToString());
            Program.Option.SaveExcel = bool.Parse(dtAuth.Rows[0]["SaveExcel"].ToString());
            Program.Option.DeleteInspectData = bool.Parse(dtAuth.Rows[0]["DeleteInspectData"].ToString());
            Program.Option.DeleteWeldData = bool.Parse(dtAuth.Rows[0]["DeleteWeldData"].ToString());

            Program.GRunYN = true;
            Program.Option.LoginID = txtUserID.Text.Trim();
            Program.Option.AuthLevel = UserAuthLevel;
            Program.Option.CompKey = UserCompKey;
            Program.Option.ProjectKey = UserProjectKey;

            string sql = string.Empty;
            sql += $"SELECT CompName from Company WHERE Compkey = {UserCompKey} ";
            DataTable temp = DBManager.Instance.GetDataTable(sql);
            if (temp != null && temp.Rows.Count != 0)
                Program.Option.CompName = temp.Rows[0][0].ToString();

            if (UserAuthLevel > 1)
            {
                sql = string.Empty;
                sql += $"SELECT ProjectName from Project WHERE ProjectKey = {UserProjectKey} ";
                DataTable temp2 = DBManager.Instance.GetDataTable(sql);
                Program.Option.ProjectName = temp2.Rows[0][0].ToString();
            }

            Properties.Settings.Default.isSave = chkSave.Checked;
            if (chkSave.Checked)
                Properties.Settings.Default.Passwd = txtPassword.Text;
            else
                Properties.Settings.Default.Passwd = string.Empty;

            if (cboLanguage.SelectedIndex == 0)
            {
                Program.ChangeLanguage("");
            }
            else
            {
                Program.ChangeLanguage("en");
            }

            Properties.Settings.Default.Save();
            this.Close();
        }

        /// <summary>
        /// DB에서 암호 가져오기
        /// </summary>
        /// <returns></returns>
        private string GetPassword()
        {
            string sql = string.Empty;
            sql += "select ui.Passwd, ui.AuthLevel, ui.CompKey, ISNULL(ui.ProjectKey, -1) ProjectKey, isPWUpdated ";
            sql += "  from UserInfo ui ";
            sql += $" where 1 = 1";
            sql += "   AND ui.isDeleted = 0 ";
            sql += $"  AND ui.UserID = '{txtUserID.Text.Trim()}' ";
            DataTable dtUser;
            string passwd = string.Empty;

            try
            {
                dtUser = DBManager.Instance.GetDataTable(sql);
                passwd = AES.AESDecrypt256(dtUser.Rows[0]["Passwd"].ToString(), Program.constance.compName);
                UserAuthLevel = int.Parse(dtUser.Rows[0]["AuthLevel"].ToString());
                UserCompKey = int.Parse(dtUser.Rows[0]["CompKey"].ToString());
                UserProjectKey = int.Parse(dtUser.Rows[0]["ProjectKey"].ToString());
                Program.isPWUpdated = bool.Parse(dtUser.Rows[0]["isPWUpdated"].ToString());
            }
            catch (Exception)
            {
                passwd = IDNotFound;
            }

            return passwd;
        }

        /// <summary>
        /// 엔터키 누를시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogIn.PerformClick();
            }
        }

        #region 종료
        /// <summary>
        /// 나가기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            Program.GRunYN = false;
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// 프로그램 종료시
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (Program.GRunYN)
                Program.SaveConfig();

            base.OnClosing(e);
        }
        #endregion
    }
}