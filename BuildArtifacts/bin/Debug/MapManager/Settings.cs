using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loki;
using Loki.Common;
using Newtonsoft.Json;

namespace MapManager
{
    class Settings : JsonSettings
    {

        private static Settings _instance;
        public static Settings Instance => _instance ?? (_instance = new Settings());

        private Settings()
            : base(GetSettingsFilePath(Configuration.Instance.Name, "MapManager.json"))
        {
        }

        public string MapStashTab { get; set; }
        public int MaxNumMaps { get; set; } = 40;
        public int SellNumMaps { get; set; } = 10;
    }
}

