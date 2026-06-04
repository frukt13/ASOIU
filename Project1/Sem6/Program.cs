using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CleanLibraryExample
{
    abstract class LibraryItem
    {
        public string Title { get; }

        protected LibraryItem(string title)
        {
            Title = title;
        }

        public abstract string GetDisplayInfo();
    }

    class Book : LibraryItem
    {
        public string Author { get; }
        public int Year { get; }

        public Book(string title, string author, int year)
            : base(title)
        {
            Author = author;
            Year = year;
        }

        public override string GetDisplayInfo()
            => $"Книга: {Title} — {Author} ({Year})";
    }

    class Magazine : LibraryItem
    {
        public int IssueNumber { get; }

        public Magazine(string title, int issueNumber)
            : base(title)
        {
            IssueNumber = issueNumber;
        }

        public override string GetDisplayInfo()
            => $"Журнал: {Title}, выпуск №{IssueNumber}";
    }

    class LibraryItemValidator
    {
        private const int MinBookYear = 1000;

        public void ValidateBook(string title, string author, int year)
        {
            ValidateTitle(title);

            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("Автор не может быть пустым");

            if (year < MinBookYear || year > DateTime.Now.Year)
                throw new ArgumentException("Некорректный год издания");
        }

        public void ValidateMagazine(string title, int issueNumber)
        {
            ValidateTitle(title);

            if (issueNumber <= 0)
                throw new ArgumentException("Номер выпуска должен быть положительным");
        }

        private void ValidateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Название не может быть пустым");
        }
    }

    interface ILogger
    {
        void Log(string message);
    }

    class FileLogger : ILogger
    {
        private readonly string _logFilePath;

        public FileLogger(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public void Log(string message)
        {
            File.AppendAllText(
                _logFilePath,
                $"{DateTime.Now:u}: {message}\n");
        }
    }

    interface ILibraryRepository
    {
        void Add(LibraryItem item);

        bool RemoveByTitle(string title);

        IReadOnlyCollection<LibraryItem> GetAll();
    }

    class InMemoryLibraryRepository : ILibraryRepository
    {
        private readonly List<LibraryItem> _items = new();

        public void Add(LibraryItem item)
        {
            _items.Add(item);
        }

        public bool RemoveByTitle(string title)
        {
            LibraryItem? item = _items
                .FirstOrDefault(i =>
                    i.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

            if (item == null)
                return false;

            _items.Remove(item);
            return true;
        }

        public IReadOnlyCollection<LibraryItem> GetAll()
        {
            return _items.AsReadOnly();
        }
    }

    interface IReportExporter
    {
        void Export(IEnumerable<LibraryItem> items);
    }

    class TextReportExporter : IReportExporter
    {
        private readonly string _reportPath;

        public TextReportExporter(string reportPath)
        {
            _reportPath = reportPath;
        }

        public void Export(IEnumerable<LibraryItem> items)
        {
            string report =
                $"Всего элементов: {items.Count()}\n" +
                $"Дата: {DateTime.Now:u}";

            File.WriteAllText(_reportPath, report);
        }
    }

    class LibraryReportPrinter
    {
        public void Print(IEnumerable<LibraryItem> items)
        {
            List<LibraryItem> itemList = items.ToList();

            Console.WriteLine($"=== Отчёт: {itemList.Count} элементов ===");

            foreach (LibraryItem item in itemList)
            {
                Console.WriteLine(item.GetDisplayInfo());
            }
        }
    }

    class LibraryService
    {
        private readonly ILibraryRepository _repository;
        private readonly LibraryItemValidator _validator;
        private readonly ILogger _logger;
        private readonly LibraryReportPrinter _reportPrinter;
        private readonly IReportExporter _reportExporter;

        public LibraryService(
            ILibraryRepository repository,
            LibraryItemValidator validator,
            ILogger logger,
            LibraryReportPrinter reportPrinter,
            IReportExporter reportExporter)
        {
            _repository = repository;
            _validator = validator;
            _logger = logger;
            _reportPrinter = reportPrinter;
            _reportExporter = reportExporter;
        }

        public void AddBook(string title, string author, int year)
        {
            _validator.ValidateBook(title, author, year);

            Book book = new(title, author, year);

            _repository.Add(book);

            _logger.Log($"Добавлена книга «{title}»");
        }

        public void AddMagazine(string title, int issueNumber)
        {
            _validator.ValidateMagazine(title, issueNumber);

            Magazine magazine = new(title, issueNumber);

            _repository.Add(magazine);

            _logger.Log($"Добавлен журнал «{title}»");
        }

        public bool RemoveItem(string title)
        {
            bool removed = _repository.RemoveByTitle(title);

            if (removed)
            {
                _logger.Log($"Удалён элемент «{title}»");
            }

            return removed;
        }

        public void PrintReport()
        {
            IReadOnlyCollection<LibraryItem> items = _repository.GetAll();

            _reportPrinter.Print(items);

            _reportExporter.Export(items);
        }
    }

    class Program
    {
        static void Main()
        {
            ILibraryRepository repository = new InMemoryLibraryRepository();

            ILogger logger =
                new FileLogger("library.log");

            LibraryItemValidator validator =
                new LibraryItemValidator();

            LibraryReportPrinter reportPrinter =
                new LibraryReportPrinter();

            IReportExporter exporter =
                new TextReportExporter("report.txt");

            LibraryService libraryService = new(
                repository,
                validator,
                logger,
                reportPrinter,
                exporter);

            libraryService.AddBook(
                "Clean Code",
                "Robert Martin",
                2008);

            libraryService.AddMagazine(
                "Science Today",
                15);

            libraryService.PrintReport();

            Console.WriteLine();

            bool removed =
                libraryService.RemoveItem("Clean Code");

            Console.WriteLine(
                removed
                    ? "Книга удалена"
                    : "Элемент не найден");

            Console.WriteLine();

            libraryService.PrintReport();
        }
    }
}