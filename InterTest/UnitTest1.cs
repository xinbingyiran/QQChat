using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Data.SqlClient;

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

        [TestMethod]
        public void TestConnection()
        {
            try
            {
                string constr = "Data Source=.;Initial Catalog=testDB;Integrated Security=False;User ID=sa;Password=sa;User Instance=False;Context Connection=False";
                using (SqlConnection conn = new SqlConnection(constr))
                {
                    conn.Open();
                    string sql = "select * from [User] where name=@username";
                    SqlParameter p = new SqlParameter("@username", "admin");
                    SqlCommand com = new SqlCommand(sql, conn);
                    com.Parameters.Add(p);
                    SqlDataReader reader = com.ExecuteReader();
                    while (reader.Read())
                    {
                        string pass = reader["pass"] as string;
                        System.Diagnostics.Trace.WriteLine(pass);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
