using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MusicPlayer
{
    class Playlist
    {
        static string[] SupportedExtensions = {".mp3", ".ogg", ".wav"};

        public string Name { get; }

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
                        Tracks.Add(new Track(file));
                    }
                }
            }
        }

        public List<Track> Tracks { get; } = new List<Track> {};

        public Playlist(string name, DirectoryInfo dir)
        {
            Name = name;
            Directory = dir;
        }
    }

    class Track
    {
        FileInfo file;
        public FileInfo File
        {
            get { return file; }
            set
            {
                file = value;
                Name = file.Name.Replace(file.Extension, "");

                if (Name.Contains(" - "))
                {
                    int dash_index = Name.IndexOf(" - ");

                    Author = Name.Substring(0, dash_index).Trim();
                    Name = Name.Substring(dash_index + 3).Trim();
                }
            }
        }

        public string Name { get; private set; }
        public string Author { get; private set; }

        public Track(FileInfo file)
        {
            File = file;
        }
    }
}
