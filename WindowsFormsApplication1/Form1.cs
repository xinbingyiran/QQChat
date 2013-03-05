using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using FirebirdSql.Data.FirebirdClient;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        private static readonly string ConStringSQL = "Data Source=.;Initial Catalog=TestDB;Persist Security Info=True;User ID=sa;Password=sa;User Instance=False;Context Connection=False";
        private static readonly string ConStringSQLite = string.Format("Data Source={0};Version=3;", "");

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FirebirdSql.Data.FirebirdClient.FbConnectionStringBuilder cs = new FirebirdSql.Data.FirebirdClient.FbConnectionStringBuilder();
            //cs.DataSource = "localhost";
            cs.Database = @"d:\firsttest.gdb";
            cs.UserID = "sysdba";
            cs.Password = "masterkey";
            cs.Dialect = 1;

            FirebirdSql.Data.FirebirdClient.FbConnection cn = new FirebirdSql.Data.FirebirdClient.FbConnection();
            cn.ConnectionString = cs.ToString();
            cn.Open();
        }
    }
}
