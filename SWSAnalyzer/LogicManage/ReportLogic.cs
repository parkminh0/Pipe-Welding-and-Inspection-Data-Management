using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWSAnalyzer
{
    public class ReportLogic
    {
        private XtraReport _MainReport;
        private DetailBand _DetailBand;

        /// <summary>
        /// 레포트 전체 판
        /// </summary>
        public XtraReport MainReport
        {
            get
            {
                return _MainReport;
            }

            //set
            //{
            //    _MainReport = value;
            //}
        }

        public DetailBand DetailBand
        {
            get
            {
                return _DetailBand;
            }

            //set
            //{
            //    _DetailBand = value;
            //}
        }

        /// <summary>
        /// 빈 레포트 생성(기본 Landscape false)
        /// </summary>
        /// <param name="mainReport">레포트 원판</param>
        /// <param name="detailBand">레포트 원판의 기본 밴드</param>
        /// <param name="aSetName">레포트명</param>
        /// <param name="bottomMargin">하단 여백</param>
        /// <param name="leftMargin">좌측 여백</param>
        /// <param name="rightMargin">우측 여백</param>
        /// <param name="topMargin">상단 여백</param>
        /// <param name="landscape">가로여부</param>
        public void makeReportCommon(string aSetName, int bottomMargin = 0, int leftMargin = 0, int rightMargin = 0, int topMargin = 0, bool landscape = false)
        {
            _MainReport = new XtraReport();
            _MainReport.Name = aSetName;
            _MainReport.Name += "_" + DateTime.Now.ToString("yyyyMMdd");

            _MainReport.Landscape = landscape;
            _MainReport.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
            _MainReport.Margins.Left = leftMargin;
            _MainReport.Margins.Right = rightMargin;
            _MainReport.Margins.Top = topMargin;
            _MainReport.Margins.Bottom = bottomMargin;

            _DetailBand = new DetailBand();
            _MainReport.Bands.Add(_DetailBand);
        }

        /// <summary>
        /// Subreport를 이용하여 합침
        /// </summary>
        /// <param name="mainReport">레포트 원판</param>
        /// <param name="detailReport">추가할 레포트</param>
        /// <param name="pagebreak">페이지 넘김 여부</param>
        public void CreateCompositeReport(XtraReport detailReport, bool pagebreak)
        {
            XRSubreport subreport = new XRSubreport();  //report Create

            DetailReportBand drb = new DetailReportBand();
            _MainReport.Bands.Add(drb);
            DetailBand dtb = new DetailBand();
            dtb.HeightF = 0; // DetailBand의 하단 여백으로 생각할 수 있다.
            drb.Bands.Add(dtb);
            //subreport.Location = new Point(1, 1);

            subreport.ReportSource = detailReport;

            // Add the subreport to the detail band.
            dtb.Controls.Add(subreport);
            if (pagebreak == true)
                dtb.PageBreak = PageBreak.AfterBand;
        }
    }
}
