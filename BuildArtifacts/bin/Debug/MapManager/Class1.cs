using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;
using log4net;
using Default.EXtensions;
using Default.EXtensions.Global;
using Loki;
using Loki.Bot;
using Loki.Common;
using Loki.Game.GameData;
using Loki.Game.Objects;
using settings = MapManager.Settings;

namespace MapManager
{
    public class Class1 : IPlugin, IStartStopEvents
    {
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();
        private Gui _gui;

        public static readonly string MapStashDataPath = Path.Combine(Configuration.Instance.Path, "MapManagerStashData.json");
        public static readonly MapData MapStashData = MapData.LoadFromJson(MapStashDataPath);

        public void Start()
        {
            Log.DebugFormat("[MapManager] Start");

            if (settings.Instance.MapStashTab != "")
            {
                var taskManager = BotStructure.TaskManager;
                taskManager.AddAfter(new SellExtraMaps(), "StashTask");
            }
        }

        public void Enable()
        {

        }

        public void Disable()
        {

        }

        public MessageResult Message(Message message)
        {
            return MessageResult.Unprocessed;
        }

        #region Unused interface methods
        public void Stop()
        {
        }

        public void Initialize()
        {
        }

        public void Deinitialize()
        {
        }

        public async Task<LogicResult> Logic(Logic logic)
        {
            return LogicResult.Unprovided;
        }
        #endregion

        public string Name => "MapManager";
        public string Description => "Plugin to control # of maps stored in stash.";
        public string Author => "Testtest";
        public string Version => "1.0";
        public UserControl Control => _gui ?? (_gui = new Gui());
        public JsonSettings Settings => settings.Instance;

    }
}
