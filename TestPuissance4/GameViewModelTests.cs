using Puissance4.ViewModels;
using System.Reflection;
using Xunit;

namespace TestPuissance4;

public class GameViewModelTests
{
    [Fact]
    public void Constructor_ShouldInitializeEmptyBoard()
    {
        // Arrange & Act
        var viewModel = new GameViewModel();
        
        // Assert
        Assert.NotNull(viewModel.Cells);
        Assert.Equal(6, viewModel.Cells.Count); // 6 rows
        Assert.Equal(7, viewModel.Cells[0].Count); // 7 columns
        
        // Check all cells are empty
        foreach (var row in viewModel.Cells)
        {
            Assert.NotNull(row);
            foreach (var cell in row)
            {
                Assert.NotNull(cell);
                Assert.Equal(0, cell.CellValue);
            }
        }
    }
    
    [Fact]
    public void PlaceToken_ShouldUpdateCellValue()
    {
        // Arrange
        var viewModel = new GameViewModel();
        string columnIndex = "3"; // Middle column
        
        // Act
        // Use reflection to call the private method
        var method = typeof(GameViewModel).GetMethod("OnPlaceToken", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(method); // Ensure method exists
        method!.Invoke(viewModel, new object[] { columnIndex });
        
        // Assert
        Assert.NotNull(viewModel.Cells);
        Assert.NotNull(viewModel.Cells[5]);
        Assert.NotNull(viewModel.Cells[5][3]);
        Assert.Equal(1, viewModel.Cells[5][3].CellValue); // Bottom row, middle column should have player 1's token
    }
    
    [Fact]
    public void PlaceToken_ShouldSwitchPlayers()
    {
        // Arrange
        var viewModel = new GameViewModel();
        string columnIndex = "3"; // Middle column
        
        // Act
        // Use reflection to call the private method twice
        var method = typeof(GameViewModel).GetMethod("OnPlaceToken", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(method); // Ensure method exists
        method!.Invoke(viewModel, new object[] { columnIndex });
        method.Invoke(viewModel, new object[] { "4" }); // Different column
        
        // Assert
        Assert.NotNull(viewModel.Cells);
        Assert.NotNull(viewModel.Cells[5]);
        Assert.NotNull(viewModel.Cells[5][3]);
        Assert.NotNull(viewModel.Cells[5][4]);
        Assert.Equal(1, viewModel.Cells[5][3].CellValue); // Player 1's token
        Assert.Equal(2, viewModel.Cells[5][4].CellValue); // Player 2's token
    }
    
    [Fact]
    public void ResetGame_ShouldClearBoard()
    {
        // Arrange
        var viewModel = new GameViewModel();
        string columnIndex = "3"; // Middle column
        
        // Place a token
        var placeMethod = typeof(GameViewModel).GetMethod("OnPlaceToken", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(placeMethod); // Ensure method exists
        placeMethod!.Invoke(viewModel, new object[] { columnIndex });
        
        // Act
        var resetMethod = typeof(GameViewModel).GetMethod("ResetGame", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(resetMethod); // Ensure method exists
        resetMethod!.Invoke(viewModel, null);
        
        // Assert
        Assert.NotNull(viewModel.Cells);
        // Check all cells are empty
        foreach (var row in viewModel.Cells)
        {
            Assert.NotNull(row);
            foreach (var cell in row)
            {
                Assert.NotNull(cell);
                Assert.Equal(0, cell.CellValue);
            }
        }
        
        // Check game status is reset
        Assert.Equal("Player 1's turn", viewModel.GameStatus);
    }
    
    [Fact]
    public void GameStatus_ShouldUpdateOnPlayerChange()
    {
        // Arrange
        var viewModel = new GameViewModel();
        string columnIndex = "3"; // Middle column
        
        // Act
        var method = typeof(GameViewModel).GetMethod("OnPlaceToken", 
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.NotNull(method); // Ensure method exists
        method!.Invoke(viewModel, new object[] { columnIndex });
        
        // Assert
        Assert.Equal("Player 2's turn", viewModel.GameStatus);
    }
} 