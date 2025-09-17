using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interop;

namespace HHSAdvWin
{
    internal class ZSystem
    {
        public enum GameStatus {  Title = 0, Play = 1, GameOver = 2, }
        public GameStatus Status { get; set; } = GameStatus.Title;

        private static ZSystem? instance = null;
        public string dataFolder { get; private set; } = string.Empty;
        private ZMap? map = null;
        private ZRules? rules = null;
        private ZWords? dict = null;
        private ZObjects? objects = null;
        private ZMessage? messages = null;
        private ZAudio? audio = null;
        private ZProperties? properties = null;

        public ZMap Map
        {
            get
            {
                return map!;
            }
        }
        public ZRules Rules
        {
            get
            {
                return rules!;
            }
        }
        public ZWords Dict
        {
            get
            {
                return dict!;
            }
        }
        public ZObjects Objects
        {
            get
            {
                return objects!;
            }
        }
        public ZMessage Messages
        {
            get
            {
                return messages!;
            }
        }
        public ZAudio Audio
        {
            get { return audio!; }
        }
        public ZProperties Properties
        {
            get
            {
                return properties!;
            }
        }
        public static ZSystem Instance { 
            get 
            {
                if (instance == null)
                {
                    instance = new ZSystem();
                }
                return instance; 
            } 
        }
        private ZSystem()
        {
            dataFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HHSAdvWin");
            map = new ZMap(System.IO.Path.Combine(dataFolder, "map.dat"));
            rules = new ZRules(System.IO.Path.Combine(dataFolder, "rule.dat"));
            dict = new ZWords(System.IO.Path.Combine(dataFolder, "highds.com"));
            objects = new ZObjects(System.IO.Path.Combine(dataFolder, "thin.dat"));
            messages = new ZMessage(System.IO.Path.Combine(dataFolder, "msg.dat"));
            audio = new ZAudio(dataFolder);
            properties = new ZProperties();
        }

        public void Init()
        {
            Properties.Load(System.IO.Path.Combine(dataFolder, "HHSAdvWin.json"));
            Status = GameStatus.Title;
            ZUserData.Instance.load(System.IO.Path.Combine(dataFolder, "data.dat"));
            map.Cursor = 76;
        }
        public void Quit()
        {
            SavePreferences();
        }
        public void SavePreferences()
        {
            Properties.Save(System.IO.Path.Combine(dataFolder, "HHSAdvWin.json"));
        }
    }
}

