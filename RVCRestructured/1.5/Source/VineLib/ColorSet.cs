using UnityEngine;
using Verse;

namespace RVCRestructured;

public class TriColorSet : IExposable
{
    private Color one;
    private Color two;
    private Color three;
    private readonly bool dyeable;

    public static TriColorSet Empty { get; } = new();

    public TriColorSet(Color one, Color two, Color three, bool dyable)
    {
        this.one = one;
        this.two = two;
        this.three = three;
        dyeable = dyable;
    }

    public TriColorSet() { }

    public bool Dyeable => dyeable;

    public Color[] Colors
    {
        get => [one, two, three];
        set => (one, two, three) = (value[0], value[1], value[2]);
    }

    //Allow access to colors via index
    public Color this[int index] => index switch
    {
        0 => one,
        1 => two,
        2 => three,

        //If something odd is inputted, return one.
        _ => one
    };

    public void ExposeData()
    {
        Scribe_Values.Look(ref one, "c1");
        Scribe_Values.Look(ref two, "c2");
        Scribe_Values.Look(ref three, "c3");
    }
}
