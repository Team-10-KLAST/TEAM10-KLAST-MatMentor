using Domain;
using Domain.Rules;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure.Persistence.Seeding;

// Seeds the exercise sets from exercise-sets.json. Called by UseSeeding/UseAsyncSeeding,
// which EF Core runs on every Migrate(), also when no new migrations were applied.
// Topics and interests are reference data in HasData; this seeder only looks them up by name.
public static class ExerciseSetSeeder
{
    private const string ResourceName = "exercise-sets.json";

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public static void Seed(DbContext context)
    {
        List<ExerciseSet> newSets = BuildNewSets(context);
        if (newSets.Count == 0)
            return;

        context.Set<ExerciseSet>().AddRange(newSets);
        context.SaveChanges();
    }

    public static async Task SeedAsync(DbContext context, CancellationToken cancellationToken)
    {
        List<ExerciseSet> newSets = BuildNewSets(context);
        if (newSets.Count == 0)
            return;

        context.Set<ExerciseSet>().AddRange(newSets);
        await context.SaveChangesAsync(cancellationToken);
    }

    // Builds the sets from the file that are not in the database yet.
    // A set is identified by its natural key: title + grade + interest (the file has no ids).
    // Every check runs before anything is saved, so an error in the file saves nothing.
    private static List<ExerciseSet> BuildNewSets(DbContext context)
    {
        List<SeedExerciseSet> fileSets = LoadFile();

        Dictionary<string, int> topicIds = context.Set<Topic>()
            .ToDictionary(t => t.Name, t => t.Id);
        Dictionary<string, int> interestIds = context.Set<Interest>()
            .ToDictionary(i => i.Name, i => i.Id);
        var existingKeys = context.Set<ExerciseSet>()
            .Select(s => new { s.Title, s.Grade, s.InterestId })
            .AsEnumerable()
            .Select(s => (s.Title, s.Grade, s.InterestId))
            .ToHashSet();

        var newSets = new List<ExerciseSet>();

        foreach (SeedExerciseSet fileSet in fileSets)
        {
            if (!topicIds.TryGetValue(fileSet.Topic, out int topicId))
                throw new InvalidOperationException(
                    $"Exercise set seed: topic '{fileSet.Topic}' in set '{fileSet.Title}' does not exist.");

            int? interestId = null;
            if (fileSet.Interest is not null)
            {
                if (!interestIds.TryGetValue(fileSet.Interest, out int foundInterestId))
                    throw new InvalidOperationException(
                        $"Exercise set seed: interest '{fileSet.Interest}' in set '{fileSet.Title}' does not exist.");
                interestId = foundInterestId;
            }

            if (existingKeys.Contains((fileSet.Title, fileSet.Grade, interestId)))
                continue;

            var set = new ExerciseSet(fileSet.Title, fileSet.Grade, topicId, interestId);

            foreach (SeedExercise exercise in fileSet.Exercises)
            {
                if (!AnswerFormat.TryParse(exercise.CorrectAnswer, out _))
                    throw new InvalidOperationException(
                        $"Exercise set seed: correct answer '{exercise.CorrectAnswer}' in set '{fileSet.Title}', " +
                        $"exercise {exercise.Position} is not a valid number.");

                set.AddExercise(exercise.Position, exercise.ExerciseText, exercise.CorrectAnswer, exercise.Unit, topicId);
            }

            newSets.Add(set);
        }

        return newSets;
    }

    private static List<SeedExerciseSet> LoadFile()
    {
        using Stream stream = typeof(ExerciseSetSeeder).Assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{ResourceName}' was not found.");

        return JsonSerializer.Deserialize<List<SeedExerciseSet>>(stream, JsonOptions)
            ?? throw new InvalidOperationException("Exercise set seed file is empty.");
    }

    // The shape of exercise-sets.json. Names, not ids: the seeder looks the ids up.
    private sealed record SeedExerciseSet(string Title, int Grade, string Topic, string? Interest, List<SeedExercise> Exercises);
    private sealed record SeedExercise(int Position, string ExerciseText, string CorrectAnswer, string? Unit);
}