using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExcelOp
{
    public static class ExcelHelper
    {
        /// <summary>   
        /// DataTable 转换为List 集合   
        /// </summary>   
        /// <typeparam name="TResult">类型</typeparam>   
        /// <param name="dt">DataTable</param>   
        /// <returns></returns>   
        public static List<TResult> ToList<TResult>(this DataTable dt) where TResult : class, new()
        {
            //创建一个属性的列表   
            List<PropertyInfo> prlist = new List<PropertyInfo>();
            //获取TResult的类型实例  反射的入口   
            Type t = typeof(TResult);
            //获得TResult 的所有的Public 属性 并找出TResult属性和DataTable的列名称相同的属性(PropertyInfo) 并加入到属性列表    
            Array.ForEach<PropertyInfo>(t.GetProperties(), p => { if (dt.Columns.IndexOf(p.Name) != -1) prlist.Add(p); });
            //创建返回的集合   
            List<TResult> oblist = new List<TResult>();

            foreach (DataRow row in dt.Rows)
            {
                //创建TResult的实例   
                TResult ob = new TResult();
                //找到对应的数据  并赋值   
                prlist.ForEach(p => { if (row[p.Name] != DBNull.Value) p.SetValue(ob, row[p.Name], null); });
                //放入到返回的集合中.   
                oblist.Add(ob);
            }
            return oblist;
        }

        /// <summary>   
        /// 转换为一个DataTable   
        /// </summary>   
        /// <typeparam name="TResult"></typeparam>   
        ///// <param name="value"></param>   
        /// <returns></returns>   
        public static DataTable ToDataTable(this IEnumerable list)
        {
            //创建属性的集合   
            List<PropertyInfo> pList = new List<PropertyInfo>();
            //获得反射的入口   
            Type type = list.AsQueryable().ElementType;
            DataTable dt = new DataTable();
            //把所有的public属性加入到集合 并添加DataTable的列   
            Array.ForEach<PropertyInfo>(
                type.GetProperties(),
                p =>
                {
                    pList.Add(p);
                    dt.Columns.Add(p.Name,
                        p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                        ? p.PropertyType.GetGenericArguments()[0]
                        : p.PropertyType
                        );
                });
            foreach (var item in list)
            {
                //创建一个DataRow实例   
                DataRow row = dt.NewRow();
                //给row 赋值   
                pList.ForEach(p => row[p.Name] = p.GetValue(item, null) ?? DBNull.Value);
                //加入到DataTable   
                dt.Rows.Add(row);
            }
            return dt;
        }


        public static Stream DataTableToExcel(DataTable SourceTable)
        {
            IWorkbook workbook = new HSSFWorkbook();
            MemoryStream ms = new MemoryStream();
            ISheet sheet = workbook.CreateSheet();
            IRow headerRow = sheet.CreateRow(0);

            // handling header.
            foreach (DataColumn column in SourceTable.Columns)
                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);

            // handling value.
            int rowIndex = 1;

            foreach (DataRow row in SourceTable.Rows)
            {
                IRow dataRow = sheet.CreateRow(rowIndex);

                foreach (DataColumn column in SourceTable.Columns)
                {
                    dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                }

                rowIndex++;
            }

            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;

            sheet = null;
            headerRow = null;
            workbook = null;

            return ms;
        }

        public static void DataTableToExcel(DataTable SourceTable, string FileName)
        {
            MemoryStream ms = DataTableToExcel(SourceTable) as MemoryStream;
            FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write);
            byte[] data = ms.ToArray();

            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();

            data = null;
            ms = null;
            fs = null;
        }

        public static DataTable ExcelToDataTable(Stream ExcelFileStream, string SheetName)
        {
            IWorkbook workbook = new HSSFWorkbook(ExcelFileStream);
            ISheet sheet = workbook.GetSheet(SheetName);

            DataTable table = new DataTable();

            IRow headerRow = sheet.GetRow(sheet.FirstRowNum);
            int cellCount = headerRow.LastCellNum;

            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }

            int rowCount = sheet.LastRowNum;

            for (int i = (sheet.FirstRowNum + 1); i < sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = table.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                    dataRow[j] = row.GetCell(j).ToString();
            }

            ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }

        public static DataTable ExcelToDataTable(Stream ExcelFileStream, int SheetIndex)
        {
            IWorkbook workbook = new HSSFWorkbook(ExcelFileStream);
            ISheet sheet = workbook.GetSheetAt(SheetIndex);

            DataTable table = new DataTable();

            IRow headerRow = sheet.GetRow(sheet.FirstRowNum);
            int cellCount = headerRow.LastCellNum;

            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                table.Columns.Add(column);
            }

            int rowCount = sheet.LastRowNum;

            for (int i = (sheet.FirstRowNum + 1); i < sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = table.NewRow();

                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }

                table.Rows.Add(dataRow);
            }

            ExcelFileStream.Close();
            workbook = null;
            sheet = null;
            return table;
        }
    }
}
