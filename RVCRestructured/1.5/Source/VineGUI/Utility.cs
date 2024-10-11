using UnityEngine;
using Verse;

namespace NesGUI;

class Utility
{
    public static int DetermineScrollDelta(Event e)
    {
        int returnInt;

        returnInt = (e.delta.y > 0) ? 1 : -1;
        returnInt = (e.shift) ? returnInt *= 100 : returnInt;
        returnInt = (e.control) ? returnInt *= 10 : returnInt;


        if (e.alt)
        {
            returnInt = (e.delta.y > 0) ? 1 : -1;

        } else if(returnInt == 1 || returnInt == -1)
        {
            returnInt = (e.delta.y > 0) ? 5 : -5;
        }


        return returnInt;
    }

    public static void NumericScrollWheelField(Rect rect, ref float val, ref string valBuffer,float min = 0, float max = float.MaxValue, string toolTipstring = null)
    {
        Widgets.TextFieldNumeric(rect, ref val, ref valBuffer,min,max);
        if (toolTipstring != null)
        {
            TooltipHandler.TipRegion(rect,toolTipstring);
        }
        //Scroll controls
        Event e = Event.current;
        if (e.isScrollWheel && Mouse.IsOver(rect))
        {
            val += DetermineScrollDelta(e);
            valBuffer = val.ToString();
        }
    }
}
