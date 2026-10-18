


using System.Text;
using Microsoft.Data.SqlClient;
using UniversityApp;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

const string ConnectionString =
    "Server=(localdb)\\MSSQLLocalDB; Database=Uni;Trusted_Connection=True;TrustServerCertificate=True;";

const int ADMIN = 1;
const int SPECIALIZATION = 2;

try
{
    var created = Scenario1_GroupAndStudent();
    Scenario2_Rollback();
    Scenario3_SoftDelete(created.groupId, created.studentId);
    Scenario4_Reports();
}
catch (SqlException ex)
{
    Console.WriteLine();
    Console.WriteLine("Помилка бази даних: " + ex.Message);
}

Console.WriteLine();
Console.ReadLine();

(int groupId, int studentId) Scenario1_GroupAndStudent()
{
    Title("1. Додавання групи і студента (Commit)");

    string groupName = "КН-26";
    using var uow = new UnitOfWork(ConnectionString);

   

    int groupId = uow.Groups.Add(SPECIALIZATION, groupName, 2026, 2030);
    int studentId = uow.Students.Add(groupId, "Тарас", "Гончар", "Петрович");
    int student2Id = uow.Students.Add(groupId, "Олена", "Мазур", "Іванівна");
    uow.Commit();

    Console.WriteLine($"Група {groupName} додана, ID = {groupId}");
    Console.WriteLine($"Студенти додані, ID = {studentId} і {student2Id}");

    Console.WriteLine("Студенти групи (дані з view):");
    foreach (var s in uow.Students.GetAll().Where(x => x.GroupID == groupId))
        Console.WriteLine("  " + s);

    return (groupId, studentId);
}

void Scenario2_Rollback()
{
    Title("2. Відкат транзакції (Rollback)");

    string rollbackName = "ТЕСТ-ВІДКАТ";
    using (var uow = new UnitOfWork(ConnectionString))
    {
        try
        {
            uow.Groups.Add(SPECIALIZATION, rollbackName, 2026, 2030);
            Console.WriteLine($"Група {rollbackName} додана (поки без Commit)");

            uow.Students.Add(999999, "Тест", "Тестовий");
            uow.Commit();
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Помилка: " + ex.Message);
            uow.Rollback();
            Console.WriteLine("Зміни скасовано");
        }
    }

    using (var uow = new UnitOfWork(ConnectionString))
    {
        bool exists = uow.Groups.GetAll().Any(g => g.GroupName == rollbackName);
        if (exists)
            Console.WriteLine($"Група {rollbackName} є в базі");
        else
            Console.WriteLine($"Групи {rollbackName} в базі немає, відкат спрацював");
    }
}

void Scenario3_SoftDelete(int groupId, int studentId)
{
    Title("3. Видалення студента (soft delete)");

    using var uow = new UnitOfWork(ConnectionString);

    Console.WriteLine("Студенти групи до видалення:");
    foreach (var s in uow.Students.GetAll().Where(x => x.GroupID == groupId))
        Console.WriteLine("  " + s);

    uow.Students.SoftDelete(studentId, ADMIN);
    uow.Commit();
    Console.WriteLine($"Студента з ID {studentId} видалено");

    Console.WriteLine("Студенти групи після видалення:");
    foreach (var s in uow.Students.GetAll().Where(x => x.GroupID == groupId))
        Console.WriteLine("  " + s);
}

void Scenario4_Reports()
{
    Title("4. Дані з view");

    using var uow = new UnitOfWork(ConnectionString);

    Console.WriteLine("Групи (vw_GroupInfo):");
    foreach (var g in uow.Groups.GetAll())
        Console.WriteLine("  " + g);

    Console.WriteLine();
    Console.WriteLine("Студенти (vw_StudentInfo):");
    foreach (var s in uow.Students.GetAll())
        Console.WriteLine("  " + s);

    Console.WriteLine();
    Console.WriteLine("Оцінки групи КН-21 (vw_GradeDetails):");

    var byStudent = uow.Grades.GetByGroup("КН-21").GroupBy(gr => gr.StudentID);
    foreach (var student in byStudent)
    {
        var first = student.First();
        string marks = string.Join(", ",
            student.Select(gr => $"{gr.DisciplineName} ({gr.AssessmentType}) {gr.Value}/{gr.MaxValue}"));
        Console.WriteLine($"  {first.LastName} {first.FirstName}: {marks}");
    }
}

void Title(string text)
{
    Console.WriteLine();
    Console.WriteLine($"=== {text} ===");
}