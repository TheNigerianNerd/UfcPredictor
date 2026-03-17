using FluentAssertions;
using HtmlAgilityPack;
using Moq;
using UfcPredictor.Lib;

namespace UfcPredictor.Tests;

public class EventServiceTests
{
    private readonly Mock<IWebLoader> _mockWebLoader;
    private readonly EventService _sut; // System Under Test

    public EventServiceTests()
    {
        _mockWebLoader = new Mock<IWebLoader>();
        _sut = new EventService(_mockWebLoader.Object);
    }

    [Fact]
    public async Task GetUpcomingEvents_ValidHtml_ReturnsPopulatedList()
    {
        // Arrange
        var html = @"
        <table>
            <tr class='b-statistics__table-row'>
                  <td class='b-statistics__table-col'>
                    <i class='b-statistics__table-content'>
                        <a href='http://ufcstats.com/event-details/69108cb8b32efe04' class='b-link b-link_style_black'>
                          UFC Fight Night: Evloev vs. Murphy
                        </a>
                        <span class='b-statistics__date'>
                          March 21, 2026
                        </span>
                    </i>
                  </td>
                  <td class='b-statistics__table-col b-statistics__table-col_style_big-top-padding'>
                    London, England, United Kingdom
                  </td>
                </tr>
        </table>";

        SetupMockDocument(html);

        // Act
        var result = await _sut.GetUpcomingEvents("http://fakeurl.com");

        // Exact matches based on the mock HTML above
        result[0].Name.Should().Be("UFC Fight Night: Evloev vs. Murphy");
        result[0].Date.Should().Be("March 21, 2026");
        result[0].Location.Should().Be("London, England, United Kingdom");
        result[0].Url.Should().Contain("69108cb8b32efe04");
    }
    [Fact]
    public async Task GetUpcomingEvents_NoMatchingRows_ReturnsEmptyList()
    {
        // Arrange
        var html = "<div>No table here</div>";
        SetupMockDocument(html);

        // Act
        var result = await _sut.GetUpcomingEvents("http://fakeurl.com");

        // Assert
        result.Should().BeEmpty();
    }
    [Fact]
    public async Task GetUpcomingEvents_MissingLinkNode_SkipsRow()
    {
        // Arrange: Row exists but <a> tag is missing
        var html = @"<tr class='b-statistics__table-row'>
                        <td><i>No Link Here</i></td>
                     </tr>";
        SetupMockDocument(html);

        // Act
        var result = await _sut.GetUpcomingEvents("http://fakeurl.com");

        // Assert
        result.Should().BeEmpty();
    }
    [Fact]
    public async Task GetUpcomingEvents_WithMessyWhitespace_ReturnsCleanStrings()
    {
        // Arrange
        var messyHtml = @"
        <tr class='b-statistics__table-row'>
                  <td class='b-statistics__table-col'>                  
                    <i class='b-statistics__table-content'>
                        <a href='http://ufcstats.com/event-details/babc6b5745335f18' class='b-link b-link_style_black'>

                          UFC Fight Night: Emmett vs. Vallejos

                        </a>
                        <span class='b-statistics__date'>
                          March 14, 2026
                        </span>
                    </i>
                  </td>
                  <td class='b-statistics__table-col b-statistics__table-col_style_big-top-padding'>
                    Las Vegas, Nevada, USA
                  </td>
                </tr>";

        SetupMockDocument(messyHtml);

        // Act
        var result = await _sut.GetUpcomingEvents("http://fake.com");

        // Assert
        result[0].Name.Should().Be("UFC Fight Night: Emmett vs. Vallejos");
    }
    [Fact]
public async Task GetUpcomingEvents_WhenLinkIsMissing_SkipsRow()
{
    // Arrange: Valid row structure but no <a> tag inside <i>
    var missingLinkHtml = @"
        <tr class='b-statistics__table-row'>
            <td><i>Link Coming Soon</i></td>
            <td><span class='b-statistics__date'>Jan 1, 2026</span></td>
        </tr>";
    
    SetupMockDocument(missingLinkHtml);

    // Act
    var result = await _sut.GetUpcomingEvents("http://fake.com");

    // Assert
    result.Should().BeEmpty();
    }
    // --- Helper Method ---
    private void SetupMockDocument(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        _mockWebLoader.Setup(x => x.LoadFromWebAsync(It.IsAny<string>()))
                   .ReturnsAsync(doc);
    }
}
