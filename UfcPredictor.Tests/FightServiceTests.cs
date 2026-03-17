using Moq;
using Xunit;
using FluentAssertions;
using HtmlAgilityPack;
using UfcPredictor.Lib;

namespace UfcPredictor.Tests;

public class FightServiceTests
{
    private readonly Mock<IWebLoader> _mockLoader;
    private readonly FightService _sut;

    public FightServiceTests()
    {
        _mockLoader = new Mock<IWebLoader>();
        _sut = new FightService(_mockLoader.Object);
    }

    [Fact]
    public async Task GetUpcomingFightsAsync_ValidTable_ReturnsFights()
    {
        // Arrange
        var html = @"
            <table>
                <tr class='b-fight-details__table-row'>
                    <td>
                        <a href='url1' class='b-link b-link_style_black'>Jon Jones</a>
                        <a href='url2' class='b-link b-link_style_black'>Stipe Miocic</a>
                    </td>
                </tr>
            </table>";
        SetupMock(html);

        // Act
        var result = await _sut.GetUpcomingFightsAsync("http://test.com");

        // Assert
        result.Should().HaveCount(1);
        result[0].FighterOne.Should().Be("Jon Jones");
        result[0].FighterTwo.Should().Be("Stipe Miocic");
    }

    [Fact]
    public async Task GetFighterDetailsAsync_ExtractsStatsCorrectly()
    {
        // Arrange: Mirrors the Adesanya HTML structure
        var html = @"
            <span class='b-content__title-highlight'>Israel Adesanya</span>
            <span class='b-content__title-record'>Record: 24-5-0</span>
            <li class='b-list__box-list-item'>
                <i class='b-list__box-item-title'>Height:</i> 6' 4""
            </li>
            <li class='b-list__box-list-item'>
                <i class='b-list__box-item-title'>Reach:</i> 80""
            </li>
            <li class='b-list__box-list-item'>
                <i class='b-list__box-item-title'>STANCE:</i> Switch
            </li>";
        SetupMock(html);

        // Act
        var result = await _sut.GetFighterDetailsAsync("http://test.com/fighter");

        // Assert
        result.Name.Should().Be("Israel Adesanya");
        result.Height.Should().Be("6' 4\"");
        result.Reach.Should().Be("80\"");
        result.Stance.Should().Be("Switch");
    }

    private void SetupMock(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        _mockLoader.Setup(x => x.LoadFromWebAsync(It.IsAny<string>()))
                   .ReturnsAsync(doc);
    }
}