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

namespace SWSAnalyzer
{
    public partial class FormChangePasswd : DevExpress.XtraEditors.XtraForm
    {
        public FormChangePasswd()
        {
            InitializeComponent();
        }

        private void FormChangePasswd_Load(object sender, EventArgs e)
        {
        }

        private void FormChangePasswd_Shown(object sender, EventArgs e)
        {
            txtPassword.Focus();
        }

        /// <summary>
        /// 닫기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 로그인
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

            string usr = LogicManager.Common.GetCurrentPasswd();
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
                Program.isPWUpdated = true;
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
            string sql = string.Format("update UserInfo set Passwd = '{0}', isPWUpdated = 1 where UserID = '{1}' ", AES.AESEncrypt256(txtPasswd1.Text, Program.constance.compName), Program.Option.LoginID);
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
    }
}