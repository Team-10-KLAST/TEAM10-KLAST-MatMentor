using System.Globalization;

namespace Domain;

//The rule for how a student's answer is read as a number.
//Kept in one place so the validator and the comparison can never disagree.
public static class AnswerFormat
{
    private static readonly CultureInfo DanishCulture = new("da-DK");

    public static bool ContainsPeriod(string? value)
        => value is not null && value.Contains('.');

    public static bool TryParse(string? value, out decimal result)
    {
        result = 0;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        var trimmed = value.Trim();

        //A period is invalid, neither as a decimal separator nor as a thousands separator.
        if (trimmed.Contains('.'))
            return false;

        return decimal.TryParse(
            trimmed,
            NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
            DanishCulture,
            out result);
    }
}