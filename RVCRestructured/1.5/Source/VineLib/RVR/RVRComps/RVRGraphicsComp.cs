using RVCRestructured.Defs;
using UnityEngine;

namespace RVCRestructured;

public class RVRGraphicsComp : CompProperties
{
    public readonly bool hasUniqueHeadApparel = false;
    public readonly bool throwApparelError = false;
    public readonly bool useEmptyApparelIfNoDefault = true;
    public readonly bool hasHair = false;
    public readonly List<RaceColors> colorGenerators = [];
    public readonly List<RenderableDef> renderableDefs = [];
    public readonly string skinColorSet = string.Empty;

    public readonly string bodyTex = string.Empty;
    public readonly string headTex = string.Empty;
    public readonly string skeleton = "Things/Pawn/Humanlike/HumanoidDessicated";
    public readonly string skull = "Things/Pawn/Humanlike/Heads/None_Average_Skull";
    public readonly string stump = "Things/Pawn/Humanlike/Heads/None_Average_Stump";
    public readonly Vector2 headSize;
    public readonly Vector2 bodySize = new(1f, 1f);

    public Dictionary<string, RaceColors> cachedColors = [];
    public RaceColors? this[string name]
    {
        get
        {
            if (cachedColors.ContainsKey(name)) return cachedColors[name];

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
            RaceColors? colors = this[skinColorSet];
            if (colors != null) return colors;

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
        compClass = typeof(GraphicsComp);
    }
}

public class GraphicsComp : ThingComp
{
    public RVRGraphicsComp Props => (RVRGraphicsComp)props;
}
