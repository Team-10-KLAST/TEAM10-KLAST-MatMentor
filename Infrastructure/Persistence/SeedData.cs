using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class SeedData
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Interest>().HasData(
            new { Id = 1, Name = "Fodbold" },
            new { Id = 2, Name = "Ridning" }
        );

        modelBuilder.Entity<Topic>().HasData(
            new { Id = 1, Name = "Handelsregning" },
            new { Id = 2, Name = "Brøk, decimaltal og procent" },
            new { Id = 3, Name = "Tid" }
        );

        modelBuilder.Entity<Student>().HasData(
            new { Id = 1, ParentEmail = "test@example.com", Grade = 8 }
        );

        modelBuilder.Entity("StudentInterests").HasData(
            new { StudentId = 1, InterestId = 1 }
        );
    }
}