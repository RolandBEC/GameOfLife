using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFClient1.Infrastructure;

namespace WPFClient1
{
    /// <summary>
    /// A view model to represent the current generation in
    /// the game of life.
    /// </summary>
    public class MainWindowViewModel : BaseViewModel
    {
        public ICommand StartCommand { get; set; }

        public ICommand StepCommand { get; set; }

        public ICommand StopCommand { get; set; }

        public ICommand RandomizeCommand { get; set; }

        public ICommand ClearCommand { get; set; }

        public ICommand ImportCommand { get; set; }

        public ICommand ExportCommand { get; set; }



        private GameOfLife.Core.GameOfLife _goL;
        public GameOfLife.Core.GameOfLife GoL
        {
            get => this._goL;
            private set => this.SetField(ref this._goL, value);
        }

        private bool _simulationRunning;
        public bool SimulationRunning
        {
            get => this._simulationRunning;
            set => this.SetField(ref this._simulationRunning, value);
        }

        private double _simSpeedFactor = 2.0;
        public double SimSpeedFactor
        {
            get => this._simSpeedFactor;
            set
            {
                if (Math.Abs(this._simSpeedFactor - value) < 0.001)
                    return;

                this.SetField(ref this._simSpeedFactor, value);
            }
        }

        private ushort _canvasSize = 128;
        public ushort CanvasSize
        {
            get => this._canvasSize;
            set => this.SetField(ref this._canvasSize, value);
        }


        private BitmapSource _golSource;
        public BitmapSource GOLSource
        {
            get => this._golSource;
            set => this.SetField(ref this._golSource, value);
        }

        public void SetGameOfLife(GameOfLife.Core.GameOfLife gol)
        {
            GoL = gol;
        }
    }
}
