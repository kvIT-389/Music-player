using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;

namespace MusicPlayer
{
    class MainWindow : Window
    {
        TracksBox TracksBox { get; }
        PlaylistsBox PlaylistsBox { get; }
        ControlsBar ControlsBar { get; }

        public MainWindow()
        {
            // Window setting 

            Width = 1000;
            Height = 640;

            MinWidth = 665;
            MinHeight = 425;

            Title = "Music player";
            Background = new SolidColorBrush(Color.FromRgb(247, 247, 247));

            // Creating application interface 

            var main_grid = new MainWindowGrid();

            ControlsBar = new ControlsBar();
            TracksBox = new TracksBox() { Player = ControlsBar.Player };
            PlaylistsBox = new PlaylistsBox() { TracksBox = TracksBox };

            main_grid.AddChild(PlaylistsBox, 0, 0);
            main_grid.AddChild(new GridSplitter()
            {
                Width = 2.5,
                HorizontalAlignment = HorizontalAlignment.Center
            }, 0, 1);
            main_grid.AddChild(TracksBox, 0, 2);
            main_grid.AddChild(ControlsBar, 1, 0, 1, 3);

            Content = main_grid;
        }
    }

    class MainWindowGrid : Grid
    {
        public MainWindowGrid()
        {
            // Rows' definition 

            RowDefinitions.Add(new RowDefinition());

            RowDefinitions.Add(new RowDefinition() {
                Height = new GridLength(48)
            });

            // Columns' definition 

            ColumnDefinitions.Add(new ColumnDefinition() {
                Width = new GridLength(200),
                MinWidth = 150,
                MaxWidth = 250
            });

            ColumnDefinitions.Add(new ColumnDefinition() {
                Width = GridLength.Auto
            });

            ColumnDefinitions.Add(new ColumnDefinition());
        }

        public void AddChild(FrameworkElement Child, 
                             int row, int column,
                             int rowspan=1, int columnspan=1)
        {
            Children.Add(Child);

            Grid.SetRow(Child, row);
            Grid.SetColumn(Child, column);

            Grid.SetRowSpan(Child, rowspan);
            Grid.SetColumnSpan(Child, columnspan);
        }
    }
}
