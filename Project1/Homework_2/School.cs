using System;

namespace Homework_2
{
    public class School
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public School(int id, string name) { Id = id; Name = name; }
        public School() : this(0, "Не указано") { }
        public override string ToString() => $"[ID: {Id}] Школа: {Name}";
    }
}
