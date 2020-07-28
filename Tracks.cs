using System.IO;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;
using System.Windows.Input;

namespace MusicPlayer
{
    public class TracksBox : ItemsControl
    {
        public TracksBox()
        {
            Padding = new Thickness(2);

            BorderThickness = new Thickness(0.6, 0.6, 0, 0.6);
            BorderBrush = Brushes.DarkGray;

            Background = Brushes.White;
        }

        public void ShowTracks(PlaylistItem playlist)
        {
            Items.Clear();
            foreach (TrackItem track in playlist.Tracks)
            {
                Items.Add(track);
            }
        }
    }

    public class TrackItem : UserControl
    {
        public FileInfo File { get; }

        public TrackItem(FileInfo file)
        {
            File = file;
            Content = file.Name.Replace(file.Extension, "");

            Padding = new Thickness(5);

            FontSize = 13;
            FontWeight = FontWeights.Medium;
            Background = Brushes.Transparent;

            Cursor = Cursors.Hand;
        }
    }
}
