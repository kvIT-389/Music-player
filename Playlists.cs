using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;
using System.Windows.Input;

namespace MusicPlayer
{
    public class PlaylistsBox : ItemsControl
    {
        DirectoryInfo music_directory;
        DirectoryInfo MusicDirectory
        {
            get { return music_directory; }
            set
            {
                music_directory = value;
                music_directory.Create();

                Items.Clear();

                AddPlaylist("All music", music_directory);
                foreach (DirectoryInfo dir in music_directory.GetDirectories())
                {
                    AddPlaylist(dir.Name, dir);
                }
            }
        }

        public TracksBox TracksBox { get; set; }

        PlaylistItem selected_playlist;
        PlaylistItem SelectedPlaylist
        {
            get { return selected_playlist; }
            set
            {
                selected_playlist?.Unselect();
                selected_playlist = value;
                selected_playlist.Select();

                TracksBox.ShowTracks(selected_playlist);
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
            var new_playlist = new PlaylistItem(name, dir);

            new_playlist.MouseLeftButtonDown += (object sender, MouseButtonEventArgs args) =>
            {
                var playlist = args.Source as PlaylistItem;
                SelectedPlaylist = playlist;
            };

            Items.Add(new_playlist);
        }
    }

    public class PlaylistItem : UserControl
    {
        static string[] SupportedExtensions = {".mp3", ".ogg", ".wav"};

        DirectoryInfo directory;
        public DirectoryInfo Directory
        {
            get { return directory; }
            set
            {
                directory = value;

                Tracks.Clear();
                foreach (FileInfo file in directory.GetFiles())
                {
                    if (SupportedExtensions.Contains(file.Extension))
                    {
                        Tracks.Add(new TrackItem(file));
                    }
                }
            }
        }

        public List<TrackItem> Tracks { get; } = new List<TrackItem> {};

        public PlaylistItem(string name, DirectoryInfo directory)
        {
            Content = name;
            Directory = directory;

            Padding = new Thickness(5);
            BorderThickness = new Thickness(0, 0, 0.4, 0.4);

            FontSize = 16;
            FontWeight = FontWeights.Medium;
            Foreground = new SolidColorBrush(Color.FromRgb(37, 37, 37));

            Cursor = Cursors.Hand;
        }

        public void Select()
        {
            Background = new LinearGradientBrush(
                Colors.White, Color.FromRgb(237, 237, 237), 50
            );
            BorderBrush = new SolidColorBrush(Color.FromRgb(123, 123, 123));
        }

        public void Unselect()
        {
            Background = Brushes.Transparent;
            BorderBrush = null;
        }
    }
}
