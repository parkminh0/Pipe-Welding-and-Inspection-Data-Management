using DevExpress.XtraGrid.Helpers;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWSAnalyzer
{
    public static class CellDrawHelper
    {
        public static void DrawCellBorderBlack(DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            Brush brush = Brushes.Black;

            e.Cache.FillRectangle(brush, new Rectangle(e.Bounds.Right - 1, e.Bounds.Y - 1, 2, e.Bounds.Height + 2));  // 오른쪽

            //e.Cache.FillRectangle(brush, new Rectangle(e.Bounds.X - 1, e.Bounds.Y - 1, e.Bounds.Width + 2, 2));

            //e.Cache.FillRectangle(brush, new Rectangle(e.Bounds.X - 1, e.Bounds.Bottom - 1, e.Bounds.Width + 2, 2));

            //e.Cache.FillRectangle(brush, new Rectangle(e.Bounds.X - 1, e.Bounds.Y - 1, 2, e.Bounds.Height + 2));

        }

		public static void DrawCellBorderWhite(DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
		{
			Brush brush = Brushes.White;
			
            e.Cache.FillRectangle(brush, new Rectangle(e.Bounds.Right - 1, e.Bounds.Y - 1, 2, e.Bounds.Height + 2));  // 오른쪽

			//e.Cache.FillRectangle(brush, new Rectangle(e.Bounds.X - 1, e.Bounds.Y - 1, e.Bounds.Width + 2, 2));

			//e.Cache.FillRectangle(brush, new Rectangle(e.Bounds.X - 1, e.Bounds.Bottom - 1, e.Bounds.Width + 2, 2));

			//e.Cache.FillRectangle(brush, new Rectangle(e.Bounds.X - 1, e.Bounds.Y - 1, 2, e.Bounds.Height + 2));
		}


		public static void DoDefaultDrawCell(GridView view, RowCellCustomDrawEventArgs e)

        {
            e.Appearance.FillRectangle(e.Cache, e.Bounds);

            ((IViewController)view.GridControl).EditorHelper.DrawCellEdit(new GridViewDrawArgs(e.Cache, (view.GetViewInfo() as GridViewInfo), e.Bounds), (e.Cell as GridCellInfo).Editor, (e.Cell as GridCellInfo).ViewInfo, e.Appearance, (e.Cell as GridCellInfo).CellValueRect.Location);
        }
    }
}
