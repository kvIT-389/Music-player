using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;

namespace MusicPlayer
{
    public class MainWindow : Window
    {
        TracksBox TracksBox;
        PlaylistsBox PlaylistsBox;
        GridSplitter Splitter;
        ControlsBar ControlsBar;

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
            Content = main_grid;

            TracksBox = new TracksBox();
            main_grid.AddChild(TracksBox, 0, 2);

            PlaylistsBox = new PlaylistsBox() { TracksBox = TracksBox };
            main_grid.AddChild(PlaylistsBox, 0, 0);

            Splitter = new GridSplitter()
            {
                Width = 2.5,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            main_grid.AddChild(Splitter, 0, 1);

            ControlsBar = new ControlsBar();
            main_grid.AddChild(ControlsBar, 1, 0, 1, 3);
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
