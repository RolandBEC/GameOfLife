using WPFClient1.Infrastructure;

namespace WPFClient1.Core
{
    /// <summary>
    /// Represents a single observable cell, its position and whether it is dead or alive.
    /// </summary>
    public class Cell : BaseViewModel
    {
        public int Row { get; private set; }

        public int Column { get; private set; }


        private bool alive;
        public bool Alive
        {
            get => alive;
            set
            {
                alive = value;
                OnPropertyChanged();
            }
        }

        public Cell(int row, int column, bool alive)
        {
            Row = row;
            Column = column;
            Alive = alive;
        }

        public override string ToString()
        {
            return string.Format(
                "Cell ({0},{1}) - {2}", Row, Column, Alive ? "Alive" : "Dead"
            );
        }
    }
}
