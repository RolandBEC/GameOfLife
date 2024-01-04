namespace WPFClient1
{
    public class MainWindowController
    {

        private readonly MainWindow _mainWindow;

        public MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();

        public MainWindowController(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }
    }
}
