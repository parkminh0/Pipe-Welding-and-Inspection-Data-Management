using DevExpress.DataAccess.Excel;
using DevExpress.Spreadsheet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWSUploader
{
    public static class ExcelImport
    {
        public static bool isOK;
        public static string ResultMessage;
        public static DataTable excelTable;

        public static DataTable ImportExcel(string filePath)
        {
            isOK = true;
            ResultMessage = string.Empty;

            try
            {
                excelTable = new DataTable();

                ExcelDataSource excelDataSource = new ExcelDataSource();
                excelDataSource.FileName = filePath;

                // Select a required worksheet.  
                ExcelWorksheetSettings excelWorksheetSettings = new ExcelWorksheetSettings();
                excelWorksheetSettings.WorksheetName = GetWorkSheetNameByIndex(0, filePath);

                // Specify import settings.  
                ExcelSourceOptions excelSourceOptions = new ExcelSourceOptions();
                excelSourceOptions.ImportSettings = excelWorksheetSettings;
                excelSourceOptions.SkipHiddenRows = false;
                excelSourceOptions.SkipHiddenColumns = false;
                excelSourceOptions.UseFirstRowAsHeader = true;
                excelDataSource.SourceOptions = excelSourceOptions;
                excelDataSource.Fill();

                //Convert To DataTable
                excelTable = ToDataTable(excelDataSource);
            }
            catch (Exception ex)
            {
                excelTable = null;
                isOK = false;
                ResultMessage = LangResx.Main.msg_ExcelError + "\r\n" + ex.Message;
            }
            return excelTable;
        }

        private static string GetWorkSheetNameByIndex(int p, string filePath)
        {
            Workbook workbook = new Workbook();
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                workbook.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.OpenXml);
            }
            WorksheetCollection worksheets = workbook.Worksheets;
            return worksheets[p].Name;
        }

        public static DataTable ToDataTable(this ExcelDataSource excelDataSource)
        {
            IList list = ((IListSource)excelDataSource).GetList();
            DevExpress.DataAccess.Native.Excel.DataView dataView = (DevExpress.DataAccess.Native.Excel.DataView)list;
            List<PropertyDescriptor> props = dataView.Columns.ToList<PropertyDescriptor>();
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (DevExpress.DataAccess.Native.Excel.ViewRow item in list)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

    }

    //public static class ExcelDataSourceExtension
    //{

    //}
}
