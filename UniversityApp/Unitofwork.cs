using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityApp.Repositories;

namespace UniversityApp
{
    public class UnitOfWork : IDisposable
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;
        private bool _finished;

        public StudentRepository Students { get; }
        public GroupRepository Groups { get; }
        public GradeRepository Grades { get; }

        public UnitOfWork(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();

            Students = new StudentRepository(_connection, _transaction);
            Groups = new GroupRepository(_connection, _transaction);
            Grades = new GradeRepository(_connection, _transaction);
        }

        public void Commit()
        {
            if (_finished) return;
            _transaction.Commit();
            _finished = true;
        }

        public void Rollback()
        {
            if (_finished) return;
            _transaction.Rollback();
            _finished = true;
        }

        public void Dispose()
        {
            if (!_finished)
            {
                Rollback();
            }
            _transaction.Dispose();
            _connection.Dispose();
        }
    }
}
