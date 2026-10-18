using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityApp.Repositories
{
    public class GroupRepository
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        public GroupRepository(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public List<GroupInfo> GetAll()
        {
            const string sql =
                "SELECT GroupID, GroupName, StartYear, EndYear, SpecializationName " +
                "FROM vw_GroupInfo ORDER BY GroupName";

            var result = new List<GroupInfo>();
            using var command = DbHelper.Query(_connection, _transaction, sql);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new GroupInfo
                {
                    GroupID = reader.GetInt32(0),
                    GroupName = reader.GetString(1),
                    StartYear = reader.GetInt16(2),
                    EndYear = reader.IsDBNull(3) ? null : reader.GetInt16(3),
                    SpecializationName = reader.GetString(4)
                });
            }
            return result;
        }

        public int Add(int specializationId, string name, int startYear, int? endYear = null)
        {
            using var command = DbHelper.Procedure(_connection, _transaction, "dbo.sp_AddGroup");
            command.AddParam("@SpecializationID", SqlDbType.Int, specializationId);
            command.AddParam("@Name", SqlDbType.NVarChar, name);
            command.AddParam("@StartYear", SqlDbType.SmallInt, (short)startYear);
            command.AddParam("@EndYear", SqlDbType.SmallInt, endYear.HasValue ? (short)endYear.Value : null);
            var newId = command.AddOutputInt("@NewGroupID");

            command.ExecuteNonQuery();
            return (int)newId.Value;
        }
    }
}
