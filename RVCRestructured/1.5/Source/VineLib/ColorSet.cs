using UnityEngine;
using Verse;

namespace RVCRestructured;

public class TriColorSet : IExposable
{
    private Color one;
    private Color two;
    private Color three;
    private readonly bool dyeable;

    public TriColorSet(Color one, Color two, Color three, bool dyable)
    {
        this.one = one;
        this.two = two;
        this.three = three;
        this.dyeable = dyable;
    }
    public TriColorSet()
    {

    }
    public bool Dyeable
    {
        get
        {
            return dyeable;
        }
    }

    public Color[] colors
    {
        get
        {
            return [one,two,three];
        }
        set
        {
            one = value[0];
            two = value[1];
            three = value[2];
        }
    }

    //Allow access to colors via index
    public Color this[int index]
    {
        get
        {
            switch (index)
            {
                case 0: return one;
                case 1: return two;
                case 2: return three;
                //If something odd is inputted, return one.
                default:
                    Log.Warning($"Tried to access a color index greater than 2 from a colorset!");
                    return one;
            }
        }
    }

    public void ExposeData()
    {
        Scribe_Values.Look(ref one, "c1");
        Scribe_Values.Look(ref two, "c2");
        Scribe_Values.Look(ref three, "c3");
    }
}
