using System;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Shapes;

namespace MusicPlayer
{
    public class ControlsBar : UserControl
    {
        PrevButton PrevButton;
        PlayPauseButton PlayPauseButton;
        NextButton NextButton;
        TimeLabel PositionLabel;
        CustomSlider SeekSlider;
        TimeLabel DurationLabel;
        TrackInfo TrackInfo;
        QueueButton QueueButton;

        public ControlsBar()
        {
            var grid = new ControlsBarGrid();
            Content = grid;

            PrevButton = new PrevButton() { Margin = new Thickness(5) };
            grid.AddChild(PrevButton, GridLength.Auto);

            PlayPauseButton = new PlayPauseButton() { Margin = new Thickness(5) };
            grid.AddChild(PlayPauseButton, GridLength.Auto);

            NextButton = new NextButton() { Margin = new Thickness(5) };
            grid.AddChild(NextButton, GridLength.Auto);

            PositionLabel = new TimeLabel();
            grid.AddChild(PositionLabel, GridLength.Auto);

            SeekSlider = new CustomSlider() { Margin = new Thickness(5) };
            grid.AddChild(SeekSlider, new GridLength(1, GridUnitType.Star));

            DurationLabel = new TimeLabel();
            grid.AddChild(DurationLabel, GridLength.Auto);

            TrackInfo = new TrackInfo();
            grid.AddChild(TrackInfo, new GridLength(215));

            QueueButton = new QueueButton() { Margin = new Thickness(5) };
            grid.AddChild(QueueButton, GridLength.Auto);

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

    class PrevButton : ButtonBase
    {
        public PrevButton()
        {
            var icon = new Grid();
            Content = icon;

            icon.Children.Add(new Rectangle()
            {
                Width = 12, Height = 12,
                Fill = Brushes.Transparent
            });
            icon.Children.Add(new Path()
            {
                Data = Geometry.Parse("M 1,0 L 1,12 M 11,1 L 2,6 L 11,11 Z"),
                Stroke = new SolidColorBrush(Color.FromRgb(67, 67, 67)),
                StrokeThickness = 1.5
            });

            Cursor = Cursors.Hand;
            ToolTip = "Previous track";
        }
    }

    class PlayPauseButton : ButtonBase
    {
        Path PlaybackIcon = new Path()
        {
            Fill = new SolidColorBrush(Color.FromRgb(67, 67, 67))
        };

        Dictionary<bool, Geometry> PlaybackIcons = new Dictionary<bool, Geometry>
        {
            {false, Geometry.Parse("M 11,9 L 21,15 L 11,21 Z")},
            {true, Geometry.Parse("M 9.5,9 L 13.5,9 L 13.5,21 L 9.5,21 Z M 20.5,9 L 16.5,9 L 16.5,21 L 20.5,21 Z")}
        };

        Dictionary<bool, string> ToolTips = new Dictionary<bool, string>
        {
            {false, "Play"}, {true, "Pause"}
        };

        bool is_playing;
        public bool IsPlaying
        {
            get { return is_playing; }
            set
            {
                is_playing = value;
                PlaybackIcon.Data = PlaybackIcons[is_playing];
                ToolTip = ToolTips[is_playing];
            }
        }

        public PlayPauseButton()
        {
            var icon = new Grid();
            Content = icon;

            icon.Children.Add(new Ellipse()
            {
                Width = 30, Height = 30,
                Fill = Brushes.Transparent,
                Stroke = new SolidColorBrush(Color.FromRgb(67, 67, 67)),
                StrokeThickness = 1.5
            });
            icon.Children.Add(PlaybackIcon);

            Cursor = Cursors.Hand;

            IsPlaying = false;
            Click += (object sender, RoutedEventArgs args) => 
            {
                IsPlaying = !IsPlaying;
            };
        }
    }

    class NextButton : ButtonBase
    {
        public NextButton()
        {
            var icon = new Grid();
            Content = icon;

            icon.Children.Add(new Rectangle()
            {
                Width = 12, Height = 12,
                Fill = Brushes.Transparent
            });
            icon.Children.Add(new Path()
            {
                Data = Geometry.Parse("M 11,0 L 11,12 M 1,1 L 10,6 L 1,11 Z"),
                Stroke = new SolidColorBrush(Color.FromRgb(67, 67, 67)),
                StrokeThickness = 1.5
            });

            Cursor = Cursors.Hand;
            ToolTip = "Next track";
        }
    }

    class TimeLabel : Label
    {
        public TimeSpan Time
        {
            set { Content = $"{value.Minutes}:{value.Seconds:d2}"; }
        }

        public TimeLabel()
        {
            Padding = new Thickness(3);
            FontSize = 14;

            Time = new TimeSpan();
        }
    }

    class CustomSlider : UserControl
    {
        Grid MainGrid = new Grid();
        Grid TrackGrid = new Grid();

        Rectangle TrackBeforeThumb = new Rectangle()
        {
            Height = 3, RadiusX = 1.5, RadiusY = 1.5,
            Fill = new SolidColorBrush(Color.FromRgb(101, 101, 101)),
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        Rectangle TrackAfterThumb = new Rectangle()
        {
            Height = 3, RadiusX = 1.5, RadiusY = 1.5,
            Fill = new SolidColorBrush(Color.FromRgb(176, 176, 176)),
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        Ellipse Thumb = new Ellipse()
        {
            Width = 11, Height = 11,
            Fill = new SolidColorBrush(Color.FromRgb(101, 101, 101)),
            StrokeThickness = 3
        };

        double value;
        public double Value
        {
            get { return this.value; }
            set
            {
                this.value = Math.Max(0, Math.Min(1, value));

                MainGrid.ColumnDefinitions[0].Width =
                TrackGrid.ColumnDefinitions[0].Width =
                new GridLength(this.value, GridUnitType.Star);

                MainGrid.ColumnDefinitions[2].Width =
                TrackGrid.ColumnDefinitions[1].Width =
                new GridLength(1 - this.value, GridUnitType.Star);
            }
        }

        public CustomSlider()
        {
            InitializeGrids();

            MainGrid.Children.Add(TrackGrid);
            Grid.SetColumnSpan(TrackGrid, 3);
            MainGrid.Children.Add(Thumb);
            Grid.SetColumn(Thumb, 1);

            TrackGrid.Children.Add(TrackBeforeThumb);
            Grid.SetColumn(TrackBeforeThumb, 0);

            TrackGrid.Children.Add(TrackAfterThumb);
            Grid.SetColumn(TrackAfterThumb, 1);

            Content = MainGrid;
            Value = 0;

            Cursor = Cursors.Hand;

            InitializeMouseEvents();
        }

        void InitializeGrids()
        {
            // MainGrid setting 

            MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = GridLength.Auto
            });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition());

            // TrackGrid setting 

            TrackGrid.ColumnDefinitions.Add(new ColumnDefinition());
            TrackGrid.ColumnDefinitions.Add(new ColumnDefinition());

            TrackGrid.Margin = new Thickness(Thumb.Width / 2 - 1.5);
        }

        void InitializeMouseEvents()
        {
            MouseLeftButtonDown += (object sender, MouseButtonEventArgs args) =>
            {
                Value = (args.GetPosition(this).X - Thumb.Width / 2) / (ActualWidth - Thumb.Width);
                args.MouseDevice.Capture(this);
            };

            MouseMove += (object sender, MouseEventArgs args) =>
            {
                if (IsMouseCaptured)
                {
                    Value = (args.GetPosition(this).X - Thumb.Width / 2) / (ActualWidth - Thumb.Width);
                }
            };

            MouseLeftButtonUp += (object sender, MouseButtonEventArgs args) =>
            {
                args.MouseDevice.Capture(null);
            };
        }
    }

    class TrackInfo : StackPanel
    {
        Label TrackNameLabel;
        Label TrackAuthorLabel;

        public string TrackName
        {
            set {TrackNameLabel.Content = value;}
        }
        public string TrackAuthor
        {
            set {TrackAuthorLabel.Content = value;}
        }

        public TrackInfo()
        {
            TrackNameLabel = new Label()
            {
                Content = "Track name",
                Padding = new Thickness(5, 0, 5, 0),
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(37, 37, 37))
            };
            Children.Add(TrackNameLabel);

            TrackAuthorLabel = new Label()
            {
                Content = "Track author",
                Padding = new Thickness(5, 0, 5, 0),
                Foreground = new SolidColorBrush(Color.FromRgb(94, 94, 94))
            };
            Children.Add(TrackAuthorLabel);
        }
    }

    class QueueButton : ButtonBase
    {
        public QueueButton()
        {
            var icon = new Grid();
            Content = icon;

            icon.Children.Add(new Rectangle()
            {
                Width = 14, Height = 14,
                Fill = Brushes.Transparent
            });
            icon.Children.Add(new Path()
            {
                Data = Geometry.Parse("M 0,2.5 L 8,2.5 L 8,5 L 0,5 Z M 0,7 L 14,7 L 14,9.5 L 0,9.5 Z M 0,11.5 L 14,11.5 L 14,14 L 0,14 Z M 10,0 L 14,2.5 L 10,5 Z"),
                Fill = new SolidColorBrush(Color.FromRgb(87, 87, 87))
            });

            Cursor = Cursors.Hand;
            ToolTip = "Queue";
        }
    }
}
