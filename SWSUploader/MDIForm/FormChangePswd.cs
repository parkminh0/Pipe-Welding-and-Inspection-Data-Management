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

namespace SWSUploader
{
    public partial class FormChangePswd : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 비밀번호 변경인지 여부
        /// </summary>
        public bool isChangePassword;

        public FormChangePswd()
        {
            InitializeComponent();
        }

        private void FormChangePswd_Load(object sender, EventArgs e)
        {
            if (isChangePassword)   //비밀번호 변경
            {
                lblPasswd1.Visible = true;
                lblPasswd2.Visible = true;
                txtPasswd1.Visible = true;
                txtPasswd2.Visible = true;
            }
            else
            {
                lblPasswd1.Visible = false;
                lblPasswd2.Visible = false;
                txtPasswd1.Visible = false;
                txtPasswd2.Visible = false;
            }
            txtPassword.Focus();
        }

        /// <summary>
        /// 나가기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 변경확인
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogIn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Text.Trim()))
            {
                XtraMessageBox.Show(LangResx.Main.msg_NowPWNull, LangResx.Main.msg_title_NowPWNull, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrEmpty(txtPasswd1.Text.Trim()) || txtPassword.Text.Trim() == txtPasswd1.Text.Trim())
            {
                XtraMessageBox.Show(LangResx.Main.msg_ChangePWNull, LangResx.Main.msg_title_ChangePWNull, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtPasswd1.Focus();
                return;
            }

            if (txtPasswd1.Text.Trim() != txtPasswd2.Text.Trim())
            {
                XtraMessageBox.Show(LangResx.Main.msg_ChangePWError, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtPasswd2.Focus();
                return;
            }

            string usr = GetPassword();
            if (usr.Trim() != txtPassword.Text)
            {
                XtraMessageBox.Show(LangResx.Main.msg_NowPWError, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtPassword.Text = string.Empty;
                txtPassword.Focus();
                return;
            }

            //암호 Update
            if (ChangePasswd())
            {
                XtraMessageBox.Show(LangResx.Main.msg_ChangePW, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
                XtraMessageBox.Show(LangResx.Main.msg_ChangeError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 암호변경
        /// </summary>
        private bool ChangePasswd()
        {
            //string sql = string.Format("update UserInfo set Passwd = '{0}' where UserID = '{0}' ", txtPasswd1.Text.Trim(), Program.Option.LoginID);
            string newPasswd = AES.AESEncrypt256(txtPasswd1.Text.Trim(), Program.constance.compName);
            string sql = string.Empty;
            sql += "UPDATE UserInfo ";
            sql += $"SET Passwd = '{newPasswd}', CreateDtm = GETDATE() ";
            sql += $"WHERE CompKey = {Program.Option.CompKey} AND UserID = '{Program.Option.LoginID}';";

            try
            {
                int result = DBManager.Instance.ExcuteDataUpdate(sql);
                if (result < 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(LangResx.Main.msg_CRUDError + "\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
        }

        private int Authority;
        private string UserName;
        /// <summary>
        /// DB에서 암호 가져오기
        /// </summary>
        /// <returns></returns>
        private string GetPassword()
        {
            string sql = string.Empty;
            sql += "select ui.Passwd, ui.AuthLevel ";
            sql += "  from UserInfo ui ";
            sql += $" where ui.CompKey = {Program.Option.CompKey}";
            sql += "   AND ui.isDeleted = 0 ";
            sql += $"   and ui.UserID = '{Program.Option.LoginID}'";
            string passwd = string.Empty;
            try
            {
                DataTable dtUser = DBManager.Instance.GetDataTable(sql);
                passwd = dtUser.Rows[0]["Passwd"].ToString();
                passwd = AES.AESDecrypt256(passwd, Program.constance.compName);
            }
            catch (Exception)
            {
                passwd = "!@#!@%^&*!@#@#@";
            }

            return passwd;
        }

        /// <summary>
        /// 엔터키 누를시
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPasswd2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogIn.PerformClick();
            }
        }
    }
}