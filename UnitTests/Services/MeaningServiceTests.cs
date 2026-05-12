using Microsoft.EntityFrameworkCore;
using Prictionary;
using Prictionary.DTOs;
using Prictionary.Models;
using Prictionary.Services.Implementation;
using Prictionary.Services.Infrastructure;
using Prictionary.Services.Interfaces;
using UnitTests.Infrastructure;

namespace UnitTests.Services;

public class MeaningServiceTests : TestsWithDbContext
{
    private readonly Mock<IAccessChecker<Meaning>> _accessCheckerMock;
    private readonly MeaningsService _service;

    public MeaningServiceTests()
    {
        _accessCheckerMock = new Mock<IAccessChecker<Meaning>>();
        _accessCheckerMock
            .Setup(a => a.CanChangeAsync(It.IsAny<Meaning>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _service = new MeaningsService(DbContext, _accessCheckerMock.Object);
    }

    private async Task<LanguageUnit> SeedLanguageUnitAsync()
    {
        var lu = new LanguageUnit { Content = "test unit" };
        DbContext.LanguageUnits.Add(lu);
        await DbContext.SaveChangesAsync();
        return lu;
    }

    private async Task<Meaning> SeedMeaningAsync(int languageUnitId, int priority, string content = "meaning")
    {
        var meaning = new Meaning
        {
            Content = content,
            LanguageUnitId = languageUnitId,
            LanguageUnit = null!,
            Priority = priority
        };
        DbContext.Meanings.Add(meaning);
        await DbContext.SaveChangesAsync();
        return meaning;
    }

    #region CreateMeaningAsync

    [Fact]
    public async Task CreateMeaningAsync_NoPosition_EmptyList_AssignsPriorityZero()
    {
        // Arrange
        var lu = await SeedLanguageUnitAsync();

        // Act
        var result = await _service.CreateMeaningAsync(lu.Id, "word");

        // Assert
        result.Success.Should().BeTrue();
        result.Value!.Priority.Should().Be(0);
    }

    [Fact]
    public async Task CreateMeaningAsync_NoPosition_ExistingMeanings_AssignsBelowLowest()
    {
        // Arrange
        const int basePriority = 1000;
        var lu = await SeedLanguageUnitAsync();
        await SeedMeaningAsync(lu.Id, basePriority);

        // Act
        var result = await _service.CreateMeaningAsync(lu.Id, "word");

        // Assert
        result.Success.Should().BeTrue();
        result.Value!.Priority.Should().Be(basePriority - Constants.MeaningPriorities.defaultInterval);
    }

    [Fact]
    public async Task CreateMeaningAsync_PositionAbove_RefIsTop_AssignsPriorityAboveRef()
    {
        // Arrange
        const int basePriority = 1000;
        var lu = await SeedLanguageUnitAsync();
        var refMeaning = await SeedMeaningAsync(lu.Id, basePriority);
        var position = new Position { Placement = Position.PlacementType.Above, MeaningId = refMeaning.Id };

        // Act
        var result = await _service.CreateMeaningAsync(lu.Id, "word", position);

        // Assert
        result.Success.Should().BeTrue();
        result.Value!.Priority.Should().Be(basePriority + Constants.MeaningPriorities.defaultInterval);
    }

    [Fact]
    public async Task CreateMeaningAsync_PositionAbove_RefHasUpperNeighbor_AssignsMidpoint()
    {
        // Arrange
        const int refPriority = 1000;
        const int upperPriority = 2000;
        var lu = await SeedLanguageUnitAsync();
        var refMeaning = await SeedMeaningAsync(lu.Id, refPriority);
        await SeedMeaningAsync(lu.Id, upperPriority);
        var position = new Position { Placement = Position.PlacementType.Above, MeaningId = refMeaning.Id };

        // Act
        var result = await _service.CreateMeaningAsync(lu.Id, "word", position);

        // Assert
        result.Success.Should().BeTrue();
        result.Value!.Priority.Should().Be((refPriority + upperPriority) / 2);
    }

    [Fact]
    public async Task CreateMeaningAsync_PositionBelow_RefIsBottom_AssignsPriorityBelowRef()
    {
        // Arrange
        const int basePriority = 1000;
        var lu = await SeedLanguageUnitAsync();
        var refMeaning = await SeedMeaningAsync(lu.Id, basePriority);
        var position = new Position { Placement = Position.PlacementType.Below, MeaningId = refMeaning.Id };

        // Act
        var result = await _service.CreateMeaningAsync(lu.Id, "word", position);

        // Assert
        result.Success.Should().BeTrue();
        result.Value!.Priority.Should().Be(basePriority - Constants.MeaningPriorities.defaultInterval);
    }

    [Fact]
    public async Task CreateMeaningAsync_PositionBelow_RefHasLowerNeighbor_AssignsMidpoint()
    {
        // Arrange
        const int lowerPriority = 500;
        const int refPriority = 1500;
        var lu = await SeedLanguageUnitAsync();
        await SeedMeaningAsync(lu.Id, lowerPriority);
        var refMeaning = await SeedMeaningAsync(lu.Id, refPriority);
        var position = new Position { Placement = Position.PlacementType.Below, MeaningId = refMeaning.Id };

        // Act
        var result = await _service.CreateMeaningAsync(lu.Id, "word", position);

        // Assert
        result.Success.Should().BeTrue();
        result.Value!.Priority.Should().Be((lowerPriority + refPriority) / 2);
    }

    [Fact]
    public async Task CreateMeaningAsync_InvalidRefMeaningId_ReturnsNotFound()
    {
        // Arrange
        var lu = await SeedLanguageUnitAsync();
        var position = new Position { Placement = Position.PlacementType.Above, MeaningId = 999 };

        // Act
        var result = await _service.CreateMeaningAsync(lu.Id, "word", position);

        // Assert
        result.IsError(out var error).Should().BeTrue();
        error.Should().Be(ServiceErrors.NotFound);
    }

    [Fact]
    public async Task CreateMeaningAsync_NoGapBetweenNeighbors_TriggersRebalance()
    {
        // Arrange
        var lu = await SeedLanguageUnitAsync();
        var bottom = await SeedMeaningAsync(lu.Id, 0);
        await SeedMeaningAsync(lu.Id, 1); // adjacent, span < 2 — no room for midpoint
        var position = new Position { Placement = Position.PlacementType.Above, MeaningId = bottom.Id };

        // Act
        var result = await _service.CreateMeaningAsync(lu.Id, "word", position);

        // Assert
        result.Success.Should().BeTrue();
        var allMeanings = await DbContext.Meanings
            .Where(m => m.LanguageUnitId == lu.Id)
            .OrderBy(m => m.Priority)
            .ToListAsync();
        allMeanings.Should().HaveCount(3)
            .And.BeInAscendingOrder(m => m.Priority);
        // New meaning is inserted above bottom (second slot in ASC order)
        allMeanings[1].Id.Should().Be(result.Value!.Id);
    }

    #endregion
    #region UpdateMeaningByIdAsync

    [Fact]
    public async Task UpdateMeaningByIdAsync_MeaningNotFound_ReturnsNotFound()
    {
        // Arrange
        var newValues = new MeaningValues { Content = "updated" };

        // Act
        var result = await _service.UpdateMeaningByIdAsync(999, newValues, "user1");

        // Assert
        result.IsError(out var error).Should().BeTrue();
        error.Should().Be(ServiceErrors.NotFound);
    }

    [Fact]
    public async Task UpdateMeaningByIdAsync_AccessDenied_ReturnsAccessDenied()
    {
        // Arrange
        var lu = await SeedLanguageUnitAsync();
        var meaning = await SeedMeaningAsync(lu.Id, 1000);
        _accessCheckerMock
            .Setup(a => a.CanChangeAsync(It.IsAny<Meaning>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var newValues = new MeaningValues { Content = "updated" };

        // Act
        var result = await _service.UpdateMeaningByIdAsync(meaning.Id, newValues, "user1");

        // Assert
        result.IsError(out var error).Should().BeTrue();
        error.Should().Be(ServiceErrors.AccessDenied);
    }

    [Fact]
    public async Task UpdateMeaningByIdAsync_ContentOnly_UpdatesContentWithoutChangingPriority()
    {
        // Arrange
        const int basePriority = 1000;
        var lu = await SeedLanguageUnitAsync();
        var meaning = await SeedMeaningAsync(lu.Id, basePriority, "original");
        var newValues = new MeaningValues { Content = "updated" };

        // Act
        var result = await _service.UpdateMeaningByIdAsync(meaning.Id, newValues, "user1");

        // Assert
        result.Success.Should().BeTrue();
        var updateResult = result.Value!;
        updateResult.Value.Content.Should().Be("updated");
        updateResult.Value.Priority.Should().Be(basePriority);
        updateResult.Status.Should().Be(MeaningsUpdateResult.UpdateStatus.PrioritiesWasntChanged);
    }

    [Fact]
    public async Task UpdateMeaningByIdAsync_PositionAbove_ValidGap_SetsCorrectPriority()
    {
        // Arrange
        const int targetPriority = 500;
        const int refPriority = 1000;
        const int upperPriority = 2000;
        var lu = await SeedLanguageUnitAsync();
        var target = await SeedMeaningAsync(lu.Id, targetPriority);
        var refMeaning = await SeedMeaningAsync(lu.Id, refPriority);
        await SeedMeaningAsync(lu.Id, upperPriority); // upper neighbor of ref
        var newValues = new MeaningValues
        {
            Position = new Position { Placement = Position.PlacementType.Above, MeaningId = refMeaning.Id }
        };

        // Act
        var result = await _service.UpdateMeaningByIdAsync(target.Id, newValues, "user1");

        // Assert
        result.Success.Should().BeTrue();
        var updateResult = result.Value!;
        updateResult.Value.Priority.Should().Be((refPriority + upperPriority) / 2);
        updateResult.Status.Should().Be(MeaningsUpdateResult.UpdateStatus.PrioritiesWasntChanged);
    }

    [Fact]
    public async Task UpdateMeaningByIdAsync_PositionBelow_ValidGap_SetsCorrectPriority()
    {
        // Arrange
        const int lowerPriority = 500;
        const int refPriority = 1500;
        const int targetPriority = 3000;
        var lu = await SeedLanguageUnitAsync();
        await SeedMeaningAsync(lu.Id, lowerPriority); // lower neighbor of ref
        var refMeaning = await SeedMeaningAsync(lu.Id, refPriority);
        var target = await SeedMeaningAsync(lu.Id, targetPriority);
        var newValues = new MeaningValues
        {
            Position = new Position { Placement = Position.PlacementType.Below, MeaningId = refMeaning.Id }
        };

        // Act
        var result = await _service.UpdateMeaningByIdAsync(target.Id, newValues, "user1");

        // Assert
        result.Success.Should().BeTrue();
        var updateResult = result.Value!;
        updateResult.Value.Priority.Should().Be((lowerPriority + refPriority) / 2);
        updateResult.Status.Should().Be(MeaningsUpdateResult.UpdateStatus.PrioritiesWasntChanged);
    }

    [Fact]
    public async Task UpdateMeaningByIdAsync_RefMeaningNotFound_ReturnsNotFound()
    {
        // Arrange
        var lu = await SeedLanguageUnitAsync();
        var target = await SeedMeaningAsync(lu.Id, 1000);
        var newValues = new MeaningValues
        {
            Position = new Position { Placement = Position.PlacementType.Above, MeaningId = 999 }
        };

        // Act
        var result = await _service.UpdateMeaningByIdAsync(target.Id, newValues, "user1");

        // Assert
        result.IsError(out var error).Should().BeTrue();
        error.Should().Be(ServiceErrors.NotFound);
    }

    [Fact]
    public async Task UpdateMeaningByIdAsync_NoGapBetweenNeighbors_ReturnsRebalancedStatus()
    {
        // Arrange
        var lu = await SeedLanguageUnitAsync();
        var bottom = await SeedMeaningAsync(lu.Id, 0);
        await SeedMeaningAsync(lu.Id, 1); // adjacent to bottom, span < 2 — no room for midpoint
        var target = await SeedMeaningAsync(lu.Id, 5000);
        var newValues = new MeaningValues
        {
            Position = new Position { Placement = Position.PlacementType.Above, MeaningId = bottom.Id }
        };

        // Act
        var result = await _service.UpdateMeaningByIdAsync(target.Id, newValues, "user1");

        // Assert
        result.Success.Should().BeTrue();
        result.Value!.Status.Should().Be(MeaningsUpdateResult.UpdateStatus.PrioritiesRebalanced);
        var allMeanings = await DbContext.Meanings
            .Where(m => m.LanguageUnitId == lu.Id)
            .OrderBy(m => m.Priority)
            .ToListAsync();
        allMeanings.Should().BeInAscendingOrder(m => m.Priority);
    }
    #endregion
}
