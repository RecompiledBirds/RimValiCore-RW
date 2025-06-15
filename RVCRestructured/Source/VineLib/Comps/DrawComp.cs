namespace RVCRestructured.Comps;

using RimWorld;
using RVCRestructured.Graphics;
using System.Collections.Generic;
using UnityEngine;
using Verse;

public class DrawCompProps : CompProperties
{
    private readonly string texPath = string.Empty;
    private readonly Vector3 offset = Vector3.zero;

    private readonly bool isAnimated = false;
    private readonly int ticksBetweenTexture = 0;
    private readonly bool lockAtLastTex = false;
    private readonly List<string> textures = [];

    public DrawCompProps()
    {
        compClass = typeof(DrawComp);
    }

    public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
    {
        if (texPath == string.Empty) yield return "texPath is not set!";

        foreach (string item in base.ConfigErrors(parentDef))
        {
            yield return item;
        }
    }

    public string TexPath => texPath;
    public Vector3 Offset => offset;
    public bool IsAnimated => isAnimated;
    public int TicksBetweenTexture => ticksBetweenTexture;
    public bool LockAtLastTex => lockAtLastTex;
    public List<string> Textures => textures;
}

public class DrawComp : ThingComp
{
    public DrawCompProps Props => (DrawCompProps)props;
    public Graphic? graphic;
    public int tex;
    public int tick;

    public override void CompTick()
    {
        if (Props.IsAnimated)
        {
            tick++;

            if (tick == Props.TicksBetweenTexture)
            {
                tick = 0;
                tex = tex < Props.Textures.Count - 1 ? tex++ : !Props.LockAtLastTex ? tex = 0 : tex = Props.Textures.Count - 1;
            }
        }
        base.CompTick();
    }

    public override void PostDraw()
    {
        Draw();
    }

    private void Draw()
    {
        Vector3 offset = Props.Offset;
        Vector3 pos = parent.DrawPos;
        pos.y += 1.5f + offset.y;
        pos.z += offset.z;
        pos.x += offset.x;

        if (Props.IsAnimated)
        {
            graphic = RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(Props.Textures[tex], parent.Graphic.drawSize);
        }
        else
        {
            graphic ??= RVG_GraphicDataBase.Get<RVG_Graphic_Multi>(Props.TexPath, parent.Graphic.drawSize);
        }

        if (parent.TryGetComp<CompPowerTrader>() != null)
        {
            if (parent.TryGetComp<CompPowerTrader>().PowerOn && FlickUtility.WantsToBeOn(parent))
            {
                graphic.Draw(pos, parent.Rotation, parent);
            }
        }
        else
        {
            graphic.Draw(pos, parent.Rotation, parent); 
        }
    }
}
