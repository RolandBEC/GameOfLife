using System.Windows.Input;
using WPFClient1.Enum;
using WPFClient1.Infrastructure;
using WPFClient1.Properties;

namespace WPFClient1
{
    /// <summary>
    /// A view model to represent the current generation in
    /// the game of life.
    /// </summary>
    public class MainWindowViewModel : BaseViewModel
    {
        public ICommand ChangeSizeCommand { get; set; }
        public ICommand RunEvolveCommand { get; set; }
        public ICommand PauseEvolveCommand { get; set; }
        public ICommand RunSingleNextGenerationCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand ToggleCellLifeCommand { get; set; }

        public int RowNumber
        {
            get => Settings.Default.RowNumber;
            set
            {
                Settings.Default.RowNumber = value;
                Settings.Default.Save();
            }
        }

        public int ColumnNumber
        {
            get => Settings.Default.ColumnNumber;
            set
            {
                Settings.Default.ColumnNumber = value;
                Settings.Default.Save();
            }
        }

        private ERunState _currentRunState = ERunState.NotStarted;
        public ERunState CurrentRunState
        {
            get => _currentRunState;
            set
            {
                _currentRunState = value;
                OnPropertyChanged();
            }
        }

        private int generationNumber;
        public int GenerationNumber
        {
            get => generationNumber;
            set
            {
                generationNumber = value;
                OnPropertyChanged();
            }
        }

        private bool _showGridLines;
        public bool ShowGridLines
        {
            get => _showGridLines;
            set
            {
                _showGridLines = value;
                OnPropertyChanged();
            }
        }


        public int DelayBetweenGeneration
        {
            get => Settings.Default.DelayBetweenGeneration;
            set
            {
                Settings.Default.DelayBetweenGeneration = value;
                Settings.Default.Save();
            }
        }
    }
}
