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
        public ICommand ResetCommand { get; set; }
        public ICommand RunCommand { get; set; }
        public ICommand RunStepCommand { get; set; }
        public ICommand PauseCommand { get; set; }


        public int WidthY
        {
            get => Settings.Default.WidthY;
            set
            {
                Settings.Default.WidthY = value;
                Settings.Default.Save();
            }
        }

        public int WidthX
        {
            get => Settings.Default.WidthX;
            set
            {
                Settings.Default.WidthX = value;
                Settings.Default.Save();
            }
        }
        public int CellSize
        {
            get => Settings.Default.CellSize;
            set
            {
                Settings.Default.CellSize = value;
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

        private long calculateNextBoardTimeMs;
        public long CalculateNextBoardTimeMs
        {
            get => calculateNextBoardTimeMs;
            set
            {
                calculateNextBoardTimeMs = value;
                OnPropertyChanged();
            }
        }

        private long calculateApplyRuleTimeMs;
        public long CalculateApplyRuleTimeMs
        {
            get => calculateApplyRuleTimeMs;
            set
            {
                calculateApplyRuleTimeMs = value;
                OnPropertyChanged();
            }
        }
    }
}
