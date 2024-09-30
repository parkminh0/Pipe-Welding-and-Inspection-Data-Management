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
    public partial class FormPopUserAuth : DevExpress.XtraEditors.XtraForm
    {
        public int ChoiceCompKey;
        public string ChoiceUserID;
        public int ChoiceAuthLevel;

        #region 폼 로드
        /// <summary>
        /// 
        /// </summary>
        public FormPopUserAuth()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormPopUserAuth_Load(object sender, EventArgs e)
        {
            SetCompanyCode();
            SetAuthLevelCode();
            GetInfo();
        }

        /// <summary>
        /// 거래처 세팅
        /// </summary>
        private void SetCompanyCode()
        {
            string sql = "select c.CompKey No, c.CompName as CodeName from Company c where c.isDeleted = 0 ";
            DataTable dtCompany = DBManager.Instance.GetDataTable(sql);
            lueCompany.Properties.DataSource = dtCompany;
            lueCompany.Properties.ValueMember = "No";
            lueCompany.Properties.DisplayMember = "CodeName";
        }

        /// <summary>
        /// 사용자권한코드 세팅
        /// </summary>
        private void SetAuthLevelCode()
        {
            string sql = string.Empty;
            sql += "select cast(DetailCode as int) AuthLevel, CodeDescr as AuthDescr ";
            sql += "  from CodeInfomation ";
            sql += " where 1 = 1 ";
            sql += "   and Category = 'AuthLevel' ";
            sql += " order by OrderNo";
            DataTable dtAuthLevel = DBManager.Instance.GetDataTable(sql);
            lueAuthLevel.Properties.DataSource = dtAuthLevel;
            lueAuthLevel.Properties.ValueMember = "AuthLevel";
            lueAuthLevel.Properties.DisplayMember = "AuthDescr";
        }
        #endregion

        /// <summary>
        /// 기존 정보 세팅
        /// </summary>
        private void GetInfo()
        {
            string sql = string.Empty;
            sql += $"SELECT * FROM UserAuth WHERE CompKey = {ChoiceCompKey} and UserID = '{ChoiceUserID}' ";
            DataTable dt = DBManager.Instance.GetDataTable(sql);

            lueCompany.EditValue = int.Parse(dt.Rows[0]["Compkey"].ToString());
            lueAuthLevel.EditValue = ChoiceAuthLevel;
            txtUserID.Text = dt.Rows[0]["UserID"].ToString();
            checkEdit1.Checked = bool.Parse(dt.Rows[0]["UploadInspectData"].ToString());
            checkEdit2.Checked = bool.Parse(dt.Rows[0]["UploadWeldData"].ToString());
            checkEdit3.Checked = bool.Parse(dt.Rows[0]["ShowInspectUploadRecord"].ToString());
            checkEdit4.Checked = bool.Parse(dt.Rows[0]["ShowWeldUploadRecord"].ToString());
            checkEdit5.Checked = bool.Parse(dt.Rows[0]["FormBeadMasterManage"].ToString());
            checkEdit6.Checked = bool.Parse(dt.Rows[0]["FormWeldMasterManage"].ToString());
            checkEdit7.Checked = bool.Parse(dt.Rows[0]["FormBeadDetailList"].ToString());
            checkEdit8.Checked = bool.Parse(dt.Rows[0]["FormBeadAndWeldList"].ToString());
            checkEdit9.Checked = bool.Parse(dt.Rows[0]["FormGeneralChart1"].ToString());
            checkEdit10.Checked = bool.Parse(dt.Rows[0]["FormPivotResult"].ToString());
            checkEdit11.Checked = bool.Parse(dt.Rows[0]["FormDashboard"].ToString());
            checkEdit12.Checked = bool.Parse(dt.Rows[0]["FormDashboard2"].ToString());
            checkEdit13.Checked = bool.Parse(dt.Rows[0]["FormDashboard3"].ToString());
            checkEdit14.Checked = bool.Parse(dt.Rows[0]["SaveExcel"].ToString());
            checkEdit15.Checked = bool.Parse(dt.Rows[0]["DeleteInspectData"].ToString());
            checkEdit16.Checked = bool.Parse(dt.Rows[0]["DeleteWeldData"].ToString());
        }

        #region 적용
        /// <summary>
        /// 권한 적용
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, EventArgs e)
        {
            string sql = string.Empty;
            sql += "UPDATE UserAuth ";
            sql += $"   SET UploadInspectData = '{checkEdit1.Checked}', ";
            sql += $"       UploadWeldData = '{checkEdit2.Checked}', ";
            sql += $"       ShowInspectUploadRecord = '{checkEdit3.Checked}', ";
            sql += $"       ShowWeldUploadRecord = '{checkEdit4.Checked}', ";
            sql += $"       FormBeadMasterManage = '{checkEdit5.Checked}', ";
            sql += $"       FormWeldMasterManage = '{checkEdit6.Checked}', ";
            sql += $"       FormBeadDetailList = '{checkEdit7.Checked}', ";
            sql += $"       FormBeadAndWeldList = '{checkEdit8.Checked}', ";
            sql += $"       FormGeneralChart1 = '{checkEdit9.Checked}', ";
            sql += $"       FormPivotResult = '{checkEdit10.Checked}', ";
            sql += $"       FormDashboard = '{checkEdit11.Checked}', ";
            sql += $"       FormDashboard2 = '{checkEdit12.Checked}', ";
            sql += $"       FormDashboard3 = '{checkEdit13.Checked}', ";
            sql += $"       SaveExcel = '{checkEdit14.Checked}', ";
            sql += $"       DeleteInspectData = '{checkEdit15.Checked}', ";
            sql += $"       DeleteWeldData = '{checkEdit16.Checked}' ";
            sql += " WHERE 1 = 1 ";
            sql += $"  AND CompKey = {ChoiceCompKey} ";
            sql += $"  AND UserID = '{ChoiceUserID}' ";

            int result = DBManager.Instance.ExcuteDataUpdate(sql);
            if (result == 1)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                XtraMessageBox.Show(LangResx.Main.msg_CRUDError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            Close();
        }
        #endregion
    }
}