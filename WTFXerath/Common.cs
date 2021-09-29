using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oasys.Common.GameObject;
using Oasys.SDK;
using SharpDX;

namespace WTFXerath.Common
{
    public static class CommonClass
    {
        public static bool IsInRange(this GameObjectBase targHero, float range) => targHero.Distance <= range + targHero.UnitComponentInfo.UnitBoundingRadius + 5;

        public static double DistanceFromPointToLine(Vector2 point, Vector2[] Line)
        {
            var l1 = Line[0];
            var l2 = Line[1];

            return Math.Abs((l2.X - l1.X) * (l1.Y - point.Y) - (l1.X - point.X) * (l2.Y - l1.Y)) /
                    Math.Sqrt(Math.Pow(l2.X - l1.X, 2) + Math.Pow(l2.Y - l1.Y, 2));
        }

        public static int CalculateQExtendableTime(this float distance)
        {
            switch (distance)
            {
                case float dfl when dfl <= 735f:
                    return 0;
                case float df1 when df1 > 735f && df1 <= 837.14f:
                    return 250;
                case float dfl when dfl > 837.14f && dfl <= 939.29f:
                    return 500;
                case float dfl when dfl > 939.29f && dfl <= 1041.43f:
                    return 750;
                case float dfl when dfl > 1041.43f && dfl <= 1143.57f:
                    return 1000;
                case float dfl when dfl > 1143.57f && dfl <= 1245.71f:
                    return 1250;
                case float dfl when dfl > 1245.71f && dfl <= 1347.86f:
                    return 1500;
                case float dfl when dfl > 1347.86f && dfl <= 1450f:
                    return 1750;
            }

            return 0;
        }

        public static void BlockOrbwalkerCalls()
        {
            Oasys.Common.Settings.Core.UseBlockInput = false;
            Orbwalker.AllowAttacking = false;
            Orbwalker.AllowMoving = false;
        }

        public static void UnblockOrbwalkerCalls()
        {
            if (!Oasys.Common.Settings.Core.UseBlockInput)
                Oasys.Common.Settings.Core.UseBlockInput = true;

            if (!Orbwalker.AllowAttacking && !Orbwalker.AllowMoving)
            {
                Orbwalker.AllowAttacking = !Orbwalker.AllowAttacking;
                Orbwalker.AllowMoving = !Orbwalker.AllowMoving;
            }
        }
    }
}
