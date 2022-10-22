using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RVCRestructured.Graphics
{
    public static class RVG_MaterialAllocator
    {
        private static readonly Dictionary<Material, MaterialInfo> references = new Dictionary<Material, MaterialInfo>();
        private static Dictionary<string, int> snapshot = new Dictionary<string, int>();

        public static int nextWarningThreshold;

        private struct MaterialInfo
        {
            public string stackTrace;
        }

        public static Material Create(Material material)
        {
            Material newMaterial = new Material(material);
            references[newMaterial] = new MaterialInfo
            {
                stackTrace = Prefs.DevMode ? Environment.StackTrace : "(unavailable)"
            };
            TryReport();
            return newMaterial;
        }

        public static Material Create(Shader shader)
        {
            Material newMaterial = new Material(shader);
            references[newMaterial] = new MaterialInfo
            {
                stackTrace = Prefs.DevMode ? Environment.StackTrace : "(unavailable)"
            };
            TryReport();
            return newMaterial;
        }

        public static void Destroy(Material material)
        {
            if (!references.ContainsKey(material))
            {
                Log.Error($"Destroying material {material}, but that material was not created through the MaterialTracker");
            }
            references.Remove(material);
            UnityEngine.Object.Destroy(material);
        }

        public static void TryReport()
        {
            if (MaterialWarningThreshold() > nextWarningThreshold)
            {
                nextWarningThreshold = MaterialWarningThreshold();
            }
            if (references.Count > nextWarningThreshold)
            {
                Log.Error($"Material allocator has allocated {references.Count} materials; this may be a sign of a material leak");
                if (Prefs.DevMode)
                {
                    MaterialReport();
                }
                nextWarningThreshold *= 2;
            }
        }

        public static int MaterialWarningThreshold()
        {
            return int.MaxValue;
        }

        [DebugOutput("System")]
        public static void MaterialReport()
        {
            foreach (string text in Enumerable.Take(
                Enumerable.Select(
                    Enumerable.OrderByDescending(
                        Enumerable.GroupBy(references, kvp => kvp.Value.stackTrace), g => Enumerable.Count(g))
                    , g => $"{Enumerable.Count(g)}: {Enumerable.FirstOrDefault(g).Value.stackTrace}")
                , 20))
            {
                Log.Error(text);
            }
        }

        [DebugOutput("System")]
        public static void MaterialSnapshot()
        {
            snapshot = new Dictionary<string, int>();
            foreach (IGrouping<string, KeyValuePair<Material, MaterialInfo>> grouping in Enumerable.GroupBy(references, kvp => kvp.Value.stackTrace))
            {
                snapshot[grouping.Key] = Enumerable.Count(grouping);
            }
        }

        [DebugOutput("System")]
        public static void MaterialDelta()
        {
            IEnumerable<string> enumerable = Enumerable.Distinct(Enumerable.Concat(Enumerable.Select(references.Values, v => v.stackTrace), snapshot.Keys));
            Dictionary<string, int> currentSnapshot = new Dictionary<string, int>();
            foreach (IGrouping<string, KeyValuePair<Material, MaterialInfo>> grouping in Enumerable.GroupBy(references, kvp => kvp.Value.stackTrace))
            {
                currentSnapshot[grouping.Key] = Enumerable.Count(grouping);
            }
            foreach (string text in Enumerable.Take(Enumerable.Select(Enumerable.OrderByDescending(Enumerable.Select(enumerable, (string k) => new KeyValuePair<string, int>(k, currentSnapshot.TryGetValue(k, 0) - snapshot.TryGetValue(k, 0))), (KeyValuePair<string, int> kvp) => kvp.Value), g => string.Format("{0}: {1}", g.Value, g.Key)), 20))
            {
                Log.Error(text);
            }
        }
    }
}
