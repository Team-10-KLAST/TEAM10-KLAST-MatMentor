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
            new { Id = 1, Name = "Brøker" },
            new { Id = 2, Name = "Procent" }
        );

        modelBuilder.Entity<ExerciseSet>().HasData(
            new { Id = 1, Title = "Øvesæt 1, Brøker, Fodbold", Grade = 8, TopicId = (int?)1, InterestId = 1 }
        );

        modelBuilder.Entity<Exercise>().HasData(
            new { Id = 1, Position = 1, ExerciseText = "Holdet scorede 3/4 af 20 mål. Hvor mange mål?", CorrectAnswer = "15", Unit = "mål", ExerciseSetId = 1, TopicId = 1 },
            new { Id = 2, Position = 2, ExerciseText = "Spilleren løb 2/5 af 800 meter. Hvor langt?", CorrectAnswer = "320", Unit = "meter", ExerciseSetId = 1, TopicId = 1 }
        );

        modelBuilder.Entity<Student>().HasData(
            new { Id = 1, UserName = "test", Password = "test", ParentEmail = "", Grade = 8, InterestId = 1 }
        );
    }
}