using FluentAssertions;
using stargripcorp.dataplatform.infra.utils.Naming;

namespace stargripcorp.dataplatform.infra.tests;

public class StringExpressionsTests
{
    [Fact]
    public void NoSpecialCharactersLowerCase_ReturnsRegexThatRemovesSpecialCharacters()
    {
        // Arrange
        var regex = StringExpressions.NoSpecialCharactersLowerCase();

        // Act
        var result = regex.Replace("abc!@#123", "");

        // Assert
        result.Should().Be("abc123");
    }

    [Fact]
    public void NoSpecialCharactersLowerCase_ReturnsRegexThatDoesNotRemoveLowerCaseAlphanumericCharacters()
    {
        // Arrange
        var regex = StringExpressions.NoSpecialCharactersLowerCase();

        // Act
        var result = regex.Replace("abc123", "");

        // Assert
        result.Should().Be("abc123");
    }

    [Fact]
    public void AllowDashes_ReturnsRegexThatRemovesSpecialCharacters()
    {
        // Arrange
        var regex = StringExpressions.AllowDashes();

        // Act
        var result = regex.Replace("abc!@#-123", "");

        // Assert
        result.Should().Be("abc-123");
    }

    [Fact]
    public void AllowDashes_ReturnsRegexThatDoesNotRemoveAlphanumericCharactersAndDashes()
    {
        // Arrange
        var regex = StringExpressions.AllowDashes();

        // Act
        var result = regex.Replace("abc-123", "");

        // Assert
        result.Should().Be("abc-123");
    }
}