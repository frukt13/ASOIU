using System;

namespace Homework_2
{
    public class Student
    {
        public int Id { get; set; }
        public int SchoolId { get; set; }
        public string Name { get; set; }
        private double _avgGrade;
        public double AverageGrade
        {
            get => _avgGrade;
            set
            {
                if (value < 0 || value > 5) throw new ArgumentException("Балл должен быть от 0 до 5.");
                _avgGrade = value;
            }
        }
        public Student(int id, int schoolId, string name, double avgGrade)
        {
            Id = id; SchoolId = schoolId; Name = name; AverageGrade = avgGrade;
        }
        public Student() : this(0, 0, "Новый ученик", 0.0) { }
        public override string ToString() => $"ID: {Id} | {Name} | Школа ID: {SchoolId} | Балл: {AverageGrade:F2}";
    }
}
