using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WPFClient1.Converters;
using WPFClient1.Core;

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

            BuildGridUI(mainController.ViewModel);
        }

        /// <summary>
        /// Builds the game of life user interface.
        /// </summary>
        /// <param name="generationViewModel">Generation view model.</param>
        public void BuildGridUI(MainWindowViewModel generationViewModel)
        {
            UniverseGrid.RowDefinitions.Clear();
            UniverseGrid.ColumnDefinitions.Clear();

            for (int row = 0; row < generationViewModel.RowNumber; row++)
            {
                UniverseGrid.RowDefinitions.Add(new RowDefinition());

                for (int column = 0; column < generationViewModel.ColumnNumber; column++)
                {
                    if (row == 0)
                        UniverseGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    // Let's use a TextBlock to visually represent a cell
                    TextBlock cellTextBlock = CreateCellTextBlock(mainController.GetCell(row, column));

                    // Position the "cell" in the grid
                    Grid.SetRow(cellTextBlock, row);
                    Grid.SetColumn(cellTextBlock, column);

                    UniverseGrid.Children.Add(cellTextBlock);
                }
            }
        }

        /// <summary>
        /// Creates a TextBlock that represents a single cell in the game of life.
        /// </summary>
        /// <param name="cell">The cell that the TextBlock will represent.</param>
        /// <returns>A TextBlock representing the cell.</returns>
        private TextBlock CreateCellTextBlock(Cell cell)
        {
            TextBlock cellTextBlock = new TextBlock();
            cellTextBlock.DataContext = cell;
            cellTextBlock.Margin = new Thickness(0, 0, 0, 0);
            cellTextBlock.Padding = new Thickness(0, 0, 0, 0);
            cellTextBlock.Cursor = Cursors.Hand;
            cellTextBlock.InputBindings.Add(CreateMouseClickInputBinding(cell));
            cellTextBlock.SetBinding(TextBlock.BackgroundProperty, CreateCellAliveBinding());

            return cellTextBlock;
        }

        /// <summary>
        /// Creates a mouse click binding for a cell so that the cell's
        /// living status can be toggled by the user.
        /// </summary>
        /// <param name="cell">The cell that this binding is for.</param>
        /// <returns>An InputBinding for the cell.</returns>
        private InputBinding CreateMouseClickInputBinding(Cell cell)
        {
            InputBinding cellTextBlockInputBinding = new InputBinding(
                mainController.ViewModel.ToggleCellLifeCommand,
                new MouseGesture(MouseAction.LeftClick)
            );
            cellTextBlockInputBinding.CommandParameter = string.Format("{0},{1}", cell.Row, cell.Column);

            return cellTextBlockInputBinding;
        }


        /// <summary>
        /// Creates a binding for the Cell.Alive property.
        /// </summary>
        /// <returns>A Binding for the cell Alive property.</returns>
        private Binding CreateCellAliveBinding()
        {
            return new Binding
            {
                Path = new PropertyPath("Alive"),
                Mode = BindingMode.TwoWay,
                Converter = new LifeToColourConverter(
                    aliveColour: Brushes.Black,
                    deadColour: Brushes.White
                )
            };
        }
    }
}
