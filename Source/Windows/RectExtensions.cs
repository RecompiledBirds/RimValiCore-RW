using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RVCRestructured.Windows
{
    public static class RectExtensions
    {
        /// <summary>
        ///     Creates a inner rect for a scroll rect, using the outer rect as base.
        ///     Decreases the inner rects width, if it is high enough for scroll bars to exist, by the width of scroll bars
        /// </summary>
        /// <param name="outerRect">the outer <see cref="Rect"/></param>
        /// <param name="innerHeight">the height of the inner rect</param>
        /// <returns></returns>
        public static Rect GetInnerScrollRect(this Rect outerRect, float innerHeight) => new Rect(outerRect)
        {
            height = innerHeight,
            width = outerRect.width - (innerHeight > outerRect.height ? 17f : 0f)
        };


        /// <summary>
        ///     Draws a highlight into the selected rect, a light highlight if <paramref name="light"/> is true, dark otherwise
        /// </summary>
        /// <param name="rect">The rect the highlight is drawn in</param>
        /// <param name="light">If the highlight is dark or light</param>
        public static void DoRectHighlight(this Rect rect, bool light)
        {
            if (light)
            {
                Widgets.DrawLightHighlight(rect);
            }
            else
            {
                Widgets.DrawHighlight(rect);
            }
        }

        /// <summary>
        ///     Creates a copy of this <see cref="Rect" /> moved by a <see cref="Vector2" />
        /// </summary>
        /// <param name="rect">the <see cref="Rect" /> to move</param>
        /// <param name="vec">the distance to move <paramref name="rect" /></param>
        /// <returns>A copy of <paramref name="rect" />, moved by the distance specified in <paramref name="vec" /></returns>
        public static Rect MoveRect(this Rect rect, Vector2 vec)
        {
            Rect newRect = new Rect(rect);
            newRect.position += vec;
            return newRect;
        }
    }
}
