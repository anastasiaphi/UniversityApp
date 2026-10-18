using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityApp
{
    public class StudentInfo
    {
        public int StudentID { get; set; }
        public int GroupID { get; set; }
        public string LastName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string? MiddleName { get; set; }
        public string GroupName { get; set; } = "";
        public string SpecializationName { get; set; } = "";

        public override string ToString() =>
            $"[{StudentID}] {LastName} {FirstName} {MiddleName}".TrimEnd() +
            $" | група {GroupName} | {SpecializationName}";
    }

    public class GroupInfo
    {
        public int GroupID { get; set; }
        public string GroupName { get; set; } = "";
        public int StartYear { get; set; }
        public int? EndYear { get; set; }
        public string SpecializationName { get; set; } = "";

        public override string ToString() =>
            $"[{GroupID}] {GroupName} | {StartYear}-{EndYear?.ToString() ?? "?"} | {SpecializationName}";
    }

    public class GradeDetail
    {
        public int GradeID { get; set; }
        public int StudentID { get; set; }
        public string GroupName { get; set; } = "";
        public string DisciplineName { get; set; } = "";
        public string AssessmentType { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public int Value { get; set; }
        public int MaxValue { get; set; }
        public DateTime GradedAt { get; set; }

        public override string ToString() =>
            $"{LastName} {FirstName} | {DisciplineName} | {AssessmentType} | {Value}/{MaxValue} | {GradedAt:dd.MM.yyyy}";
    }
}
