using UnityEngine;
using Verse;

namespace RVCRestructured.Windows;

public static class RectExtensions
{
    private const float WAVE_SPEED_MULT = 3f;

    /// <summary>
    ///     Creates a inner rect for a scroll rect, using the outer rect as base.
    ///     Decreases the inner rects width, if it is high enough for scroll bars to exist, by the width of scroll bars
    /// </summary>
    /// <param name="outerRect">the outer <see cref="Rect"/></param>
    /// <param name="innerHeight">the height of the inner rect</param>
    /// <returns></returns>
    public static Rect GetInnerScrollRect(this Rect outerRect, float innerHeight) => new(outerRect)
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
        Rect newRect = new(rect);
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
        Rect newRect = new(rect);
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
        Rect newRect = new(rect);

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
        Rect newRect = new(rect);

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
    public static Rect AlignXWith(this Rect rect, Rect other) => new(other.x, rect.y, other.width, rect.height);

    /// <summary>
    ///     Flips a <see cref="Rect"/> <paramref name="rect"/> horizontally
    /// </summary>
    /// <param name="rect">the rect to be flipped</param>
    /// <returns>A flipped rect</returns>
    public static Rect FlipHorizontal(this Rect rect) => new(rect.x + rect.width, rect.y, rect.width * -1, rect.height);

    /// <summary>
    ///     Draws a number of colored <paramref name="bars"/> into a <paramref name="rect"/> with an width of <paramref name="outsideLineThickness"/> leaving a margin of <paramref name="insideBarMargin"/>
    /// </summary>
    /// <param name="rect">the rect the drawing will be done in</param>
    /// <param name="outsideLineThickness">the thickness of a box drawn around the drawing</param>
    /// <param name="insideBarMargin">the margin left from the box to the bars</param>
    /// <param name="bars">the bars drawn inside the specified rectangle, drawing the first to last bar</param>
    public static void DrawProjectedCurrentBar(this Rect rect, Color outsideLineColor, int outsideLineThickness, int insideBarMargin, params (float percentage, Color color, TipSignal tooltip, bool doWaveAnimation)[] bars)
    {
        Rect coloredBaseRect = rect.ContractedBy(insideBarMargin + outsideLineThickness);

        GUI.color = outsideLineColor;
        Widgets.DrawBox(rect, outsideLineThickness);
        GUI.color = Color.white;

        float prevBarPercentage = float.MaxValue;
        Rect? nextBarRect = null;

        for (int i = 0;  i < bars.Length; i++)
        {
            (float percentage, Color color, TipSignal tooltip, bool doWaveAnimation) bar = bars[i];

            if (DoesBarHaveErrors(prevBarPercentage, bar)) continue;

            //Ensure that the rectangle areas of the bars don't overlap so that the tip regions don't overlap
            Rect barRect = GetRectForBar(coloredBaseRect, bars, ref nextBarRect, i);
            Color barColor = GetWaveColor(bar);

            Widgets.DrawBoxSolid(barRect, barColor);
            TooltipHandler.TipRegion(barRect, bar.tooltip);

            prevBarPercentage = Mathf.Min(prevBarPercentage, bar.percentage);
        }
    }

    private static Rect GetRectForBar(Rect coloredBaseRect, (float percentage, Color color, TipSignal tooltip, bool doWaveAnimation)[] bars, ref Rect? nextBarRect, int currentBarIndex)
    {
        Rect barRect = nextBarRect ?? coloredBaseRect.LeftPart(bars[currentBarIndex].percentage).RoundedCeil();
        if (bars.Length - 1 == currentBarIndex) return barRect;

        nextBarRect = coloredBaseRect.LeftPart(bars[currentBarIndex + 1].percentage).RoundedCeil();
        float xPos = Mathf.Max(nextBarRect.Value.xMax, coloredBaseRect.x);

        barRect = new Rect(xPos, barRect.y, barRect.xMax - xPos, barRect.height);

        return barRect;
    }

    /// <summary>
    ///     Get the <see cref="Color"/> the inner <paramref name="bar"/> should have in respect to the wave animation
    /// </summary>
    /// <param name="bar">the bar the color is calculated for</param>
    /// <returns>the <see cref="Color"/> the <paramref name="bar"/> should be displayed with</returns>
    private static Color GetWaveColor((float percentage, Color color, TipSignal tooltip, bool doWaveAnimation) bar)
    {
        if (!bar.doWaveAnimation) return bar.color;

        Color barColor = bar.color;

        float waveLerp = (Mathf.Sin(Time.time * WAVE_SPEED_MULT) + 1) / 2;
        Color waveColorAdjusted = Color.Lerp(barColor, Color.white, 0.5f); //bar color mixed with white at 50% ratio
        barColor = Color.Lerp(barColor, waveColorAdjusted, waveLerp);

        return barColor;
    }

    /// <summary>
    ///     Checks if any given <paramref name="bar"/> has errors in respect to the <paramref name="prevBarPercentage"/>, logging them as long as <see cref="VineSettings.debugMode"/> is set to <c>true</c>
    /// </summary>
    /// <param name="prevBarPercentage">the percentage of the previous bar</param>
    /// <param name="bar">the bar about to be drawn next</param>
    /// <returns>if a <paramref name="bar"/> has errors in respect to the <paramref name="prevBarPercentage"/></returns>
    private static bool DoesBarHaveErrors(float prevBarPercentage, (float percentage, Color color, TipSignal tooltip, bool doWaveAnimation) bar)
    {
        if (prevBarPercentage < bar.percentage)
        {
            VineLog.Log($"Bar with tooltip: {bar.tooltip.ToStringSafe()} is specified larger than previous bar and will be skipped.", RVCLogType.Error, debugOnly: true);
            return true;
        }

        if (bar.percentage < 0 || bar.percentage > 1)
        {
            VineLog.Log($"Bar with tooltip: {bar.tooltip.ToStringSafe()} is larger than 1 or smaller than 0 (has barPercentage of: {bar.percentage}", RVCLogType.Error, debugOnly: true);
            return true;
        }

        return false;
    }
}
