using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityApp.Repositories
{
    public class GradeRepository
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        public GradeRepository(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

     
        public List<GradeDetail> GetByGroup(string groupName)
        {
            const string sql =
                "SELECT GradeID, StudentID, GroupName, DisciplineName, AssessmentType, " +
                "FirstName, LastName, [Value], MaxValue, GradedAt " +
                "FROM vw_GradeDetails WHERE GroupName = @GroupName " +
                "ORDER BY DisciplineName, LastName";

            var result = new List<GradeDetail>();
            using var command = DbHelper.Query(_connection, _transaction, sql);
            command.AddParam("@GroupName", SqlDbType.NVarChar, groupName);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new GradeDetail
                {
                    GradeID = reader.GetInt32(0),
                    StudentID = reader.GetInt32(1),
                    GroupName = reader.GetString(2),
                    DisciplineName = reader.GetString(3),
                    AssessmentType = reader.GetString(4),
                    FirstName = reader.GetString(5),
                    LastName = reader.GetString(6),
                    Value = reader.GetInt32(7),
                    MaxValue = reader.GetInt32(8),
                    GradedAt = reader.GetDateTime(9)
                });
            }
            return result;
        }
    }
}
