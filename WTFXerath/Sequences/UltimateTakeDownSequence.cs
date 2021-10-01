using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTFXerath.Common;
using Oasys.SDK;
using Oasys.SDK.SpellCasting;
using Oasys.SDK.Events;
using Oasys.SDK.InputProviders;
using Oasys.Common.Enums.GameEnums;
using Oasys.Common.GameObject;
using SharpDX;

namespace WTFXerath.Sequences
{
    public class UltimateTakeDownSequence : SequenceBase
    {
        public override void Initialize()
        {
            CoreEvents.OnCoreMainTick += CoreEvents_OnCoreMainTick;
            CoreEvents.OnCoreMainTick += CoreEvents_OnCoreMainTick1;

            base.Initialize();
        }

        public override void Unload()
        {
            CoreEvents.OnCoreMainTick -= CoreEvents_OnCoreMainTick;
            CoreEvents.OnCoreMainTick -= CoreEvents_OnCoreMainTick1;

            base.Unload();
        }

        private static GameObjectBase RTargetableCachedTarget { get; set; }
        private Task CoreEvents_OnCoreMainTick()
        {
            RTargetableCachedTarget = UnitManager.EnemyChampions
                .Where(x => x.IsInRange(5000/*Max R Cast Range*/)
                         && x.IsAlive
                         && x.IsVisible)
                .OrderBy(x => x.Distance)
                .ToList().FirstOrDefault();

            return Task.CompletedTask;
        }

        private Task CoreEvents_OnCoreMainTick1()
        {
            var rSpellClass = UnitManager.MyChampion.GetSpellBook().GetSpellClass(SpellSlot.R);

            if (KeyboardProvider.IsShiftPressed()
                && RTargetableCachedTarget != null && RTargetableCachedTarget != default(GameObjectBase) 
                && rSpellClass.IsSpellReady
                && UnitManager.MyChampion.Mana >= rSpellClass.SpellData.ResourceCost)
            {
                var dir = Vector3.Normalize((RTargetableCachedTarget.AIManager.NavTargetPosition - RTargetableCachedTarget.Position));
                var calcEndVec = RTargetableCachedTarget.Position + dir * ((RTargetableCachedTarget.UnitComponentInfo.UnitBaseMoveSpeed / (RTargetableCachedTarget.AIManager.NavTargetPosition - RTargetableCachedTarget.Position).Length()) + 600);

                SpellCastProvider.CastSpell(CastSlot.R);

                System.Threading.Thread.Sleep(627);

                for (int i = 0; i < 3; i++)
                {
                    MouseProvider.SetCursor((int)RTargetableCachedTarget.W2S.X, (int)RTargetableCachedTarget.W2S.Y);
                    SpellCastProvider.CastSpell(CastSlot.R, RTargetableCachedTarget.AIManager.IsMoving ? calcEndVec : RTargetableCachedTarget.Position, 500);
                }
            }

            return Task.CompletedTask;
        }
    }
}
