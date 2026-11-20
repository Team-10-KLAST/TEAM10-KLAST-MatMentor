using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceTestDataWithExerciseSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ExerciseSets",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ExerciseSets",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Handelsregning");

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Brøk, decimaltal og procent");

            migrationBuilder.InsertData(
                table: "Topics",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Tid" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.InsertData(
                table: "ExerciseSets",
                columns: new[] { "Id", "Grade", "InterestId", "Title", "TopicId" },
                values: new object[,]
                {
                    { 1, 8, 1, "Øvesæt 1, Brøker, Fodbold", 1 },
                    { 2, 8, null, "Øvesæt 1, Brøker, Standard", 1 }
                });

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Brøker");

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Procent");

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "CorrectAnswer", "ExerciseSetId", "ExerciseText", "Position", "TopicId", "Unit" },
                values: new object[,]
                {
                    { 1, "15", 1, "Holdet scorede 3/4 af 20 mål. Hvor mange mål?", 1, 1, "mål" },
                    { 2, "320", 1, "Spilleren løb 2/5 af 800 meter. Hvor langt?", 2, 1, "meter" },
                    { 3, "15", 2, "Hvad er 3/4 af 20?", 1, 1, null },
                    { 4, "320", 2, "Hvad er 2/5 af 800?", 2, 1, null }
                });
        }
    }
}
