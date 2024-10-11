using RVCRestructured.Defs;
using UnityEngine;
using Verse;

namespace RVCRestructured;

public class RVRGraphicsComp : CompProperties
{
    public bool hasUniqueHeadApparel = false;
    public bool throwApparelError = false;
    public bool useEmptyApparelIfNoDefault = true;
    public bool hasHair = false;
    public List<RaceColors> colorGenerators = [];
    public List<RenderableDef> renderableDefs = [];
    public string skinColorSet;

    public string bodyTex;
    public string headTex;
    public string skeleton = "Things/Pawn/Humanlike/HumanoidDessicated";
    public string skull = "Things/Pawn/Humanlike/Heads/None_Average_Skull";
    public string stump = "Things/Pawn/Humanlike/Heads/None_Average_Stump";
    public Vector2 headSize;
    public Vector2 bodySize = new(1f, 1f);

    public  Dictionary<string, RaceColors> cachedColors = [];
    public RaceColors this[string name]
    {
        get
        {
            if (cachedColors.ContainsKey(name))
                return cachedColors[name];

            for (int i = 0; i < colorGenerators.Count; i++)
            {
                if (colorGenerators[i].name == name)
                {
                    cachedColors[name] = colorGenerators[i];
                    return colorGenerators[i];
                }
            }
            return null;
        }
    }

    public RaceColors GetSkinColorGenerator
    {
        get
        {
            //If none is defined, use generator 0
            RaceColors colors = this[this.skinColorSet];
            if (colors != null)
                return colors;

            //If we can't find any that match, log an error and create a temporary/debug color generator.
            Log.Error($"Could not find a color generator named {skinColorSet}.");
            RaceColors debugColors = new()
            {
                name = skinColorSet,
                colorGenerator = new TriColorGenerator()
                {
                    colorOne = new ColorGenerator_Single() { color = Color.red },
                    colorThree = new ColorGenerator_Single() { color = Color.green },
                    colorTwo = new ColorGenerator_Single() { color = Color.blue }
                }
            };
            colorGenerators.Add(debugColors);
            return debugColors;
        }
    }

    public RVRGraphicsComp()
    {
        this.compClass = typeof(GraphicsComp);
    }
}

public class GraphicsComp : ThingComp
{
    public RVRGraphicsComp Props
    {
        get
        {
            return props as RVRGraphicsComp;
        }
    }
}
