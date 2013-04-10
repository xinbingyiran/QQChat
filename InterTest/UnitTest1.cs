using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace InterTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestExcel()
        {
            string filename = "test.xls";
            ExcelOp.ExcelFile file = null;
            if (!File.Exists(filename))
            {
                file = new ExcelOp.ExcelFile();
                file.SaveAs(filename);
            }
            file = ExcelOp.ExcelFile.LoadFromFile(filename);
            file.Save();         
        }
    }
}
