using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFClient1.Infrastructure;
using WPFClient1.Core;

namespace WPFClient1
{
    public class MainWindowController : IDisposable
    {
        public bool CanvasActive { get; set; }

        private readonly MainWindow _mainWindow;
        private Timer _timer;

        private readonly byte[] _worldArray = new byte[512 * 512];

        private const double TIMER_INITIAL_SPEED = 1000d / 15d;

        private bool _simulationCalculating;

        public MainWindowViewModel ViewModel { get; }

        public MainWindowController(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            ViewModel = new MainWindowViewModel();
            ViewModel.StartCommand = new RelayCommand(ExecuteStartCommand, (o) => !ViewModel.SimulationRunning);
            ViewModel.StepCommand = new RelayCommand(ExecuteStepCommand, (o) => !ViewModel.SimulationRunning);
            ViewModel.StopCommand = new RelayCommand(ExecuteStopCommand, (o) => ViewModel.SimulationRunning);
            ViewModel.RandomizeCommand = new RelayCommand(ExecuteRandomizeCommand, (o) => !ViewModel.SimulationRunning);
            ViewModel.ClearCommand = new RelayCommand(ExecuteClearCommand, (o) => !ViewModel.SimulationRunning);
            ViewModel.ImportCommand = new RelayCommand(ExecuteImportCommand, (o) => !ViewModel.SimulationRunning);
            ViewModel.ExportCommand = new RelayCommand(ExecuteExportCommand, (o) => !ViewModel.SimulationRunning);
            ViewModel.GenerateWorldCommand = new RelayCommand(ExecuteGenerateWorldCommand, (o) => !ViewModel.SimulationRunning);

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(ViewModel.SimSpeedFactor):
                    _ = this._timer?.Change(0, (int)(TIMER_INITIAL_SPEED / ViewModel.SimSpeedFactor));
                    break;
                case nameof(ViewModel.CanvasSize):
                    this.ResizeAndRandomize(ViewModel.CanvasSize);
                    break;
            }
        }

        private void ExecuteGenerateWorldCommand(object parameter)
        {
            ViewModel.SetGameOfLife(new GameOfLife(ViewModel.CanvasSize));
            ViewModel.GoL.Randomize();
            UpdateImage();
        }

        public void Init()
        {
            ViewModel.SetGameOfLife(new GameOfLife(ViewModel.CanvasSize));
            ViewModel.GoL.Randomize();
            UpdateImage();
        }

        private void ExecuteStartCommand(object parameter)
        {
            this._timer = new Timer(this.SimulationStep, true, 0, (int)(TIMER_INITIAL_SPEED / this.ViewModel.SimSpeedFactor));
            this.ViewModel.SimulationRunning = true;
        }

        private void ExecuteStepCommand(object parameter)
        {
            this.SimulationStep();
        }

        private void ExecuteStopCommand(object parameter)
        {

            this._timer?.Dispose();
            this._timer = null;

            this.ViewModel.SimulationRunning = false;
        }

        private void ExecuteRandomizeCommand(object parameter)
        {
            this.ViewModel.GoL.Randomize();
            this.UpdateImage();
        }

        private void ExecuteClearCommand(object parameter)
        {
            this.ViewModel.GoL.Reset();
            this.UpdateImage();
        }

        private void ExecuteImportCommand(object parameter)
        {
            //var wasRunning = this.ViewModel.SimulationRunning;
            //if (wasRunning)
            //    ExecuteStopCommand(parameter: null);

            //var ofd = new OpenFileDialog
            //{
            //    Title = "Open File",
            //    Multiselect = false,
            //    Filter = "Image Files|*.png;*.gif;*.jpg;*.jpeg;*.bmp",
            //};

            //var dialogShown = ofd.ShowDialog();
            //if (dialogShown.HasValue && dialogShown.Value)
            //{
            //    BitmapSource bitmap;
            //    try
            //    {
            //        bitmap = new BitmapImage(new Uri(ofd.FileName));
            //    }
            //    catch (NotSupportedException)
            //    {
            //        _ = MessageBox.Show("The file needs to be a valid image.");
            //        return;
            //    }

            //    if (bitmap.PixelWidth != bitmap.PixelHeight)
            //    {
            //        // Image needs to be square
            //        _ = MessageBox.Show("The image needs to be square.");
            //        return;
            //    }

            //    var maxSize = this.ViewModel.SizeSlider.Maximum;
            //    if (bitmap.PixelWidth > maxSize)
            //    {
            //        var scaleFactor = maxSize / bitmap.PixelWidth;
            //        bitmap = new TransformedBitmap(bitmap, new ScaleTransform(scaleFactor, scaleFactor));
            //    }

            //    // Convert imported bitmap to Gray8 format
            //    var fcb = new FormatConvertedBitmap();
            //    fcb.BeginInit();
            //    fcb.Source = bitmap;
            //    fcb.DestinationFormat = PixelFormats.Gray8;
            //    fcb.EndInit();

            //    // pixArray will be copied to a bitmap later on
            //    // golArray will be passed to the GameOfLife instance
            //    var pixArray = new byte[bitmap.PixelWidth * bitmap.PixelHeight];
            //    var golArray = new bool[pixArray.Length];
            //    fcb.CopyPixels(pixArray, bitmap.PixelWidth, 0);

            //    // Pixels brighter than 0x7f (127) are considered "white"
            //    for (var i = 0; i < pixArray.Length; i++)
            //        golArray[i] = pixArray[i] > 0x7f;

            //    // Update canvas size and GameOfLife board
            //    this.ViewModel.CanvasSize = (ushort)bitmap.PixelWidth;
            //    this.ViewModel.GoL.World = golArray;
            //    this.UpdateImage();
            //}
            //else
            //{
            //    if (wasRunning)
            //        ExecuteStartCommand(parameter: null);
            //}

        }

        private void ExecuteExportCommand(object parameter)
        {

            var wasRunning = this.ViewModel.SimulationRunning;
            if (wasRunning)
                ExecuteStopCommand(parameter: null);

            var sfd = new SaveFileDialog
            {
                Title = "Export As",
                FileName = "gameoflife.png",
                Filter = "PNG Files|*.png",
            };

            var dialogShown = sfd.ShowDialog();
            if (dialogShown.HasValue && dialogShown.Value)
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(this.ViewModel.GOLSource));

                using var fileStream = new FileStream(sfd.FileName, FileMode.Create);
                encoder.Save(fileStream);
            }

            if (wasRunning)
                ExecuteStartCommand(parameter: null);
        }

        private void UpdateImage()
        {
            for (var i = 0; i < ViewModel.GoL.World.Length; i++)
            {
                this._worldArray[i] = ViewModel.GoL.World[i] ? byte.MaxValue : byte.MinValue;
            }

            // this prevents crashes during app shutdown
            if (Application.Current == null || Application.Current.Dispatcher == null)
                return;

            // when called as a timer callback, this method will execute on a different thread
            // so we'll have to manually re-route this to the main thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                var bs = BitmapSource.Create(
                    ViewModel.GoL.Size.Width, 
                    ViewModel.GoL.Size.Height,
                    96, 
                    96, 
                    PixelFormats.Gray8,
                    null, 
                    this._worldArray, 
                    ViewModel.GoL.Size.Width);
                ViewModel.GOLSource = bs;
            });
        }

        private void ResizeAndRandomize(ushort newSize)
        {
            this.ViewModel.GoL.Resize(newSize);
            this.ViewModel.GoL.Randomize();
            this.UpdateImage();
        }

        private async void SimulationStep(object stateInfo = null)
        {
            if (this._simulationCalculating)
                return;

            this._simulationCalculating = true;

            await this.ViewModel.GoL.UpdateAsync();
            this.UpdateImage();

            this._simulationCalculating = false;
        }

        public void OnMouseOverCanvas(object sender, MouseEventArgs e)
        {
            if (!this.CanvasActive)
                return;

            bool newCellState;
            var maxCoordVal = this.ViewModel.GoL.Size.Width - 1; // Using .Width here because in this scenario, height and width are the same

            if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Released)
                newCellState = true;
            else if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Pressed)
                newCellState = false;
            else
            {
                this.CanvasActive = false;
                return;
            }

            var canvasPoint = e.GetPosition((Canvas)sender);
            var scaleDivisor = ((Canvas)sender).ActualWidth / this.ViewModel.GoL.Size.Width; // see above
            var canvasX = (int)Math.Floor(canvasPoint.X / scaleDivisor);
            var canvasY = (int)Math.Floor(canvasPoint.Y / scaleDivisor);

            if ((canvasX < 0) | (canvasX > maxCoordVal))
                return;

            if ((canvasY < 0) | (canvasY > maxCoordVal))
                return;

            this.ViewModel.GoL.SetCell(canvasX, canvasY, newCellState);

            if (!this.ViewModel.SimulationRunning)
                this.UpdateImage();
        }

        public void Dispose()
        {
            this._timer?.Dispose();
            this._timer = null;

            this.ViewModel.SimulationRunning = false;
        }
    }
}
