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
    public partial class FormPopItem : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 편집구분 --> 1:신규 2:수정 3:삭제
        /// </summary>
        public int EditMode;
        public int ChoiceCompKey;
        public int ChoiceKey;

        public FormPopItem()
        {
            InitializeComponent();
        }

        private void FormPopItem_Load(object sender, EventArgs e)
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
                lueCompany.EditValue = ChoiceCompKey;
            }
            catch (Exception)
            {
            }
        }

        private void FormPopItem_Shown(object sender, EventArgs e)
        {
            switch (EditMode)
            {
                case 1:
                    this.Text = "검사항목명 신규";
                    btnApply.Text = "신    규";
                    txtColName.Focus();
                    break;
                case 2:
                    this.Text = "검사항목명 수정";
                    btnApply.Text = "수    정";
                    txtDescr.Focus();
                    GetInfo();
                    break;
                case 3:
                    this.Text = "검사항목명 삭제";
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
            string sql = string.Format("select * from ItemMaster where ItemKey = {0} ", ChoiceKey);
            DataTable dtData;
            try
            {
                dtData = DBManager.Instance.GetDataTable(sql);
                lueCompany.EditValue = ChoiceCompKey;
                txtItemKey.Text = ChoiceKey.ToString();
                txtColName.Text = dtData.Rows[0]["ColName"].ToString();
                chkIsNumeric.Checked = bool.Parse(dtData.Rows[0]["isNumeric"].ToString());
                spnDecPoint.Value = int.Parse(dtData.Rows[0]["DecPoint"].ToString());
                chkIsMatCode.Checked = bool.Parse(dtData.Rows[0]["isMatCode"].ToString());
                spnSeqNo.Value = int.Parse(dtData.Rows[0]["SeqNo"].ToString());
                txtDescr.Text = dtData.Rows[0]["Descr"].ToString();
            }
            catch (Exception)
            {
                dtData = null;
            }
        }

        /// <summary>
        /// 닫기
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
            if (EditMode > 1 && string.IsNullOrWhiteSpace(txtColName.Text))
            {
                XtraMessageBox.Show("검사대상 항목명을 반드시 입력해 주세요.", "컬럼명", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    ChoiceKey = LogicManager.Common.GetNextKey("ItemKey");
                    txtItemKey.Text = ChoiceKey.ToString();
                    sql = string.Empty;
                    sql += "INSERT INTO [ItemMaster] ";
                    sql += "		([ItemKey] ";
                    sql += "		,[CompKey] ";
                    sql += "		,[ColName] ";
                    sql += "		,[isNumeric] ";
                    sql += "		,[DecPoint] ";
                    sql += "		,[isMatCode] ";
                    sql += "		,[SeqNo] ";
                    sql += "		,[Descr] ";
                    sql += "		,[isDeleted] ";
                    sql += "		,[CreateID] ";
                    sql += "		,[CreateDtm]) ";
                    sql += "	VALUES ";
                    sql += "		(" + ChoiceKey + " ";
                    sql += "		," + ChoiceCompKey + " ";
                    sql += "		,'" + txtColName.Text + "' ";
                    sql += "		," + (chkIsNumeric.Checked ? 1 : 0) + " ";
                    sql += "		," + spnDecPoint.Value + " ";
                    sql += "		," + (chkIsMatCode.Checked ? 1 : 0) + " ";
                    sql += "		," + spnSeqNo.Value + " ";
                    sql += "		,'" + txtDescr.Text + "' ";
                    sql += "		,0 ";
                    sql += "		,'sys' ";
                    sql += "		,CURRENT_DATE)";
                    break;
                case 2: //변경
                    sql = string.Empty;
                    sql += "UPDATE [ItemMaster] ";
                    sql += "SET    [ColName] = '" + txtColName.Text + "' , ";
                    sql += "       [isNumeric] = " + (chkIsNumeric.Checked ? 1 : 0) + ", ";
                    sql += "       [DecPoint] = " + spnDecPoint.Value + ", ";
                    sql += "       [isMatCode] = " + (chkIsMatCode.Checked ? 1 : 0) + ", ";
                    sql += "       [SeqNo] = " + spnSeqNo.Value + ", ";
                    sql += "       [Descr] = '" + txtDescr.Text + "', ";
                    sql += "       [CreateDtm] = CURRENT_DATE ";
                    sql += "WHERE  [ItemKey] = " + ChoiceKey; ;
                    break;
                case 3: //삭제
                    sql = string.Empty;
                    sql += "UPDATE [ItemMaster] ";
                    sql += "   SET [isDeleted] = 1 ";
                    sql += "      ,[CreateDtm] = CURRENT_DATE ";
                    sql += " WHERE [ItemKey] = " + ChoiceKey;
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