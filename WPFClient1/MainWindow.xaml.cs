using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            this.InitializeComponent();

            mainController = new MainWindowController(this);
            this.DataContext = mainController.ViewModel;

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            mainController.Init();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            mainController.Dispose();
        }

        private void GOLCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.mainController.CanvasActive = true;

            // ensure a simple click adds/removes pixels as well
            this.GOLCanvas_MouseMove(sender, e);
        }

        private void GOLCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.mainController.CanvasActive = false;
        }

        private void GOLCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            this.mainController.OnMouseOverCanvas(sender, e);
        }

        private void Slider_Scroll(object sender, MouseWheelEventArgs e)
        {
            var slider = (Slider)sender;
            var sign = Math.Sign(e.Delta);

            if (sign == 0)
                return;

            if ((int)slider.Value == (int)slider.Minimum && sign < 0)
                return;

            if ((int)slider.Value == (int)slider.Maximum && sign > 0)
                return;

            slider.Value = Math.Max(Math.Min(slider.Value + (slider.LargeChange * sign), slider.Maximum), slider.Minimum);
        }
    }
}
