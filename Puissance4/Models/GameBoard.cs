using System;
using System.Collections.Generic;

namespace Puissance4.Models
{
    public class GameBoard
    {
        // Standard Connect 4 board dimensions
        public const int Rows = 6;
        public const int Columns = 7;

        // 0 = empty, 1 = player 1, 2 = player 2
        private int[,] _board;

        public GameBoard()
        {
            _board = new int[Rows, Columns];
            // Initialize the board with all cells empty (0)
            Reset();
        }

        public void Reset()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    _board[row, col] = 0;
                }
            }
        }

        public int GetCellValue(int row, int column)
        {
            if (row < 0 || row >= Rows || column < 0 || column >= Columns)
                throw new ArgumentOutOfRangeException("Invalid cell position");

            return _board[row, column];
        }

        // Returns the board as a 2D array for the ViewModel to use
        public int[,] GetBoard()
        {
            return (int[,])_board.Clone();
        }

        // Places a token in the specified column at the lowest available position
        // Returns the row where the token was placed, or -1 if the column is full
        public int PlaceToken(int column, int playerNumber)
        {
            if (column < 0 || column >= Columns)
                throw new ArgumentOutOfRangeException(nameof(column), "Column index out of range");

            if (playerNumber != 1 && playerNumber != 2)
                throw new ArgumentOutOfRangeException(nameof(playerNumber), "Player number must be 1 or 2");

            // Start from the bottom row and move up until we find an empty cell
            for (int row = Rows - 1; row >= 0; row--)
            {
                if (_board[row, column] == 0)
                {
                    _board[row, column] = playerNumber;
                    return row;
                }
            }

            // Column is full
            return -1;
        }

        // Checks if the specified player has won
        public bool CheckForWin(int playerNumber, int lastRow, int lastColumn)
        {
            // Check horizontal
            if (CheckDirection(playerNumber, lastRow, lastColumn, 0, 1))
                return true;

            // Check vertical
            if (CheckDirection(playerNumber, lastRow, lastColumn, 1, 0))
                return true;

            // Check diagonal (top-left to bottom-right)
            if (CheckDirection(playerNumber, lastRow, lastColumn, 1, 1))
                return true;

            // Check diagonal (bottom-left to top-right)
            if (CheckDirection(playerNumber, lastRow, lastColumn, -1, 1))
                return true;

            return false;
        }

        // Checks if the board is full (draw)
        public bool IsBoardFull()
        {
            for (int col = 0; col < Columns; col++)
            {
                if (_board[0, col] == 0) // Check top row
                    return false;
            }
            return true;
        }

        // Checks if a column is full
        public bool IsColumnFull(int column)
        {
            return _board[0, column] != 0;
        }

        // Helper method to check for 4 in a row in a specific direction
        private bool CheckDirection(int playerNumber, int row, int col, int rowDelta, int colDelta)
        {
            int count = 1; // Start with 1 for the piece just placed

            // Check in the positive direction
            for (int i = 1; i < 4; i++)
            {
                int newRow = row + (i * rowDelta);
                int newCol = col + (i * colDelta);

                if (newRow < 0 || newRow >= Rows || newCol < 0 || newCol >= Columns)
                    break;

                if (_board[newRow, newCol] == playerNumber)
                    count++;
                else
                    break;
            }

            // Check in the negative direction
            for (int i = 1; i < 4; i++)
            {
                int newRow = row - (i * rowDelta);
                int newCol = col - (i * colDelta);

                if (newRow < 0 || newRow >= Rows || newCol < 0 || newCol >= Columns)
                    break;

                if (_board[newRow, newCol] == playerNumber)
                    count++;
                else
                    break;
            }

            return count >= 4;
        }
    }
} 