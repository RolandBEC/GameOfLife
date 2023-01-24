using System.Windows;

namespace WPFClient1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The Generation view model.
        /// </summary>
        private MainWindowController mainController;

        /// <summary>
        /// Initialises the view model, user interface and data binding from the view model
        /// to the interface.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            mainController = new MainWindowController(this);
            DataContext = mainController.ViewModel;
            Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            mainController.CreateBoard();
        }
    }
}
