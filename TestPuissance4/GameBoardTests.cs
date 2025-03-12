using Puissance4.Models;
using Xunit;

namespace TestPuissance4;

public class GameBoardTests
{
    [Fact]
    public void PlaceToken_ShouldPlaceAtBottomRow()
    {
        // Arrange
        var gameBoard = new GameBoard();
        int column = 3;
        int player = 1;
        
        // Act
        int rowPlaced = gameBoard.PlaceToken(column, player);
        
        // Assert
        Assert.Equal(GameBoard.Rows - 1, rowPlaced); // Should be placed at the bottom row
        Assert.Equal(player, gameBoard.GetCellValue(rowPlaced, column));
    }
    
    [Fact]
    public void PlaceToken_ShouldStackOnTopOfExistingTokens()
    {
        // Arrange
        var gameBoard = new GameBoard();
        int column = 3;
        
        // Act
        int firstRow = gameBoard.PlaceToken(column, 1); // Player 1 places first
        int secondRow = gameBoard.PlaceToken(column, 2); // Player 2 places on top
        
        // Assert
        Assert.Equal(GameBoard.Rows - 1, firstRow); // First token at bottom
        Assert.Equal(firstRow - 1, secondRow); // Second token one row above
        Assert.Equal(1, gameBoard.GetCellValue(firstRow, column));
        Assert.Equal(2, gameBoard.GetCellValue(secondRow, column));
    }
    
    [Fact]
    public void PlaceToken_ShouldReturnNegativeOneWhenColumnIsFull()
    {
        // Arrange
        var gameBoard = new GameBoard();
        int column = 3;
        
        // Fill the column
        for (int i = 0; i < GameBoard.Rows; i++)
        {
            int row = gameBoard.PlaceToken(column, 1);
            Assert.True(row >= 0); // Ensure token was placed successfully
        }
        
        // Act
        int result = gameBoard.PlaceToken(column, 2);
        
        // Assert
        Assert.Equal(-1, result);
    }
    
    [Fact]
    public void CheckForWin_ShouldDetectHorizontalWin()
    {
        // Arrange
        var gameBoard = new GameBoard();
        int player = 1;
        int row = GameBoard.Rows - 1; // Bottom row
        
        // Place 4 tokens in a row horizontally
        for (int col = 0; col < 4; col++)
        {
            int placedRow = gameBoard.PlaceToken(col, player);
            Assert.Equal(row, placedRow); // Ensure token was placed in the bottom row
        }
        
        // Act & Assert
        Assert.True(gameBoard.CheckForWin(player, row, 3)); // Check from the last placed token
    }
    
    [Fact]
    public void CheckForWin_ShouldDetectVerticalWin()
    {
        // Arrange
        var gameBoard = new GameBoard();
        int player = 2;
        int column = 3;
        int lastRow = -1; // Initialize to invalid value
        
        // Place 4 tokens in the same column
        for (int i = 0; i < 4; i++)
        {
            int row = gameBoard.PlaceToken(column, player);
            Assert.True(row >= 0); // Ensure token was placed successfully
            lastRow = row; // Keep track of the last row
        }
        
        // Act & Assert
        Assert.True(lastRow >= 0); // Ensure we have a valid row
        Assert.True(gameBoard.CheckForWin(player, lastRow, column));
    }
    
    [Fact]
    public void CheckForWin_ShouldDetectDiagonalWin()
    {
        // Arrange
        var gameBoard = new GameBoard();
        int player = 1;
        
        // Create a diagonal win pattern
        // X . . .
        // O X . .
        // O O X .
        // O O O X
        
        // Place tokens to create the pattern
        Assert.True(gameBoard.PlaceToken(0, 2) >= 0); // Column 0: Player 2
        Assert.True(gameBoard.PlaceToken(0, 2) >= 0); // Column 0: Player 2
        Assert.True(gameBoard.PlaceToken(0, 2) >= 0); // Column 0: Player 2
        int row0 = gameBoard.PlaceToken(0, player); // Column 0: Player 1 (top)
        Assert.True(row0 >= 0);
        
        Assert.True(gameBoard.PlaceToken(1, 2) >= 0); // Column 1: Player 2
        Assert.True(gameBoard.PlaceToken(1, 2) >= 0); // Column 1: Player 2
        int row1 = gameBoard.PlaceToken(1, player); // Column 1: Player 1
        Assert.True(row1 >= 0);
        
        Assert.True(gameBoard.PlaceToken(2, 2) >= 0); // Column 2: Player 2
        int row2 = gameBoard.PlaceToken(2, player); // Column 2: Player 1
        Assert.True(row2 >= 0);
        
        int row3 = gameBoard.PlaceToken(3, player); // Column 3: Player 1
        Assert.True(row3 >= 0);
        
        // Act & Assert
        Assert.True(gameBoard.CheckForWin(player, row3, 3));
    }
    
    [Fact]
    public void IsBoardFull_ShouldReturnTrueWhenBoardIsFull()
    {
        // Arrange
        var gameBoard = new GameBoard();
        
        // Fill the board with alternating players
        for (int col = 0; col < GameBoard.Columns; col++)
        {
            for (int i = 0; i < GameBoard.Rows; i++)
            {
                int player = (col + i) % 2 + 1; // Alternating 1 and 2
                int row = gameBoard.PlaceToken(col, player);
                Assert.True(row >= 0); // Ensure token was placed successfully
            }
        }
        
        // Act & Assert
        Assert.True(gameBoard.IsBoardFull());
    }
    
    [Fact]
    public void Reset_ShouldClearTheBoard()
    {
        // Arrange
        var gameBoard = new GameBoard();
        
        // Place some tokens
        Assert.True(gameBoard.PlaceToken(0, 1) >= 0);
        Assert.True(gameBoard.PlaceToken(1, 2) >= 0);
        Assert.True(gameBoard.PlaceToken(2, 1) >= 0);
        
        // Act
        gameBoard.Reset();
        
        // Assert - Check that all cells are empty (0)
        for (int row = 0; row < GameBoard.Rows; row++)
        {
            for (int col = 0; col < GameBoard.Columns; col++)
            {
                Assert.Equal(0, gameBoard.GetCellValue(row, col));
            }
        }
    }
} 