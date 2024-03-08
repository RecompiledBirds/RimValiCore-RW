using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RVCRestructured.Windows
{
    public static class ColorExtensions
    {
        public static Color MoveTowards(this Color current, Color target, float speed = 1f)
        {
            return new Color
            (
                Mathf.MoveTowards(current.r, target.r, speed * Time.deltaTime),
                Mathf.MoveTowards(current.g, target.g, speed * Time.deltaTime),
                Mathf.MoveTowards(current.b, target.b, speed * Time.deltaTime)
            );
        }
    }
}
