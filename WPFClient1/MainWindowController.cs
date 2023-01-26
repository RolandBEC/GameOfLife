using GameOfLife.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPFClient1.Enum;
using WPFClient1.Infrastructure;

namespace WPFClient1
{
    public class MainWindowController
    {
        private System.Diagnostics.Stopwatch _stopWatch = new System.Diagnostics.Stopwatch();
        private DispatcherTimer dTimer;
        private CellModel[,] _cellGrid;
        private bool[,] _currentUniverse;
        Rectangle[,] BoardRef;
        int LiveCells = 0;

        private readonly MainWindow _mainWindow;
        private readonly EvolutionAlgorithm _evolutionAlgorithm = new EvolutionAlgorithm();

        public MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();

        public MainWindowController(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;


            ViewModel.ChangeSizeCommand = new RelayCommand<object>(_ => ExecuteChangeSizeCommand());
            ViewModel.RunStepCommand = new RelayCommand<object>(_ => ExecuteRunStepCommand(), _ => CanEvolveGeneration());
            ViewModel.PauseCommand = new RelayCommand<object>(_ => ExecutePauseCommand(), _ => ViewModel.CurrentRunState == ERunState.Running);
            ViewModel.RunCommand = new RelayCommand<object>(_ => ExecuteRunCommand(), _ => CanRunEvolveGeneration());

            ViewModel.ResetCommand = new RelayCommand<object>(
                _ => ExecuteResetCommand(),
                _ => CanResetGame()
            );

        }

        public void CreateBoard()
        {
            _mainWindow.cBoard.Children.Clear();
            BoardRef = new Rectangle[ViewModel.WidthX, ViewModel.WidthY];
            _cellGrid = new CellModel[ViewModel.WidthX, ViewModel.WidthY];
            for (int i = 0; i < ViewModel.WidthX; i++)
            {
                for (int j = 0; j < ViewModel.WidthY; j++)
                {
                    CellModel Cell = new CellModel { State = false, X = i, Y = j };
                    _cellGrid[i, j] = Cell;
                    Rectangle r = new Rectangle
                    {
                        Width = ViewModel.CellSize,
                        Height = ViewModel.CellSize,
                        Stroke = Brushes.Black,
                        StrokeThickness = 0.5,
                        Fill = Brushes.Black,
                        Tag = Cell
                    };
                    r.MouseDown += R_MouseDown;
                    r.MouseMove += R_MouseMove; ;
                    BoardRef[i, j] = r;
                    Canvas.SetLeft(r, j * ViewModel.CellSize);
                    Canvas.SetTop(r, i * ViewModel.CellSize);
                    _mainWindow.cBoard.Children.Add(r);
                }
            }
        }

        private CellModel lastSelectedCell;
        private void R_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton != MouseButtonState.Pressed ||
                (ViewModel.CurrentRunState != ERunState.NotStarted && ViewModel.CurrentRunState != ERunState.Paused))
                return;


            var cell = (CellModel)(sender as Rectangle).Tag;
            if(cell != lastSelectedCell)
            {
                ChangeCellState(cell);
            }
            lastSelectedCell = cell;
            _currentUniverse = null;
        }

        private void R_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.CurrentRunState == ERunState.NotStarted || ViewModel.CurrentRunState == ERunState.Paused)
            {
                var cell = (CellModel)(sender as Rectangle).Tag;
                ChangeCellState(cell);
                _currentUniverse = null;
            }
        }

        private async void ExecuteRunCommand()
        {
            dTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, ViewModel.DelayBetweenGeneration) };
            _currentUniverse = GetUniversionFromCurrentCellGrid();
            ViewModel.CurrentRunState = ERunState.Running;
            dTimer.Tick += DispatcherTimer_Tick;
            _stopWatch.Start();
            dTimer.Start();
        }

        private void ExecuteRunStepCommand()
        {
            if (LiveCells > 0)
            {
                if (_currentUniverse == null)
                    _currentUniverse = GetUniversionFromCurrentCellGrid();

                _stopWatch.Restart();

                bool[,] newUniverse = _evolutionAlgorithm.calculateNextBoard(_currentUniverse);

                ViewModel.CalculateNextBoardTimeMs = _stopWatch.ElapsedMilliseconds;

                ApplyRules(_currentUniverse, newUniverse);

                _stopWatch.Stop();
                ViewModel.CalculateApplyRuleTimeMs = _stopWatch.ElapsedMilliseconds - ViewModel.CalculateNextBoardTimeMs;

                _currentUniverse = newUniverse;
            }
            else
            {
                ViewModel.CurrentRunState = ERunState.Finished;
                _currentUniverse = null;
                dTimer.Stop();
            }
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            ExecuteRunStepCommand();
        }

        private bool[,] GetUniversionFromCurrentCellGrid()
        {
            bool[,] tmpUniverse = new bool[ViewModel.WidthX, ViewModel.WidthY];

            for (int i = 0; i < ViewModel.WidthX; i++)
            {
                for (int j = 0; j < ViewModel.WidthY; j++)
                {
                    tmpUniverse[i,j] = _cellGrid[i,j].State;
                }
            }
            return tmpUniverse;
        }

        void ApplyRules(bool[,] currentUniverse, bool[,] newUniverse)
        {
            for (int i = 0; i < ViewModel.WidthX; i++)
            {
                for (int j = 0; j < ViewModel.WidthY; j++)
                {
                    if(!currentUniverse[i, j])
                    {
                        if(newUniverse[i, j]) 
                            ChangeCellState(_cellGrid[i, j]);
                    }
                    else
                    {
                        if(!newUniverse[i, j])
                            ChangeCellState(_cellGrid[i, j]);
                    }
                }
            }

            if (LiveCells == 0)
                StopGame();
        }

        void ChangeCellState(CellModel cell)
        {
            if (!cell.State)
            {
                cell.State = true;
                BoardRef[cell.X, cell.Y].Fill = Brushes.White;
                LiveCells++;
            }
            else
            {
                cell.State = false;
                BoardRef[cell.X, cell.Y].Fill = Brushes.Black;
                LiveCells--;
            }
        }

        void StopGame()
        {
            dTimer.Stop();
            ViewModel.CurrentRunState = ERunState.Finished;
        }
                
        private bool CanRunEvolveGeneration()
        {
            return ViewModel.CurrentRunState == ERunState.NotStarted || ViewModel.CurrentRunState == ERunState.Paused;
        }

        private bool CanEvolveGeneration()
        {
            return ViewModel.CurrentRunState == ERunState.NotStarted || ViewModel.CurrentRunState == ERunState.Paused;
        }

        private void ExecutePauseCommand()
        {
            dTimer.Stop();
            ViewModel.CurrentRunState = ERunState.Paused;
        }

        void ExecuteResetCommand()
        {
            foreach (var tile in BoardRef)
            {
                if (tile.Tag != null)
                {
                    var cell = (CellModel)tile.Tag;
                    if (cell.State)
                        ChangeCellState(cell);
                }
            }
            ViewModel.CurrentRunState = ERunState.NotStarted;
        }

        /// <summary>
        /// Determines if the game can be reset.
        /// </summary>
        /// <returns>A boolean value which indicates if the game can be reset.</returns>
        private bool CanResetGame()
        {
            return ViewModel.CurrentRunState != ERunState.NotStarted;
        }

        public void ExecuteChangeSizeCommand()
        {
            ExecuteResetCommand();
            CreateBoard();
        }
    }
}
