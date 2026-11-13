using Domain.Rules;
using UnitTests.TestHelpers;

namespace UnitTests.Domain.Rules;

public class AnswerFormatTests
{
    // Decimal numbers written with a comma are accepted
    public static TheoryData<string, decimal> DecimalsWithComma => new()
    {
        { "3,5", 3.5m },
        { "15,0", 15.0m },
        { "-0,25", -0.25m },
    };

    [Theory]
    [Trait("TestCase", "TC-001")]
    [MemberData(nameof(DecimalsWithComma))]
    public void TryParse_DecimalWithComma_ReturnsTrueAndNumber(string input, decimal expected)
    {
        var success = AnswerFormat.TryParse(input, out var result);

        Assert.True(success);
        Assert.Equal(expected, result);
    }

    // Whole numbers, including negative numbers and zero, are accepted
    public static TheoryData<string, decimal> WholeNumbers => new()
    {
        { "15", 15m },
        { "-2", -2m },
        { "0", 0m },
    };

    [Theory]
    [Trait("TestCase", "TC-002")]
    [MemberData(nameof(WholeNumbers))]
    public void TryParse_WholeNumber_ReturnsTrueAndNumber(string input, decimal expected)
    {
        var success = AnswerFormat.TryParse(input, out var result);

        Assert.True(success);
        Assert.Equal(expected, result);
    }

    // Spaces before and after the answer are removed before it is read
    [Fact]
    [Trait("TestCase", "TC-003")]
    public void TryParse_AnswerWithSurroundingSpaces_ReturnsTrueAndNumber()
    {
        var input = " 4 ";

        var success = AnswerFormat.TryParse(input, out var result);

        Assert.True(success);
        Assert.Equal(4m, result);
    }

    // An empty answer is rejected
    [Theory]
    [Trait("TestCase", "TC-004")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void TryParse_EmptyAnswer_ReturnsFalse(string? input)
    {
        var success = AnswerFormat.TryParse(input, out _);

        Assert.False(success);
    }

    // An answer with a period is rejected, whether the period is meant as a decimal or a thousands separator
    [Theory]
    [Trait("TestCase", "TC-005")]
    [InlineData("3.5")]
    [InlineData("1.000")]
    public void TryParse_AnswerWithPeriod_ReturnsFalse(string input)
    {
        var success = AnswerFormat.TryParse(input, out _);

        Assert.False(success);
    }

    // An answer that is not a number is rejected
    [Theory]
    [Trait("TestCase", "TC-006")]
    [InlineData("tre")]
    [InlineData("1/2")]
    [InlineData("3 cm")]
    public void TryParse_AnswerThatIsNotANumber_ReturnsFalse(string input)
    {
        var success = AnswerFormat.TryParse(input, out _);

        Assert.False(success);
    }

    // The answer is read the same way when the server runs with English settings
    [Theory]
    [Trait("TestCase", "TC-012")]
    [InlineData("3,5", true)]
    [InlineData("3.5", false)]
    public void TryParse_EnglishCurrentCulture_GivesSameResultAsDanish(string input, bool expectedSuccess)
    {
        using var culture = new CultureScope("en-US");

        var success = AnswerFormat.TryParse(input, out _);

        Assert.Equal(expectedSuccess, success);
    }
}