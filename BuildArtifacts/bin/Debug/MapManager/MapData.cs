using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Default.EXtensions;
using Default.EXtensions.CachedObjects;
using Default.EXtensions.Global;
using Default.MapBot;
using Loki.Game.GameData;
using Loki.Game.Objects;
using Loki.Bot;
using Newtonsoft.Json;
using System.IO;

namespace MapManager
{
    public class MapData
    {
        private readonly int[] _counts = new int[1];

        public void IncreaseMapCount()
        {
            ++_counts[0];
        }

        public int GetMapCount()
        {
            return _counts[0];
        }

        public void SetMapCount(int newCount)
        {
            _counts[0] = newCount;
        }

        public void Reset()
        {
                _counts[0] = 0;
        }

        public void SyncWithStashTab()
        {
            Reset();

            var numMaps = Inventories.StashTabItems
                .Where(m => m.IsMap() && m.Rarity != Rarity.Unique)
                .Count();

            SetMapCount(numMaps);

            SaveToJson(Class1.MapStashDataPath);
        }


        public void SaveToJson(string path)
        {
            var json = JsonConvert.SerializeObject(_counts, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public static MapData LoadFromJson(string path)
        {
            var data = new MapData();

            if (!File.Exists(path))
                return data;

            var json = File.ReadAllText(path);

            if (string.IsNullOrWhiteSpace(json))
            {
                GlobalLog.Info("[MapManager] Fail to load stash data from json. File is empty.");
                return data;
            }
            int[] jsonCounts;
            try
            {
                jsonCounts = JsonConvert.DeserializeObject<int[]>(json);
            }
            catch (Exception)
            {
                GlobalLog.Info("[MapManager] Fail to load stash data from json. Exception during json deserialization.");
                return data;
            }
            if (jsonCounts == null)
            {
                GlobalLog.Info("[MapManager] Fail to load stash data from json. Json deserealizer returned null.");
                return data;
            }
            Array.Copy(jsonCounts, data._counts, data._counts.Length);
            return data;
        }

    }
}
