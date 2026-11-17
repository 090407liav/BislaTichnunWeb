using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
                        _connStr = ConfigurationManager.ConnectionStrings["BisaltDB"].ConnectionString;
                    return _connStr;
                }
            }

            public static SqlConnection Open()
            {
                var conn = new SqlConnection(ConnectionString);
                conn.Open();
                return conn;
            }
    }
        
}
