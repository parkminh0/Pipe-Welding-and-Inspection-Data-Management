using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SWSAnalyzer
{
    public partial class FormPopProject : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 편집구분 --> 1:신규 2:수정 3:삭제
        /// </summary>
        /// 
        public int EditMode;
        public int ChoiceProjectKey;
        public int ChoiceCompKey;
        private string ChoiceProjectName;

        #region 폼 로드
        /// <summary>
        /// 
        /// </summary>
        public FormPopProject()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormPopProject2_Load(object sender, EventArgs e)
        {
            SetCompanyCode();
        }

        /// <summary>
        /// 폼 쇼운
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormPopProject2_Shown(object sender, EventArgs e)
        {
            switch (EditMode)
            {
                case 1:
                    this.Text = LangResx.Main.title_ProjNew;
                    btnApply.Text = LangResx.Main.btnNew;
                    txtProjectName.Focus();
                    break;
                case 2:
                    this.Text = LangResx.Main.title_ProjMod;
                    btnApply.Text = LangResx.Main.btnModify;
                    txtDescr.Focus();
                    GetInfo();
                    break;
                case 3:
                    this.Text = LangResx.Main.title_ProjDel;
                    btnApply.Text = LangResx.Main.btnDelete;
                    btnApply.Focus();
                    lueCompany.ReadOnly = txtProjectName.ReadOnly = txtDescr.ReadOnly = true;
                    GetInfo();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 거래처 콤보
        /// </summary>
        private void SetCompanyCode()
        {
            string sql = "select c.CompKey No, c.CompName as Company from Company c where c.isDeleted = 0 ";
            DataTable dtCompany = DBManager.Instance.GetDataTable(sql);
            lueCompany.Properties.DataSource = dtCompany;
            lueCompany.Properties.ValueMember = "No";
            lueCompany.Properties.DisplayMember = "Company";
            lueCompany.EditValue = ChoiceCompKey;
        }

        /// <summary>
        /// 기존 정보 읽어오기
        /// </summary>
        private void GetInfo()
        {
            string sql = string.Format("select ProjectKey, ProjectName, Descr, CompKey from Project where isDeleted = 0 and ProjectKey = {0} ", ChoiceProjectKey);
            DataTable dtData;
            try
            {
                dtData = DBManager.Instance.GetDataTable(sql);
                txtProjectKey.Text = dtData.Rows[0]["ProjectKey"].ToString();
                txtProjectName.Text = dtData.Rows[0]["ProjectName"].ToString();
                ChoiceProjectName = dtData.Rows[0]["ProjectName"].ToString();
                lueCompany.EditValue = int.Parse(dtData.Rows[0]["CompKey"].ToString());
                txtDescr.Text = dtData.Rows[0]["Descr"].ToString();
            }
            catch (Exception)
            {
                dtData = null;
            }
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
            string sql = string.Empty;
            if (EditMode <= 2)
            {
                if (string.IsNullOrWhiteSpace(txtProjectName.Text))
                {
                    XtraMessageBox.Show(LangResx.Main.msg_ProjNull, LangResx.Main.msg_title_ProjNull, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                switch (EditMode)
                {
                    case 1:
                        sql += "select COUNT(*) from Project ";
                        sql += $"where ProjectName = '{txtProjectName.Text}' AND isDeleted = 0 ";
                        break;
                    case 2:
                        sql += "select COUNT(*) from Project ";
                        sql += $"where '{ChoiceProjectName}' != '{txtProjectName.Text}' AND ProjectName = '{txtProjectName.Text}' and isDeleted = 0 ";
                        break;
                }
                if (DBManager.Instance.GetIntScalar(sql) > 0)
                {
                    XtraMessageBox.Show(LangResx.Main.msg_ProjExist, LangResx.Main.msg_title_Exist, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                if (XtraMessageBox.Show(LangResx.Main.msg_Del, LangResx.Main.msg_title_Del, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }

            List<string> sqls = new List<string>();
            sql = string.Empty;

            switch (EditMode)
            {
                case 1: //신규
                    ChoiceProjectKey = LogicManager.Common.GetNextKey("ProjectKey");
                    txtProjectKey.Text = ChoiceProjectKey.ToString();
                    sql = string.Empty;
                    sql += "INSERT INTO [Project] ";
                    sql += "           ([ProjectKey] ";
                    sql += "           ,[ProjectName] ";
                    sql += "           ,[CompKey] ";
                    sql += "           ,[isDeleted] ";
                    sql += "           ,[Descr] ";
                    sql += "           ,[CreateID] ";
                    sql += "           ,[CreateDtm]) ";
                    sql += "	VALUES ";
                    sql += "		(" + ChoiceProjectKey + " ";
                    sql += "		,'" + txtProjectName.Text + "' ";
                    sql += "		," + ChoiceCompKey + " ";
                    sql += "		,0 ";
                    sql += "		,'" + txtDescr.Text + "' ";
                    sql += "		,'" + Program.Option.LoginID + "' ";
                    sql += "		,GETDATE()) ";
                    break;
                case 2: //변경
                    sql = string.Empty;
                    sql += "UPDATE [Project] ";
                    sql += "   SET [ProjectName] = '" + txtProjectName.Text + "' ";
                    sql += "      ,[CompKey] = " + lueCompany.EditValue + " ";
                    sql += "      ,[Descr] = '" + txtDescr.Text + "' ";
                    sql += " WHERE [ProjectKey] = " + ChoiceProjectKey;
                    break;
                case 3: //삭제
                    sql = string.Empty;
                    sql += "UPDATE [Project] ";
                    sql += "   SET [isDeleted] = 1 ";
                    sql += "      ,[CreateDtm] = GETDATE() ";
                    sql += " WHERE [ProjectKey] = " + ChoiceProjectKey;
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
                XtraMessageBox.Show(LangResx.Main.msg_CRUDError + "\r\n" + result, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
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