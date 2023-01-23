using GameOfLife.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFClient1.Core;
using WPFClient1.Enum;
using WPFClient1.Infrastructure;

namespace WPFClient1
{
    public class MainWindowController
    {
        private readonly MainWindow _mainWindow;
        private IEvolutionEngine engine;
        private readonly EvolutionAlgorithm _evolutionAlgorithm = new EvolutionAlgorithm();

        public MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();

        public MainWindowController(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            ResetEngine();

            ViewModel.ChangeSizeCommand = new RelayCommand<object>(_ => ExecuteChangeSizeCommand());
            ViewModel.RunSingleNextGenerationCommand = new RelayCommand<object>(_ => ExecuteEvolveGenerationCommand(), _ => CanEvolveGeneration());
            ViewModel.PauseEvolveCommand = new RelayCommand<object>(_ => ViewModel.CurrentRunState = ERunState.Paused, _ => ViewModel.CurrentRunState == ERunState.Running);
            ViewModel.RunEvolveCommand = new RelayCommand<object>(_ => ExecuteRunEvolveGeneration(), _ => CanRunEvolveGeneration());

            ViewModel.ResetCommand = new RelayCommand<object>(
                _ => ResetGame(),
                _ => CanResetGame()
            );

            ViewModel.ToggleCellLifeCommand = new RelayCommand<string>(
                (cellRowColumn) => ToggleCellLife(cellRowColumn),
                _ => CanToggleCellLife()
            );
        }

        /// <summary>
        /// Gets the specified cell from the current generation.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <param name="column">Column index.</param>
        /// <returns></returns>
        public Cell GetCell(int row, int column)
        {
            return engine.GetCell(row, column);
        }

        private void ExecuteChangeSizeCommand()
        {
            ResetEngine();
            _mainWindow.BuildGridUI(ViewModel);
            ResetGame();
        }

        private void ResetEngine()
        {
            engine = new EvolutionEngine(new Generation(ViewModel.RowNumber, ViewModel.ColumnNumber));
            ViewModel.GenerationNumber = engine.CurrentGenerationNumber;
        }


        /// <summary>
        /// Evolves the current generation.
        /// </summary>
        private async void ExecuteRunEvolveGeneration()
        {
            bool[,] oldUniverse, newUniverse = engine.GetCurrentGeneration().GetUniverse();

            ViewModel.CurrentRunState = ERunState.Running;

            while (ViewModel.CurrentRunState == ERunState.Running)
            {
                oldUniverse = newUniverse;
                newUniverse = _evolutionAlgorithm.calculateNextBoard(oldUniverse);

                ViewModel.CurrentRunState = EvolveCurrentGeneration(newUniverse);

                await Task.Delay(ViewModel.DelayBetweenGeneration);
            }

            if (ViewModel.CurrentRunState == ERunState.Finished)
            {
                ResetGame();
            }

            CommandManager.InvalidateRequerySuggested();
        }


        private void ExecuteEvolveGenerationCommand()
        {
            bool[,] oldUniverse = engine.GetCurrentGeneration().GetUniverse();
            bool[,] newUniverse = _evolutionAlgorithm.calculateNextBoard(oldUniverse);

            ViewModel.CurrentRunState = EvolveCurrentGeneration(newUniverse) == ERunState.Finished ? ERunState.Finished : ERunState.Paused;
            if (ViewModel.CurrentRunState == ERunState.Finished)
            {
                ResetGame();
            }
            CommandManager.InvalidateRequerySuggested();
        }


        private ERunState EvolveCurrentGeneration(bool[,] targetUniverse)
        {
            EvolutionEngineActionResult result = engine.UpdateGeneration(targetUniverse);

            ViewModel.GenerationNumber = result.GenerationNumber;

            return result.EvolutionEnded ? ERunState.Finished : ERunState.Running;
        }

        private bool CanRunEvolveGeneration()
        {
            return ViewModel.CurrentRunState == ERunState.NotStarted || ViewModel.CurrentRunState == ERunState.Paused;
        }

        private bool CanEvolveGeneration()
        {
            return ViewModel.CurrentRunState == ERunState.NotStarted || ViewModel.CurrentRunState == ERunState.Paused;
        }

        /// <summary>
        /// Resets the game of life.
        /// </summary>
        private void ResetGame()
        {
            EvolutionEngineActionResult result = engine.ResetGeneration();

            ViewModel.GenerationNumber = result.GenerationNumber;

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

        /// <summary>
        /// Makes a specfied cell alive or dead.
        /// </summary>
        /// <param name="cellRowColumn">Formatted string identifying a particular cell. Format is "rowIndex,columnIndex"<param>
        private void ToggleCellLife(string cellRowColumn)
        {
            string[] cellRowColumnSplit = cellRowColumn.Split(',');

            int row = int.Parse(cellRowColumnSplit[0]);
            int column = int.Parse(cellRowColumnSplit[1]);

            engine.ToggleCellLife(row, column);
        }

        /// <summary>
        /// Determines if the cell life can be toggled.
        /// </summary>
        /// <returns>A boolea value which indicates if the cell life can be toggled.</returns>
        private bool CanToggleCellLife()
        {
            return ViewModel.CurrentRunState == ERunState.NotStarted || ViewModel.CurrentRunState == ERunState.Paused;
        }
    }
}
