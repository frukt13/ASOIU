using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using Microsoft.Data.Sqlite;

namespace Homework_2
{
    // ==========================================
    // КЛАССЫ-МОДЕЛИ (ВАРИАНТ 26)
    // ==========================================

    public class School
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public School(int id, string name) { Id = id; Name = name; }
        public School() : this(0, "") { }
        public override string ToString() => $"[{Id}] {Name}";
    }

    public class Student
    {
        public int Id { get; set; }
        public int SchoolId { get; set; }
        public string Name { get; set; }
        private double _avgGrade;

        public double AvgGrade
        {
            get => _avgGrade;
            set
            {
                if (value < 0) throw new ArgumentException("Балл не может быть отрицательным");
                _avgGrade = value;
            }
        }

        public Student(int id, int schoolId, string name, double avgGrade)
        {
            Id = id; SchoolId = schoolId; Name = name; AvgGrade = avgGrade;
        }
        public Student() : this(0, 0, "", 0.0) { }
        public override string ToString() => $"[{Id}] {Name}, Школа #{SchoolId}, балл: {AvgGrade:F1}";
    }

    // ==========================================
    // МЕНЕДЖЕР БАЗЫ ДАННЫХ
    // ==========================================

    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager(string dbPath)
        {
            _connectionString = $"Data Source={dbPath}";
            CreateTables();
        }

        private void CreateTables()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS schools (
                    id INTEGER PRIMARY KEY,
                    name TEXT NOT NULL
                );
                CREATE TABLE IF NOT EXISTS students (
                    id INTEGER PRIMARY KEY,
                    school_id INTEGER NOT NULL,
                    name TEXT NOT NULL,
                    avg_grade REAL NOT NULL,
                    FOREIGN KEY (school_id) REFERENCES schools(id)
                );";
            cmd.ExecuteNonQuery();
        }

        public void ImportFromCsv(string schoolCsv, string studentCsv)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var checkCmd = new SqliteCommand("SELECT COUNT(*) FROM schools", conn);
            if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0) return;

            if (File.Exists(schoolCsv))
            {
                foreach (var line in File.ReadAllLines(schoolCsv).Skip(1))
                {
                    var p = line.Split(';');
                    if (p.Length < 2) continue;
                    var cmd = new SqliteCommand("INSERT INTO schools (id, name) VALUES (@id, @name)", conn);
                    cmd.Parameters.AddWithValue("@id", int.Parse(p[0]));
                    cmd.Parameters.AddWithValue("@name", p[1]);
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine($"[OK] Загружено: {Path.GetFileName(schoolCsv)}");
            }

            if (File.Exists(studentCsv))
            {
                foreach (var line in File.ReadAllLines(studentCsv).Skip(1))
                {
                    var p = line.Split(';');
                    if (p.Length < 4) continue;
                    var cmd = new SqliteCommand("INSERT INTO students (id, school_id, name, avg_grade) VALUES (@id, @sid, @name, @g)", conn);
                    cmd.Parameters.AddWithValue("@id", int.Parse(p[0]));
                    cmd.Parameters.AddWithValue("@sid", int.Parse(p[1]));
                    cmd.Parameters.AddWithValue("@name", p[2]);
                    double grade = double.Parse(p[3].Replace(',', '.'), CultureInfo.InvariantCulture);
                    cmd.Parameters.AddWithValue("@g", grade);
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine($"[OK] Загружено: {Path.GetFileName(studentCsv)}");
            }
        }

        public List<School> GetAllSchools()
        {
            var list = new List<School>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var r = new SqliteCommand("SELECT * FROM schools", conn).ExecuteReader();
            while (r.Read()) list.Add(new School(r.GetInt32(0), r.GetString(1)));
            return list;
        }

        public List<Student> GetAllStudents()
        {
            var list = new List<Student>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var r = new SqliteCommand("SELECT * FROM students", conn).ExecuteReader();
            while (r.Read()) list.Add(new Student(r.GetInt32(0), r.GetInt32(1), r.GetString(2), r.GetDouble(3)));
            return list;
        }

        public (string[] cols, List<string[]> rows) ExecuteQuery(string sql)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var r = new SqliteCommand(sql, conn).ExecuteReader();
            var cols = Enumerable.Range(0, r.FieldCount).Select(r.GetName).ToArray();
            var rows = new List<string[]>();
            while (r.Read())
            {
                var row = new string[r.FieldCount];
                for (int i = 0; i < r.FieldCount; i++) row[i] = r.GetValue(i).ToString();
                rows.Add(row);
            }
            return (cols, rows);
        }
    }

    public class ReportBuilder
    {
        private readonly DatabaseManager _db;
        private string _sql = "", _title = "";
        private string[] _headers = Array.Empty<string>();
        private int[] _widths = Array.Empty<int>();

        public ReportBuilder(DatabaseManager db) => _db = db;
        public ReportBuilder Query(string sql) { _sql = sql; return this; }
        public ReportBuilder Title(string title) { _title = title; return this; }
        public ReportBuilder Header(params string[] h) { _headers = h; return this; }
        public ReportBuilder ColumnWidths(params int[] w) { _widths = w; return this; }

        public void Print()
        {
            var (cols, rows) = _db.ExecuteQuery(_sql);
            Console.WriteLine($"\n=== {_title} ===");
            var h = _headers.Length > 0 ? _headers : cols;
            for (int i = 0; i < h.Length; i++)
                Console.Write(h[i].PadRight(_widths.Length > i ? _widths[i] : 20));
            Console.WriteLine("\n" + new string('─', 60));

            foreach (var row in rows)
            {
                for (int i = 0; i < row.Length; i++)
                    Console.Write(row[i].PadRight(_widths.Length > i ? _widths[i] : 20));
                Console.WriteLine();
            }
        }
    }

    // ==========================================
    // ОСНОВНАЯ ПРОГРАММА
    // ==========================================

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            // Логика поиска корня проекта (поднимаемся на 3 уровня выше bin/Debug/net...)
            string baseDir = AppContext.BaseDirectory;
            string projectRoot = baseDir;
            try
            {
                projectRoot = Directory.GetParent(baseDir).Parent.Parent.Parent.FullName;
            }
            catch
            {
                projectRoot = baseDir; // На всякий случай
            }

            string dbPath = Path.Combine(baseDir, "school_system.db");
            string schoolCsv = Path.Combine(projectRoot, "schools.csv");
            string studentCsv = Path.Combine(projectRoot, "students.csv");

            // Если не нашли в корне, попробуем рядом с EXE
            if (!File.Exists(schoolCsv)) schoolCsv = Path.Combine(baseDir, "schools.csv");
            if (!File.Exists(studentCsv)) studentCsv = Path.Combine(baseDir, "students.csv");

            var db = new DatabaseManager(dbPath);
            db.ImportFromCsv(schoolCsv, studentCsv);

            string choice;
            do
            {
                Console.WriteLine("\n--- ВАРИАНТ 26: ШКОЛЫ ---");
                Console.WriteLine("1 - Список школ");
                Console.WriteLine("2 - Список учеников");
                Console.WriteLine("3 - Отчет по школам (JOIN)");
                Console.WriteLine("4 - Рейтинг успеваемости (GROUP BY)");
                Console.WriteLine("0 - Выход");
                Console.Write("Ваш выбор: ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        db.GetAllSchools().ForEach(Console.WriteLine);
                        break;
                    case "2":
                        db.GetAllStudents().ForEach(Console.WriteLine);
                        break;
                    case "3":
                        new ReportBuilder(db)
                            .Query("SELECT s.name, sch.name, s.avg_grade FROM students s JOIN schools sch ON s.school_id = sch.id")
                            .Title("Ученики и школы")
                            .Header("Ученик", "Школа", "Ср. балл")
                            .ColumnWidths(20, 20, 10).Print();
                        break;
                    case "4":
                        new ReportBuilder(db)
                            .Query("SELECT sch.name, ROUND(AVG(s.avg_grade), 2) as av FROM students s JOIN schools sch ON s.school_id = sch.id GROUP BY sch.name ORDER BY av DESC")
                            .Title("Рейтинг школ")
                            .Header("Название школы", "Средний балл")
                            .ColumnWidths(25, 15).Print();
                        break;
                }
            } while (choice != "0");
        }
    }
}
