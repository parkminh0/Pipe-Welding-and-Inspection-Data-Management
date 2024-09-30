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
using DevExpress.Utils.Design;

namespace SWSAnalyzer
{
    public partial class FormPopCompany : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 편집구분 --> 1:신규 2:수정 3:삭제
        /// </summary>
        public int EditMode;
        public int ChoiceKey;
        private string ChoiceCompName;

        #region 폼 로드
        /// <summary>
        /// 
        /// </summary>
        public FormPopCompany()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 폼 쇼운
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormPopCompany_Shown(object sender, EventArgs e)
        {
            switch (EditMode)
            {
                case 1:
                    this.Text = LangResx.Main.title_CompNew;
                    btnApply.Text = LangResx.Main.btnNew;
                    txtCompName.Focus();
                    break;
                case 2:
                    this.Text = LangResx.Main.title_CompMod;
                    btnApply.Text = LangResx.Main.btnModify;
                    txtCompName.Focus();
                    GetInfo();
                    break;
                case 3:
                    this.Text = LangResx.Main.title_CompDel;
                    btnApply.Text = LangResx.Main.btnDelete;
                    txtCompName.Enabled = txtDescr.Enabled = false;
                    btnApply.Focus();
                    GetInfo();
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
            string sql = string.Format("select CompKey, CompName, Descr from Company where CompKey = {0} ", ChoiceKey);
            DataTable dtData;
            try
            {
                dtData = DBManager.Instance.GetDataTable(sql);
                txtCompKey.Text = dtData.Rows[0]["CompKey"].ToString();
                txtCompName.Text = dtData.Rows[0]["CompName"].ToString();
                ChoiceCompName = dtData.Rows[0]["CompName"].ToString();
                txtDescr.Text = dtData.Rows[0]["Descr"].ToString();
            }
            catch (Exception)
            {
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
            if (string.IsNullOrWhiteSpace(txtCompName.Text))
            {
                XtraMessageBox.Show(LangResx.Main.msg_CompNull, LangResx.Main.msg_title_CompNull, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string sql = string.Empty;
            if (EditMode <= 2)
            {
                switch (EditMode)
                {
                    case 1:
                        sql += "select COUNT(*) from Company ";
                        sql += $"where CompName = '{txtCompName.Text}' AND isDeleted = 0 ";
                        break;
                    case 2:
                        sql += "select COUNT(*) from Company ";
                        sql += $"where '{ChoiceCompName}' != '{txtCompName.Text}' AND CompName = '{txtCompName.Text}' and isDeleted = 0 ";
                        break;
                }
                if (DBManager.Instance.GetIntScalar(sql) > 0)
                {
                    XtraMessageBox.Show(LangResx.Main.msg_CompExist, LangResx.Main.msg_title_Exist, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                if (XtraMessageBox.Show(LangResx.Main.msg_Del, LangResx.Main.msg_title_Del, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }
                
            List<string> sqls = new List<string>();
            switch (EditMode)
            {
                case 1: //신규
                    ChoiceKey = LogicManager.Common.GetNextKey("CompKey");
                    txtCompKey.Text = ChoiceKey.ToString();
                    sql = string.Empty;
                    sql += "INSERT INTO [Company] ";
                    sql += "		([CompKey] ";
                    sql += "		,[CompName] ";
                    sql += "		,[Identify] ";
                    sql += "		,[Descr] ";
                    sql += "		,[isDeleted] ";
                    sql += "		,[CreateID] ";
                    sql += "		,[CreateDtm]) ";
                    sql += "	VALUES ";
                    sql += "		(" + ChoiceKey + " ";
                    sql += "		,'" + txtCompName.Text + "' ";
                    sql += "		,'P/260Ekktntyo2plNyVwFg==' "; //LocalClient
                    sql += "		,'" + txtDescr.Text + "' ";
                    sql += "		,0 ";
                    sql += "		,'" + Program.Option.LoginID + "' ";
                    sql += "		,GETDATE()) ";
                    break;
                case 2: //변경
                    sql = string.Empty;
                    sql += "UPDATE [Company] ";
                    sql += "   SET [CompName] = '" + txtCompName.Text + "' ";
                    sql += "      ,[Descr] = '" + txtDescr.Text + "' ";
                    sql += " WHERE [CompKey] = " + ChoiceKey;
                    break;
                case 3: //삭제
                    sql = string.Empty;
                    sql += "UPDATE [Company] ";
                    sql += "   SET [isDeleted] = 1 ";
                    sql += "      ,[CreateDtm] = GETDATE() ";
                    sql += " WHERE [CompKey] = " + ChoiceKey;
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
        /// 나가기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
        #endregion
    }
}