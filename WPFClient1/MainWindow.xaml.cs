
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

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
