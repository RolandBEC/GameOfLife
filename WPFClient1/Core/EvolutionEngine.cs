using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WPFClient1.Core
{/// <summary>
 /// The engine that implements Conway's Game of Life rules.
 /// </summary>
    public class EvolutionEngine : IEvolutionEngine
    {
        const int UnderPopulationThreshold = 2;
        const int OverPopulationThreshold = 3;
        const int ReproductionThreshold = 3;

        /// <summary>
        /// Gets the current generation.
        /// </summary>
        private Generation CurrentGeneration { get; set; }

        /// <summary>
        /// Gets the current generation number.
        /// </summary>
        public int CurrentGenerationNumber { get; private set; }

        /// <summary>
        /// Initialises a new instance of the LifeEngine with a specified initial generation.
        /// </summary>
        /// <param name="initialGeneration">The initial generation to start from.</param>
        public EvolutionEngine(Generation initialGeneration)
        {
            CurrentGeneration = initialGeneration;
            CurrentGenerationNumber = 1;
        }

        /// <summary>
        /// Applies Conway's life rules to evolve the current generation into the next generation.
        /// </summary>
        /// <returns>An EvolutionEngineActionResult.</returns>
        public EvolutionEngineActionResult EvolveGeneration()
        {
            IList<Tuple<Cell, bool>> cellLifeChangeTupleList = new List<Tuple<Cell, bool>>();

            Parallel.For(0, CurrentGeneration.RowNumber, row =>
            {
                for (int column = 0; column < CurrentGeneration.ColumnNumber; column++)
                {
                    Cell cell = CurrentGeneration.GetCell(row, column);

                    int numberOfAliveNeighbors = GetNumberOfAliveNeighbors(CurrentGeneration, cell);

                    if (cell.Alive && (numberOfAliveNeighbors < UnderPopulationThreshold || numberOfAliveNeighbors > OverPopulationThreshold))
                    {
                        cellLifeChangeTupleList.Add(new Tuple<Cell, bool>(cell, false));
                    }
                    else if (!cell.Alive && numberOfAliveNeighbors == ReproductionThreshold)
                    {
                        cellLifeChangeTupleList.Add(new Tuple<Cell, bool>(cell, true));
                    }
                }
            });

            if (cellLifeChangeTupleList.Any())
            {
                CurrentGenerationNumber++;

                Parallel.ForEach(
                    cellLifeChangeTupleList,
                    tuple => tuple.Item1.Alive = tuple.Item2
                );
            }

            return new EvolutionEngineActionResult(
                evolutionEnded: !cellLifeChangeTupleList.Any(),
                generationNumber: CurrentGenerationNumber
            );
        }

        /// <summary>
        /// Resets the current generation.
        /// </summary>
        /// <returns>An EvolutionEngineActionResult.</returns>
        public EvolutionEngineActionResult ResetGeneration()
        {
            CurrentGeneration.Reset();

            CurrentGenerationNumber = 1;

            return new EvolutionEngineActionResult(
                evolutionEnded: false,
                generationNumber: CurrentGenerationNumber
            );
        }

        public int GetUniverseRowSize()
        {
            return CurrentGeneration.RowNumber;
        }

        public int GetUniverseRColumnSize()
        {

            return CurrentGeneration.ColumnNumber;
        }


        /// <summary>
        /// Gets the specified cell in the current generation.
        /// </summary>
        /// <param name="row">Row index of cell.</param>
        /// <param name="column">COlumn index of cell.</param>
        /// <returns>The specified cell.</returns>
        public Cell GetCell(int row, int column)
        {
            return CurrentGeneration.GetCell(row, column);
        }

        /// <summary>
        /// Sets a particular cell in the current generation to be dead or alive.
        /// </summary>
        /// <param name="row">Row index of the cell.</param>
        /// <param name="column">Column index of the cell.</param>
        /// <param name="alive">A boolean value that indicates if this cell is dead or alive.</param>
        public void SetCell(int row, int column, bool alive)
        {
            CurrentGeneration.SetCell(row, column, alive);
        }

        /// <summary>
        /// Toggles the living status of a cell in the current generatio.
        /// </summary>
        /// <param name="row">Row index of the cell.</param>
        /// <param name="column">Colummn index of cell.</param>
        public void ToggleCellLife(int row, int column)
        {
            CurrentGeneration.ToggleCellLife(row, column);
        }

        /// <summary>
        /// Gets the number of neighbor cells that are alive for a particular cell.
        /// </summary>
        /// <param name="generation">The generation.</param>
        /// <param name="cell">The cell whose living neighbour are being counted.</param>
        /// <returns>Number of alive neighbours for the specified cell.</returns>
        private int GetNumberOfAliveNeighbors(Generation generation, Cell cell)
        {
            int numberOfAliveNeighbours = 0;

            var neighboringCells = new List<Cell>
            {
                generation.GetCell(cell.Row - 1, cell.Column - 1),
                generation.GetCell(cell.Row - 1, cell.Column + 1),
                generation.GetCell(cell.Row, cell.Column + 1),
                generation.GetCell(cell.Row + 1, cell.Column + 1),
                generation.GetCell(cell.Row + 1, cell.Column),
                generation.GetCell(cell.Row + 1, cell.Column - 1),
                generation.GetCell(cell.Row, cell.Column - 1),
                generation.GetCell(cell.Row - 1, cell.Column)
            };

            neighboringCells.ForEach(
                neighboringCell => numberOfAliveNeighbours +=
                    (neighboringCell != null && neighboringCell.Alive) ? 1 : 0
            );
            return numberOfAliveNeighbours;
        }
    }
}
