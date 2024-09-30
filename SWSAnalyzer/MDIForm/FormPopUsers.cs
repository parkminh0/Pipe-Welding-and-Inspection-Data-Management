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
using DevExpress.CodeParser;
using System.Runtime.Remoting.Messaging;

namespace SWSAnalyzer
{
    public partial class FormPopUsers : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 편집구분 --> 1:신규 2:수정 3:삭제
        /// </summary>
        public int EditMode;
        public string ChoiceUserID;
        public string ChoiceAdminID;
        public string ChoiceManagerID;

        public int? ChoiceCompKey = null;
        public int? ChoiceAuthLevel = null;
        public int? ChoiceProjectKey = null;

        #region 폼 로드
        /// <summary>
        /// 
        /// </summary>
        public FormPopUsers()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormPopUser_Load(object sender, EventArgs e)
        {
            SetCompanyCode();
            SetAuthLevelCode();
            SetProject();
        }

        /// <summary>
        /// 폼 쇼운
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormPopUser_Shown(object sender, EventArgs e)
        {
            switch (EditMode)
            {
                case 1:
                    this.Text = LangResx.Main.title_UserNew;
                    btnApply.Text = LangResx.Main.btnNew;
                    txtUserID.Enabled = true;
                    lueCompany.Focus();
                    break;
                case 2:
                    this.Text = LangResx.Main.title_UserMod;
                    btnApply.Text = LangResx.Main.btnModify;
                    txtUserID.Enabled = false;
                    txtUserID.Focus();
                    GetInfo();
                    if (ChoiceAuthLevel == 0)
                        lueCompany.ReadOnly = lueAuthLevel.ReadOnly = true;
                    break;
                case 3:
                    this.Text = LangResx.Main.title_UserDel;
                    btnApply.Text = LangResx.Main.btnDelete;
                    btnApply.Focus();
                    GetInfo();
                    lueCompany.ReadOnly = lueProject.ReadOnly = lueAuthLevel.ReadOnly = lueAdmin.ReadOnly = lueManager.ReadOnly = dteExpire.ReadOnly = txtUserID.ReadOnly = txtPassword.ReadOnly = txtDescr.ReadOnly = txtPhoneNo.ReadOnly = txtUserName.ReadOnly = true;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 기존 정보 읽어오기
        /// </summary>
        private void GetInfo()
        {
            string sql = string.Empty;
            sql += "select ui.UserID ";
            sql += "	  ,ui.UserName ";
            sql += "	  ,ui.Passwd ";
            sql += "	  ,ui.AuthLevel ";
            sql += "	  ,ui.PhoneNo ";
            sql += "	  ,ui.Descr ";
            sql += "      ,ui.ExpireDtm ";
            sql += "      ,ui.CompKey ";
            sql += "      ,ISNULL(ui.ProjectKey, -1) ProjectKey ";
            sql += "      ,ui.ParentAdminID ";
            sql += "      ,ui.ParentManagerID ";
            sql += " from UserInfo ui ";
            sql += $"where ui.CompKey = {ChoiceCompKey} ";
            sql += $"  and ui.UserID = '{ChoiceUserID}' ";
            sql += "   and ui.isdeleted = 0 ";

            DataTable dtData = DBManager.Instance.GetDataTable(sql);
            try
            {
                lueCompany.EditValue = int.Parse(dtData.Rows[0]["Compkey"].ToString());
                lueAuthLevel.EditValue = int.Parse(dtData.Rows[0]["AuthLevel"].ToString());
                ChoiceAuthLevel = int.Parse(dtData.Rows[0]["AuthLevel"].ToString());
                if (int.Parse(dtData.Rows[0]["ProjectKey"].ToString()) != -1)
                    lueProject.EditValue = int.Parse(dtData.Rows[0]["ProjectKey"].ToString());
                lueAdmin.EditValue = dtData.Rows[0]["ParentAdminID"].ToString();
                lueManager.EditValue = dtData.Rows[0]["ParentManagerID"].ToString();
                txtUserID.Text = dtData.Rows[0]["UserID"].ToString();
                txtUserName.Text = dtData.Rows[0]["UserName"].ToString();
                txtPassword.Text = AES.AESDecrypt256(dtData.Rows[0]["Passwd"].ToString(), Program.constance.compName);
                txtPhoneNo.Text = dtData.Rows[0]["PhoneNo"].ToString();
                txtDescr.Text = dtData.Rows[0]["Descr"].ToString();
                dteExpire.DateTime = DateTime.Parse(dtData.Rows[0]["ExpireDtm"].ToString());
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region 컨트롤 세팅
        /// <summary>
        /// 거래처 세팅
        /// </summary>
        private void SetCompanyCode()
        {
            string sql = "select c.CompKey No, c.CompName as Company from Company c where c.isDeleted = 0 ";
            DataTable dtCompany = DBManager.Instance.GetDataTable(sql);
            lueCompany.Properties.DataSource = dtCompany;
            lueCompany.Properties.ValueMember = "No";
            lueCompany.Properties.DisplayMember = "Company";
        }

        /// <summary>
        /// 사용자권한코드 세팅
        /// </summary>
        private void SetAuthLevelCode()
        {
            string sql = string.Empty;
            sql += "select cast(DetailCode as int) AuthLevel, CodeDescr Auth ";
            sql += "  from CodeInfomation ";
            sql += " where 1 = 1 ";
            sql += "   and Category = 'AuthLevel' ";
            if (ChoiceAuthLevel == null || ChoiceAuthLevel != 0)
                sql += "   and DetailCode != 0 "; // Super Admin 제외
            sql += " order by OrderNo";
            DataTable dtAuthLevel = DBManager.Instance.GetDataTable(sql);
            lueAuthLevel.Properties.DataSource = dtAuthLevel;
            lueAuthLevel.Properties.ValueMember = "AuthLevel";
            lueAuthLevel.Properties.DisplayMember = "Auth";
            lueAuthLevel.EditValue = ChoiceAuthLevel;
        }

        /// <summary>
        /// 프로젝트 세팅
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetProject()
        {
            string sql = string.Empty;
            sql += "SELECT ProjectKey No, ProjectName Project from Project WHERE isdeleted = 0 ";
            DataTable dtProject = DBManager.Instance.GetDataTable(sql);
            lueProject.Properties.DataSource = dtProject;
            lueProject.Properties.ValueMember = "No";
            lueProject.Properties.DisplayMember = "Project";
            lueProject.EditValue = ChoiceProjectKey;
        }
        #endregion

        #region lookupedit valuechange
        /// <summary>
        /// 권한 수정 시 초기화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lueAuthLevel_EditValueChanged(object sender, EventArgs e)
        {
            if (lueAuthLevel.EditValue == null || int.Parse(lueAuthLevel.EditValue.ToString()) <= 1) // SA or Admin
            {
                lueProject.EditValue = lueAdmin.EditValue = lueManager.EditValue = null;
                lueProject.ReadOnly = lueAdmin.ReadOnly = lueManager.ReadOnly = true;
            }
            else // Manager or Operator
            {
                lueProject.ReadOnly = false;
                if (int.Parse(lueAuthLevel.EditValue.ToString()) == 2) // Manager
                {
                    lueAdmin.ReadOnly = false;
                    lueManager.ReadOnly = true;
                    lueManager.EditValue = null;
                }
                else // Operator
                {
                    lueAdmin.ReadOnly = lueManager.ReadOnly = false;
                }
            }
        }

        /// <summary>
        /// 프로젝트 수정 시 초기화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lueProject_EditValueChanged(object sender, EventArgs e)
        {
            lueAdmin.EditValue = lueManager.EditValue = null;
            lueAdmin.ReadOnly = lueManager.ReadOnly = true;

            if (lueAuthLevel.EditValue == null || int.Parse(lueAuthLevel.EditValue.ToString()) <= 1 || lueProject.EditValue == null)
                return;

            SetAdmin();
            SetManager();
            if (int.Parse(lueAuthLevel.EditValue.ToString()) == 2) //Manager
            {
                lueAdmin.ReadOnly = false;
                lueManager.ReadOnly = true;
            }
            else //Operator
            {
                lueAdmin.ReadOnly = lueManager.ReadOnly = false;
            }
        }

        /// <summary>
        /// Admin 세팅(Manager or Operator일 경우만)
        /// </summary>
        private void SetAdmin()
        {
            string sql = string.Empty;
            sql += "select u.UserID AdminID ";
            sql += "  from project p ";
            sql += "  join UserInfo u ";
            sql += "    on u.CompKey = p.CompKey ";
            sql += "   and u.AuthLevel = 1 ";
            sql += "   and u.isDeleted = 0 ";
            sql += $"  and u.UserID != '{ChoiceUserID}' ";
            sql += " where 1 = 1 ";
            sql += "   and p.isdeleted = 0 ";
            sql += $"  and p.ProjectKey = {lueProject.EditValue} ";
            DataTable dtAdmin = DBManager.Instance.GetDataTable(sql);
            lueAdmin.Properties.DataSource = dtAdmin;
            lueAdmin.Properties.ValueMember = "AdminID";
            lueAdmin.Properties.DisplayMember = "AdminID";
        }

        /// <summary>
        /// Manager 세팅(Operator일 경우만)
        /// </summary>
        private void SetManager()
        {
            string sql = string.Empty;
            sql += "select u.UserID ManagerID";
            sql += "  from project p ";
            sql += "  join UserInfo u ";
            sql += "    on u.ProjectKey = p.ProjectKey ";
            sql += "   and u.AuthLevel = 2 ";
            sql += "   and u.isDeleted = 0 ";
            sql += $"  and u.UserID != '{ChoiceUserID}' ";
            sql += " where 1 = 1 ";
            sql += "   and p.isdeleted = 0 ";
            sql += $"  and p.ProjectKey = {lueProject.EditValue} ";
            DataTable dtManagaer = DBManager.Instance.GetDataTable(sql);
            lueManager.Properties.DataSource = dtManagaer;
            lueManager.Properties.ValueMember = "ManagerID";
            lueManager.Properties.DisplayMember = "ManagerID";
        }
        #endregion

        #region CRUD
        /// <summary>
        /// 적용
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, EventArgs e)
        {
            if (EditMode == 3)
            {
                if (XtraMessageBox.Show(LangResx.Main.msg_Del, LangResx.Main.msg_title_Del, MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
                    return;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(lueCompany.Text))
                {
                    XtraMessageBox.Show(LangResx.Main.msg_CompanyNull, LangResx.Main.msg_title_CompanyNull, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lueCompany.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(lueAuthLevel.Text))
                {
                    XtraMessageBox.Show(LangResx.Main.msg_AuthLevelNull, LangResx.Main.msg_title_AuthLevelNull, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lueAuthLevel.Focus();
                    return;
                }
                if (int.Parse(lueAuthLevel.EditValue.ToString()) > 1)
                {
                    if (string.IsNullOrWhiteSpace(lueProject.Text))
                    {
                        XtraMessageBox.Show(LangResx.Main.msg_ProjectNull, LangResx.Main.msg_title_ProjectNull, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        lueProject.Focus();
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(lueAdmin.Text))
                    {
                        XtraMessageBox.Show(LangResx.Main.msg_AdminNull, LangResx.Main.msg_title_AdminNull, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        lueAdmin.Focus();
                        return;
                    }

                    if (int.Parse(lueAuthLevel.EditValue.ToString()) == 3) // Operator일 경우
                    {
                        if (string.IsNullOrWhiteSpace(lueManager.Text))
                        {
                            XtraMessageBox.Show(LangResx.Main.msg_ManagerNull, LangResx.Main.msg_title_ManagerNull, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            lueManager.Focus();
                            return;
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(dteExpire.Text))
                {
                    XtraMessageBox.Show(LangResx.Main.msg_ExpireNull, LangResx.Main.msg_title_ExpireNull, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dteExpire.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtUserID.Text))
                {
                    XtraMessageBox.Show(LangResx.Main.msg_UserIDNull, LangResx.Main.msg_title_UserIDNull, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtUserID.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtUserName.Text))
                {
                    XtraMessageBox.Show(LangResx.Main.msg_UserNameNull, "User name", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtUserName.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    XtraMessageBox.Show(LangResx.Main.msg_UserPWNull, "Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtPassword.Focus();
                    return;
                }
            }

            ChoiceUserID = txtUserID.Text.Trim();
            List<string> sqls = new List<string>();
            string sql = string.Empty;

            switch (EditMode)
            {
                case 1: //신규
                    if (isExists()) //중복검사
                        return;

                    sql = string.Empty;
                    sql += "INSERT INTO [UserInfo] ";
                    sql += "           ([CompKey] ";
                    sql += "           ,[UserID] ";
                    sql += "           ,[UserName] ";
                    sql += "           ,[Passwd] ";
                    sql += "           ,[AuthLevel] ";
                    sql += "           ,[ProjectKey] ";
                    sql += "           ,[ParentAdminID] ";
                    sql += "           ,[ParentManagerID] ";
                    sql += "           ,[PhoneNo] ";
                    sql += "           ,[Descr] ";
                    sql += "           ,[isDeleted] ";
                    sql += "           ,[CreateID] ";
                    sql += "           ,[CreateDtm] ";
                    sql += "           ,[ExpireDtm] ";
                    sql += "           ,[isPWUpdated]) ";
                    sql += "     VALUES ";
                    sql += $"           ({lueCompany.EditValue} ";
                    sql += $"           ,'{txtUserID.Text.Trim()}' ";
                    sql += $"           ,'{txtUserName.Text}' ";
                    sql += $"           ,'{AES.AESEncrypt256(txtPassword.Text, Program.constance.compName)}' ";
                    sql += $"           ,{lueAuthLevel.EditValue.ToString()} ";
                    if (int.Parse(lueAuthLevel.EditValue.ToString()) <= 1) //projectkey & ParentAdminID & ParentManagerID
                    {
                        sql += "            ,'' ";
                        sql += $"           ,'{txtUserID.Text.Trim()}' ";
                        sql += "            ,'' ";
                    }
                    else if (int.Parse(lueAuthLevel.EditValue.ToString()) == 2)
                    {

                        sql += $"            ,{lueProject.EditValue} ";
                        sql += $"           ,'{lueAdmin.EditValue}' ";
                        sql += "            ,'' ";
                    }
                    else
                    {
                        sql += $"            ,{lueProject.EditValue} ";
                        sql += $"           ,'{lueAdmin.EditValue}' ";
                        sql += $"           ,'{lueManager.EditValue}' ";
                    }
                    sql += $"           ,'{txtPhoneNo.Text}' ";
                    sql += $"           ,'{txtDescr.Text}' ";
                    sql += "           ,0 ";
                    sql += $"           ,'{Program.Option.LoginID}' ";
                    sql += "           ,GETDATE() ";
                    sql += $"          ,'{dteExpire.DateTime.ToString("yyyy-MM-dd")}' ";
                    sql += "           ,0 )";

                    sqls.Add(sql);
                    sql = string.Empty;
                    sql += "INSERT INTO UserAuth ";
                    sql += "VALUES (";
                    sql += $"      {lueCompany.EditValue}, ";
                    sql += $"      '{ChoiceUserID}', ";
                    if (int.Parse(lueAuthLevel.EditValue.ToString()) == 3) // Operator
                    {
                        sql += " 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ";
                    }
                    else if (int.Parse(lueAuthLevel.EditValue.ToString()) == 2) // Manager
                    {
                        sql += " 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 ";
                    }
                    else
                    {
                        sql += " 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0 ";
                    }
                    sql += " ) ";
                    break;
                case 2: //변경
                    if (isExists()) //중복검사
                        return;

                    sql = string.Empty;
                    sql += "UPDATE [UserInfo] ";
                    sql += $"   SET [CompKey] = {lueCompany.EditValue} ";
                    sql += $"      ,[AuthLevel] = {lueAuthLevel.EditValue} ";
                    if (lueProject.EditValue == null)
                    {
                        sql += $"      ,[ProjectKey] = NULL ";
                    }
                    else
                    {
                        sql += $"      ,[ProjectKey] = {lueProject.EditValue} ";
                    }
                    sql += $"      ,[Passwd] = '{AES.AESEncrypt256(txtPassword.Text, Program.constance.compName)}' ";
                    if (int.Parse(lueAuthLevel.EditValue.ToString()) == 1)
                    {
                        sql += $"      ,[ParentAdminID] = '{ChoiceUserID}' ";
                    }
                    else if (int.Parse(lueAuthLevel.EditValue.ToString()) > 1)
                    {
                        sql += $"      ,[ParentAdminID] = '{lueAdmin.EditValue}' ";
                    }
                    sql += $"      ,[ParentManagerID] = '{lueManager.EditValue}' ";
                    sql += $"      ,[PhoneNo] = '{txtPhoneNo.Text}' ";
                    sql += $"      ,[Descr] = '{txtDescr.Text}' ";
                    sql += $"      ,[ExpireDtm] = '{dteExpire.DateTime.ToString("yyyy-MM-dd")}' ";
                    sql += $" WHERE UserID = '{ChoiceUserID}' ";
                    sql += $"   AND CompKey = {ChoiceCompKey} ";
                    break;
                case 3: //삭제
                    if (ChoiceAuthLevel != 3)
                    {
                        sql = string.Empty;
                        sql += "SELECT COUNT(*) FROM UserInfo ";
                        sql += " WHERE 1 = 1 ";
                        if (ChoiceAuthLevel == 1)
                        {
                            sql += $"   AND ParentAdminID = N'{ChoiceUserID}' ";
                            sql += $"   AND ParentAdminID != UserID ";
                        }
                        else
                        {
                            sql += $"   AND ParentManagerID = {ChoiceUserID} ";
                        }
                        int tmp = DBManager.Instance.GetIntScalar(sql);
                        if (tmp > 0)
                        {
                            XtraMessageBox.Show(LangResx.Main.msg_UserDelError, "Delete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }

                    sql = string.Empty;
                    sql += "UPDATE UserInfo ";
                    sql += "   SET isDeleted = 0 ";
                    sql += $"WHERE CompKey = {ChoiceCompKey} AND UserID = '{ChoiceUserID}' ";
                    break;
                default:
                    break;
            }
            sqls.Add(sql);

            string result = DBManager.Instance.ExcuteTransaction(sqls);
            if (string.IsNullOrWhiteSpace(result))
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                XtraMessageBox.Show(LangResx.Main.msg_CRUDError + "\r\n" + result, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        /// <summary>
        /// 신규시 ID중복 검사
        /// </summary>
        /// <returns></returns>
        private bool isExists()
        {
            try
            {
                string sql = string.Empty;
                if (int.Parse(lueAuthLevel.EditValue.ToString()) == 1)
                {
                    switch (EditMode)
                    {
                        case 1:
                            sql += "SELECT COUNT(*) From UserInfo ";
                            sql += " WHERE 1 = 1 ";
                            sql += $"  AND CompKey = {lueCompany.EditValue} ";
                            sql += $"  AND AuthLevel = {lueAuthLevel.EditValue} ";
                            sql += "   and isDeleted = 0 ";
                            break;
                        case 2:
                            sql += "SELECT COUNT(*) From UserInfo ";
                            sql += " WHERE 1 = 1 ";
                            sql += $"  AND UserID != '{txtUserID.Text}' ";
                            sql += $"  AND CompKey = {lueCompany.EditValue} ";
                            sql += $"  AND AuthLevel = {lueAuthLevel.EditValue} ";
                            sql += "   and isDeleted = 0 ";
                            break;
                    }

                    if (DBManager.Instance.GetIntScalar(sql) > 0)
                    {
                        XtraMessageBox.Show(string.Format(LangResx.Main.msg_AuthExist1, lueCompany.Text), LangResx.Main.msg_title_AuthExist, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return true;
                    }
                    return false;
                }

                if (int.Parse(lueAuthLevel.EditValue.ToString()) == 2)
                {
                    switch (EditMode)
                    {
                        case 1:
                            sql += "SELECT COUNT(*) From UserInfo ";
                            sql += " WHERE 1 = 1 ";
                            sql += $"  AND CompKey = {lueCompany.EditValue} ";
                            sql += $"  AND AuthLevel = {lueAuthLevel.EditValue} ";
                            sql += $"  AND ProjectKey = {lueProject.EditValue} ";
                            sql += "   and isDeleted = 0 ";
                            break;
                        case 2:
                            sql += "SELECT COUNT(*) From UserInfo ";
                            sql += " WHERE 1 = 1 ";
                            sql += $"  AND CompKey = {lueCompany.EditValue} ";
                            sql += $"  AND AuthLevel = {lueAuthLevel.EditValue} ";
                            sql += $"  AND ProjectKey = {lueProject.EditValue} ";
                            sql += $"  AND UserID != '{txtUserID.Text}' ";
                            sql += "   and isDeleted = 0 ";
                            break;
                    }

                    if (DBManager.Instance.GetIntScalar(sql) > 0)
                    {
                        XtraMessageBox.Show(string.Format(LangResx.Main.msg_AuthExist2, lueCompany.Text, lueAuthLevel.Text), LangResx.Main.msg_title_AuthExist, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return true;
                    }
                    return false;
                }

                if (EditMode == 1)
                {
                    sql = string.Empty;
                    sql += $"select count(*) from UserInfo ui where ui.UserID = '{ChoiceUserID}' ";
                    if (DBManager.Instance.GetIntScalar(sql) > 0)
                    {
                        XtraMessageBox.Show("이미 존재하거나 이전에 사용하였던 User ID입니다.\r\n새로운 ID를 입력해 주세요.", "ID중복", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception e)
            {
                XtraMessageBox.Show(LangResx.Main.msg_CRUDError + "\r\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return false;
        }
        #endregion

        #region 닫기
        /// <summary>
        /// 닫기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}