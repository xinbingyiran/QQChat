using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using System.Data;

namespace ExcelOp
{
    public class ExcelFile
    {
        private IWorkbook _workbook;
        private ISheet _currentSheet;
        private string _filename;

        public ExcelFile()
        {
            _workbook = new HSSFWorkbook();
            _currentSheet = _workbook.CreateSheet("Sheet1");
        }

        public ExcelFile(string filename)
        {
            using (FileStream file = new FileStream(filename, FileMode.Open))
            {
                _workbook = new HSSFWorkbook(file);
                _currentSheet = null;
                if (_workbook.NumberOfSheets > 0 && _workbook.ActiveSheetIndex >= 0)
                {
                    _currentSheet = _workbook.GetSheetAt(_workbook.ActiveSheetIndex);
                }
                _filename = filename;
            }
        }

        public static ExcelFile LoadFromFile(string filename)
        {
            return new ExcelFile(filename);
        }

        public bool SaveAs(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.CreateNew))
            {
                _workbook.Write(fs);
            }
            _filename = filename;
            return true;
        }

        public bool Save()
        {
            if (_filename == null)
                throw new ArgumentNullException("文件名为空");
            using (FileStream fs = new FileStream(_filename, FileMode.CreateNew))
            {
                _workbook.Write(fs);
            }
            return true;
        }

        public bool SaveTo(Stream stream)
        {
            try
            {
                _workbook.Write(stream);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SetCurrentSheet(int sheetIndex)
        {
            _currentSheet = _workbook.GetSheetAt(sheetIndex);
            _workbook.SetActiveSheet(sheetIndex);
            return _currentSheet != null;
        }

        public string[] GetRow(int rowindex)
        {
            if (_currentSheet == null)
            {
                return null;
            }
            IRow row = _currentSheet.GetRow(rowindex);
            if (row == null)
            {
                return null;
            }
            int count = row.LastCellNum + 1;
            string[] item = new string[count];
            for (int i = 0; i < count; i++)
            {
                item[i] = row.GetCell(i).StringCellValue;
            }
            return item;
        }

        public List<string[]> GetRows()
        {
            if (_currentSheet == null)
            {
                return null;
            }
            int count = _currentSheet.LastRowNum + 1;
            List<string[]> rows = new List<string[]>(count);
            for (int i = 0; i < count; i++)
            {
                IRow row = _currentSheet.GetRow(i);
                if (row == null)
                {
                    rows.Add(null);
                }
                int ccount = row.LastCellNum + 1;
                string[] item = new string[ccount];
                for (int j = 0; j < ccount; j++)
                {
                    item[i] = row.GetCell(j).StringCellValue;
                }
            }
            return rows;
        }

        public bool SetRow(int rowIndex, string[] columns)
        {
            if (_currentSheet == null)
            {
                return false;
            }
            if (rowIndex < 0)
            {
                return false;
            }
            IRow row = _currentSheet.GetRow(rowIndex);
            if (row == null)
            {
                row = _currentSheet.CreateRow(rowIndex);
            }
            int count = columns.Length;
            for (int i = 0; i < count; i++)
            {
                ICell cell = row.GetCell(i);
                if (cell == null)
                {
                    cell = row.CreateCell(i);
                }
                cell.SetCellType(CellType.STRING);
                cell.SetCellValue(columns[i]);
            }
            return true;
        }

        public bool InsertRow(int rowIndex, string[] columns)
        {
            if (_currentSheet == null)
            {
                return false;
            }
            if (rowIndex < 0)
            {
                return false;
            }
            if (rowIndex <= _currentSheet.LastRowNum)
            {
                _currentSheet.ShiftRows(rowIndex, _currentSheet.LastRowNum, 1);
            }
            return SetRow(rowIndex, columns);
        }

        public bool AppendRow(string[] columns)
        {
            if (_currentSheet == null)
            {
                return false;
            }
            return SetRow(_currentSheet.LastRowNum + 1, columns);
        }

        public bool DeleteRow(int rowIndex)
        {
            if (_currentSheet == null)
            {
                return false;
            }
            if (rowIndex < 0)
            {
                return false;
            }
            _currentSheet.CreateRow(rowIndex);
            if (rowIndex >= _currentSheet.LastRowNum)
            {
                _currentSheet.ShiftRows(rowIndex, _currentSheet.LastRowNum, -1);
            }
            return true;
        }
    }
}
