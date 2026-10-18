using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityApp
{
    internal static class DbHelper
    {
        public static SqlCommand Procedure(SqlConnection connection, SqlTransaction transaction, string name)
        {
            return new SqlCommand(name, connection, transaction)
            {
                CommandType = CommandType.StoredProcedure
            };
        }

        public static SqlCommand Query(SqlConnection connection, SqlTransaction transaction, string sql)
        {
            return new SqlCommand(sql, connection, transaction);
        }

      
        public static void AddParam(this SqlCommand command, string name, SqlDbType type, object? value)
        {
            command.Parameters.Add(name, type).Value = value ?? DBNull.Value;
        }

        public static SqlParameter AddOutputInt(this SqlCommand command, string name)
        {
            var parameter = command.Parameters.Add(name, SqlDbType.Int);
            parameter.Direction = ParameterDirection.Output;
            return parameter;
        }
    }
}
