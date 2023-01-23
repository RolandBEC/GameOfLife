using System;
using System.Text;

namespace WPFClient1.Core
{
    public class Generation
    {
        /// <summary>
        /// A two-dimensional array representing a finite universe.
        /// </summary>
        private readonly Cell[,] universe;

        /// <summary>
        /// Get the size of the universe.
        /// </summary>
        public int RowNumber { get; private set; }
        public int ColumnNumber { get; private set; }

        public Generation(int rowNumber, int columnNumber)
        {
            universe = new Cell[rowNumber, columnNumber];
            RowNumber = rowNumber;
            ColumnNumber = columnNumber;

            Initialise();
        }

        private void Initialise()
        {
            for (int row = 0; row < RowNumber; row++)
                for (int column = 0; column < ColumnNumber; column++)
                    universe[row, column] = new Cell(row, column, false);
        }

        /// <summary>
        /// Resets the generation.
        /// </summary>
        public void Reset()
        {
            for (int row = 0; row < RowNumber; row++)
                for (int column = 0; column < ColumnNumber; column++)
                    SetCell(row, column, false);
        }

        /// <summary>
        /// Gets a specified cell.
        /// </summary>
        /// <param name="row">Row index of the cell.</param>
        /// <param name="column">Column index of the cell.</param>
        /// <returns>The specified cell.</returns>
        public Cell GetCell(int row, int column)
        {
            if (row < 0 || row >= RowNumber)
                return null;

            if (column < 0 || column >= ColumnNumber)
                return null;

            return universe[row, column];
        }

        /// <summary>
        /// Sets a particular cell in the universe to be dead or alive.
        /// </summary>
        /// <param name="row">Row index of the cell.</param>
        /// <param name="column">Column index of the cell.</param>
        /// <param name="alive">A boolean value that indicates if this cell is dead or alive.</param>
        public void SetCell(int row, int column, bool alive)
        {
            Cell cell = GetCell(row, column);

            if (cell == null)
                throw new ArgumentOutOfRangeException(
                    "The specified row and column do not map to a valid cell."
                );

            cell.Alive = alive;
        }

        /// <summary>
        /// Toggles the living status of a cell.
        /// </summary>
        /// <param name="row">Row index of the cell.</param>
        /// <param name="column">Colummn index of cell.</param>
        public void ToggleCellLife(int row, int column)
        {
            Cell cell = GetCell(row, column);
            cell.Alive = !cell.Alive;
        }

        /// <summary>
        /// Builds a string representation of this generation.
        /// </summary>
        /// <returns>String representation of this generation.</returns>
        public override string ToString()
        {
            StringBuilder gridString = new StringBuilder();

            for (int row = 0; row < RowNumber; row++)
            {
                for (int column = 0; column < ColumnNumber; column++)
                {
                    gridString.Append(
                        string.Format("{0} ", GetCell(row, column).Alive ? "1" : "0")
                    );
                }

                gridString.AppendLine();
            }

            return gridString.ToString();
        }
    }
}
