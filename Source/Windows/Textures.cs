using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RVCRestructured.Windows
{
    [StaticConstructorOnStartup]
    public static class IconTextures
    {
        public static readonly Texture2D iconCustomize = ContentFinder<Texture2D>.Get("GUI/Button/CustomizeButton");
    }

    [StaticConstructorOnStartup]
    public static class ColorTextures
    {
        static ColorTextures() { }

        public static readonly Texture2D BarFullTexYellow = SolidColorMaterials.NewSolidColorTexture(Color.yellow);
        public static readonly Texture2D BarFullTexGreen = SolidColorMaterials.NewSolidColorTexture(Color.green);
        public static readonly Texture2D BarFullTexBlue = SolidColorMaterials.NewSolidColorTexture(Color.blue);
        public static readonly Texture2D BarFullTexRed = SolidColorMaterials.NewSolidColorTexture(Color.red);
    }
}
