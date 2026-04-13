using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Homework_2
{
    public class ReportBuilder
    {
        private readonly DatabaseManager _db;
        private string _sql, _title;
        private string[] _headers;
        private int[] _widths;

        public ReportBuilder(DatabaseManager db) => _db = db;
        public ReportBuilder Query(string sql) { _sql = sql; return this; }
        public ReportBuilder Title(string t) { _title = t; return this; }
        public ReportBuilder Header(params string[] h) { _headers = h; return this; }
        public ReportBuilder ColumnWidths(params int[] w) { _widths = w; return this; }

        public string Build()
        {
            var (cols, rows) = _db.ExecuteQuery(_sql);
            var sb = new StringBuilder();
            sb.AppendLine($"\n=== {_title.ToUpper()} ===");
            var displayHeaders = _headers ?? cols;
            for (int i = 0; i < displayHeaders.Length; i++)
                sb.Append(displayHeaders[i].PadRight(_widths?[i] ?? 20));
            sb.AppendLine().AppendLine(new string('-', 60));
            foreach (var row in rows)
            {
                for (int i = 0; i < row.Length; i++)
                    sb.Append(row[i].PadRight(_widths?[i] ?? 20));
                sb.AppendLine();
            }
            return sb.ToString();
        }
        public void Print() => Console.WriteLine(Build());
        public void SaveToFile(string path) { File.WriteAllText(path, Build()); Console.WriteLine($"Отчет сохранен: {path}"); }
    }
}
