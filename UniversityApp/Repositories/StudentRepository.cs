using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityApp.Repositories
{
    public class StudentRepository
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        public StudentRepository(SqlConnection connection, SqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public List<StudentInfo> GetAll()
        {
            const string sql =
                "SELECT StudentID, GroupID, LastName, FirstName, MiddleName, GroupName, SpecializationName " +
                "FROM vw_StudentInfo ORDER BY LastName, FirstName";

            var result = new List<StudentInfo>();
            using var command = DbHelper.Query(_connection, _transaction, sql);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new StudentInfo
                {
                    StudentID = reader.GetInt32(0),
                    GroupID = reader.GetInt32(1),
                    LastName = reader.GetString(2),
                    FirstName = reader.GetString(3),
                    MiddleName = reader.IsDBNull(4) ? null : reader.GetString(4),
                    GroupName = reader.GetString(5),
                    SpecializationName = reader.GetString(6)
                });
            }
            return result;
        }

        public int Add(int groupId, string firstName, string lastName,
                       string? middleName = null, DateTime? birthDate = null, string? phone = null)
        {
            using var command = DbHelper.Procedure(_connection, _transaction, "dbo.sp_AddStudent");
            command.AddParam("@GroupID", SqlDbType.Int, groupId);
            command.AddParam("@FirstName", SqlDbType.NVarChar, firstName);
            command.AddParam("@LastName", SqlDbType.NVarChar, lastName);
            command.AddParam("@MiddleName", SqlDbType.NVarChar, middleName);
            command.AddParam("@BirthDate", SqlDbType.Date, birthDate);
            command.AddParam("@Phone", SqlDbType.NVarChar, phone);
            var newId = command.AddOutputInt("@NewStudentID");

            command.ExecuteNonQuery();
            return (int)newId.Value;
        }
               
        public void SoftDelete(int studentId, int? modifiedByUserId)
        {
            using var command = DbHelper.Procedure(_connection, _transaction, "dbo.sp_SoftDeleteStudent");
            command.AddParam("@StudentID", SqlDbType.Int, studentId);
            command.AddParam("@ModifiedByUserID", SqlDbType.Int, modifiedByUserId);
            command.ExecuteNonQuery();
        }

    }
}
