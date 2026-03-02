using FluentAssertions;
using HealthTrack.Infrastructure.Services;

namespace HealthTrack.Infrastructure.Tests.Services;

public class DrugInteractionServiceTests
{
    private readonly DrugInteractionService _service = new();

    [Fact]
    public async Task CheckInteractions_KnownPair_WarfarinAndAspirin_ShouldReturnInteraction()
    {
        // Arrange
        var medication = "Warfarin";
        var currentMedications = new List<string> { "Aspirin" };

        // Act
        var result = await _service.CheckInteractionsAsync(medication, currentMedications);

        // Assert
        result.Should().HaveCount(1);
        result[0].Drug1.Should().Be("Warfarin");
        result[0].Drug2.Should().Be("Aspirin");
        result[0].Severity.Should().Be("High");
        result[0].Description.Should().Contain("bleeding");
    }

    [Fact]
    public async Task CheckInteractions_KnownPair_ReversedOrder_ShouldStillReturnInteraction()
    {
        // Arrange
        var medication = "Aspirin";
        var currentMedications = new List<string> { "Warfarin" };

        // Act
        var result = await _service.CheckInteractionsAsync(medication, currentMedications);

        // Assert
        result.Should().HaveCount(1);
        result[0].Drug1.Should().Be("Aspirin");
        result[0].Drug2.Should().Be("Warfarin");
    }

    [Fact]
    public async Task CheckInteractions_UnknownPair_ShouldReturnEmpty()
    {
        // Arrange
        var medication = "Vitamin D";
        var currentMedications = new List<string> { "Calcium" };

        // Act
        var result = await _service.CheckInteractionsAsync(medication, currentMedications);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CheckInteractions_CaseInsensitive_ShouldFindInteraction()
    {
        // Arrange
        var medication = "warfarin";
        var currentMedications = new List<string> { "aspirin" };

        // Act
        var result = await _service.CheckInteractionsAsync(medication, currentMedications);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task CheckInteractions_MixedCase_ShouldFindInteraction()
    {
        // Arrange
        var medication = "WARFARIN";
        var currentMedications = new List<string> { "aSPiRiN" };

        // Act
        var result = await _service.CheckInteractionsAsync(medication, currentMedications);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task CheckInteractions_MultipleCurrentMeds_ShouldReturnMultipleInteractions()
    {
        // Arrange - Warfarin interacts with both Aspirin and Ibuprofen
        var medication = "Warfarin";
        var currentMedications = new List<string> { "Aspirin", "Ibuprofen" };

        // Act
        var result = await _service.CheckInteractionsAsync(medication, currentMedications);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(i => i.Drug2 == "Aspirin");
        result.Should().Contain(i => i.Drug2 == "Ibuprofen");
    }

    [Fact]
    public async Task CheckInteractions_EmptyCurrentMedications_ShouldReturnEmpty()
    {
        // Arrange
        var medication = "Warfarin";
        var currentMedications = new List<string>();

        // Act
        var result = await _service.CheckInteractionsAsync(medication, currentMedications);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CheckInteractions_SimvastatinAndAmiodarone_ShouldReturnHighSeverity()
    {
        // Arrange
        var medication = "Simvastatin";
        var currentMedications = new List<string> { "Amiodarone" };

        // Act
        var result = await _service.CheckInteractionsAsync(medication, currentMedications);

        // Assert
        result.Should().HaveCount(1);
        result[0].Severity.Should().Be("High");
        result[0].Description.Should().Contain("rhabdomyolysis");
    }

    [Fact]
    public async Task CheckInteractions_LisinoprilAndPotassium_ShouldReturnModerateSeverity()
    {
        // Arrange
        var medication = "Lisinopril";
        var currentMedications = new List<string> { "Potassium" };

        // Act
        var result = await _service.CheckInteractionsAsync(medication, currentMedications);

        // Assert
        result.Should().HaveCount(1);
        result[0].Severity.Should().Be("Moderate");
        result[0].Description.Should().Contain("hyperkalemia");
    }

    [Fact]
    public async Task CheckInteractions_SubstringMatch_ShouldFindInteraction()
    {
        // Arrange - the service uses .Contains(), so "Warfarin 5mg" should match "Warfarin"
        var medication = "Warfarin 5mg";
        var currentMedications = new List<string> { "Aspirin 81mg" };

        // Act
        var result = await _service.CheckInteractionsAsync(medication, currentMedications);

        // Assert
        result.Should().HaveCount(1);
    }
}
