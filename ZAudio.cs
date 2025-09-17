// ZAudio for HHSAdvSDL

using System;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Windows.Media;

namespace HHSAdvWin
{
    public class ZAudio
    {
        private string[] soundFiles = {
            "highschool", "charumera", "explosion", string.Empty,  "in_toilet", "acid",
        };
        public string DataFolder { get; private set; }
        private MediaPlayer player = new MediaPlayer();
        public ZAudio(string d)
        {
            DataFolder = d;
        }
        public void Play(int id)
        {
            if (id < 0 || id >= soundFiles.Length || string.IsNullOrEmpty(soundFiles[id])) return;
            StringBuilder mp3 = new StringBuilder(soundFiles[id]);
            mp3.Append(".mp3");
            string af = System.IO.Path.Combine(DataFolder, mp3.ToString());
            if (File.Exists(af))
            {
                player.Open(new Uri(af, UriKind.Absolute));
                player.Play();
            }
        }
    }
}
