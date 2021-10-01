using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTFXerath.Common;
using Oasys.SDK;
using Oasys.SDK.Events;
using Oasys.SDK.SpellCasting;
using Oasys.Common.GameObject;
using Oasys.Common.Enums.GameEnums;
using SharpDX;

namespace WTFXerath.Sequences
{
    public class HarrasingSequence : SequenceBase
    {
        public override void Initialize()
        {
            CoreEvents.OnCoreMainInputAsync += CoreEvents_OnCoreMainInputAsync;
            CoreEvents.OnCoreMainInputRelease += CoreEvents_OnCoreMainInputRelease;
            CoreEvents.OnCoreMainTick += CoreEvents_OnCoreMainTick;

            base.Unload();
        }

        public override void Unload()
        {
            CoreEvents.OnCoreMainInputAsync -= CoreEvents_OnCoreMainInputAsync;
            CoreEvents.OnCoreMainInputRelease -= CoreEvents_OnCoreMainInputRelease;
            CoreEvents.OnCoreMainTick -= CoreEvents_OnCoreMainTick;

            base.Unload();
        }

        public static bool IsInQCastMode = false;

        public int ReleaseTick = 0;
        private GameObjectBase CachedTarget { get; set; }

        private Task CoreEvents_OnCoreMainTick()
        {
            CachedTarget = UnitManager.EnemyChampions
                .Where(x => x.IsInRange(1600/*Max Q Cast Range*/)
                         && x.IsAlive
                         && x.IsVisible)
                .OrderBy(x => x.Distance)
                .ToList().FirstOrDefault();

            return Task.CompletedTask;
        }

        private Task CoreEvents_OnCoreMainInputAsync()
        {
            var spellBook = UnitManager.MyChampion.GetSpellBook();

            if (CachedTarget != null && CachedTarget != default(GameObjectBase) && spellBook.GetSpellClass(SpellSlot.Q).IsSpellReady)
                HarrasCachedTarget();

            return Task.CompletedTask;
        }

        private Task CoreEvents_OnCoreMainInputRelease()
        {
            if (CachedTarget != null && ReleaseTick > 0)
            {
                var dir = Vector3.Normalize((CachedTarget.AIManager.NavTargetPosition - CachedTarget.Position));
                var calcEndVec = CachedTarget.Position + dir * ((CachedTarget.UnitComponentInfo.UnitBaseMoveSpeed / (CachedTarget.AIManager.NavTargetPosition - CachedTarget.Position).Length()) + 200);

                SpellCastProvider.ReleaseChargeSpell(SpellCastSlot.Q, CachedTarget.AIManager.IsMoving ? calcEndVec : CachedTarget.Position, 0);
                IsInQCastMode = false;
            }

            if (CachedTarget == null && ReleaseTick > 0)
            {
                GameEngine.IssueOrder(GameEngine.OrderType.Stop, UnitManager.MyChampion.Position);
                IsInQCastMode = false;
            }

            return Task.CompletedTask;
        }

        public void HarrasCachedTarget()
        {
            var spellBook = UnitManager.MyChampion.GetSpellBook();

            if (ReleaseTick == 0 && UnitManager.MyChampion.Mana >= spellBook.GetSpellClass(SpellSlot.Q).SpellData.ResourceCost)
                ChargeInitialQ();

            if (Main.CacheTick >= ReleaseTick)
            {
                ReleaseQCast();
                IsInQCastMode = false;
                ReleaseTick = 0;
            }

            CommonClass.UnblockOrbwalkerCalls();
        }

        private void ChargeInitialQ()
        {
            IsInQCastMode = true;
            CommonClass.BlockOrbwalkerCalls();

            SpellCastProvider.StartChargeSpell(SpellCastSlot.Q);

            var timeToExtendFor = CachedTarget.Distance.CalculateQExtendableTime();
            ReleaseTick = Main.CacheTick + timeToExtendFor;
        }

        private void ReleaseQCast()
        {
            CommonClass.BlockOrbwalkerCalls();

            var dir = Vector3.Normalize((CachedTarget.AIManager.NavTargetPosition - CachedTarget.Position));
            var calcEndVec = CachedTarget.Position + dir * ((CachedTarget.UnitComponentInfo.UnitBaseMoveSpeed / (CachedTarget.AIManager.NavTargetPosition - CachedTarget.Position).Length()) + 200);

            if (CachedTarget != null)
                SpellCastProvider.ReleaseChargeSpell(SpellCastSlot.Q, CachedTarget.AIManager.IsMoving ? calcEndVec : CachedTarget.Position, 0);
            else
                GameEngine.IssueOrder(GameEngine.OrderType.Stop, UnitManager.MyChampion.Position);

            IsInQCastMode = false;
        }
    }
}
