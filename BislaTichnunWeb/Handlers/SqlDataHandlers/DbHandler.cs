using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.OleDb;
using System.Configuration;

namespace BislaTichnunWeb
{
    public static class DbHandler
    {
        private static string _connStr;

        public static string ConnectionString
        {
            get
            {
                if (_connStr == null)
                    _connStr = ConfigurationManager.ConnectionStrings["BISLATDB"].ConnectionString;

                return _connStr;
            }
        }

        public static OleDbConnection Open()
        {
            var conn = new OleDbConnection(ConnectionString);
            conn.Open();
            return conn;
        }
    }

}