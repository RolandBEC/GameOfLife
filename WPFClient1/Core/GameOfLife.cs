using System;
using System.Linq;
using System.Threading.Tasks;
using WPFClient1.Infrastructure;

namespace WPFClient1.Core
{
    public sealed class GameOfLife : BaseViewModel
    {
        private int _generation;
        private bool[] _nextGen;
        private FieldSize _size;
        private bool[] _world;

        public GameOfLife(FieldSize size)
        {
            this.Resize(size);
        }

        public GameOfLife(ushort size)
        {
            this.Resize(new FieldSize(size, size));
        }

        public FieldSize Size
        {
            get => this._size;
            private set
            {
                this.SetField(ref this._size, value);
                this.OnPropertyChanged(nameof(this.TotalCells));
            }
        }

        public int Generation
        {
            get => this._generation;
            private set => this.SetField(ref this._generation, value);
        }

        public bool[] World
        {
            get => this._world;
            set
            {
                this.SetField(ref this._world, value);
                this.OnPropertyChanged(nameof(this.ActiveCells));
            }
        }

        public bool WrapAround { get; set; }

        public int TotalCells => this.Size.Width * this.Size.Height;

        public int ActiveCells => this.World.Count(c => c);

        public void Reset()
        {
            this.World = new bool[this.TotalCells];
            this._nextGen = new bool[this.TotalCells];

            this.Generation = 1;
        }

        public void Resize(FieldSize newSize)
        {
            this.Size = newSize;
            this.Reset();
        }

        public void Resize(ushort newSize)
        {
            this.Size = new FieldSize(newSize, newSize);
            this.Reset();
        }

        public void Randomize(double fraction = 0.2)
        {
            this.Reset();

            Random r = new();
            bool[] newWorld = new bool[this.TotalCells];
            double num = this.TotalCells * fraction;
            for (var i = 0; i < num; i++)
            {
                var x = r.Next(this.Size.Width);
                var y = r.Next(this.Size.Height);

                var idx = (y * this.Size.Width) + x;
                newWorld[idx] = true;
            }

            this.World = newWorld;
        }

        private bool GetWorldCell(int x, int y)
        {
            return this._world[(y * this.Size.Width) + x];
        }

        public void SetCell(int x, int y, bool newValue, bool nextGen = false, bool propertyChanged = true)
        {
            int idx = (y * this.Size.Width) + x;

            if (nextGen)
                this._nextGen[idx] = newValue;
            else
                this._world[idx] = newValue;

            if (propertyChanged)
            {
                this.OnPropertyChanged(nameof(this.ActiveCells));
                this.OnPropertyChanged(nameof(this.World));
            }
        }

        private static int Mod(int i, int m)
        {
            int r = i % m;

            return r < 0 ? r + m : r;
        }

        private bool IsNeighborAlive(int x, int y, int offsetX, int offsetY)
        {
            int newX = x + offsetX;
            int newY = y + offsetY;

            if (!this.WrapAround && (newX < 0 || newX >= this.Size.Width || newY < 0 || newY >= this.Size.Height))
                return false;

            if (!this.WrapAround)
                return this.GetWorldCell(newX, newY);

            newX = Mod(newX, this.Size.Width);
            newY = Mod(newY, this.Size.Height);

            return this.GetWorldCell(newX, newY);
        }

        private void GenerationStep(int idx)
        {
            int x = idx % this.Size.Width;
            int y = idx / this.Size.Width;

            byte neighbors = 0;

            // check for all 8 neighbors
            if (this.IsNeighborAlive(x, y, -1, -1))
                neighbors++;
            if (this.IsNeighborAlive(x, y, 0, -1))
                neighbors++;
            if (this.IsNeighborAlive(x, y, 1, -1))
                neighbors++;

            if (this.IsNeighborAlive(x, y, -1, 0))
                neighbors++;
            if (this.IsNeighborAlive(x, y, 1, 0))
                neighbors++;

            if (this.IsNeighborAlive(x, y, -1, 1))
                neighbors++;
            if (this.IsNeighborAlive(x, y, 0, 1))
                neighbors++;
            if (this.IsNeighborAlive(x, y, 1, 1))
                neighbors++;

            bool isAlive = this.GetWorldCell(x, y);

            // Rules : https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life#Rules 
            bool stillAlive = isAlive && neighbors is 2 or 3;

            stillAlive |= !isAlive && neighbors == 3;

            this.SetCell(x, y, stillAlive, true, false);
        }

        public async Task UpdateAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                _ = Parallel.For(0, this.TotalCells, this.GenerationStep);
            });

            Buffer.BlockCopy(this._nextGen, 0, this._world, 0, this._world.Length);

            this.Generation++;
            this.OnPropertyChanged(nameof(this.World));
            this.OnPropertyChanged(nameof(this.ActiveCells));
        }
    }
}
