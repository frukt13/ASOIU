using Homework3.Variant26.Models;
using Microsoft.EntityFrameworkCore;

namespace Homework3.Variant26.Data;

/// <summary>Контекст базы данных приложения.</summary>
public sealed class AppDbContext : DbContext
{
    /// <summary>Таблица школ.</summary>
    public DbSet<School> Schools => Set<School>();

    /// <summary>Таблица учеников.</summary>
    public DbSet<Student> Students => Set<Student>();

    /// <summary>Настраивает подключение к SQLite.</summary>
    /// <param name="optionsBuilder">Построитель параметров контекста.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=homework3_variant26.db");
        }
    }

    /// <summary>Настраивает связь один-ко-многим и ограничения модели.</summary>
    /// <param name="modelBuilder">Построитель модели Entity Framework Core.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<School>()
            .HasIndex(school => school.Name)
            .IsUnique();

        modelBuilder.Entity<School>()
            .HasMany(school => school.Students)
            .WithOne(student => student.School)
            .HasForeignKey(student => student.SchoolId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
