using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Oasys.SDK;
using Oasys.SDK.Tools;
using Oasys.SDK.Events;
using Oasys.SDK.SpellCasting;
using Oasys.SDK.Rendering;
using Oasys.Common.GameObject;
using Oasys.Common.Extensions;
using Oasys.Common.GameObject.Clients;
using Oasys.Common.GameObject.ObjectClass;
using Oasys.Common.Enums.GameEnums;
using WTFXerath.Common;
using SharpDX;
using SharpDX.Mathematics.Interop;

namespace WTFXerath
{
    public class Main
    {
        [OasysModuleEntryPoint]
        public static void Execute()
        {
            GameEvents.OnGameLoadComplete += GameEvents_OnGameLoadComplete;
        }

        private static Task GameEvents_OnGameLoadComplete()
        {
            if(UnitManager.MyChampion.ModelName != "Xerath")
            {
                Logger.Log($"You are currently playing as {UnitManager.MyChampion.Name} which is not supported by this module.", LogSeverity.Warning);
                return Task.CompletedTask;
            }

            CoreEvents.OnCoreMainTick += CoreEvents_OnCoreMainTick;
            GameEvents.OnGameMatchComplete += GameEvents_OnGameMatchComplete;

            SequenceManager.InitializeSequences();

            return Task.CompletedTask;
        }

        private static Task GameEvents_OnGameMatchComplete()
        {
            SequenceManager.UnloadSequences();

            return Task.CompletedTask;
        }

        public static int CacheTick = 0;
        private static Task CoreEvents_OnCoreMainTick()
        {
            CacheTick += 10;

            return Task.CompletedTask;
        }
    }
}
