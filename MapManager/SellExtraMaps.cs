using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Default.EXtensions;
using Default.EXtensions.CachedObjects;
using Default.EXtensions.Positions;
using Default.MapBot;
using Loki.Bot;
using Loki.Common;
using Loki.Game;
using Loki.Game.Objects;
using Loki.Game.GameData;
using settings = MapManager.Settings;

namespace MapManager
{
    class SellExtraMaps : ErrorReporter, ITask
    {
        private static bool _shouldUpdateStashData;

        public async Task<bool> Run()
        {
            if (ErrorLimitReached)
                return false;

            var area = World.CurrentArea;

            if (!area.IsTown && !area.IsHideoutArea)
                return false;

            if (_shouldUpdateStashData)
            {
                GlobalLog.Debug("[SellExtraMapsTask] Updating map tab stash data (every Start)");

                if (!await OpenMapTab())
                    return true;
                else
                    Class1.MapStashData.SyncWithStashTab();
                _shouldUpdateStashData = false;
            }
            else
            {
                if(!await Inventories.OpenStashTab(settings.Instance.MapStashTab))
                {
                    ReportError();
                    return true;
                }

                Class1.MapStashData.SyncWithStashTab();

                if (Class1.MapStashData.GetMapCount() > settings.Instance.MaxNumMaps)
                {
                    var mapsInStashTab = Inventories.StashTabItems
                        .Where(m => m.IsMap() && m.Rarity != Rarity.Unique)
                        .OrderBy(m => m.Priority())
                        .ThenBy(m => m.MapTier).Take(settings.Instance.SellNumMaps);



                    foreach (var mapItem in mapsInStashTab)
                    {
                        if (!await Inventories.FastMoveFromStashTab(mapItem.LocationTopLeft))
                        {
                            ReportError();
                            return true;
                        }
                    }

                    await Wait.SleepSafe(300);

                    Class1.MapStashData.SyncWithStashTab();

                    var mapSellItems = Inventories.InventoryItems
                        .Where(m => m.IsMap())
                        .Select(i => i.LocationTopLeft)
                        .ToList();

                    if (mapSellItems.Count == 0)
                    {
                        GlobalLog.Error("[SellExtraMapsTask] Unknown error. There are no map items in player's inventory after taking them from stash.");
                        ReportError();
                        return true;
                    }

                    if (!await TownNpcs.SellItems(mapSellItems))
                        ReportError();
                }
                else
                    return false;
            }

            return true;
        }

        public SellExtraMaps()
        {
            ErrorLimitMessage = "[SellExtraMapsTask] Too many errors. This task will be disabled until combat area change.";
        }

        public void Start()
        {
            _shouldUpdateStashData = true;
        }

        public MessageResult Message(Message message)
        {
            return MessageResult.Unprocessed;
        }

        private async Task<bool> OpenMapTab()
        {
            // Check Map Tab
            if (!await Inventories.OpenStashTab(settings.Instance.MapStashTab))
            {
                ReportError();
                return false;
            }
            var mapTabInfo = LokiPoe.InGameState.StashUi.StashTabInfo;
            if (mapTabInfo.IsPremiumSpecial)
            {
                GlobalLog.Error($"[SellExtraMapsTask] Invalid map stash tab type: {mapTabInfo.TabType}. This tab cannot be used for map manager plugin.");
                BotManager.Stop();
                return false;
            }
            return true;
        }

        #region Unused interface methods

        public void Tick()
        {
        }

        public void Stop()
        {
        }

        public async Task<LogicResult> Logic(Logic logic)
        {
            return LogicResult.Unprovided;
        }

        public string Name => "SellExtraMapsTask";
        public string Description => "Task that sells excess map items to vendor.";
        public string Author => "Testtest";
        public string Version => "1.0";

        #endregion

    }
}
