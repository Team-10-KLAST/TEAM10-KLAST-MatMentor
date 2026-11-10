using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeExerciseSetInterestOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "InterestId",
                table: "ExerciseSets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "ExerciseSets",
                columns: new[] { "Id", "Grade", "InterestId", "Title", "TopicId" },
                values: new object[] { 2, 8, null, "Øvesæt 1, Brøker, Standard", 1 });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "CorrectAnswer", "ExerciseSetId", "ExerciseText", "Position", "TopicId", "Unit" },
                values: new object[,]
                {
                    { 3, "15", 2, "Hvad er 3/4 af 20?", 1, 1, null },
                    { 4, "320", 2, "Hvad er 2/5 af 800?", 2, 1, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                keyValue: 2);

            migrationBuilder.AlterColumn<int>(
                name: "InterestId",
                table: "ExerciseSets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
