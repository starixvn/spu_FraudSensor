namespace Fanex.NAP.FS.Library.Core
{
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public abstract class FSExportExcel
    {
        #region Constructor
        protected FSExportExcel(
            string filePath,
            string sheetName,
            IEnumerable<Legend> legends,
            IEnumerable<string> headers)
        {
            FilePath = filePath;
            RowIndex = 0;
            SheetName = string.IsNullOrEmpty(sheetName) ? "Sheet1" : sheetName;
            ReportInfo = legends;
            Headers = headers;
            Workbook = new HSSFWorkbook();
            Sheet = Workbook.CreateSheet(SheetName);
            DefaultFont = CreateDefaultFont();
            DefaultBoldFont = CreateDefaultFont((short)FontBoldWeight.Bold);
            DefaultCellStyleAlignLeft = CreateDefaultCellStyle();
            DefaultCellStyleAlignCenter = CreateDefaultCellStyle(HorizontalAlignment.Center);
            DefaultCellStyleAlignRight = CreateDefaultCellStyle(HorizontalAlignment.Right);
        }
        #endregion

        #region Attributes
        public string FilePath { get; set; }

        public string FileName { get; set; }

        public string SheetName { get; set; }

        public IEnumerable<Legend> ReportInfo { get; set; }

        public IEnumerable<string> Headers { get; set; }

        public HSSFWorkbook Workbook { get; set; }

        public ISheet Sheet { get; set; }

        public IFont DefaultFont { get; set; }

        public IFont DefaultBoldFont { get; set; }

        public ICellStyle DefaultCellStyleAlignLeft { get; set; }

        public ICellStyle DefaultCellStyleAlignCenter { get; set; }

        public ICellStyle DefaultCellStyleAlignRight { get; set; }

        public int RowIndex { get; set; }
        #endregion

        #region Major functions
        protected virtual void GenerateReportInfo()
        {
            Sheet.CreateRow(RowIndex++);
            foreach (var info in ReportInfo)
            {
                var row = Sheet.CreateRow(RowIndex);
                SetUpACell(row, 1, info.Key, ReportInfoFont(), DefaultCellStyleAlignLeft);
                SetUpACell(row, 2, info.Value, ReportInfoFont(), ReportInfoCellStyle());
                RowIndex++;
            }

            Sheet.CreateRow(RowIndex++);
        }

        protected virtual void GenerateReportLegend()
        {
            // DO NOTHING BY DEFAULT
        }

        protected virtual void GenerateReportHeader()
        {
            var row = Sheet.CreateRow(RowIndex);
            var cellCounter = 0;
            foreach (var header in Headers)
            {
                SetUpACell(row, cellCounter, header, ReportHeaderFont(), ReportHeaderCellStyle());
                cellCounter++;
            }

            RowIndex++;
        }

        protected abstract void GenerateReportContent();

        protected abstract void SetColumnsWidth();

        protected virtual string WriteFile()
        {
            var saveAsFileName = FilePath + "/" + SheetName + "_" + DateTime.Today.ToString("dMMMyyyy") + ".xls";
            using (var fileData = new FileStream(saveAsFileName, FileMode.Create))
            {
                Workbook.Write(fileData);
            }

            return SheetName + "_" + DateTime.Today.ToString("dMMMyyyy") + ".xls";
        }

        public string ExportFile()
        {
            GenerateReportInfo();
            GenerateReportLegend();
            GenerateReportHeader();
            GenerateReportContent();
            SetColumnsWidth();
            AutoSizeColumnsWidth();

            return WriteFile();
        }

        public string GenerateExcelFile()
        {
            GenerateReportInfo();
            GenerateReportLegend();
            GenerateReportHeader();
            GenerateReportContent();
            SetColumnsWidth();
            AutoSizeColumnsWidth();

            return SheetName + "_" + DateTime.Today.ToString("dMMMyyyy") + ".xls";
        }

        #endregion

        #region Utilities

        protected void SetUpACell(
            IRow currentRow,
            int cellIndex,
            string cellValue,
            IFont cellFont,
            ICellStyle cellStyle)
        {
            var cell = currentRow.CreateCell(cellIndex);
            cell.SetCellValue(cellValue);
            cellStyle.SetFont(cellFont);
            cell.CellStyle = cellStyle;
        }

        private IFont CreateDefaultFont(short isBolded = (short)FontBoldWeight.None)
        {
            var defaultFont = Workbook.CreateFont();
            defaultFont.FontHeightInPoints = 9;
            defaultFont.FontName = "Tahoma";
            defaultFont.Boldweight = isBolded;
            defaultFont.Color = IndexedColors.Black.Index;

            return defaultFont;
        }

        private ICellStyle CreateDefaultCellStyle(HorizontalAlignment alignment = HorizontalAlignment.Left)
        {
            var defaultCellStyle = Workbook.CreateCellStyle();
            defaultCellStyle.BorderLeft = BorderStyle.Thin;
            defaultCellStyle.BorderTop = BorderStyle.Thin;
            defaultCellStyle.BorderRight = BorderStyle.Thin;
            defaultCellStyle.BorderBottom = BorderStyle.Thin;
            defaultCellStyle.WrapText = false;
            defaultCellStyle.FillForegroundColor = IndexedColors.White.Index;
            defaultCellStyle.FillPattern = FillPattern.SolidForeground;
            defaultCellStyle.Alignment = alignment;

            return defaultCellStyle;
        }

        protected virtual void RefineDataBeforeBinding()
        {
            // DO NOTHING BY DEFAULT
        }

        protected virtual void AutoSizeColumnsWidth()
        {
            // DO NOTHING BY DEFAULT
        }

        #endregion

        #region Fonts
        protected virtual IFont ReportInfoFont()
        {
            return DefaultFont;
        }

        protected virtual IFont ReportLegendFont()
        {
            return DefaultFont;
        }

        protected virtual IFont ReportHeaderFont()
        {
            var font = Workbook.CreateFont();
            font.FontHeightInPoints = 10;
            font.FontName = "Tahoma";
            font.Boldweight = (short)FontBoldWeight.Bold;
            font.Color = IndexedColors.White.Index;

            return font;
        }

        protected virtual IFont ReportContentFont()
        {
            return DefaultFont;
        }

        protected virtual IFont ReportFooterFont()
        {
            return DefaultFont;
        }

        #endregion

        #region Cell-Styles

        protected virtual ICellStyle ReportInfoCellStyle()
        {
            return DefaultCellStyleAlignLeft;
        }

        protected virtual ICellStyle ReportLegendCellStyle()
        {
            return DefaultCellStyleAlignLeft;
        }

        protected virtual ICellStyle ReportHeaderCellStyle()
        {
            var cellStyle = Workbook.CreateCellStyle();
            cellStyle = DecorateCellAlignment(cellStyle, HorizontalAlignment.Center);
            cellStyle = DecorateCellWrapText(cellStyle, true);
            cellStyle = DecorateCellBackground(cellStyle, IndexedColors.RoyalBlue.Index);

            return cellStyle;
        }

        protected virtual ICellStyle ReportContentCellStyle(HorizontalAlignment alignment = HorizontalAlignment.Left)
        {
            ICellStyle cellStyle;
            switch (alignment)
            {
                case HorizontalAlignment.Right:
                    cellStyle = DefaultCellStyleAlignRight;
                    break;
                case HorizontalAlignment.Center:
                    cellStyle = DefaultCellStyleAlignCenter;
                    break;
                default:
                    cellStyle = DefaultCellStyleAlignLeft;
                    break;
            }

            return cellStyle;
        }

        protected virtual ICellStyle ReportFooterCellStyle()
        {
            return DefaultCellStyleAlignLeft;
        }

        #endregion

        #region Cell-Styles Decorator

        protected ICellStyle DecorateCellBorder(
            ICellStyle cellStyle,
            bool top = true,
            bool left = true,
            bool bottom = true,
            bool right = true,
            BorderStyle borderStyle = BorderStyle.Thin)
        {
            var newCellStyle = cellStyle;
            newCellStyle.BorderTop = top ? borderStyle : BorderStyle.None;
            newCellStyle.BorderLeft = left ? borderStyle : BorderStyle.None;
            newCellStyle.BorderBottom = bottom ? borderStyle : BorderStyle.None;
            newCellStyle.BorderRight = right ? borderStyle : BorderStyle.None;

            return newCellStyle;
        }

        protected ICellStyle DecorateCellAlignment(ICellStyle cellStyle, HorizontalAlignment alignment, VerticalAlignment valignment = VerticalAlignment.Center)
        {
            var newCellStyle = cellStyle;
            newCellStyle.Alignment = alignment;
            newCellStyle.VerticalAlignment = valignment;

            return newCellStyle;
        }

        protected ICellStyle DecorateCellBackground(ICellStyle cellStyle, short backgroundColor, FillPattern fillPattern = FillPattern.SolidForeground)
        {
            var newCellStyle = cellStyle;
            newCellStyle.FillForegroundColor = backgroundColor;
            newCellStyle.FillPattern = fillPattern;

            return newCellStyle;
        }

        protected ICellStyle DecorateCellWrapText(ICellStyle cellStyle, bool decision)
        {
            var newCellStyle = cellStyle;
            newCellStyle.WrapText = decision;

            return newCellStyle;
        }

        #endregion
    }
}
