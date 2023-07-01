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

        /// <summary>
        ///     Creates a copy of a <see cref="Rect" /> with its left edge moved by <paramref name="scaleBy" />, while
        ///     maintaining its width.
        /// </summary>
        /// <param name="rect">The <see cref="Rect" /> to modify</param>
        /// <param name="scaleBy">The amount of units to scale <paramref name="rect" /> by</param>
        /// <returns>A copy of <paramref name="rect" /> with its left edge moved to the left by <paramref name="scaleBy" /> units</returns>
        public static Rect ScaleX(this Rect rect, float scaleBy)
        {
            Rect newRect = new Rect(rect);
            newRect.xMin -= scaleBy;
            return newRect;
        }

        /// <summary>
        ///     Devides a <see cref="Rect"/> <paramref name="rect"/> vertically into <see cref="int"/> <paramref name="times"/> amount of pieces
        /// </summary>
        /// <param name="rect">the initial <see cref="Rect"/> that is to be devided</param>
        /// <param name="times">the amount of times it should be devided</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with <paramref name="times"/> amount of pieces </returns>
        public static IEnumerable<Rect> DivideVertical(this Rect rect, int times)
        {
            for (int i = 0; i < times; i++)
            {
                yield return rect.TopPartPixels(rect.height / times).MoveRect(new Vector2(0f, rect.height / times * i));
            }
        }

        /// <summary>
        ///     Devides a <see cref="Rect"/> <paramref name="rect"/> horizontally into <see cref="int"/> <paramref name="times"/> amount of pieces
        /// </summary>
        /// <param name="rect">the initial <see cref="Rect"/> that is to be devided</param>
        /// <param name="times">the amount of times it should be devided</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with <paramref name="times"/> amount of pieces </returns>
        public static IEnumerable<Rect> DivideHorizontal(this Rect rect, int times)
        {
            for (int i = 0; i < times; i++)
            {
                yield return rect.LeftPartPixels(rect.width / times).MoveRect(new Vector2(rect.width / times * i, 0f));
            }
        }

        /// <summary>
        ///     Makes a text button that executes an <see cref="Action"/>
        /// </summary>
        /// <param name="rect">The <see cref="Rect"/> to draw the button in</param>
        /// <param name="label">The label of the button</param>
        /// <param name="action">The <see cref="Action"/> that is executed</param>
        public static void DrawButtonText(this Rect rect, string label, Action action, bool disable = false)
        {
            if (disable) return;
            if (Widgets.ButtonText(rect, label))
            {
                action();
            }
        }

        /// <summary>
        ///     Contracts a <see cref="Rect"/> vertically
        /// </summary>
        /// <param name="rect">the <paramref name="rect"/> to be contracted</param>
        /// <param name="amount">the <see cref="int"/> <paramref name="amount"/> by which the rect is to be contracted by</param>
        /// <returns>A new <see cref="Rect"/> that is contracted by the <see cref="int"/> <paramref name="amount"/></returns>
        public static Rect ContractVertically(this Rect rect, int amount)
        {
            Rect newRect = new Rect(rect);

            newRect.y += amount;
            newRect.height -= amount * 2;

            return newRect;
        }

        /// <summary>
        ///     Contracts a <see cref="Rect"/> horizontally
        /// </summary>
        /// <param name="rect">the <paramref name="rect"/> to be contracted</param>
        /// <param name="amount">the <see cref="int"/> <paramref name="amount"/> by which the rect is to be contracted by</param>
        /// <returns>A new <see cref="Rect"/> that is contracted by the <see cref="int"/> <paramref name="amount"/></returns>
        public static Rect ContractHorizontally(this Rect rect, int amount)
        {
            Rect newRect = new Rect(rect);

            newRect.x += amount;
            newRect.width -= amount * 2;

            return newRect;
        }


        /// <summary>
        ///     Changes the <see cref="Rect"/> <paramref name="rect"/>s x and width to the x and width of the <see cref="Rect"/> <paramref name="other"/>
        /// </summary>
        /// <param name="rect">The <see cref="Rect"/> to be changed</param>
        /// <param name="other">The <see cref="Rect"/> that has the variables to change to</param>
        /// <returns></returns>
        public static Rect AlignXWith(this Rect rect, Rect other) => new Rect(other.x, rect.y, other.width, rect.height);

        /// <summary>
        ///     Flips a <see cref="Rect"/> <paramref name="rect"/> horizontally
        /// </summary>
        /// <param name="rect">the rect to be flipped</param>
        /// <returns>A flipped rect</returns>
        public static Rect FlipHorizontal(this Rect rect) => new Rect(rect.x + rect.width, rect.y, rect.width * -1, rect.height);
    }
}
