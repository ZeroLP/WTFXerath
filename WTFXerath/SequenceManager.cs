using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WTFXerath.Sequences;
using Oasys.SDK.Tools;

namespace WTFXerath
{
    public class SequenceManager
    {
        private static List<SequenceBase> InitializedSequences { get; set; }

        private static bool IsSequenceListPopulated => InitializedSequences != null && InitializedSequences.Count != 0;

        static SequenceManager()
        {
            Logger.Log($"[SequenceManager] Populating sequences.");

            InitializedSequences = new List<SequenceBase>()
            {
                new HaltingSequence(),
                new HarrasingSequence(),
                new UltimateTakeDownSequence()
            };
        }

        public static void InitializeSequences()
        {
            if(IsSequenceListPopulated)
            {
                foreach(var seq in InitializedSequences)
                {
                    try
                    {
                        seq.Initialize();
                    }
                    catch(Exception ex)
                    {
                        Logger.Log($"[SequenceManager] Exception occurred whilst initializing sequences. {ex.Message}" +
                                   $"\n{ex.StackTrace}", LogSeverity.Danger);
                    }
                }
            }
        }

        public static void UnloadSequences()
        {
            if(IsSequenceListPopulated)
            {
                foreach (var seq in InitializedSequences)
                {
                    try
                    {
                        seq.Unload();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"[SequenceManager] Exception occurred whilst unloading sequences. {ex.Message}" +
                                   $"\n{ex.StackTrace}", LogSeverity.Danger);
                    }
                }
            }
        }
    }
}
