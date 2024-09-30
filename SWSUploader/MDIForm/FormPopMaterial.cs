using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace SWSUploader
{
    public partial class FormPopMaterial : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 편집구분 --> 1:신규 2:수정 3:삭제
        /// </summary>
        public int EditMode;
        public int ChoiceCompKey;
        public int ChoiceKey;

        public FormPopMaterial()
        {
            InitializeComponent();
        }
        private void FormPopMaterial_Load(object sender, EventArgs e)
        {
            SetCombo();
        }
        private void SetCombo()
        {
            string sql = "select CompKey KeyNo, CompName 거래처명 from Company where isDeleted = 0 ";
            try
            {
                DataTable dtCompList = DBManager.Instance.GetDataTable(sql);
                lueCompany.Properties.DataSource = dtCompList;
                lueCompany.Properties.ValueMember = "KeyNo";
                lueCompany.Properties.DisplayMember = "거래처명";
            }
            catch (Exception)
            {
            }
        }

        private void FormPopMaterial_Shown(object sender, EventArgs e)
        {
            switch (EditMode)
            {
                case 1:
                    this.Text = "자재코드 신규";
                    btnApply.Text = "신    규";
                    txtMatCode.Focus();
                    break;
                case 2:
                    this.Text = "자재코드 수정";
                    btnApply.Text = "수    정";
                    txtDescr.Focus();
                    GetInfo();
                    break;
                case 3:
                    this.Text = "자재코드 삭제";
                    btnApply.Text = "삭    제";
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
            string sql = string.Format("select * from Material where MatKey = {0} ", ChoiceKey);
            DataTable dtData;
            try
            {
                dtData = DBManager.Instance.GetDataTable(sql);
                lueCompany.EditValue = ChoiceCompKey;
                txtMatKey.Text = ChoiceKey.ToString();
                txtMatCode.Text = dtData.Rows[0]["MatName"].ToString();
                txtDescr.Text = dtData.Rows[0]["Descr"].ToString();
            }
            catch (Exception)
            {
                dtData = null;
            }
        }

        /// <summary>
        /// 나가기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// 적용
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, EventArgs e)
        {
            if (EditMode > 1 && string.IsNullOrWhiteSpace(txtMatCode.Text))
            {
                XtraMessageBox.Show("자재코드를 반드시 입력해 주세요.", "자재코드", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //string msg = string.Empty;
            //switch (EditMode)
            //{
            //    case 1: //신규
            //        msg = "입력하신 내용으로 등록하시겠습니까?";
            //        break;
            //    case 2: //변경
            //        msg = "입력하신 내용으로 수정하시겠습니까?";
            //        break;
            //    case 3: //삭제
            //        msg = "선택하신 내용을 삭제하시겠습니까?";
            //        break;
            //    default:
            //        msg = string.Empty;
            //        return;
            //}

            //if (XtraMessageBox.Show(msg, "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
            //    return;

            List<string> sqls = new List<string>();
            string sql = string.Empty;

            switch (EditMode)
            {
                case 1: //신규
                    ChoiceKey = LogicManager.Common.GetNextKey("MatKey");
                    txtMatKey.Text = ChoiceKey.ToString();
                    sql = string.Empty;
                    sql += "INSERT INTO [Material] ";
                    sql += "		([MatKey] ";
                    sql += "		,[CompKey] ";
                    sql += "		,[MatCode] ";
                    sql += "		,[MatName] ";
                    sql += "		,[Descr] ";
                    sql += "		,[isDeleted] ";
                    sql += "		,[CreateID] ";
                    sql += "		,[CreateDtm]) ";
                    sql += "	VALUES ";
                    sql += "		(" + ChoiceKey + " ";
                    sql += "		," + ChoiceCompKey + " ";
                    sql += "		,'" + txtMatCode.Text + "' ";
                    sql += "		,'" + txtMatCode.Text + "' ";
                    sql += "		,'" + txtDescr.Text + "' ";
                    sql += "		,0 ";
                    sql += "		,'sys' ";
                    sql += "		,CURRENT_DATE)";
                    break;
                case 2: //변경
                    sql = string.Empty;
                    sql += "UPDATE [Material] ";
                    sql += "SET [MatCode] = '" + txtMatCode.Text + "' ";
                    sql += ",[MatName] = '" + txtMatCode.Text + "' ";
                    sql += ",[Descr] = '" + txtDescr.Text + "' ";
                    sql += ",[CreateDtm] = CURRENT_DATE ";
                    sql += "WHERE [MatKey] = " + ChoiceKey;
                    break;
                case 3: //삭제
                    sql = string.Empty;
                    sql += "UPDATE [Material] ";
                    sql += "   SET [isDeleted] = 1 ";
                    sql += "      ,[CreateDtm] = CURRENT_DATE ";
                    sql += " WHERE [MatKey] = " + ChoiceKey;
                    break;
                default:
                    break;
            }

            sqls.Add(sql);
            if (DBManager.Instance.ExcuteTransaction(sqls) == "")
            {
                //XtraMessageBox.Show("정상적으로 적용되었습니다.", "성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                XtraMessageBox.Show("오류가 발생하여 DB에 반영하지 못했습니다.", "실패", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }
    }
}