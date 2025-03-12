using Puissance4.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace Puissance4.ViewModels
{
    public class GameViewModel : BaseViewModel
    {
        private GameBoard _gameBoard;
        private ObservableCollection<ObservableCollection<CellViewModel>> _cells;
        private int _currentPlayer = 1; // Player 1 starts
        private bool _gameOver = false;
        private string _gameStatus = "Player 1's turn";

        public ObservableCollection<ObservableCollection<CellViewModel>> Cells
        {
            get => _cells;
            set => SetProperty(ref _cells, value);
        }

        public string GameStatus
        {
            get => _gameStatus;
            set => SetProperty(ref _gameStatus, value);
        }

        public ICommand PlaceTokenCommand { get; private set; }
        public ICommand ResetGameCommand { get; private set; }

        public GameViewModel()
        {
            _cells = new ObservableCollection<ObservableCollection<CellViewModel>>();
            _gameBoard = new GameBoard();
            InitializeBoard();
            
            // Initialize commands
            PlaceTokenCommand = new Command<string>(OnPlaceToken, CanPlaceToken);
            ResetGameCommand = new Command(ResetGame);
        }

        private bool CanPlaceToken(string columnIndexStr)
        {
            // Can't place tokens if the game is over
            if (_gameOver)
                return false;

            // Check if the column is valid and not full
            if (int.TryParse(columnIndexStr, out int columnIndex) && columnIndex >= 0 && columnIndex < GameBoard.Columns)
            {
                return !_gameBoard.IsColumnFull(columnIndex);
            }

            return false;
        }

        private void OnPlaceToken(string columnIndexStr)
        {
            if (int.TryParse(columnIndexStr, out int columnIndex) && columnIndex >= 0 && columnIndex < GameBoard.Columns)
            {
                // Place the token in the game board model
                int rowPlaced = _gameBoard.PlaceToken(columnIndex, _currentPlayer);
                
                // If rowPlaced is -1, the column is full
                if (rowPlaced >= 0)
                {
                    // Update the cell in our view model
                    Cells[rowPlaced][columnIndex].CellValue = _currentPlayer;
                    
                    // Check for win
                    if (_gameBoard.CheckForWin(_currentPlayer, rowPlaced, columnIndex))
                    {
                        _gameOver = true;
                        GameStatus = $"Player {_currentPlayer} wins!";
                        // Refresh the command's CanExecute state
                        ((Command<string>)PlaceTokenCommand).ChangeCanExecute();
                        return;
                    }
                    
                    // Check for draw
                    if (_gameBoard.IsBoardFull())
                    {
                        _gameOver = true;
                        GameStatus = "Game ended in a draw!";
                        // Refresh the command's CanExecute state
                        ((Command<string>)PlaceTokenCommand).ChangeCanExecute();
                        return;
                    }
                    
                    // Switch players
                    _currentPlayer = _currentPlayer == 1 ? 2 : 1;
                    GameStatus = $"Player {_currentPlayer}'s turn";
                    
                    // Refresh the command's CanExecute state for all columns
                    ((Command<string>)PlaceTokenCommand).ChangeCanExecute();
                }
            }
        }

        private void ResetGame()
        {
            _gameBoard.Reset();
            _currentPlayer = 1;
            _gameOver = false;
            GameStatus = "Player 1's turn";
            InitializeBoard();
            
            // Refresh the command's CanExecute state
            ((Command<string>)PlaceTokenCommand).ChangeCanExecute();
        }

        private void InitializeBoard()
        {
            Cells = new ObservableCollection<ObservableCollection<CellViewModel>>();

            for (int row = 0; row < GameBoard.Rows; row++)
            {
                var rowCells = new ObservableCollection<CellViewModel>();
                
                for (int col = 0; col < GameBoard.Columns; col++)
                {
                    rowCells.Add(new CellViewModel
                    {
                        Row = row,
                        Column = col,
                        CellValue = _gameBoard.GetCellValue(row, col)
                    });
                }
                
                Cells.Add(rowCells);
            }
        }
    }

    public class CellViewModel : BaseViewModel
    {
        private int _row;
        private int _column;
        private int _cellValue; // 0 = empty, 1 = player 1, 2 = player 2

        public int Row
        {
            get => _row;
            set => SetProperty(ref _row, value);
        }

        public int Column
        {
            get => _column;
            set => SetProperty(ref _column, value);
        }

        public int CellValue
        {
            get => _cellValue;
            set 
            { 
                if (SetProperty(ref _cellValue, value))
                {
                    // Explicitly notify that CellColor has changed when CellValue changes
                    OnPropertyChanged(nameof(CellColor));
                }
            }
        }

        public Color CellColor
        {
            get
            {
                return CellValue switch
                {
                    0 => Colors.LightGray,
                    1 => Colors.Red, // More visible color for player 1
                    2 => Colors.Yellow, // More visible color for player 2
                    _ => Colors.LightGray
                };
            }
        }
    }
} 