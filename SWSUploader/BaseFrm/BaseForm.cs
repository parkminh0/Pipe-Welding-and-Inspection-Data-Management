using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;

namespace SWSUploader
{
    public partial class BaseForm : XtraForm
    {
        //public BaseForm()
        //{
        //    InitializeComponent();
        //}
        public OptionInfo OCF
        {
            get
            {
                return Program.Option;
            }
        }

        DBManager dbm;
        public DBManager DBM
        {
            get
            {
                if (dbm == null)
                {
                    dbm = DBManager.Instance;
                }
                return dbm;
            }
        }
       
        public WarningMSG WMSG
        {
            get
            {
                return Program.WMSG;
            }
        }

        private void BaseForm_Load(object sender, EventArgs e)
        {

        }

        static object lastEdit;
        /// <summary>
        /// 첫 MouseUp 이벤트 발생 히 자동 전체선택 활성화
        /// </summary>
        /// <param name="edit">설정할 컨트롤</param>
        public void EnableAutoSelectAllOnFirstMouseUp(TextEdit edit)
        {
            edit.MaskBox.MouseUp += MaskBox_MouseUp;
            edit.MaskBox.Enter += MaskBox_Enter;
        }

        /// <summary>
        /// 마스크박스 진입 시 컨트롤 저장
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MaskBox_Enter(object sender, EventArgs e)
        {
            lastEdit = sender;
        }

        /// <summary>
        /// 마스크박스 MouseUp 시 진입했을 경우 전체선택 후 저장 컨트롤 삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MaskBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (lastEdit == sender)
                (sender as TextBox).SelectAll();
            lastEdit = null;
        }
        
        /// <summary>
        /// 상속폼의 새로고침 기능 상속용 메소드
        /// </summary>
        public virtual void DoRefresh()
        {

        }        

        /// <summary>
        /// 빈 레포트 생성(기본 Landscape false)
        /// </summary>
        /// <param name="mainReport">레포트 원판</param>
        /// <param name="detailBand">레포트 원판의 기본 밴드</param>
        /// <param name="aSetName">레포트명</param>
        /// <param name="leftMargin">좌측 여백</param>
        /// <param name="rightMargin">우측 여백</param>
        /// <param name="topMargin">상단 여백</param>
        /// <param name="bottomMargin">하단 여백</param>
        //protected void makeReportCommon(ref XtraReport mainReport, DetailBand detailBand, string aSetName, int leftMargin = 0, int rightMargin = 0, int topMargin = 0, int bottomMargin = 0)
        //{
        //    mainReport = new XtraReport();
        //    mainReport.Name = aSetName;
        //    mainReport.Name += "_" + DateTime.Now.ToString("yyyyMMdd");

        //    mainReport.Landscape = false;
        //    mainReport.PaperKind = System.Drawing.Printing.PaperKind.A4;
        //    mainReport.Margins.Left = leftMargin;
        //    mainReport.Margins.Right = rightMargin;
        //    mainReport.Margins.Top = topMargin;
        //    mainReport.Margins.Bottom = bottomMargin;

        //    detailBand = new DetailBand();
        //    mainReport.Bands.Add(detailBand);
        //}

        /// <summary>
        /// Subreport를 이용하여 합침
        /// </summary>
        /// <param name="mainReport">레포트 원판</param>
        /// <param name="detailReport">추가할 레포트</param>
        /// <param name="pagebreak">페이지 넘김 여부</param>
        //protected void CreateCompositeReport(XtraReport mainReport, XtraReport detailReport, bool pagebreak)
        //{
        //    XRSubreport subreport = new XRSubreport();  //report Create

        //    DetailReportBand drb = new DetailReportBand();
        //    mainReport.Bands.Add(drb);
        //    DetailBand dtb = new DetailBand();
        //    dtb.HeightF = 0; // DetailBand의 하단 여백으로 생각할 수 있다.
        //    drb.Bands.Add(dtb);
        //    //subreport.Location = new Point(1, 1);

        //    subreport.ReportSource = detailReport;

        //    // Add the subreport to the detail band.
        //    dtb.Controls.Add(subreport);
        //    if (pagebreak == true)
        //        dtb.PageBreak = PageBreak.AfterBand;
        //}
    }
}