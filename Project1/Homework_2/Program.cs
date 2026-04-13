using System;
using System.IO;

namespace Homework_2
{
    class Program
    {
        static void Main()
        {
            // База создастся в папке с .exe
            var db = new DatabaseManager("school_data.db");

            // Пытаемся найти файлы в разных местах
            string sFile = "schools.csv";
            string stFile = "students.csv";

            if (!File.Exists(sFile)) sFile = "../../../schools.csv";
            if (!File.Exists(stFile)) stFile = "../../../students.csv";

            // Импортируем только если база пустая (чтобы не дублировать)
            if (db.GetAllSchools().Count == 0)
            {
                db.ImportFromCsv(sFile, stFile);
                Console.WriteLine("Данные импортированы из CSV.");
            }

            while (true)
            {
                Console.WriteLine("\n--- МЕНЮ (Вариант 26) ---");
                Console.WriteLine("1. Список школ");
                Console.WriteLine("2. Список всех учеников");
                Console.WriteLine("3. Добавить ученика");
                Console.WriteLine("4. Отчет: Ученики по школам");
                Console.WriteLine("5. Сохранить отчет в файл");
                Console.WriteLine("0. Выход");
                Console.Write("Выбор: ");

                string choice = Console.ReadLine();
                if (choice == "0") break;

                switch (choice)
                {
                    case "1":
                        foreach (var s in db.GetAllSchools()) Console.WriteLine(s);
                        break;
                    case "2":
                        var (_, rows) = db.ExecuteQuery("SELECT * FROM students");
                        if (rows.Count == 0) Console.WriteLine("Учеников пока нет.");
                        foreach (var r in rows) Console.WriteLine(string.Join(" | ", r));
                        break;
                    case "3":
                        AddStudentInteraction(db);
                        break;
                    case "4":
                        GetMainReport(db).Print();
                        break;
                    case "5":
                        GetMainReport(db).SaveToFile("report.txt");
                        break;
                }
            }
        }

        static void AddStudentInteraction(DatabaseManager db)
        {
            Console.WriteLine("\n--- ДОБАВЛЕНИЕ УЧЕНИКА ---");
            var schools = db.GetAllSchools();
            foreach (var s in schools) Console.WriteLine(s);

            Console.Write("Введите ID школы: ");
            if (!int.TryParse(Console.ReadLine(), out int sid)) return;

            Console.Write("Введите ФИО: ");
            string name = Console.ReadLine();

            Console.Write("Введите балл: ");
            if (!double.TryParse(Console.ReadLine().Replace('.', ','), out double grade)) return;

            try
            {
                db.AddStudent(new Student(0, sid, name, grade));
                Console.WriteLine("Готово!");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        static ReportBuilder GetMainReport(DatabaseManager db)
        {
            return new ReportBuilder(db)
                .Query("SELECT s.name, st.name, st.avg_grade FROM students st JOIN schools s ON st.school_id = s.id")
                .Title("Список учеников")
                .Header("Школа", "Ученик", "Балл")
                .ColumnWidths(25, 20, 10);
        }
    }
}
