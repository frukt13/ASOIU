using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Linq;

namespace Homework_2
{
    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager(string dbName)
        {
            _connectionString = $"Data Source={dbName}";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS schools (id INTEGER PRIMARY KEY, name TEXT);
                CREATE TABLE IF NOT EXISTS students (
                    id INTEGER PRIMARY KEY AUTOINCREMENT, 
                    school_id INTEGER, 
                    name TEXT, 
                    avg_grade REAL,
                    FOREIGN KEY(school_id) REFERENCES schools(id));";
            cmd.ExecuteNonQuery();
        }

        public void ImportFromCsv(string schoolCsv, string studentCsv)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // 1. Импорт школ (структура файла: id;name)
            if (File.Exists(schoolCsv))
            {
                var lines = File.ReadAllLines(schoolCsv).Skip(1);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var p = line.Split(';');
                    if (p.Length >= 2)
                    {
                        if (int.TryParse(p[0].Trim(), out int id))
                        {
                            var cmd = new SqliteCommand("INSERT OR IGNORE INTO schools (id, name) VALUES (@id, @n)", connection);
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.Parameters.AddWithValue("@n", p[1].Trim());
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            // 2. Импорт учеников (структура файла: id;school_id;name;avg_grade)
            if (File.Exists(studentCsv))
            {
                var lines = File.ReadAllLines(studentCsv).Skip(1);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var p = line.Split(';');

                    // Проверяем, что в строке минимум 4 элемента
                    if (p.Length >= 4)
                    {
                        // p[1] - school_id, p[2] - name, p[3] - avg_grade
                        if (int.TryParse(p[1].Trim(), out int sid))
                        {
                            var cmd = new SqliteCommand("INSERT INTO students (school_id, name, avg_grade) VALUES (@sid, @n, @g)", connection);
                            cmd.Parameters.AddWithValue("@sid", sid);
                            cmd.Parameters.AddWithValue("@n", p[2].Trim());

                            string gradeStr = p[3].Replace(',', '.').Trim();
                            if (double.TryParse(gradeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double grade))
                            {
                                cmd.Parameters.AddWithValue("@g", grade);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        public List<School> GetAllSchools()
        {
            var list = new List<School>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var reader = new SqliteCommand("SELECT * FROM schools", conn).ExecuteReader();
            while (reader.Read())
            {
                list.Add(new School(reader.GetInt32(0), reader.GetString(1)));
            }
            return list;
        }

        public void AddStudent(Student s)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = new SqliteCommand("INSERT INTO students (school_id, name, avg_grade) VALUES (@sid, @n, @g)", conn);
            cmd.Parameters.AddWithValue("@sid", s.SchoolId);
            cmd.Parameters.AddWithValue("@n", s.Name);
            cmd.Parameters.AddWithValue("@g", s.AverageGrade);
            cmd.ExecuteNonQuery();
        }

        public (string[] cols, List<string[]> rows) ExecuteQuery(string sql)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var reader = new SqliteCommand(sql, conn).ExecuteReader();

            var cols = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++) cols[i] = reader.GetName(i);

            var rows = new List<string[]>();
            while (reader.Read())
            {
                var row = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++) row[i] = reader.GetValue(i).ToString();
                rows.Add(row);
            }
            return (cols, rows);
        }
    }
}
