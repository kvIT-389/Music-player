using System;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MusicPlayer
{
    class AudioPlayer : MediaPlayer
    {
        public PlayPrevButton PlayPrevButton { get; } = new PlayPrevButton();
        public PlayPauseButton PlayPauseButton { get; } = new PlayPauseButton();
        public PlayNextButton PlayNextButton { get; } = new PlayNextButton();
        public TimeLabel PositionLabel { get; } = new TimeLabel();
        public CustomSlider SeekSlider { get; } = new CustomSlider();
        public TimeLabel DurationLabel { get; } = new TimeLabel();
        public TrackInfo TrackInfo { get; } = new TrackInfo();
        public QueueButton QueueButton { get; } = new QueueButton();

        List<Track> TracksQueue;
        Track OpenedTrack;

        bool IsPlaying
        {
            set
            {
                if (value) { Play(); }
                else { Pause(); }

                UpdateTimer.IsEnabled = value;
                PlayPauseButton.IsPlaying = value;
            }
        }

        TimeSpan Duration
        {
            get
            {
                return new TimeSpan(1) + (
                    NaturalDuration.HasTimeSpan ?
                    NaturalDuration.TimeSpan :
                    new TimeSpan()
                );
            }
        }

        DispatcherTimer UpdateTimer = new DispatcherTimer()
        {
            Interval = new TimeSpan(0, 0, 0, 0, 100)
        };

        public AudioPlayer()
        {
            // Events 

            PlayPrevButton.Click += (sender, args) =>
            {
                PlayPrev();
            };
            PlayPauseButton.Click += (sender, args) =>
            {
                IsPlaying = PlayPauseButton.IsPlaying;
            };
            PlayNextButton.Click += (sender, args) =>
            {
                PlayNext();
            };

            SeekSlider.MouseLeftButtonDown += (sender, args) =>
            {
                PositionLabel.Time = this.Duration * SeekSlider.Value;
            };
            SeekSlider.MouseMove += (sender, args) =>
            {
                PositionLabel.Time = this.Duration * SeekSlider.Value;
            };
            SeekSlider.MouseLeftButtonUp += (sender, args) =>
            {
                Position = this.Duration * SeekSlider.Value;
            };

            MediaOpened += (sender, args) =>
            {
                DurationLabel.Time = this.Duration;
                TrackInfo.Track = OpenedTrack;

                IsPlaying = true;
            };
            MediaEnded += (sender, args) => { PlayNext(); };

            UpdateTimer.Tick += (source, args) =>
            {
                if (!SeekSlider.IsMouseCaptured)
                {
                    PositionLabel.Time = Position;
                    SeekSlider.Value = Position / this.Duration;
                }
            };
        }

        public void StartNewQueue(List<Track> tracks, Track start_track)
        {
            TracksQueue = tracks;
            OpenNewTrack(start_track);
        }

        void OpenNewTrack(Track track)
        {
            Open(new Uri(track.File.FullName));
            OpenedTrack = track;
        }

        void PlayPrev()
        {
            var next_track_index = TracksQueue.IndexOf(OpenedTrack) - 1;

            if (next_track_index >= 0)
            {
                if (Position < new TimeSpan(0, 0, 5))
                {
                    OpenNewTrack(TracksQueue[next_track_index]);
                }
                else
                {
                    Position = new TimeSpan();
                }
            }
            else
            {
                Stop();
                IsPlaying = false;

                PositionLabel.Time = new TimeSpan();
                SeekSlider.Value = 0;
            }
        }

        void PlayNext()
        {
            var next_track_index = TracksQueue.IndexOf(OpenedTrack) + 1;

            if (next_track_index < TracksQueue.Count)
            {
                OpenNewTrack(TracksQueue[next_track_index]);
            }
            else
            {
                Stop();
                IsPlaying = false;

                PositionLabel.Time = this.Duration;
                SeekSlider.Value = 1;
            }
        }
    }

    class PlayPrevButton : ButtonBase
    {
        public PlayPrevButton()
        {
            var icon = new Grid();

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

            Content = icon;
            ToolTip = "Previous track";

            Cursor = Cursors.Hand;
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

            icon.Children.Add(new Ellipse()
            {
                Width = 30, Height = 30,
                Fill = Brushes.Transparent,
                Stroke = new SolidColorBrush(Color.FromRgb(67, 67, 67)),
                StrokeThickness = 1.5
            });
            icon.Children.Add(PlaybackIcon);

            Content = icon;
            IsPlaying = false;

            Cursor = Cursors.Hand;

            Click += (sender, args) => { IsPlaying = !IsPlaying; };
        }
    }

    class PlayNextButton : ButtonBase
    {
        public PlayNextButton()
        {
            var icon = new Grid();

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

            Content = icon;
            ToolTip = "Next track";

            Cursor = Cursors.Hand;
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
            Padding = new Thickness();
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
            Height = 5, RadiusX = 1.5, RadiusY = 1.5,
            Fill = new SolidColorBrush(Color.FromRgb(101, 101, 101)),
            Stroke = Brushes.Transparent,
            StrokeThickness = 2,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        Rectangle TrackAfterThumb = new Rectangle()
        {
            Height = 5, RadiusX = 1.5, RadiusY = 1.5,
            Fill = new SolidColorBrush(Color.FromRgb(176, 176, 176)),
            Stroke = Brushes.Transparent,
            StrokeThickness = 2,
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

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs args)
        {
            Value = (args.GetPosition(this).X - Thumb.Width / 2) / (ActualWidth - Thumb.Width);
            args.MouseDevice.Capture(this);
        }

        protected override void OnMouseMove (MouseEventArgs args)
        {
            if (IsMouseCaptured)
            {
                Value = (args.GetPosition(this).X - Thumb.Width / 2) / (ActualWidth - Thumb.Width);
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs args)
        {
            args.MouseDevice.Capture(null);
        }
    }

    class TrackInfo : StackPanel
    {
        Label TrackNameLabel;
        Label TrackAuthorLabel;

        public Track Track
        {
            set
            {
                TrackNameLabel.Content = value.Name;

                if (value.Author == null)
                {
                    TrackAuthorLabel.Content = "Unknown";
                }
                else
                {
                    TrackAuthorLabel.Content = value.Author;
                }
            }
        }

        public TrackInfo()
        {
            TrackNameLabel = new Label()
            {
                Content = "Track name",
                Padding = new Thickness(),
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Color.FromRgb(37, 37, 37))
            };
            Children.Add(TrackNameLabel);

            TrackAuthorLabel = new Label()
            {
                Content = "Track author",
                Padding = new Thickness(),
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
