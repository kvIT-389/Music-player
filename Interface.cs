using System;
using System.IO;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Media;

namespace MusicPlayer
{
    class PlaylistsBox : ItemsControl
    {
        public TracksBox TracksBox { get; set; }

        DirectoryInfo MusicDirectory
        {
            set
            {
                value.Create();

                Items.Clear();

                AddPlaylist("All music", value);
                foreach (DirectoryInfo dir in value.GetDirectories())
                {
                    AddPlaylist(dir.Name, dir);
                }
            }
        }

        PlaylistItem selected_playlist;
        PlaylistItem SelectedPlaylist
        {
            get { return selected_playlist; }
            set
            {
                if (selected_playlist != value)
                {
                    selected_playlist?.Unselect();
                    selected_playlist = value;
                    selected_playlist.Select();

                    TracksBox?.ShowPlaylist(selected_playlist.Playlist);
                }
            }
        }

        public PlaylistsBox()
        {
            MusicDirectory = new DirectoryInfo(
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
            );

            Padding = new Thickness(2.5);
            BorderThickness = new Thickness(0, 0.6, 0.6, 0.6);

            BorderBrush = Brushes.DarkGray;
            Background = Brushes.White;
        }

        void AddPlaylist(string name, DirectoryInfo dir)
        {
            var new_item = new PlaylistItem(new Playlist(name, dir));

            new_item.MouseLeftButtonDown += (sender, args) =>
            {
                SelectedPlaylist = (args.Source as PlaylistItem);
            };

            Items.Add(new_item);
        }
    }

    class PlaylistItem : UserControl
    {
        public Playlist Playlist { get; }

        bool IsSelected;

        public PlaylistItem(Playlist playlist)
        {
            Playlist = playlist;
            Content = playlist.Name;

            // Styles 

            Padding = new Thickness(5);
            BorderThickness = new Thickness();

            Background = Brushes.Transparent;
            BorderBrush = new SolidColorBrush(Color.FromRgb(123, 123, 123));

            FontSize = 16;
            FontWeight = FontWeights.Medium;
            Foreground = new SolidColorBrush(Color.FromRgb(37, 37, 37));

            Cursor = Cursors.Hand;

            // Events 

            MouseEnter += (sender, args) =>
            {
                if (!IsSelected)
                {
                    Background = new LinearGradientBrush(
                        Colors.White, Color.FromRgb(242, 242, 242), 0
                    );
                }
            };

            MouseLeave += (sender, args) =>
            {
                if (!IsSelected)
                {
                    Background = Brushes.Transparent;
                }
            };
        }

        public void Select()
        {
            IsSelected = true;

            BorderThickness = new Thickness(0, 0, 0.4, 0.4);
        }

        public void Unselect()
        {
            IsSelected = false;

            BorderThickness = new Thickness();
            Background = Brushes.Transparent;
        }
    }

    class TracksBox : ItemsControl
    {
        public AudioPlayer Player { get; set; }

        Playlist ShowedPlaylist;

        public TracksBox()
        {
            Padding = new Thickness(2);
            BorderThickness = new Thickness(0.6, 0.6, 0, 0.6);

            BorderBrush = Brushes.DarkGray;
            Background = Brushes.White;
        }

        public void ShowPlaylist(Playlist playlist)
        {
            ShowedPlaylist = playlist;

            Items.Clear();
            foreach (Track track in playlist.Tracks)
            {
                var new_item = new TrackItem(track);

                new_item.MouseLeftButtonUp += (sender, args) =>
                {
                    Player?.StartNewQueue(
                        ShowedPlaylist.Tracks,
                        (args.Source as TrackItem).Track
                    );
                };

                Items.Add(new_item);
            }
        }
    }

    class TrackItem : UserControl
    {
        Track track;
        public Track Track
        {
            get { return track; }
            set
            {
                track = value;

                var content = new StackPanel() { Orientation = Orientation.Horizontal };

                if (track.Author != null)
                {
                    content.Children.Add(new Label()
                    {
                        Content = $"{track.Author} - ",
                        Padding = new Thickness(),
                        Foreground = new SolidColorBrush(Color.FromRgb(113, 113, 113))
                    });
                }

                content.Children.Add(new Label()
                {
                    Content = track.Name,
                    Padding = new Thickness(),
                    Foreground = new SolidColorBrush(Color.FromRgb(37, 37, 37))
                });

                Content = content;
            }
        }

        public TrackItem(Track track)
        {
            Track = track;

            // Styles 

            Padding = new Thickness(5);
            Background = Brushes.Transparent;

            FontSize = 13;
            FontWeight = FontWeights.Medium;

            Cursor = Cursors.Hand;

            // Events 

            MouseEnter += (sender, args) =>
            {
                Background = new LinearGradientBrush(
                    Color.FromRgb(242, 242, 242),
                    Color.FromRgb(251, 251, 251), 0
                );
            };

            MouseLeave += (sender, args) =>
            {
                Background = Brushes.Transparent;
            };
        }
    }

    class ControlsBar : UserControl
    {
        public AudioPlayer Player { get; }

        public ControlsBar()
        {
            Player = new AudioPlayer();

            Player.PlayPrevButton.Margin = new Thickness(5);
            Player.PlayPauseButton.Margin = new Thickness(5);
            Player.PlayNextButton.Margin = new Thickness(5);
            Player.PositionLabel.Margin = new Thickness(3);
            Player.SeekSlider.Margin = new Thickness(5);
            Player.DurationLabel.Margin = new Thickness(3);
            Player.TrackInfo.Margin = new Thickness(5, 0, 5, 0);
            Player.QueueButton.Margin = new Thickness(5);

            var main_grid = new ControlsBarGrid();

            main_grid.AddChild(Player.PlayPrevButton, GridLength.Auto);
            main_grid.AddChild(Player.PlayPauseButton, GridLength.Auto);
            main_grid.AddChild(Player.PlayNextButton, GridLength.Auto);
            main_grid.AddChild(Player.PositionLabel, GridLength.Auto);
            main_grid.AddChild(Player.SeekSlider, new GridLength(1, GridUnitType.Star));
            main_grid.AddChild(Player.DurationLabel, GridLength.Auto);
            main_grid.AddChild(Player.TrackInfo, new GridLength(215));
            main_grid.AddChild(Player.QueueButton, GridLength.Auto);

            Content = main_grid;

            Margin = new Thickness(5, 4, 5, 4);
        }
    }

    class ControlsBarGrid : Grid
    {
        public void AddChild(FrameworkElement child, GridLength column_width)
        {
            ColumnDefinitions.Add(new ColumnDefinition() { Width = column_width });

            Children.Add(child);
            Grid.SetColumn(child, Children.Count - 1);

            child.VerticalAlignment = VerticalAlignment.Center;
        }
    }
}
